using Microsoft.EntityFrameworkCore;

namespace carRentalApp.Models
{
    // This class is used to define the application's database context
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // DbSets to represent tables in the database
        public DbSet<Car> Cars { get; set; }
        public DbSet<User> Users { get; set; }
    }
}