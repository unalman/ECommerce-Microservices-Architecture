namespace WebApp.Services.OrderStatus
{
    public class OrderStatusNotificationService
    {
        private readonly object _subscriptionLock = new();
        private readonly Dictionary<string, HashSet<Subscription>> _subscriptionsByBuyerId = new();

        public IDisposable SubscribeToOrderStatusNotification(string buyerId, Func<Task> callback) {
            var subscription = new Subscription(this, buyerId, callback);

            lock (_subscriptionLock) {
                if (!_subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions))
                {
                    subscriptions = [];
                    _subscriptionsByBuyerId.Add(buyerId, subscriptions);
                }
                subscriptions.Add(subscription);
            }
            return subscription;
        }

        public Task NotifyOrderStatusChangedAsync(string buyerId) {
            lock (_subscriptionLock) {
                return _subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions)
                    ? Task.WhenAll(subscriptions.Select(x=>x.NotifyAsync()))
                    : Task.CompletedTask;
            }
        }

        private void Unsubscribe(string buyerId, Subscription subscription)
        {
            lock (_subscriptionLock)
            {
                if (_subscriptionsByBuyerId.TryGetValue(buyerId, out var subscriptions))
                {
                    subscriptions.Remove(subscription);
                    if (subscriptions.Count == 0)
                    {
                        _subscriptionsByBuyerId.Remove(buyerId);
                    }
                }
            }
        }
        private class Subscription(OrderStatusNotificationService owner, string buyerId, Func<Task> callback) : IDisposable
        {
            public Task NotifyAsync()
            {
                return callback();
            }

            public void Dispose()
                => owner.Unsubscribe(buyerId, this);
        }
    }
}
