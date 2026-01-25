using System.ComponentModel.DataAnnotations;

namespace WebHooks.API.Model
{
    public class WebhookSubscription
    {
        public int Id { get; set; }
        public WebhookType Type { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string DestUrl { get; set; }
        public string Token { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
