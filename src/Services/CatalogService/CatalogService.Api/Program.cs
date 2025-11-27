using CatalogService.Api.Apis;
using CatalogService.Api.Extensions;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationService();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
var withApiVersioning = builder.Services.AddApiVersioning();
builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

app.UseStatusCodePages();

app.MapCatalogApi();

app.UseDefaultOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
