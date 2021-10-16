using Microsoft.EntityFrameworkCore;

namespace BUPTReportOnline.Models
{
    public class BROContext : DbContext
    {
        public BROContext(DbContextOptions<BROContext> options) : base(options)
        {
        }
        public BROContext()
        {

        }
        public DbSet<User> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                IsAdmin = true,
                GUID = "REMOVE_INIT",
                Registered = true
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
