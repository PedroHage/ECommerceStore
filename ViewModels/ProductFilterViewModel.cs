using ECommerceStore.Models;

namespace ECommerceStore.ViewModels
{
    public class ProductFilterViewModel
    {
        public string? Name { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? CategoryId { get; set; }

        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
    }
}
