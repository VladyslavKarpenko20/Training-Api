using Training_Api.DtoModels;

namespace Training_Api.Interface
{
    public interface IWorkoutServices
    {
        Task AddWorkout(WorkoutWriteDto workoutWrite, int userId);

        Task<List<WorkoutReadDto>> GetMyWorkout(int userId);
    }
}
