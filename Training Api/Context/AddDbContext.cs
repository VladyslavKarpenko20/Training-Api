using Microsoft.EntityFrameworkCore;
using Training_Api.Models;


namespace Training_Api.Context
{
    public class AddDbContext : DbContext
    {
        public DbSet<User> User { get; set; }


        public AddDbContext(DbContextOptions<AddDbContext> options)
            : base(options)
        {
        }


    }
}
