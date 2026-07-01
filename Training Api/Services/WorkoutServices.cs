using Training_Api.DtoModels;
using Training_Api.Interface;
using Training_Api.Exceptions;
using Training_Api.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using Microsoft.EntityFrameworkCore;
using Training_Api.Enums;

namespace Training_Api.Services
{
    public class WorkoutServices : IWorkoutServices
    {
        private readonly IWorkoutRepository _repository;

        public WorkoutServices(IWorkoutRepository repository, IUserRepository userRepository) 
        {
            _repository = repository;
        }


        public async Task AddWorkout(WorkoutWriteDto workoutWrite, int userId)
        {
            if (workoutWrite.Workouts.Count == 0)
                throw new BadRequestExceptions("There must be at least one workout in the workout list");

            if (workoutWrite.endDate < workoutWrite.startDate)
                throw new BadRequestExceptions("The start time cannot be later than the end time");

            if (workoutWrite.endDate > workoutWrite.startDate.AddDays(1) )
                throw new BadRequestExceptions("The training session cannot last longer than one day");

            foreach(var workoutExercise in workoutWrite.Workouts)
            {
                if(workoutExercise.Repetitions < 1 || workoutExercise.Weight < 1 || string.IsNullOrWhiteSpace(workoutExercise.Name))  
                    throw new BadRequestExceptions("Invalid WorkoutExercie data");
            };

            var workout = new Workout
            {
                UserId = userId,
                startDate = workoutWrite.startDate,
                endDate = workoutWrite.endDate,
                WorkoutExercise = workoutWrite.Workouts.Select(x => new WorkoutExercise 
                {
                    Name = x.Name,
                    Repetitions = x.Repetitions,
                    Weight = x.Weight
                }).ToList()
            };


            if (await _repository.WorkoutTimeCheck(userId, workoutWrite.startDate, workoutWrite.endDate, null))
                throw new BadRequestExceptions("You already have a workout scheduled for that time");

            var now = DateTimeOffset.UtcNow;

            if (workout.startDate <= now && workout.endDate >= now)
                workout.Status = Status.InProgres;
            else if (workout.endDate < now)
                workout.Status = Status.Completed;
            else if (workout.startDate > now)
                workout.Status = Status.Planned;
            
            await _repository.AddWorkout(workout);
        }

