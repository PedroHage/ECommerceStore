using Microsoft.EntityFrameworkCore;
using ECommerceStore.Models;

namespace ECommerceStore.Data.Services
{
    public class ProductsService
    {
        private readonly ECommerceStoreContext _context;

        public ProductsService(ECommerceStoreContext context)
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
            if (!await _context.Products.AnyAsync(p => p.Id == product.Id))
                throw new KeyNotFoundException();

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            var purchaseItems = await _context.PurchaseItems.Where(pi => pi.ProductId == product.Id).ToListAsync();
            foreach (var item in purchaseItems)
            {
                item.ProductId = null;
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
    }
}
