using BasketService.Api.Core.Application.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServiceDefaults;
using System.Text;

namespace BasketService.Api.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationsServices(this IHostApplicationBuilder builder)
        {

            //var jwtSetting = builder.Configuration.GetSection("Identity");
            //var key = Encoding.UTF8.GetBytes(jwtSetting["Key"]!);

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = false,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,

            //        ValidIssuer = jwtSetting["Issuer"],
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ClockSkew = TimeSpan.Zero
            //    };
            //});

            builder.AddDefaultAuthentication();

            builder.Services.AddSingleton<IBasketRepository, RedisBasketRepository>();
        }
    }
}
