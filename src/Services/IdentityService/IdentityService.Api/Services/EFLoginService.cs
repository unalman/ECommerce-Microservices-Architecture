using IdentityService.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Api.Services
{
    public class EFLoginService(UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager) : ILoginService<ApplicationUser>
    {

        public async Task<ApplicationUser?> FindByUsername(string user)
        {
            return await _userManager.FindByNameAsync(user);
        }

        public async Task<bool> ValidateCredentials(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<SignInResult> CheckPasswordSignIn(ApplicationUser user, string password, bool lockoutOnFailure = false)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
        }

        public Task SignIn(ApplicationUser user)
        {
            return _signInManager.SignInAsync(user, true);
        }

        public Task SingInAsync(ApplicationUser user, AuthenticationProperties properties, string authenticationMethod = null)
        {
            return _signInManager.SignInAsync(user, properties, authenticationMethod);
        }


    }
}
