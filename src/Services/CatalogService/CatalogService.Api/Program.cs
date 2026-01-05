using CatalogService.Api.Apis;
using CatalogService.Api.Extensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationService();
builder.Services.AddProblemDetails();

var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseStatusCodePages();

app.MapCatalogApi();

app.UseDefaultOpenApi();

app.Run();
