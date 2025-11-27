using System.ComponentModel.DataAnnotations;

namespace IdentityService.Api.Models
{
    public record LoginResponseModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserToken { get; set; }
    }
}
