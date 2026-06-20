using Training_Api.Models;

namespace Training_Api.Interface
{
    public interface IWorkoutRepository
    {
        Task AddWorkout(Workout workout);

    }
}
