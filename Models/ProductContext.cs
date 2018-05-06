// Models/ProductContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace sklep.Models
{
    public class ProductContext : IdentityDbContext<User>
    {
        public ProductContext (DbContextOptions<ProductContext> options)
            : base(options)
        {
        }

        public DbSet<sklep.Models.Product> Product { get; set; }
        public DbSet<sklep.Models.User> User { get; set; }
        public DbSet<sklep.Models.Cart> Cart { get; set; }
        public DbSet<sklep.Models.InCart> InCart { get; set; }
    }
}