using ECommerceStore.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerceStore.ViewModels
{
    public class EditProductViewModel
    {
        [Required]
        public Product Product { get; set; } = null!;
        public IEnumerable<Category>? Categories { get; set; } = null!;
        public IFormFile? ImageFile { get; set; } = null;
    }
}
