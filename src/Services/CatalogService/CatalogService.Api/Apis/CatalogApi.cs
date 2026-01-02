using CatalogService.Api.Core.Application;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using CatalogService.Api.IntegrationEvents.Events;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace CatalogService.Api.Apis
{
    public static class CatalogApi
    {
        public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
        {
            var vApi = app.NewVersionedApi("Catalog");
            var api = vApi.MapGroup("api/catalog");
            api.MapGet("/items", GetAllItems)
                .WithName("ListItems")
                .WithSummary("List catalog items")
                .WithDescription("Get a paginated list of items in the catalog.")
                .WithTags("Items");
            app.MapGet("/items/{id:int}", GetItemById)
                .WithName("GetItem")
                .WithSummary("Get catalog item")
                .WithDescription("Get an item from the catalog.")
                .WithTags("Items");
            app.MapGet("/item/{id:int}/pic", GetItemPictureById)
                .WithName("GetItemPicture")
                .WithSummary("Get catalog item picture")
                .WithDescription("Get the picture for a catalog item.")
                .WithTags("Items");

            app.MapGet("/items/type/{typeId}/brand/{brandId?}", GetItemsByBrandAndTypeId)
                .WithName("GetItemsByBrandAndTypeId")
                .WithSummary("Get catalog items by type and brand")
                .WithDescription("Get catalog items of the specified type and brand")
                .WithTags("Types");
            app.MapGet("/items/type/all/brand/{brandId:int?}", GetItemsByBrand)
                .WithName("GetItemsByBrand")
                .WithSummary("Get catalog items by brand")
                .WithDescription("Get catalog items of the specified brand")
                .WithTags("Brands");
            app.MapGet("/catalogtypes", [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
            async (CatalogContext context) => await context.CatalogTypes.OrderBy(x => x.Type).ToListAsync())
                .WithName("ListItemTypes")
                .WithSummary("List catalog item types")
                .WithDescription("Get a list of the types of catalog items")
                .WithTags("Types");
            app.MapGet("/catalogbrands", [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problems+json")]
            async (CatalogContext context) => await context.CatalogBrands.OrderBy(x => x.Brand).ToListAsync())
                .WithName("listItemBrands")
                .WithSummary("List catalog item brands")
                .WithDescription("Get a list of the types of catalog items")
                .WithTags("Brands");

            app.MapPut("/items/{id:int}", UpdateItem)
                .WithName("UpdateItem")
                .WithSummary("Create or replace a catalog item")
                .WithDescription("Create or replace a catalog item")
                .WithTags("Items");
            app.MapPost("/items", CreateItem)
                .WithName("CreateItem")
                .WithSummary("Create a catalog item")
                .WithDescription("Create a new item in the catalog");
            app.MapDelete("/items/{id:int}", DeleteItemById)
                .WithName("DeleteItem")
                .WithSummary("Delete catalog item")
                .WithDescription("Delete the specified catalog item");
            return app;
        }
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
        public static async Task<Ok<PaginatedItems<CatalogItem>>> GetAllItems(
                [AsParameters] PaginationRequest paginationRequest,
                [AsParameters] CatalogServices services,
                [Description("The name of the items to return")] string? name,
                [Description("The type of the items to return")] int? type,
                [Description("The brand of the items to return")] int? brand)
        {
            var pageSize = paginationRequest.PageSize;
            var pageIndex = paginationRequest.PageIndex;

            var root = (IQueryable<CatalogItem>)services.Context.CatalogItems;
            if (name is not null)
                root = root.Where(x => x.Name.StartsWith(name));
            if (type is not null)
                root = root.Where(x => x.CatalogTypeId == type);
            if (brand is not null)
                root = root.Where(x => x.CatalogBrandId == brand);

            var totalItems = await root.LongCountAsync();
            var itemsOnPage = await root
                .OrderBy(x => x.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
            return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
        }
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
        public static async Task<Results<Ok<CatalogItem>, NotFound, BadRequest<ProblemDetails>>> GetItemById(
                HttpContext httpContext,
                [AsParameters] CatalogServices services,
                [Description("The catalog item id")] int id)
        {
            if (id <= 0)
            {
                return TypedResults.BadRequest<ProblemDetails>(new()
                {
                    Detail = "Id is not valid"
                });
            }
            var item = await services.Context.CatalogItems.Include(x => x.CatalogBrand).SingleOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(item);
        }
        [ProducesResponseType<byte[]>(StatusCodes.Status200OK, "application/octet-stream",
            [ "image/png", "image/gif", "image/jpeg", "image/bmp", "image/tiff",
          "image/wmf", "image/jp2", "image/svg+xml", "image/webp" ])]
        public static async Task<Results<PhysicalFileHttpResult, NotFound>> GetItemPictureById(CatalogContext context,
            IWebHostEnvironment environment,
            [Description("The catalog item id")] int id)
        {
            var item = await context.CatalogItems.FindAsync(id);
            if (item is null || item.PictureFileName is null) return TypedResults.NotFound();

            var path = GetFullPath(environment.ContentRootPath, item.PictureFileName);

            string imageFileExtension = Path.GetExtension(path);
            string mimetype = GetImageMimeTypeFromImageFileExtension(imageFileExtension);
            DateTime lastModified = File.GetLastWriteTimeUtc(path);

            return TypedResults.PhysicalFile(path, mimetype, lastModified: lastModified);
        }
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
        public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrandAndTypeId(
            [AsParameters] PaginationRequest paginationRequest,
            [AsParameters] CatalogServices services,
            [Description("The typeId of items to return")] int typeId,
            [Description("The brand of items to return")] int? brandId)
        {
            return await GetAllItems(paginationRequest, services, null, typeId, brandId);
        }
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
        public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByBrand(
            [AsParameters] PaginationRequest paginationRequest,
            [AsParameters] CatalogServices services,
            [Description("The brand of items to return")] int brandId)
        {
            return await GetAllItems(paginationRequest, services, null, null, brandId);
        }
        public static async Task<Results<Created, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> UpdateItem(
            HttpContext httpContext,
            [Description("The id of the catalog item")] int id,
            [AsParameters] CatalogServices services,
            CatalogItem productToUpdate)
        {
            var catalogItem = await services.Context.CatalogItems.SingleOrDefaultAsync(x => x.Id == id);
            if (catalogItem == null)
                return TypedResults.NotFound<ProblemDetails>(new() { Detail = $"Item with id {id} not found" });

            var catalogEntry = services.Context.Entry(catalogItem);
            catalogEntry.CurrentValues.SetValues(productToUpdate);

            var priceEntry = catalogEntry.Property(x => x.Price);
            if (priceEntry.IsModified)  // Save product's data and publish integration event through the Event Bus if price has changed
            {
                var priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, priceEntry.OriginalValue);

                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                await services.EventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

                // Publish through the Event Bus and mark the saved event as published
                await services.EventService.PublishThroughEventBusAsync(priceChangedEvent);
            }
            else// Just save the updated product because the Product's Price hasn't changed.
            {
                await services.Context.SaveChangesAsync();
            }
            return TypedResults.Created($"/api/catalog/items/{id}");
        }
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
        public static async Task<Created> CreateItem(
            [AsParameters] CatalogServices services,
            CatalogItem product)
        {
            var item = new CatalogItem(product.Name)
            {
                Id = product.Id,
                CatalogBrandId = product.CatalogBrandId,
                CatalogTypeId = product.CatalogTypeId,
                Description = product.Description,
                PictureFileName = product.PictureFileName,
                Price = product.Price,
            };
            services.Context.Add(item);
            await services.Context.SaveChangesAsync();
            return TypedResults.Created($"/api/catalog/items/{item.Id}");
        }
        public static async Task<Results<NoContent, NotFound>> DeleteItemById(
             [AsParameters] CatalogServices service,
             [Description("The id of the catalog item to delete")] int id)
        {
            var item = service.Context.CatalogItems.SingleOrDefault(x => x.Id == id);
            if (item == null) return TypedResults.NotFound();

            service.Context.CatalogItems.Remove(item);
            await service.Context.SaveChangesAsync();
            return TypedResults.NoContent();
        }



        private static string GetFullPath(string contentRootPath, string pictureFileName) => Path.Combine(contentRootPath, "Pics", pictureFileName);
        private static string GetImageMimeTypeFromImageFileExtension(string extension) => extension switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".bmp" => "image/bmp",
            ".tiff" => "image/tiff",
            ".wmf" => "image/wmf",
            ".jp2" => "image/jp2",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            _ => "application/octet-stream",
        };
    }
}
