using System.ComponentModel.DataAnnotations;

namespace CatalogService.Api.Core.Domain
{
    public class CatalogBrand
    {
        public CatalogBrand(string brand)
        {
            Brand = brand;
        }
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }
    }
}
