using Training_Api.DtoModels;

namespace Training_Api.Interface
{
    public interface IWorkoutServices
    {
        Task AddWorkout(WorkoutWriteDto workoutWrite, int userId);

        Task<List<WorkoutReadDto>> GetMyWorkout(int userId);

        List<WorkoutReadDto> GetAllWorkout(int Page, int PageSize);

        Task DeleteMyWorkout(int workoutId, int userId);
        
    }
}
