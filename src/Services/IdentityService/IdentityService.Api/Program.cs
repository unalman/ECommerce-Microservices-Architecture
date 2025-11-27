using IdentityService.Api.Apis;
using IdentityService.Api.Data;
using IdentityService.Api.Extensions;
using IdentityService.Api.Models;
using IdentityService.Api.Services;
using Microsoft.AspNetCore.Identity;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddApplicationService();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//var jwtSetting = builder.Configuration.GetSection("Jwt");
//var key = Encoding.UTF8.GetBytes(jwtSetting["Key"]);

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
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,

//        ValidIssuer = jwtSetting["Issuer"],
//        ValidAudience = jwtSetting["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ClockSkew = TimeSpan.Zero
//    };
//});
//builder.Services.AddAuthorization();

builder.Services.AddTransient<ILoginService<ApplicationUser>, EFLoginService>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

app.UseStatusCodePages();

app.MapAuthenticationApi();

app.UseDefaultOpenApi();

app.UseHttpsRedirection();

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
