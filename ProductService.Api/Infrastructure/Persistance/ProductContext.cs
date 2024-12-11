using Microsoft.EntityFrameworkCore;

namespace ProductService.Api.Infrastructure.Persistance
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

        public DbSet<Models.Product> Products { get; set; }
    }
}
