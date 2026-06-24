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


    }
}
