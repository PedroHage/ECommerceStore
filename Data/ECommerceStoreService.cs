using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Data
{
    public class ECommerceStoreService
    {
        private readonly ECommerceStoreContext _context;

        public ECommerceStoreService(ECommerceStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(string? name, double? minPrice, double? maxPrice, int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name.Trim()));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            return await query.ToListAsync();
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new KeyNotFoundException();
            }
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            // Look, this is not efficient, it was just a way to fix a bug. NEED IMPROVEMENT
            var existentProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existentProduct == null)
                throw new KeyNotFoundException();
            
            existentProduct.Name = product.Name;
            existentProduct.Description = product.Description;
            existentProduct.Price = product.Price;
            existentProduct.Category = product.Category;
            existentProduct.CategoryId = product.CategoryId;
            existentProduct.StockQuantity = product.StockQuantity;
            if (product.ImageData != null)
            {
                existentProduct.ImageData = product.ImageData;
                existentProduct.ImageMimeType = product.ImageMimeType;
            }
            _context.Products.Update(existentProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // Categories
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                throw new KeyNotFoundException();
            }
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
    
        //Cart
        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(c => (c.ProductId == productId && c.UserId == userId));
            
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
    }
}