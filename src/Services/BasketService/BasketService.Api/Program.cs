using BasketService.Api.Extensions;
using BasketService.Api.Grpc;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using RabbitMQ.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddApplicationsServices();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => sp.ConfigureRedis(builder.Configuration));

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService",
        EventBusType = EventBusType.RabbitMQ,
        Connection = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "unal",
            Password = "unal"
        }
    };
    return EventBusFactory.Create(config, sp);
});

builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();

builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP (REST ya da baþka kullaným varsa) için port — isteðe baðlý
    options.ListenAnyIP(5200, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;  // HTTP/1
    });

    // gRPC / HTTP2 için ayrý port
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps(); // Geliþtirme ortamý için dev sertifika kullanýlýr
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();
await eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapGrpcService<BasketService.Api.Grpc.BasketService>();

app.Run();
