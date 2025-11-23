using CatalogService.Api.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json;

namespace CatalogService.Api.Infrastructure.Context
{
    public partial class CatalogContextSeed(
        IWebHostEnvironment env) : IDbSeeder<CatalogContext>
    {
        public async Task SeedAsync(CatalogContext context)
        {
            var contentRootPath = env.ContentRootPath;

            await context.Database.OpenConnectionAsync();
            await ((NpgsqlConnection)context.Database.GetDbConnection()).ReloadTypesAsync();
            if (!context.CatalogItems.Any())
            {
                var sourcePath = Path.Combine(contentRootPath, "Infrastructure", "Setup", "catalog.json");
                var sourceJson = File.ReadAllText(sourcePath);
                var sourceItems = JsonSerializer.Deserialize<CatalogSourceEntry[]>(sourceJson) ?? Array.Empty<CatalogSourceEntry>();

                context.CatalogBrands.RemoveRange(context.CatalogBrands);
                await context.CatalogBrands.AddRangeAsync(sourceItems.Select(x => x.Brand).Distinct()
                    .Where(brandName => brandName != null)
                    .Select(brandName => new CatalogBrand(brandName!)));

                context.CatalogTypes.RemoveRange(context.CatalogTypes);
                await context.CatalogTypes.AddRangeAsync(sourceItems.Select(x => x.Type).Distinct()
                    .Where(typeName => typeName != null)
                    .Select(typeName => new CatalogType(typeName!)));

                await context.SaveChangesAsync();

                var brandIdsByName = await context.CatalogBrands.ToDictionaryAsync(x => x.Brand, x => x.Id);
                var typeIdsByName = await context.CatalogTypes.ToDictionaryAsync(x => x.Type, x => x.Id);

                var catalogItems = sourceItems
                    .Where(source => source.Name != null && source.Brand != null && source.Type != null)
                    .Select(source => new CatalogItem(source.Name)
                    {
                        Id = source.Id,
                        Description = source.Description,
                        Price = source.Price,
                        PictureFileName = $"{source.Id}.webp",
                        CatalogBrandId = brandIdsByName[source.Brand!],
                        CatalogTypeId = typeIdsByName[source.Type!]
                    }).ToArray();
                await context.CatalogItems.AddRangeAsync(catalogItems);
                await context.SaveChangesAsync();

            }
        }
        private class CatalogSourceEntry
        {
            public int Id { get; set; }
            public string? Type { get; set; }
            public string? Brand { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public decimal Price { get; set; }
        }
    }
}
