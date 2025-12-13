using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor receives DbContext options (like connection string)
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) 
        { }

        // DbSet represents a table in the database
        public DbSet<User> Users { get; set; }

        // Optional: Customize model creation
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Store enum as string in the database
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
