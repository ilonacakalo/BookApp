using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    public class BookAppContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public BookAppContext(DbContextOptions options) : base(options)
        { 
        }

        protected BookAppContext()
        {
        }
    }
}
