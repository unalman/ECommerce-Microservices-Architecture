using EventBus.Base.Abstraction;
using OrderProcessor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();

var app = builder.Build();


app.Run();
