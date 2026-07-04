using Training_Api.DtoModels;
using Training_Api.Enums;

namespace Training_Api.Interface
{
    public interface IWorkoutServices
    {
        Task AddWorkout(WorkoutWriteDto workoutWrite, int userId);

        Task<List<WorkoutReadDto>> GetMyWorkout(int userId, int page, int pageSize);

        Task<List<WorkoutReadDto>> GetAllWorkout(int page, int pageSize);

        Task DeleteMyWorkout(int workoutId, int userId);

        Task<List<WorkoutReadDto>> SearchWorkoutByData(DateTimeOffset? startData, DateTimeOffset? endData, int page, int pageSize);

        Task UpdateMyWorkoutDate(int workoutId, int userId, DateTimeOffset newDate, DateTimeOffset endDate);

        Task UpdateMyWorkoutExcercise(int workoutId, int workoutExcerciseId, int userId, WorkoutExerciseRequestDto updateWorkout);

        Task DeleteMyWorkoutExcercise(int workoutId, int workoutExcerciseId, int userId);

        Task AddMyWorkoutExercise(int workoutId, int userId, WorkoutExerciseRequestDto addWorkoutExercise);

        Task<List<WorkoutReadDto>> SearchMyWorkoutByStatus(Status status, int userId, int page, int pageSize);

        Task CancelMyWorkout(int workoutId, int userId);

        Task<List<WorkoutExerciseRequestDto>> GetMyExerciseByName(string name, int page, int pageSize, int userId);

        Task<WorkoutsStatsDto> GetWorkoutsStats(int userId);
    }
}
