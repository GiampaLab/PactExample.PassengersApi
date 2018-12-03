using Microsoft.EntityFrameworkCore;

namespace PassengersApi
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