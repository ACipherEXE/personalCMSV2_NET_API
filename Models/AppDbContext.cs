using Microsoft.EntityFrameworkCore;

namespace personalCMSV2_NET_API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ContentModel> ContentModel { get; set; }
    }
}
