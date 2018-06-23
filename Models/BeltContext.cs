using Microsoft.EntityFrameworkCore;
 
namespace BeltExam.Models
{
    public class BeltContext : DbContext
    {
        public BeltContext(DbContextOptions<BeltContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }
        public DbSet<Plan> Plans { get; set; }
    }
}
