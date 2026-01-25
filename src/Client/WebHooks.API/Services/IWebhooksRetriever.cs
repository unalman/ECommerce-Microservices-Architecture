using WebHooks.API.Model;

namespace WebHooks.API.Services
{
    public interface IWebhooksRetriever
    {
        Task<IEnumerable<WebhookSubscription>> GetSubscriptionOfType(WebhookType type);
    }
}
