using System.ComponentModel.DataAnnotations;

namespace CatalogService.Api.Core.Domain
{
    public class CatalogType
    {
        public CatalogType(string type)
        {
            Type = type;
        }
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
