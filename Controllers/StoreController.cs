using ECommerceStore.Data;
using ECommerceStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceStore.Controllers
{
    [Authorize(Roles = "User")]
    public class StoreController : Controller
    {
        private ECommerceStoreService _eCommerceStoreService;

        public StoreController(ECommerceStoreService eCommerceStoreService)
        {
            _eCommerceStoreService = eCommerceStoreService;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            return View(await _eCommerceStoreService.GetProductAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _eCommerceStoreService.AddToCartAsync(userId, productId, quantity);
            return RedirectToAction(nameof(Cart));
        }

        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _eCommerceStoreService.GetCartItemsAsync(userId);
            return View(cartItems);
        }

        public async Task<IActionResult> RemoveCartItem(int id)
        {
            var cartItem = await _eCommerceStoreService.GetCartItemAsync(id);
            await _eCommerceStoreService.RemoveCartItemAsync(cartItem);
            return RedirectToAction(nameof(Cart), new { cartItem.UserId });
        }

        public async Task<IActionResult> PurchaseHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var purchaseItems = await _eCommerceStoreService.GetPurchaseItemsAsync(userId);
            return View(purchaseItems);
        }


        [AllowAnonymous]
        public async Task<IActionResult> GetImage(int id)
        {
            var purchaseItem = await _eCommerceStoreService.GetPurchaseItemAsync(id);

            if (purchaseItem == null || purchaseItem.ImageData == null)
                return NotFound();

            return File(purchaseItem.ImageData, purchaseItem.ImageMimeType!);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseCartItems()
        {
            await _eCommerceStoreService.PurchaseCartItemsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return RedirectToAction("Cart");
        }
    }
}
