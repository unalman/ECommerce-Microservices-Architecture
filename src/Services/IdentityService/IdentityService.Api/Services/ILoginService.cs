using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Api.Services
{
    public interface ILoginService<T>
    {
        Task<bool> ValidateCredentials(T user, string password);

        Task<T> FindByUsername(string user);

        Task SignIn(T user);

        Task SignInAsync(T user, AuthenticationProperties properties, string authenticationMethod = null);
    }
}
