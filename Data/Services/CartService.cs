using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Data.Services
{
    public class CartService
    {
        private readonly ECommerceStoreContext _context;

        public CartService(ECommerceStoreContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);

            if (existingCartItem == null)
            {
                var cartItem = new CartItem()
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _context.CartItems.AddAsync(cartItem);
            }
            else
            {
                existingCartItem.Quantity += quantity;
                _context.CartItems.Update(existingCartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(CartItem cartIten)
        {
            _context.CartItems.Remove(cartIten);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
        {
            return await _context.CartItems.Include(c => c.User).Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<CartItem> GetCartItemAsync(int id)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id);
            if (cartItem == null)
            {
                throw new KeyNotFoundException();
            }
            return cartItem;
        }

        public async Task<IEnumerable<CartItem>> GetValidCartItemsAsync(string userId)
        {
            var cartItems = await _context
                .CartItems.Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .Where(c => c.Product.StockQuantity >= c.Quantity)
                .ToListAsync();
            return cartItems;
        }
    }
}
