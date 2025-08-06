using ECommerceStore.Data.Services;
using ECommerceStore.Models;
using ECommerceStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace ECommerceStore.Controllers
{
    [Authorize(Roles = "User")]
    public class StoreController : Controller
    {
        private ProductsService _productService;
        private CategoryService _categoriesService;
        private CartService _cartService;
        private PurchaseItemService _purchaseItemService;
        private PurchaseService _purchaseService;

        public StoreController(ProductsService productService, CategoryService categoriesService, CartService cartService, PurchaseItemService purchaseItemService, PurchaseService purchaseService)
        {
            _productService = productService;
            _categoriesService = categoriesService;
            _cartService = cartService;
            _purchaseItemService = purchaseItemService;
            _purchaseService = purchaseService;
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
                Categories = await _categoriesService.GetCategoriesAsync(),
                Products = await _productService.GetProductsAsync(name, minPrice, maxPrice, categoryId)
            };

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            return View(await _productService.GetProductAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddToCartAsync(userId, productId, quantity);
            return RedirectToAction(nameof(Cart));
        }

        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            return View(cartItems);
        }

        public async Task<IActionResult> RemoveCartItem(int id)
        {
            var cartItem = await _cartService.GetCartItemAsync(id);
            await _cartService.RemoveCartItemAsync(cartItem);
            return RedirectToAction(nameof(Cart), new { cartItem.UserId });
        }

        public async Task<IActionResult> PurchaseHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var purchaseItems = await _purchaseItemService.GetPurchaseItemsAsync(userId);
            return View(purchaseItems);
        }


        [AllowAnonymous]
        public async Task<IActionResult> GetImage(int id)
        {
            var purchaseItem = await _purchaseItemService.GetPurchaseItemAsync(id);

            if (purchaseItem == null || purchaseItem.ImageData == null)
                return NotFound();

            return File(purchaseItem.ImageData, purchaseItem.ImageMimeType!);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetValidCartItemsAsync(userId);
            var lineItems = cartItems.Select(ci => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    UnitAmount = (long)(ci.Product.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = ci.Product.Name
                    }
                },
                Quantity = ci.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/store/paymentSuccess?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = Url.Action("PaymentCancel", "Store", null, Request.Scheme)
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }

        public async Task<IActionResult> PaymentSuccess(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
                return Forbid();

            var service = new SessionService();
            var session = await service.GetAsync(session_id);

            if (session.PaymentStatus != "paid")
                return Forbid();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetValidCartItemsAsync(userId);
            await _purchaseService.PurchaseCartItemsAsync(userId);
            return View(cartItems);
        }

        public IActionResult PaymentCancel()
        {
            return View();
        }
    }
}
