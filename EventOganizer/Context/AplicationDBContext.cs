using EventOganizer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventOganizer.Context
{
    public class AplicationDBContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AplicationDBContext(DbContextOptions<AplicationDBContext> options) : base(options)
        {

        }

        // Define your DbSet<T> properties here
        public DbSet<Ticket> Tickets { get; set; }

        // ... other DbSet properties ...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add any additional model configuration here if needed
        }
    }
}
