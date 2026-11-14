using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Data
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
