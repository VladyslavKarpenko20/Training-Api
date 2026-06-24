using Training_Api.DtoModels;
using Training_Api.Interface;
using Training_Api.Exceptions;
using Training_Api.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteMyWorkout(int workoutId, int userId)
        {
            var workout = await _repository.GetWorkoutByIdAndUser(userId, workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            await _repository.DeleteMyWorkout(workout);
        }

        public async Task<List<Workout>> SearchWorkoutByData(DateTimeOffset? startDat, DateTimeOffset? endDate, int Page, int PageSize)
        {
            if (Page < 1 || PageSize < 1 || PageSize > 10000)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            if (startDat > endDate)
                throw new BadRequestExceptions("The start time cannot be greater than the end time");

            Console.WriteLine($"start {startDat} || end {endDate} ");

            IQueryable<Workout> listWorkout = _repository.GetAllWorkout();

            if (startDat != null)
                listWorkout = listWorkout.Where(x => x.Date >= startDat);
            if (endDate != null)
                listWorkout = listWorkout.Where(x => x.Date <= endDate);

            return await listWorkout.Skip((Page - 1) * PageSize).Take(PageSize).ToListAsync();
        }

    }
}
