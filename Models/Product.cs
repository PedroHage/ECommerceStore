using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommerceStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required, Range(0, double.MaxValue), DisplayFormat(DataFormatString = "{0:C}")]
        public double Price { get; set; }
        [Required, Range(0, int.MaxValue), DisplayName("Stock quantity")]
        public int StockQuantity { get; set; }
        [Required, DisplayName("Image Url")]
        public string ImageUrl { get; set; } = string.Empty;
        [Required, DisplayName("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
