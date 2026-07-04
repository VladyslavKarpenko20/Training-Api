using Microsoft.EntityFrameworkCore;
using Training_Api.Context;
using Training_Api.Interface;
using Training_Api.Models;

namespace Training_Api.Repository
{
    public class UserRepository(AddDbContext context) : IUserRepository
    {


        public async Task<User?> SearchUserByEmail(string email)
        {
            return await context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> SearchUserByName(string name) 
        {
            return await context.User.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task AddUser(User user)
        {
            await context.User.AddAsync(user);

            await context.SaveChangesAsync();
        }


        public IQueryable<User> GetAllUser()
        {
            return context.User.AsQueryable();
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await context.User.Include(w => w.Workouts).ThenInclude(w => w.WorkoutExercise).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task DeleteUser(User user)
        {
            context.User.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task GiveRoleAdmin(User user)
        {
            context.User.Update(user);

            await context.SaveChangesAsync();
        }

        public async Task GiveRoleUser(User user)
        {
            context.User.Update(user);

            await context.SaveChangesAsync();
        }
    }
}
