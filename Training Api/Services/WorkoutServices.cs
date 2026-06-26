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

        public async Task<List<WorkoutReadDto>> GetMyWorkout(int userId, int Page, int PageSize)
        {
            if (Page < 1 || PageSize < 1 || PageSize > 10000)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            var listWorkout = await _repository.GetMyWorkout(userId);

            return listWorkout.Skip((Page - 1) * PageSize).Take(PageSize).Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                Date = x.Date,
                UserId = x.UserId,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Id = y.Id,
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

            return listWorkout.Skip((Page - 1) * PageSize).Take(PageSize).Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                Date = x.Date,
                UserId = x.UserId,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Id = y.Id,
                    Name = y.Name,
                    Repetitions = y.Repetitions,
                    Weight = y.Weight
                }).ToList()

            }).ToList();
        }

        public async Task DeleteMyWorkout(int workoutId, int userId)
        {
            var workout = await _repository.GetWorkoutByIdAndUser(userId, workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            await _repository.DeleteMyWorkout(workout);
        }

        public async Task<List<WorkoutReadDto>> SearchWorkoutByData(DateTimeOffset? startDat, DateTimeOffset? endDate, int Page, int PageSize)
        {
            if (Page < 1 || PageSize < 1 || PageSize > 10000)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            if (startDat > endDate)
                throw new BadRequestExceptions("The start time cannot be greater than the end time");


            IQueryable<Workout> listWorkout = _repository.GetAllWorkout();

            if (startDat != null)
                listWorkout = listWorkout.Where(x => x.Date >= startDat);
            if (endDate != null)
                listWorkout = listWorkout.Where(x => x.Date <= endDate);

            var workout = listWorkout.Skip((Page - 1) * PageSize).Take(PageSize).Select(x => new WorkoutReadDto 
            {
                Id = x.Id,
                UserId = x.UserId,
                Date = x.Date,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Id = y.Id,
                    Weight = y.Weight,
                    Name = y.Name,
                    Repetitions = y.Repetitions
                }).ToList()
            }).ToList();


            return workout;
        }

        public async Task UpdateMyWorkoutDate(int workoutId, int userId, DateTimeOffset? newDate)
        {

            if (newDate == null || newDate > DateTimeOffset.Now.AddYears(1) || newDate < DateTimeOffset.Now.AddYears(-1))
                throw new BadRequestExceptions("Invalid time Data");

            var workout = await _repository.GetWorkoutByIdAndUser(userId, workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            workout.Date = newDate.Value;

            await _repository.UpdateMyWorkoutDate(workout);
        }


        public async Task UpdateMyWorkoutExcercise(int workoutId , int workoutExcerciseId, int userId, WorkoutExerciseRequestDto updateWorkout)
        {
            var workout = await _repository.GetWorkoutByIdAndUser(userId , workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            var workoutExcercise = await _repository.GetWorkoutExcerciseById(workoutId, workoutExcerciseId);

            if (workoutExcercise == null)
                throw new NotFoundExceptions("Workout excercise not found");

            if (string.IsNullOrWhiteSpace(updateWorkout.Name) || updateWorkout.Weight < 0 || updateWorkout.Repetitions < 1)
                throw new BadRequestExceptions("Invalid data");

            workoutExcercise.Weight = updateWorkout.Weight;
            workoutExcercise.Repetitions = updateWorkout.Repetitions;
            workoutExcercise.Name = updateWorkout.Name;

            await _repository.UpdateMyWorkoutExcercise(workoutExcercise);

        } 


        public async Task DeleteMyWorkoutExcercise(int workoutId, int workoutExcerciseId, int userId)
        {
            var workout = await _repository.GetWorkoutByIdAndUser(userId,workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            var workoutExcercise = await _repository.GetWorkoutExcerciseById(workoutId, workoutExcerciseId);

            if (workoutExcercise == null)
                throw new NotFoundExceptions("WorkoutExercise not found");

            await _repository.DeleteMyWorkoutExercise(workoutExcercise);
        }

        public async Task AddMyWorkoutExercise(int workoutId , int userId, WorkoutExerciseRequestDto addWorkoutExercise)
        {
            var workout = await _repository.GetWorkoutByIdAndUser(userId, workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            if (string.IsNullOrWhiteSpace(addWorkoutExercise.Name) || addWorkoutExercise.Weight < 0 || addWorkoutExercise.Repetitions < 1)
                throw new BadRequestExceptions("Invalid data");

            var workoutExercise = new WorkoutExercise
            {
                Name = addWorkoutExercise.Name,
                Repetitions = addWorkoutExercise.Repetitions,
                Weight = addWorkoutExercise.Weight,
                WorkoutId = workoutId
            };

            await _repository.AddMyWorkoutExercise(workoutExercise);
        }
 


    }
}
