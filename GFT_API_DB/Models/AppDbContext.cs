using Microsoft.EntityFrameworkCore;

namespace GFT_API_DB.Models
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Order> Order { get; set; }
    }
}
