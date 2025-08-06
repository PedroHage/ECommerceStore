using ECommerceStore.Data.Services;
using ECommerceStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ProductsService _productService;
        private readonly CategoryService _categoriesService;
        
        public ProductsController(ProductsService productsService, CategoryService categoriesService)
        {
            _productService = productsService;
            _categoriesService = categoriesService;
        }

        public async Task<IActionResult> Index(string? name, double? minPrice, double? maxPrice, int? categoryId)
        {
            var model = new ProductFilterViewModel
            {
                Name = name,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CategoryId = categoryId,
                Categories = await _categoriesService.GetCategoriesAsync(),
                Products = await _productService.GetProductsAsync(name, minPrice, maxPrice, categoryId)
            };
            
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductAsync(id);
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ViewModel = new EditProductViewModel
            {
                Product = await _productService.GetProductAsync(id),
                Categories = await _categoriesService.GetCategoriesAsync()
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

            await _productService.UpdateProductAsync(model.Product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductAsync(id);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductAsync(id);
            await _productService.DeleteProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Create()
        {
            var ViewModel = new EditProductViewModel
            {
                Product = new Models.Product(),
                Categories = await _categoriesService.GetCategoriesAsync()
            };
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoriesService.GetCategoriesAsync();
                return View(model);
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await model.ImageFile.CopyToAsync(memoryStream);

                model.Product.ImageData = memoryStream.ToArray();
                model.Product.ImageMimeType = model.ImageFile.ContentType;
            }

            await _productService.AddProductAsync(model.Product);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetImage(int id)
        {
            var product = await _productService.GetProductAsync(id);

            if (product == null || product.ImageData == null)
                return NotFound();

            return File(product.ImageData, product.ImageMimeType!);
        }
    }
}
