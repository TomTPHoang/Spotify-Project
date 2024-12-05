using Microsoft.EntityFrameworkCore;

namespace Spotify_Project.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UserStat> UserStats { get; set; }
    }
}
