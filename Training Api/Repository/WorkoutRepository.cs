using Microsoft.EntityFrameworkCore;
using Training_Api.Context;
using Training_Api.Enums;
using Training_Api.Interface;
using Training_Api.Models;

namespace Training_Api.Repository
{
    public class WorkoutRepository(AddDbContext context) : IWorkoutRepository
    {
        public async Task AddWorkout(Workout workout)
        {
            await context.Workout.AddAsync(workout);

            await context.SaveChangesAsync();
        }

        public IQueryable<Workout> GetMyWorkout(int userId)
        {
            return context.Workout.AsNoTracking().Include(x => x.WorkoutExercise).Where(x => x.UserId == userId).AsQueryable();
        }

        public IQueryable<Workout> GetAllWorkout()
        {
            return context.Workout.AsNoTracking().Include(x => x.WorkoutExercise);
        }


        public async Task DeleteMyWorkout(Workout workout)
        {
            context.Workout.Remove(workout);

            await context.SaveChangesAsync();
        }

        public async Task<Workout?> GetWorkoutByIdAndUser(int userId, int workoutId)
        {
            return await context.Workout.Include(x => x.WorkoutExercise).FirstOrDefaultAsync(x => x.Id == workoutId && x.UserId == userId);
        }

        public async Task UpdateMyWorkout(Workout workout)
        {
            context.Workout.Update(workout);

            await context.SaveChangesAsync();
        }

        public async Task UpdateMyWorkoutExcercise(WorkoutExercise workout)
        {
            context.WorkoutExercise.Update(workout);

            await context.SaveChangesAsync();
        }

        public async Task<WorkoutExercise?> GetWorkoutExcerciseById(int workoutId, int workoutExcerciseId)
        {
            return await context.WorkoutExercise.FirstOrDefaultAsync(x => x.Id == workoutExcerciseId && x.WorkoutId == workoutId);
        }

        public async Task DeleteMyWorkoutExercise(WorkoutExercise workoutExercise)
        {
            context.WorkoutExercise.Remove(workoutExercise);

            await context.SaveChangesAsync();
        }

        public async Task AddMyWorkoutExercise(WorkoutExercise workoutExercise)
        {
            await context.WorkoutExercise.AddAsync(workoutExercise);

            await context.SaveChangesAsync();
        }

        public async Task<bool> WorkoutTimeCheck(int userId, DateTimeOffset startDate, DateTimeOffset endDate, int? workoutId)
        {
            if (workoutId == null)
                return await context.Workout.AnyAsync(x => x.UserId == userId && x.StartDate < endDate && x.EndDate > startDate && x.Status != Status.Cancelled);

            else
                return await context.Workout.AnyAsync(x => x.UserId == userId && x.StartDate < endDate && x.EndDate > startDate && x.Id != workoutId && x.Status != Status.Cancelled);
        }

        public  IQueryable<WorkoutExercise> GetMyExerciseByName(string nameExercise, int userId)
        {
            return context.WorkoutExercise.AsNoTracking().Where(x => x.Workout != null && x.Workout.UserId == userId && x.Name!.ToLower() == nameExercise.ToLower());     
        }

        public IQueryable<Workout> GetWorkoutByStatistics(int userId)
        {
            return context.Workout.AsNoTracking().Where(x => x.UserId == userId);
        }

        public IQueryable<WorkoutExercise> GetWorkoutExerciseByStatistics(int userId)
        {
            return context.WorkoutExercise.AsNoTracking().Where(x => x.Workout != null && x.Workout.UserId == userId);
        }
    
    }
}
