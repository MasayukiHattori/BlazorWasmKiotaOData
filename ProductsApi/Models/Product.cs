using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Range(0, 1000000)]
        public decimal Price { get; set; }
    }
}
