using System.ComponentModel.DataAnnotations;

namespace PickNest.Models
{
    public class Products
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
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

        [Required]
        public string ImageFileName { get; set; } = "";
        
        public DateTime CreatedAt { get; set; }
    }
}
