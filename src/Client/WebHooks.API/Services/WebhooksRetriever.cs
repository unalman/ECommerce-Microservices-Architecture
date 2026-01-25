using Microsoft.EntityFrameworkCore;
using WebHooks.API.Infrastructure;
using WebHooks.API.Model;

namespace WebHooks.API.Services
{
    public class WebhooksRetriever(WebhooksContext db) : IWebhooksRetriever
    {
        public async Task<IEnumerable<WebhookSubscription>> GetSubscriptionOfType(WebhookType type)
        {
            return await db.Subscriptions.Where(x=> x.Type == type).ToListAsync();
        }
    }
}
