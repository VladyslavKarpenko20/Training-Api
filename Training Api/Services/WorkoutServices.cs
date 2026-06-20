using Training_Api.DtoModels;
using Training_Api.Interface;
using Training_Api.Exceptions;
using Training_Api.Models;

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

    }
}
