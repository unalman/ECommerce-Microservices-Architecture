using IdentityService.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Api.Apis
{
    public static class AuthenticationApi
    {
        public static IEndpointRouteBuilder MapAuthenticationApi(this IEndpointRouteBuilder app)
        {
            var vApi = app.NewVersionedApi();
            var api = vApi.MapGroup("api/auth");
            api.MapGet("/Token", GetToken)
                .WithName("GetToken")
                .WithSummary("Get a token")
                .WithDescription("Authentication using username and password")
                .WithTags("Authentication");
            return app;
        }
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
        public static async Task<Results<Ok<LoginResponseModel>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> GetToken(HttpContext httpContext,
            [AsParameters] AuthenticationService services,
            [Description("The username")] string username,
            [Description("The password")] string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                return TypedResults.BadRequest<ProblemDetails>(new()
                {
                    Detail = "username is not valid"
                });
            }
            if (string.IsNullOrEmpty(password))
            {
                return TypedResults.BadRequest<ProblemDetails>(new()
                {
                    Detail = "password is not valid"
                });
            }

            var user = await services.LoginService.FindByUsername(username);
            if (user == null || string.IsNullOrEmpty(user.UserName))
            {
                return TypedResults.NotFound<ProblemDetails>(new()
                {
                    Detail = "Username is wrong"
                });
            }

            var result = await services.LoginService.CheckPasswordSignIn(user, password, false);
            if (!result.Succeeded)
            {
                return TypedResults.NotFound<ProblemDetails>(new()
                {
                    Detail = "Password is wrong"
                });
            }

            var token = GenerateJWTToken(user, services.Configuration);

            var loginResponseModel = new LoginResponseModel()
            {
                UserName = user.UserName,
                UserToken = token,
            };

            return TypedResults.Ok<LoginResponseModel>(loginResponseModel);
        }
        private static string GenerateJWTToken(ApplicationUser user, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName!)
            };
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
