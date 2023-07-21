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
        public DbSet<Account> Accounts { get; set; }
    }
}
