using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Ordering.FunctionalTests
{
    public sealed class OrderingApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly IHost _app;

        public IResourceBuilder<PostgresServerResource> Postgres { get; private set; }
        public IResourceBuilder<PostgresServerResource> IdentityDB { get; private set; }
        public IResourceBuilder<ProjectResource> IdentityApi { get; private set; }

        private string _postgresConnectionString;

        public OrderingApiFixture()
        {
            var options = new DistributedApplicationOptions { AssemblyName = typeof(OrderingApiFixture).Assembly.FullName, DisableDashboard = true };
            var appBuilder = DistributedApplication.CreateBuilder(options);
            Postgres = appBuilder.AddPostgres("OrderingDB");
            IdentityDB = appBuilder.AddPostgres("IdentityDB");
            IdentityApi = appBuilder.AddProject<Projects.IdentityService_Api>("identity-api").WithReference(IdentityDB);
            _app = appBuilder.Build();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string> {
                    { $"ConnectionStrings:{Postgres.Resource.Name}", _postgresConnectionString },
                });
            });
            return base.CreateHost(builder);
        }

        public new async Task DisposeAsync()
        {
            await base.DisposeAsync();
            await _app.StopAsync();
            if (_app is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                _app.Dispose();
            }
        }

        public async ValueTask InitializeAsync()
        {
            await _app.StartAsync();
            _postgresConnectionString = await Postgres.Resource.GetConnectionStringAsync();
        }
    }
}
