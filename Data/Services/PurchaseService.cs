using ECommerceStore.Models;

namespace ECommerceStore.Data.Services
{
    public class PurchaseService
    {
        private readonly ECommerceStoreContext _context;
        private readonly CartService _cartService;
        private readonly ProductsService _productService;

        public PurchaseService(ECommerceStoreContext context, CartService cartService, ProductsService productService)
        {
            _context = context;
            _cartService = cartService;
            _productService = productService;
        }

        public async Task PurchaseCartItemsAsync(string userId)
        {
            var cartItems = await _cartService.GetValidCartItemsAsync(userId);

            foreach (var cartItem in cartItems)
            {
                var purchaseItem = new PurchaseItem()
                {
                    UserId = userId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    ProductPrice = cartItem.Product.Price,
                    Name = cartItem.Product.Name,
                    Description = cartItem.Product.Description,
                    ImageData = cartItem.Product.ImageData,
                    ImageMimeType = cartItem.Product.ImageMimeType
                };
                await _context.PurchaseItems.AddAsync(purchaseItem);
                var product = await _productService.GetProductAsync(cartItem.ProductId);
                product.StockQuantity -= cartItem.Quantity;
            }
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}
