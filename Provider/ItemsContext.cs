using Microsoft.EntityFrameworkCore;

namespace Provider
{
    public class ItemsContext : DbContext
    {
        public ItemsContext()
        {}

        public ItemsContext(DbContextOptions<ItemsContext> options)
        : base(options)
        {}

        public DbSet<Item> Items { get; set; }
    }
}