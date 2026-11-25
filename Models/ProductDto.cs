using System.ComponentModel.DataAnnotations;

namespace PickNest.Models
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; } = "";
        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = "";
        [Required]
        [MaxLength(100)]

        public string Category { get; set; } = "";
        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }
        
        public string Description { get; set; }

        public IFormFile? ImageFileName { get; set; }
    }
}
