using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Data.Services
{
    public class PurchaseItemService
    {
        private readonly ECommerceStoreContext _context;

        public PurchaseItemService(ECommerceStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseItem>> GetPurchaseItemsAsync(string userId)
        {
            return await _context.PurchaseItems.Include(c => c.User).Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<PurchaseItem> GetPurchaseItemAsync(int id)
        {
            var purchaseItem = await _context.PurchaseItems.Include(p => p.User).Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == id);
            if (purchaseItem == null)
            {
                throw new KeyNotFoundException();
            }
            return purchaseItem;
        }
    }
}
