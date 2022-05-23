using Microsoft.EntityFrameworkCore;

namespace Pfps.API.Data
{
    public class PfpsContext : DbContext
    {
        public PfpsContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}