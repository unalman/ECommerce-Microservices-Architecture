using ECommerce.BasketService.Api.Grpc;
using EventBus.Base.Abstraction;
using EventBus.Base.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.AI;
using Microsoft.IdentityModel.JsonWebTokens;
using ServiceDefaults;
using WebApp.Services;
using WebApp.Services.OrderStatus;
using WebApp.Services.OrderStatus.EventHandling;
using WebApp.Services.OrderStatus.Events;
using WebAppComponents.Services;

namespace WebApp.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationService(this IHostApplicationBuilder builder)
        {
            builder.AddAuthenticationServices();

            builder.AddRabbitMqEventBus("EventBus")
                .AddEventBusSubscriptions();

            builder.Services.AddHttpForwarderWithServiceDiscovery();

            builder.Services.AddScoped<BasketState>();
            builder.Services.AddScoped<LogOutService>();
            builder.Services.AddSingleton<BasketService>();
            builder.Services.AddSingleton<OrderStatusNotificationService>();
            builder.Services.AddSingleton<IProductImageUrlProvider, ProductImageUrlProvider>();
            builder.AddAIServices();

            // HTTP and GRPC client registrations
            builder.Services.AddGrpcClient<Basket.BasketClient>(x => x.Address = new("http://basket-api"))
                .AddAuthToken();

            builder.Services.AddHttpClient<CatalogService>(o => o.BaseAddress = new("https+http://catalog-api"))
                .AddApiVersion(2.0)
                .AddAuthToken();

            builder.Services.AddHttpClient<OrderingService>(x => x.BaseAddress = new("https+http://ordering-api"))
                .AddApiVersion(1.0)
                .AddAuthToken();
        }
        public static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
        {
            eventBus.AddSubscription<OrderStatusChangedToAwaitingValidationIntegrationEvent, OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
            eventBus.AddSubscription<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
            eventBus.AddSubscription<OrderStatusChangedToStockConfirmedIntegrationEvent, OrderStatusChangedToStockConfirmedIntegrationEventHandler>();
            eventBus.AddSubscription<OrderStatusChangedToShippedIntegrationEvent, OrderStatusChangedToShippedIntegrationEventHandler>();
            eventBus.AddSubscription<OrderStatusChangedToCancelledIntegrationEvent, OrderStatusChangedToCancelledIntegrationEventHandler>();
            eventBus.AddSubscription<OrderStatusChangedToSubmittedIntegrationEvent, OrderStatusChangedToSubmittedIntegrationEventHandler>();
        }
        public static void AddAuthenticationServices(this IHostApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            var services = builder.Services;

            JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            var identityUrl = configuration.GetRequiredValue("IdentityUrl");
            var callBackUrl = configuration.GetRequiredValue("CallBackUrl");
            var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

            //Add authentication services
            services.AddAuthorization();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = identityUrl;
                options.SignedOutRedirectUri = callBackUrl;
                options.ClientId = "webapp";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("orders");
                options.Scope.Add("basket");
            });

            // blazor auth service
            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddCascadingAuthenticationState();
        }
        public static void AddAIServices(this IHostApplicationBuilder builder)
        {
            ChatClientBuilder chatClientBuilder = null;
            if (builder.Configuration["OllamaEnabled"] is string ollamaEnabled && bool.Parse(ollamaEnabled))
            {
                chatClientBuilder = builder.AddOllamaApiClient("chat")
                    .AddChatClient();
            }
            else if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("chatModel")))
            {
                chatClientBuilder = builder.AddOpenAIClientFromConfiguration("chatModel")
                    .AddChatClient();
            }
            chatClientBuilder?.UseFunctionInvocation();
        }

        public static async Task<string?> GetBuyerIdAsync(this AuthenticationStateProvider authenticationStateProvider)
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.FindFirst("sub")?.Value;
        }

        public static async Task<string?> GetUserNameAsync(this AuthenticationStateProvider authenticationStateProvider)
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            return user.FindFirst("name")?.Value;
        }
    }
}
