using Microsoft.EntityFrameworkCore;
using Training_Api.Context;
using Training_Api.Interface;
using Training_Api.Models;

namespace Training_Api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AddDbContext _context;


        public UserRepository(AddDbContext context) 
        {
            _context = context;
        }

        public async Task<User?> SearchUserByEmail(string email)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> SearchUserByName(string name) 
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task AddUser(User user)
        {
            await _context.User.AddAsync(user);

            await _context.SaveChangesAsync();
        }


        public IQueryable<User> GetAllUser()
        {
            return _context.User.AsQueryable();
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _context.User.Include(w => w.Workouts).ThenInclude(w => w.WorkoutExercise).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task DeleteUser(User user)
        {
            _context.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task GiveRoleAdmin(User user)
        {
            _context.Update(user);

            await _context.SaveChangesAsync();
        }
    }
}
