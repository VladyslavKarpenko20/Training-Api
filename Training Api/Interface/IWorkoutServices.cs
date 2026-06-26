using Training_Api.DtoModels;
using Training_Api.Models;

namespace Training_Api.Interface
{
    public interface IWorkoutServices
    {
        Task AddWorkout(WorkoutWriteDto workoutWrite, int userId);

        Task<List<WorkoutReadDto>> GetMyWorkout(int userId, int Page, int PageSize);

        List<WorkoutReadDto> GetAllWorkout(int Page, int PageSize);

        Task DeleteMyWorkout(int workoutId, int userId);

        Task<List<WorkoutReadDto>> SearchWorkoutByData(DateTimeOffset? startData, DateTimeOffset? endData, int Page, int PageSize);

        Task UpdateMyWorkoutDate(int workoutId, int userId, DateTimeOffset? newDate);

        Task UpdateMyWorkoutExcercise(int workoutId, int workoutExcerciseId, int userId, WorkoutExerciseRequestDto updateWorkout);

        Task DeleteMyWorkoutExcercise(int workoutId, int workoutExcerciseId, int userId);

        Task AddMyWorkoutExercise(int workoutId, int userId, WorkoutExerciseRequestDto addWorkoutExercise);
    }
}
