using Training_Api.DtoModels;
using Training_Api.Models;

namespace Training_Api.Interface
{
    public interface IWorkoutServices
    {
        Task AddWorkout(WorkoutWriteDto workoutWrite, int userId);

        Task<List<WorkoutReadDto>> GetMyWorkout(int userId);

        List<WorkoutReadDto> GetAllWorkout(int Page, int PageSize);

        Task DeleteMyWorkout(int workoutId, int userId);

        Task<List<Workout>> SearchWorkoutByData(DateTimeOffset? startData, DateTimeOffset? endData, int Page, int PageSize);

    }
}
