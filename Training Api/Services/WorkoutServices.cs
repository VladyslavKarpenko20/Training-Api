using Microsoft.EntityFrameworkCore;
using Training_Api.DtoModels;
using Training_Api.Enums;
using Training_Api.Exceptions;
using Training_Api.Interface;
using Training_Api.Models;

namespace Training_Api.Services
{
    public class WorkoutServices(IWorkoutRepository repository) : IWorkoutServices
    {
        

        public async Task AddWorkout(WorkoutWriteDto workoutWrite, int userId)
        {
            if (workoutWrite.WorkoutsExercise.Count == 0)
                throw new BadRequestExceptions("There must be at least one workout in the workout list");

            if (workoutWrite.EndDate < workoutWrite.StartDate)
                throw new BadRequestExceptions("The start time cannot be later than the end time");

            if (workoutWrite.EndDate > workoutWrite.StartDate.AddDays(1) )
                throw new BadRequestExceptions("The training session cannot last longer than one day");

            foreach(var workoutExercise in workoutWrite.WorkoutsExercise)
            {
                if(workoutExercise.Repetitions < 1 || workoutExercise.Weight < 0 || string.IsNullOrWhiteSpace(workoutExercise.Name))  
                    throw new BadRequestExceptions("Invalid WorkoutExercie data");
            }

            var workout = new Workout
            {
                UserId = userId,
                StartDate = workoutWrite.StartDate,
                EndDate = workoutWrite.EndDate,
                WorkoutExercise = workoutWrite.WorkoutsExercise.Select(x => new WorkoutExercise 
                {
                    Name = x.Name,
                    Repetitions = x.Repetitions,
                    Weight = x.Weight
                }).ToList()
            };


            if (await repository.WorkoutTimeCheck(userId, workoutWrite.StartDate, workoutWrite.EndDate, null))
                throw new BadRequestExceptions("You already have a workout scheduled for that time");

            var now = DateTimeOffset.UtcNow;

            if (workout.StartDate <= now && workout.EndDate >= now)
                workout.Status = Status.InProgres;
            else if (workout.EndDate < now)
                workout.Status = Status.Completed;
            else if (workout.StartDate > now)
                workout.Status = Status.Planned;
            
            await repository.AddWorkout(workout);
        }

