using Training_Api.DtoModels;
using Training_Api.Enums;
using Training_Api.Models;

namespace Training_Api.Interface
{
    public interface IWorkoutServices
    {
        Task AddWorkout(WorkoutWriteDto workoutWrite, int userId);

        Task<List<WorkoutReadDto>> GetMyWorkout(int userId, int Page, int PageSize);

        Task<List<WorkoutReadDto>> GetAllWorkout(int Page, int PageSize);

        Task DeleteMyWorkout(int workoutId, int userId);

        Task<List<WorkoutReadDto>> SearchWorkoutByData(DateTimeOffset? startData, DateTimeOffset? endData, int Page, int PageSize);

        Task UpdateMyWorkoutDate(int workoutId, int userId, DateTimeOffset? newDate, DateTimeOffset? endDate);

        Task UpdateMyWorkoutExcercise(int workoutId, int workoutExcerciseId, int userId, WorkoutExerciseRequestDto updateWorkout);

        Task DeleteMyWorkoutExcercise(int workoutId, int workoutExcerciseId, int userId);

        Task AddMyWorkoutExercise(int workoutId, int userId, WorkoutExerciseRequestDto addWorkoutExercise);

        Task<List<WorkoutReadDto>> SearchMyWorkoutByStatus(Status status, int userId, int Page, int PageSize);

        Task CancelMyWorkout(int workoutId, int userId);
    }
}
