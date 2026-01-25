using OrderProcessor.Extensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddBasicServicesDefaults();
builder.AddApplicationServices();

var app = builder.Build();

app.MapDefaultEndpoints();

await app.RunAsync();
