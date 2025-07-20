using ECommerceStore.Data;
using ECommerceStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceStore.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var ViewModel = new EditProductViewModel
            {
                Product = await _eCommerceStoreService.GetProductAsync(id),
                Categories = await _eCommerceStoreService.GetCategoriesAsync()
            };
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductViewModel model)
        {
            if (id != model.Product.Id)
                return NotFound();

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await model.ImageFile.CopyToAsync(memoryStream);

                model.Product.ImageData = memoryStream.ToArray();
                model.Product.ImageMimeType = model.ImageFile.ContentType;
            }

            await _eCommerceStoreService.UpdateProductAsync(model.Product);
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
            var ViewModel = new EditProductViewModel
            {
                Product = new Models.Product(),
                Categories = await _eCommerceStoreService.GetCategoriesAsync()
            };
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _eCommerceStoreService.GetCategoriesAsync();
                return View(model);
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await model.ImageFile.CopyToAsync(memoryStream);

                model.Product.ImageData = memoryStream.ToArray();
                model.Product.ImageMimeType = model.ImageFile.ContentType;
            }

            await _eCommerceStoreService.AddProductAsync(model.Product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetImage(int id)
        {
            var product = await _eCommerceStoreService.GetProductAsync(id);

            if (product == null || product.ImageData == null)
                return NotFound();

            return File(product.ImageData, product.ImageMimeType!);
        }
    }
}
