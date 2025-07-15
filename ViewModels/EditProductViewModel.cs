using ECommerceStore.Models;

namespace ECommerceStore.ViewModels
{
    public class EditProductViewModel
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<Category> Categories { get; set; } = null!;
    }
}
