using CatalogService.Api.Infrastructure.Context;
using CatalogService.Api.IntegrationEvents;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Core.Domain
{
    public class CatalogServices(
            CatalogContext context,
            [FromServices] ICatalogIntegrationEventService eventService)
    {
        public CatalogContext Context { get; set; } = context;
        public ICatalogIntegrationEventService EventService { get; set; } = eventService;
    }
}
