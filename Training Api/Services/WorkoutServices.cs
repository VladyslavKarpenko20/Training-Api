using Training_Api.DtoModels;
using Training_Api.Interface;
using Training_Api.Exceptions;
using Training_Api.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace Training_Api.Services
{
    public class WorkoutServices : IWorkoutServices
    {
        private readonly IWorkoutRepository _repository;

        public WorkoutServices(IWorkoutRepository repository) 
        {
            _repository = repository;
        }


        public async Task AddWorkout(WorkoutWriteDto workoutWrite, int userId)
        {
            if (workoutWrite.Workouts.Count == 0)
                throw new BadRequestExceptions("There must be at least one workout in the workout list");

            var workout = new Workout
            {
                UserId = userId,
                Date = workoutWrite.Date,
                WorkoutExercise = workoutWrite.Workouts.Select(x => new WorkoutExercise 
                {
                    Name = x.Name,
                    Repetitions = x.Repetitions,
                    Weight = x.Weight
                }).ToList()
            };

            await _repository.AddWorkout(workout);
        }

        public async Task<List<WorkoutReadDto>> GetMyWorkout(int userId)
        {
            var listWorkout = await _repository.GetMyWorkout(userId);

            return listWorkout.Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                Date = x.Date,
                UserId = x.UserId,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Name = y.Name,
                    Weight = y.Weight,
                    Repetitions = y.Repetitions
                }).ToList()
            }).ToList();

        }

        public List<WorkoutReadDto> GetAllWorkout(int Page, int PageSize)
        {
            if (Page < 1 || PageSize < 1 || PageSize > 10000)
                throw new BadRequestExceptions("Invalid data Page or PageSize");

            var listWorkout = _repository.GetAllWorkout();

            return listWorkout.Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                Date = x.Date,
                UserId = x.UserId,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Name = y.Name,
                    Repetitions = y.Repetitions,
                    Weight = y.Weight
                }).ToList()

            }).Skip((Page - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}
