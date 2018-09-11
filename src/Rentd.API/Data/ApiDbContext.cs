using Microsoft.EntityFrameworkCore;
using Rentd.API.Models;

namespace Rentd.API.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public ApiDbContext(DbContextOptions ops) : base(ops)
        {
            
        }
    }
}