using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Training_Api.Models;


namespace Training_Api.Context
{
    public class AddDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<User> User { get; set; }

        public DbSet<Workout> Workout {  get; set; }

        public DbSet<WorkoutExercise> WorkoutExercise { get; set; }


        public AddDbContext(DbContextOptions<AddDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var admin = new User
            {
                Id = 10,
                Email = "Admin@gmail.com",
                Name = "Admin",
                Role = Role.Role.Admin,
            };

            var _passwordHasher = new PasswordHasher<User>();

            var userPassword = _passwordHasher.HashPassword(admin, _configuration["AdminPassword:Password"] ?? "123456");
            admin.Password = userPassword;

            modelBuilder.Entity<User>().HasData(admin);
        }


    }
}
