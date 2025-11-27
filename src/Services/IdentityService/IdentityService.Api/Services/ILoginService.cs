using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Api.Services
{
    public interface ILoginService<T>
    {
        Task<bool> ValidateCredentials(T user, string password);

        Task<SignInResult> CheckPasswordSignIn(T user, string password, bool lockoutOnFailure = false);

        Task<T?> FindByUsername(string user);

        Task SignIn(T user);

        Task SingInAsync(T user, AuthenticationProperties properties, string authenticationMethod = null);
    }
}
