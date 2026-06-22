using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using NpgsqlTypes;
using Training_Api.Context;
using Training_Api.Interface;
using Training_Api.Models;

namespace Training_Api.Repository
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly AddDbContext _context;

        public WorkoutRepository(AddDbContext context)
        {
            _context = context;
        }

        public async Task AddWorkout(Workout workout)
        {
            await _context.Workout.AddAsync(workout);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Workout>> GetMyWorkout(int userId)
        {
            return await _context.Workout.Include(x => x.WorkoutExercise).Include(y => y.User).Where(x => x.UserId == userId).ToListAsync();
        }

        public IQueryable<Workout> GetAllWorkout()
        {
            return _context.Workout.AsQueryable();
        }


        public async Task DeleteMyWorkout(Workout workout)
        {
            _context.Workout.Remove(workout);

            await _context.SaveChangesAsync();  
        }

        public async Task<Workout?> GetWorkoutByIdAndUser(int userId , int workoutId)
        {
            return await _context.Workout.FirstOrDefaultAsync(x => x.Id == workoutId && x.UserId == userId);
        }

    }
}
