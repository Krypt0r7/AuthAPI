using JWTAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTAPI.Helpers
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}