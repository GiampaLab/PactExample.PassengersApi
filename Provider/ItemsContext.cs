using Microsoft.EntityFrameworkCore;

namespace Provider
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {}

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {}

        public DbSet<Passenger> Passengers { get; set; }
    }
}