        public async Task<List<WorkoutReadDto>> GetMyWorkout(int userId, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            IQueryable<Workout> listWorkout = repository.GetMyWorkout(userId);


            return await listWorkout.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
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

        public async Task<List<WorkoutReadDto>> GetAllWorkout(int page, int pageSize)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new BadRequestExceptions("Invalid data Page or PageSize");

            IQueryable<Workout> listWorkout = repository.GetAllWorkout();

            return await listWorkout.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
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
            var workout = await repository.GetWorkoutByIdAndUser(userId, workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            await repository.DeleteMyWorkout(workout);
        }

        public async Task<List<WorkoutReadDto>> SearchWorkoutByData(DateTimeOffset? startDat, DateTimeOffset? endDate, int page, int pageSize, int userId)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            if (startDat > endDate)
                throw new BadRequestExceptions("The start time cannot be greater than the end time");


            IQueryable<Workout> listWorkout = repository.GetAllWorkout();

            listWorkout = listWorkout.Where(x => x.UserId == userId);
            
            if (startDat != null)
                listWorkout = listWorkout.Where(x => x.StartDate >= startDat);
            if (endDate != null)
                listWorkout = listWorkout.Where(x => x.EndDate <= endDate);
            
            var workout = listWorkout.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new WorkoutReadDto 
            {
                Id = x.Id,
                UserId = x.UserId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
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

            var workout = await repository.GetWorkoutByIdAndUser(userId, workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            if (workout.Status == Status.Cancelled)
                throw new BadRequestExceptions("You cannot change a canceled workout");

            if (await repository.WorkoutTimeCheck(userId, newStartDate, newEndDate, workoutId))
                throw new BadRequestExceptions("You already have a workout scheduled for that time");

            var now = DateTimeOffset.UtcNow;

            if (newStartDate <= now && newEndDate >= now)
                workout.Status = Status.InProgres;
            else if (newEndDate < now)
                workout.Status = Status.Completed;
            else if (newStartDate > now)
                workout.Status = Status.Planned;

            workout.StartDate = newStartDate;
            workout.EndDate = newEndDate;

            await repository.UpdateMyWorkout(workout);
        }


        public async Task UpdateMyWorkoutExcercise(int workoutId , int workoutExcerciseId, int userId, WorkoutExerciseRequestDto updateWorkout)
        {
            var workout = await repository.GetWorkoutByIdAndUser(userId , workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            var workoutExcercise = await repository.GetWorkoutExcerciseById(workoutId, workoutExcerciseId);

            if (workoutExcercise == null)
                throw new NotFoundExceptions("Workout exercise not found");

            if (string.IsNullOrWhiteSpace(updateWorkout.Name) || updateWorkout.Weight < 0 || updateWorkout.Repetitions < 1)
                throw new BadRequestExceptions("Invalid WorkoutExercise data");

            workoutExcercise.Weight = updateWorkout.Weight;
            workoutExcercise.Repetitions = updateWorkout.Repetitions;
            workoutExcercise.Name = updateWorkout.Name;

            await repository.UpdateMyWorkoutExcercise(workoutExcercise);

        } 


        public async Task DeleteMyWorkoutExcercise(int workoutId, int workoutExcerciseId, int userId)
        {
            var workout = await repository.GetWorkoutByIdAndUser(userId,workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            var workoutExercise = await repository.GetWorkoutExcerciseById(workoutId, workoutExcerciseId);

            if (workoutExercise == null)
                throw new NotFoundExceptions("WorkoutExercise not found");

            await repository.DeleteMyWorkoutExercise(workoutExercise);
        }

        public async Task AddMyWorkoutExercise(int workoutId , int userId, WorkoutExerciseRequestDto addWorkoutExercise)
        {
            var workout = await repository.GetWorkoutByIdAndUser(userId, workoutId);

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

            await repository.AddMyWorkoutExercise(workoutExercise);
        }

        public async Task<List<WorkoutReadDto>> SearchMyWorkoutByStatus(Status status, int userId, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            var listWorkout = repository.GetAllWorkout();

            listWorkout = listWorkout.Where(x => x.UserId == userId && x.Status == status);

            return await listWorkout.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new WorkoutReadDto
            {
                Id = x.Id,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
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
            var workout = await repository.GetWorkoutByIdAndUser(userId,workoutId);

            if (workout == null)
                throw new NotFoundExceptions("Workout not found");

            if (workout.Status == Status.Completed)
                throw new BadRequestExceptions("You cannot cancel a workout that has already been completed");

            if (workout.Status == Status.Cancelled)
                throw new BadRequestExceptions("Training has already been canceled");

            workout.Status = Status.Cancelled;

            await repository.UpdateMyWorkout(workout);
        }


        public async Task<List<WorkoutExerciseRequestDto>> GetMyExerciseByName(string name, int page, int pageSize, int userId)
        {
            if (string.IsNullOrEmpty(name)) 
                throw new BadRequestExceptions("The name cannot be empty");

            if (page < 1 || pageSize > 100 || pageSize < 1)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            IQueryable<WorkoutExercise> listExercise = repository.GetMyExerciseByName(name, userId);

            return await listExercise.Skip((page - 1) * pageSize).Take(pageSize).Select(x => new WorkoutExerciseRequestDto
            {
                Name = x.Name,
                Repetitions= x.Repetitions,
                Weight  = x.Weight
            }).ToListAsync();

        }

        public async Task<WorkoutsStatsDto> GetWorkoutsStats(int userId)
        {

            var workoutQuery = repository.GetWorkoutByStatistics(userId);

            var workoutExerciseQuery = repository.GetWorkoutExerciseByStatistics(userId);


            var totalWorkout = await workoutQuery.CountAsync();

            var totalWorkoutExercise = await workoutExerciseQuery.CountAsync();

            var totalComplateWorkout = await workoutQuery.Where(x => x.Status == Status.Completed).CountAsync();

            var totalCanceledWorkout = await workoutQuery.Where(x => x.Status == Status.Cancelled).CountAsync();

            var totalInProgresWorkout = await workoutQuery.Where(x => x.Status == Status.InProgres).CountAsync();

            var totalPlannedWorkout = await workoutQuery.Where(x => x.Status == Status.Planned).CountAsync();

            var maxWeight = await workoutExerciseQuery.MaxAsync(x => x.Weight);

            var mostCommonExercise = await workoutExerciseQuery
                .GroupBy(x => x.Name)
                .OrderByDescending(x => x.Count())
                .Select(x => new
            {
                Name = x.Key,
                Count = x.Count()
            })
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            var workoutStats = new WorkoutsStatsDto
            {
                TotalWorkout =  totalWorkout,
                TotalWorkoutExercise =  totalWorkoutExercise,
                TotalComplateWorkout =  totalComplateWorkout,
                TotalCanceledWorkout =  totalCanceledWorkout,
                TotalInProgresWorkout =  totalInProgresWorkout,
                TotalPlannedWorkout =  totalPlannedWorkout,
                AvarageExercisPerWorkout = totalWorkout == 0 ? 0 : (double)totalWorkoutExercise  /  totalWorkout,
                MaxWeight = maxWeight,
                MostCommonExercise =  mostCommonExercise

            };

            return workoutStats;
        }
        
        
        
    }
    
}
