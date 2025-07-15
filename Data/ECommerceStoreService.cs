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

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
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
            existentProduct.ImageUrl = product.ImageUrl;
            existentProduct.StockQuantity = product.StockQuantity;
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
    }
}
