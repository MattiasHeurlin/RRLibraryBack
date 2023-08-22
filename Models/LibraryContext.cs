using Microsoft.EntityFrameworkCore;

namespace BackEnd.Models
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Quote> Quotes { get; set; }

    }
    
}
