using BasketService.Api.Extensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddBasicServicesDefaults();
builder.AddApplicationsServices();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGrpcService<BasketService.Api.Grpc.BasketService>();

app.Run();
