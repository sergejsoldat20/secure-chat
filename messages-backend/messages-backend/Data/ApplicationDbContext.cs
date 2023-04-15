using messages_backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace messages_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Message> Message { get; set; }    
    }
}
