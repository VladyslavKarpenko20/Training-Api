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
    }
}
