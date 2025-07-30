using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ECommerceStore.Models
{
    public class PurchaseItem
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IdentityUser User { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ProductPrice { get; set; }
        public int Quantity { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageMimeType { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    }
}
