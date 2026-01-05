namespace IdentityService.Api.Services
{
    public interface IRedirectService
    {
        string ExtractRedirectUriFromReturnUrl(string url);
    }
}
