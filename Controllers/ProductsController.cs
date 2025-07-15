using ECommerceStore.Data;
using ECommerceStore.Models;
using ECommerceStore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ECommerceStoreService _eCommerceStoreService;
        
        public ProductsController(ECommerceStoreService eCommerceStoreService)
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
            var product = await _eCommerceStoreService.GetProductAsync(id);
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ViewModel = new EditProductViewModel();
            ViewModel.Categories = await _eCommerceStoreService.GetCategoriesAsync();
            ViewModel.Product = await _eCommerceStoreService.GetProductAsync(id);
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,StockQuantity,CategoryId,Description,ImageUrl")] Product product)
        {
            Console.WriteLine(product.Id);
            if (id != product.Id)
                return NotFound();

            await _eCommerceStoreService.UpdateProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _eCommerceStoreService.GetProductAsync(id);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _eCommerceStoreService.GetProductAsync(id);
            await _eCommerceStoreService.DeleteProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            var ViewModel = new EditProductViewModel();
            ViewModel.Categories = await _eCommerceStoreService.GetCategoriesAsync();
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            await _eCommerceStoreService.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }
    }
}
