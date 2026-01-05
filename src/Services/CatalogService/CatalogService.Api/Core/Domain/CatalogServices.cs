using CatalogService.Api.Infrastructure.Context;
using CatalogService.Api.IntegrationEvents;
using CatalogService.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CatalogService.Api.Core.Domain
{
    public class CatalogServices(
            CatalogContext context,
            ICatalogAI catalogAI,
            IOptions<CatalogOptions> options,
            ILogger<CatalogServices> logger,
            [FromServices] ICatalogIntegrationEventService eventService)
    {
        public CatalogContext Context { get; set; } = context;
        public ICatalogAI CatalogAI { get; set; } = catalogAI;
        public IOptions<CatalogOptions> Options { get; set; } = options;
        public ILogger<CatalogServices> Logger { get; set; } = logger;
        public ICatalogIntegrationEventService EventService { get; set; } = eventService;
    }
}
