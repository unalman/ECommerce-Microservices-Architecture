using CatalogService.Api.Infrastructure.Context;

namespace CatalogService.Api.Core.Domain
{
    public class CatalogServices(
            CatalogContext context
        )
    {
        public CatalogContext Context { get; set; } = context;
    }
}
