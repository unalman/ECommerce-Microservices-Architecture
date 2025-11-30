using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ServiceDefaults
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder) {
            var services = builder.Services;
            var configuration = builder.Configuration;

            // {
            //   "Identity": {
            //     "Url": "http://identity",
            //     "Audience": "basket"
            //    }
            // }

            var identitySection = configuration.GetSection("Identity");
            if (!identitySection.Exists())
            {
                return services;
            }

            var key = Encoding.UTF8.GetBytes(identitySection["Key"]!);

            // prevent from mapping "sub" claim to nameidentifier.
            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            services.AddAuthentication().AddJwtBearer(options => {
                //var identityUrl = identitySection.GetRequiredValue("Url");
                //var audience = identitySection.GetRequiredValue("Audience");

                //options.Authority = identityUrl;
                //options.RequireHttpsMetadata = false;
                //options.Audience = audience;
                //options.TokenValidationParameters.ValidateAudience = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = identitySection["Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddAuthorization();

            return services;
        }
    }
}
