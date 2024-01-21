using boxtureAssignment.Models;
using Microsoft.EntityFrameworkCore;

namespace boxtureAssignment.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
