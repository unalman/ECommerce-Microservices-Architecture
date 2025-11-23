using System.ComponentModel.DataAnnotations;

namespace CatalogService.Api.Core.Domain
{
    public class CatalogItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? PictureFileName { get; set; }

        public string? PictureUri { get; set; }

        public int CatalogTypeId { get; set; }

        public CatalogType? CatalogType { get; set; }

        public int CatalogBrandId { get; set; }

        public CatalogBrand? CatalogBrand { get; set; }

        public CatalogItem(string name)
        {
            Name = name;
        }
    }
}
