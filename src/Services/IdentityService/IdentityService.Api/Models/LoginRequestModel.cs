using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.Models
{
    public record LoginRequestModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
