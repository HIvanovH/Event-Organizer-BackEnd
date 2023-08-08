using EventOganizer.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventOganizer.Context
{
    public class AplicationDBContext : DbContext
    {
        public AplicationDBContext(DbContextOptions<AplicationDBContext> options) : base(options)
        {

        }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<CartItem> CartItems { get; set; }
    }
}
