using Microsoft.EntityFrameworkCore;

namespace personalCMSV2_NET_API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ContentModel> ContentModel { get; set; }
        public DbSet<ContentEntry> ContentEntry { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ContentEntry>()
                .Property(e => e.Id)
                .HasColumnType("id")
                .HasConversion<Guid>();
        }
    }
}
