using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Data
{
    public class ECommerceStoreContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public ECommerceStoreContext(DbContextOptions<ECommerceStoreContext> options): base(options) { }
    }
}
