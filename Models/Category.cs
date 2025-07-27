using System.ComponentModel.DataAnnotations;

namespace ECommerceStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(30)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(250)]
        public string? Description { get; set; }
    }
}
