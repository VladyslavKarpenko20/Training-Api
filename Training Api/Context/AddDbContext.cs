using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Training_Api.Models;
using Training_Api.Enums;


namespace Training_Api.Context
{
    public class AddDbContext(DbContextOptions<AddDbContext> options, IConfiguration configuration)
        : DbContext(options)
    {
        public DbSet<User> User { get; set; }

        public DbSet<Workout> Workout {  get; set; }

        public DbSet<WorkoutExercise> WorkoutExercise { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var admin = new User
            {
                Id = 10,
                Email = "Admin@gmail.com",
                Name = "Admin",
                Role = Role.Admin,
            };

            var passwordHasher = new PasswordHasher<User>();

            var userPassword = passwordHasher.HashPassword(admin, configuration["AdminPassword:Password"] ?? "123456");
            admin.Password = userPassword;

            modelBuilder.Entity<User>().HasData(admin);
            
            
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Workouts)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.UserId)   
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Workout>()
                .HasMany(w => w.WorkoutExercise)
                .WithOne(x => x.Workout)
                .HasForeignKey(x => x.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);  
        }


    }
}
