using ECommerce.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var redis = builder.AddRedis("redis");
var rabbitMq = builder.AddRabbitMQ("eventbus")
    .WithLifetime(ContainerLifetime.Persistent);
var postgres = builder.AddPostgres("postgres")
    .WithImage("ankane/pgvector")
    .WithImageTag("latest")
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogdb");
var identityDb = postgres.AddDatabase("identitydb");
var orderDb = postgres.AddDatabase("orderingdb");
var webhooksDb = postgres.AddDatabase("webhooksdb");

var launchProfileName = ShoudUseHttpForEndpoints() ? "http" : "https";

//Services

var identityApi = builder.AddProject<Projects.IdentityService_Api>("identity-api", launchProfileName)
    .WithExternalHttpEndpoints()
    .WithReference(identityDb);

var identityEndPoint = identityApi.GetEndpoint(launchProfileName);

var basketApi = builder.AddProject<Projects.BasketService_Api>("basket-api")
    .WithReference(redis)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("Identity__Url", identityEndPoint);
redis.WithParentRelationship(basketApi);

var catalogApi = builder.AddProject<Projects.CatalogService_Api>("catalog-api")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(catalogDb);

var orderingApi = builder.AddProject<Projects.OrderService_Api>("ordering-api")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(orderDb).WaitFor(orderDb)
    .WithHttpHealthCheck("/health")
    .WithEnvironment("Identity__Url", identityEndPoint);

builder.AddProject<Projects.OrderProcessor>("order-processor")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(orderDb)
    .WaitFor(orderingApi);  // wait for the orderingApi to be ready because that contains the EF migrations

var webHooksApi = builder.AddProject<Projects.WebHooks_API>("webhooks-api")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(webhooksDb)
    .WithEnvironment("Identity__Url", identityEndPoint);

//reverse proxies
builder.AddYarp("mobile-bff")
    .WithExternalHttpEndpoints()
    .ConfigureMobileBffRoutes(catalogApi, orderingApi, identityApi);

var webhooksClient = builder.AddProject<Projects.WebHookClient>("webhooksclient", launchProfileName)
    .WithReference(webHooksApi)
    .WithEnvironment("IdentityUrl", identityEndPoint);

var webApp = builder.AddProject<Projects.WebApp>("webapp", launchProfileName)
    .WithExternalHttpEndpoints()
    .WithUrls(x => x.Urls.ForEach(y => y.DisplayText = $"Online Store ({y.Endpoint?.EndpointName})"))
    .WithReference(basketApi)
    .WithReference(catalogApi)
    .WithReference(orderingApi)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithEnvironment("IdentityUrl", identityEndPoint);

// set to true if you want to use OpenAI
bool useOpenAI = false;
if (useOpenAI)
{
    builder.AddOpenAI(catalogApi, webApp, OpenAITarget.OpenAI); // set to AzureOpenAI if you want to use Azure OpenAI
}

bool useOllama = false;
if (useOllama)
{
    builder.AddOllama(catalogApi, webApp);
}

// Wire up the callback urls (self referencing)
webApp.WithEnvironment("CallBackUrl", webApp.GetEndpoint(launchProfileName));
webhooksClient.WithEnvironment("CallBackUrl", webhooksClient.GetEndpoint(launchProfileName));

// Identity has a reference to all of the apps for callback urls, this is a cyclic reference
identityApi.WithEnvironment("BasketApiClient", basketApi.GetEndpoint("http"))
           .WithEnvironment("OrderingApiClient", orderingApi.GetEndpoint("http"))
           .WithEnvironment("WebhooksApiClient", webHooksApi.GetEndpoint("http"))
           .WithEnvironment("WebhooksWebClient", webhooksClient.GetEndpoint(launchProfileName))
           .WithEnvironment("WebAppClient", webApp.GetEndpoint(launchProfileName));

builder.Build().Run();


static bool ShoudUseHttpForEndpoints()
{
    const string EnvVarName = "ECOMMERCE_USE_HTTP_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(EnvVarName);
    return int.TryParse(envValue, out int result) && result == 1;
}