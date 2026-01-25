using WebHooks.API.Model;

namespace WebHooks.API.Services
{
    public interface IWebhooksSender
    {
        Task SendAll(IEnumerable<WebhookSubscription> receivers, WebhookData data);
    }
}
