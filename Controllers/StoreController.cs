using ECommerceStore.Data;
using ECommerceStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerceStore.Controllers
{
    public class StoreController : Controller
    {
        private ECommerceStoreService _eCommerceStoreService;

        public StoreController(ECommerceStoreService eCommerceStoreService)
        {
            _eCommerceStoreService = eCommerceStoreService;
        }

        public async Task<IActionResult> Index(string? name, double? minPrice, double? maxPrice, int? categoryId)
        {
            var model = new ProductFilterViewModel
            {
                Name = name,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CategoryId = categoryId,
                Categories = await _eCommerceStoreService.GetCategoriesAsync(),
                Products = await _eCommerceStoreService.GetProductsAsync(name, minPrice, maxPrice, categoryId)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await _eCommerceStoreService.GetProductAsync(id));
        }


    }
}
