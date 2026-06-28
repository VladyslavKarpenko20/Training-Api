using Training_Api.Models;

namespace Training_Api.Interface
{
    public interface IWorkoutRepository
    {
        Task AddWorkout(Workout workout);

        Task<List<Workout>> GetMyWorkout(int userId);

        IQueryable<Workout> GetAllWorkout();

        Task DeleteMyWorkout(Workout workout);

        Task<Workout?> GetWorkoutByIdAndUser(int userId, int workoutId);

        Task UpdateMyWorkout(Workout workout);

        Task UpdateMyWorkoutExcercise(WorkoutExercise workout);

        Task<WorkoutExercise?> GetWorkoutExcerciseById(int workoutId, int workoutExcerciseId);

        Task DeleteMyWorkoutExercise(WorkoutExercise workoutExercise);

        Task AddMyWorkoutExercise(WorkoutExercise workoutExercise);

    }
}
