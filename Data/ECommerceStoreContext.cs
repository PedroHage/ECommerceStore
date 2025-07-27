using ECommerceStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ECommerceStore.Data
{
    public class ECommerceStoreContext : IdentityDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public ECommerceStoreContext(DbContextOptions<ECommerceStoreContext> options): base(options) { }
    }
}
