using IdentityService.Api.Services;

namespace IdentityService.Api.Models
{
    public class AuthenticationService(ILoginService<ApplicationUser> loginService,
        IConfiguration configuration)
    {
        public ILoginService<ApplicationUser> LoginService { get; set; } = loginService;

        public IConfiguration Configuration { get; set; } = configuration;
    }
}
