using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using NpgsqlTypes;
using System.Threading.Tasks;
using Training_Api.Context;
using Training_Api.Enums;
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
            return await _context.Workout.AsNoTracking().Include(x => x.WorkoutExercise).Include(y => y.User).Where(x => x.UserId == userId).ToListAsync();
        }

        public IQueryable<Workout> GetAllWorkout()
        {
            return _context.Workout.AsNoTracking().Include(x => x.WorkoutExercise).AsQueryable();
        }


        public async Task DeleteMyWorkout(Workout workout)
        {
            _context.Workout.Remove(workout);

            await _context.SaveChangesAsync();  
        }

        public async Task<Workout?> GetWorkoutByIdAndUser(int userId , int workoutId)
        {
            return await _context.Workout.Include(x => x.WorkoutExercise).FirstOrDefaultAsync(x => x.Id == workoutId && x.UserId == userId);
        }

        public async Task UpdateMyWorkout(Workout workout)
        {
            _context.Workout.Update(workout);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateMyWorkoutExcercise(WorkoutExercise workout)
        {
            _context.WorkoutExercise.Update(workout);

            await _context.SaveChangesAsync();
        }

        public async Task<WorkoutExercise?> GetWorkoutExcerciseById(int workoutId , int workoutExcerciseId)
        {
            return await _context.WorkoutExercise.FirstOrDefaultAsync(x => x.Id == workoutExcerciseId && x.WorkoutId == workoutId);
        }

        public async Task DeleteMyWorkoutExercise(WorkoutExercise workoutExercise)
        {
            _context.WorkoutExercise.Remove(workoutExercise);

            await _context.SaveChangesAsync();
        }

        public async Task AddMyWorkoutExercise(WorkoutExercise workoutExercise)
        {
            await _context.WorkoutExercise.AddAsync(workoutExercise);

            await _context.SaveChangesAsync();
        }

    }
}