        public async Task<List<WorkoutReadDto>> GetMyWorkout(int userId, int Page, int PageSize)
        {
            if (Page < 1 || PageSize < 1 || PageSize > 100)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            IQueryable<Workout> listWorkout = _repository.GetMyWorkout(userId);


            return await listWorkout.Skip((Page - 1) * PageSize).Take(PageSize).Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                startDate = x.startDate,
                endDate = x.endDate,
                UserId = x.UserId,
                Status = x.Status,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Id = y.Id,
                    Name = y.Name,
                    Weight = y.Weight,
                    Repetitions = y.Repetitions
                }).ToList()
            }).ToListAsync();

        }

        public async Task<List<WorkoutReadDto>> GetAllWorkout(int Page, int PageSize)
        {
            if (Page < 1 || PageSize < 1 || PageSize > 100)
                throw new BadRequestExceptions("Invalid data Page or PageSize");

            IQueryable<Workout> listWorkout = _repository.GetAllWorkout();

            return await listWorkout.Skip((Page - 1) * PageSize).Take(PageSize).Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                startDate = x.startDate,
                endDate = x.endDate,
                UserId = x.UserId,
                Status = x.Status,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Id = y.Id,
                    Name = y.Name,
                    Repetitions = y.Repetitions,
                    Weight = y.Weight
                }).ToList()

            }).ToListAsync();
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
            if (Page < 1 || PageSize < 1 || PageSize > 100)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            if (startDat > endDate)
                throw new BadRequestExceptions("The start time cannot be greater than the end time");


            IQueryable<Workout> listWorkout = _repository.GetAllWorkout();
            
            if (startDat != null)
                listWorkout = listWorkout.Where(x => x.startDate >= startDat);
            if (endDate != null)
                listWorkout = listWorkout.Where(x => x.endDate <= endDate);
            
            var workout = listWorkout.Skip((Page - 1) * PageSize).Take(PageSize).Select(x => new WorkoutReadDto 
            {
                Id = x.Id,
                UserId = x.UserId,
                startDate = x.startDate,
                endDate = x.endDate,
                Status = x.Status,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Id = y.Id,
                    Weight = y.Weight,
                    Name = y.Name,
                    Repetitions = y.Repetitions
                }).ToList()
            }).ToListAsync();


            return await workout;
        }

        public async Task UpdateMyWorkoutDate(int workoutId, int userId, DateTimeOffset newStartDate, DateTimeOffset newEndDate)
        {

            if (newStartDate > DateTimeOffset.Now.AddYears(1) || newStartDate < DateTimeOffset.Now.AddYears(-1)
                || newEndDate > DateTimeOffset.Now.AddYears(1) || newEndDate < DateTimeOffset.Now.AddYears(-1))
                throw new BadRequestExceptions("Invalid time Data");

            if (newEndDate > newStartDate.AddDays(1))
                throw new BadRequestExceptions("The training session cannot last longer than one day");

            if (newEndDate < newStartDate)
                throw new BadRequestExceptions("The start time cannot be later than the end time");

            var workout = await _repository.GetWorkoutByIdAndUser(userId, workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            if (workout.Status == Status.Cancelled)
                throw new BadRequestExceptions("You cannot change a canceled workout");

            if (await _repository.WorkoutTimeCheck(userId, newStartDate, newEndDate, workoutId))
                throw new BadRequestExceptions("You already have a workout scheduled for that time");

            var now = DateTimeOffset.UtcNow;

            if (newStartDate <= now && newEndDate >= now)
                workout.Status = Status.InProgres;
            else if (newEndDate < now)
                workout.Status = Status.Completed;
            else if (newStartDate > now)
                workout.Status = Status.Planned;

            workout.startDate = newStartDate;
            workout.endDate = newEndDate;

            await _repository.UpdateMyWorkout(workout);
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
                throw new BadRequestExceptions("Invalid WorkoutExercise data");

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

        public async Task<List<WorkoutReadDto>> SearchMyWorkoutByStatus(Status status, int userId, int Page, int PageSize)
        {
            if (Page < 1 || PageSize < 1 || PageSize > 100)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            var listWorkout = _repository.GetAllWorkout();

            listWorkout = listWorkout.Where(x => x.UserId == userId && x.Status == status);

            return await listWorkout.Skip((Page - 1) * PageSize).Take(PageSize).Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                startDate = x.startDate,
                endDate = x.endDate,
                UserId = x.UserId,
                Status = x.Status,
                WorkoutExerciseShort = x.WorkoutExercise.Select(y => new WorkoutExerciseShortDto
                {
                    Id = y.Id,
                    Name = y.Name,
                    Repetitions = y.Repetitions,
                    Weight = y.Weight
                }).ToList(),
            
            
            }).ToListAsync();

        }

        public async Task CancelMyWorkout(int workoutId, int userId)
        {
            var workout = await _repository.GetWorkoutByIdAndUser(userId,workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            if (workout.Status == Status.Completed)
                throw new BadRequestExceptions("You cannot cancel a workout that has already been completed");

            if (workout.Status == Status.Cancelled)
                throw new BadRequestExceptions("Training has already been canceled");

            workout.Status = Status.Cancelled;

            await _repository.UpdateMyWorkout(workout);
        }


        public async Task<List<WorkoutExerciseRequestDto>> GetMyExerciseByName(string Name, int Page, int PageSize, int userId)
        {
            if (string.IsNullOrEmpty(Name)) 
                throw new BadRequestExceptions("The name cannot be empty");

            if (Page < 1 || PageSize > 100 || PageSize < 1)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            IQueryable<WorkoutExercise> listExercise = _repository.GetMyExerciseByName(Name, userId);

            return await listExercise.Skip((Page - 1) * PageSize).Take(PageSize).Select(x => new WorkoutExerciseRequestDto
            {
                Name = x.Name,
                Repetitions= x.Repetitions,
                Weight  = x.Weight
            }).ToListAsync();

        }

    }
}
