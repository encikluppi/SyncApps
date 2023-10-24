using Microsoft.EntityFrameworkCore;
using SyncApps.Models;

namespace SyncApps.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options) 
        {
            
        }

        public DbSet<Platform> Platform { get; set; }
        public DbSet<Well> Well { get; set; }
    }
}
