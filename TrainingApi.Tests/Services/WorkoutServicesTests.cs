using Moq;
using Training_Api.Exceptions;
using Training_Api.Interface;
using FluentAssertions;
using MockQueryable;
using Training_Api.Services;
using Training_Api.DtoModels;
using Training_Api.Models;
using Training_Api.Enums;


namespace TrainingApi.Tests.Services;



public class WorkoutServicesTests
{
    
    private readonly Mock<IWorkoutRepository> _workoutRepositoryMoc;
    private readonly WorkoutServices _workoutServices;

    public WorkoutServicesTests()
    {
        _workoutRepositoryMoc = new Mock<IWorkoutRepository>();
        _workoutServices = new WorkoutServices(_workoutRepositoryMoc.Object);
    }
    
    [Fact]
    public async Task AddWorkout_WhenDurationExceedsOneDay_ShouldThrowBadRequestException()
    {
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(1).AddHours(1);
        
        
        var dto = new WorkoutWriteDto
        {
            StartDate =  startDate,
            EndDate =  endDate,
            WorkoutsExercise =
            [
                new WorkoutExerciseWriteDto
                {
                    Name = "Bench Press",
                    Repetitions = 10,
                    Weight = 100,
                }
            ]
        };

        int userId = 1;
        
        var act = async () => await  _workoutServices.AddWorkout(dto, userId);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("The training session cannot last longer than one day");

    }


    [Fact]
    public async Task AddWorkout_WhenWorkoutExerciseIsEmpty_ShouldThrowBadRequestException()
    {
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);
        
        var dto = new WorkoutWriteDto
        {
            StartDate =  startDate,
            EndDate =  endDate,
            WorkoutsExercise = new List<WorkoutExerciseWriteDto>()
        };
        
        const int userId = 1;  
        
        var act = async () =>  await _workoutServices.AddWorkout(dto, userId);
        
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("There must be at least one workout in the workout list");
        
        
    }

    [Fact]
    public async Task AddWorkout_WhenStartTimeLaterThanEndTime_ShouldThrowBadRequestException()
    {
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = DateTimeOffset.UtcNow.AddHours(1);
        
        var dto = new WorkoutWriteDto
        {
            StartDate =  startDate,
            EndDate =  endDate,
            WorkoutsExercise =  
            [
                new WorkoutExerciseWriteDto()
                {
                    Name = "Bench Press",
                    Repetitions = 10,
                    Weight = 100,
                }
            ]
        };
        
        const int userId = 1;
        
        var act = async () => await  _workoutServices.AddWorkout(dto, userId); 
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("The start time cannot be later than the end time");
        
        
    }

    [Fact]
    public async Task AddWorkout_WhenTimeAlreadyTaken_ShouldThrowBadRequestException()
    {
        
        var startDate =  DateTimeOffset.UtcNow.AddDays(1);
        var endDate =  startDate.AddHours(1);
        
        
        var dto = new WorkoutWriteDto
        {
            StartDate = startDate,
            EndDate = endDate,
            WorkoutsExercise = 
            [
                new WorkoutExerciseWriteDto()
                {
                    Name = "Bench Press",
                    Repetitions = 10,
                    Weight = 100,
                }
            ]

        };
        
        const int userId = 1; 
        
        _workoutRepositoryMoc.Setup(repository => repository.WorkoutTimeCheck(userId, dto.StartDate, dto.EndDate, null)).ReturnsAsync(true);
        
        
        var act =  async () => await  _workoutServices.AddWorkout(dto, userId);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("You already have a workout scheduled for that time");
        
        _workoutRepositoryMoc.Verify(repository =>  repository.AddWorkout(It.IsAny<Workout>()), Times.Never);
    }


    [Theory]
    [InlineData("", 10, 100)]
    [InlineData("Bench Prees", -1, 100)]
    [InlineData("Bench Prees", 1, -10)]
    public async Task  AddWorkout_WhenExerciseDataIsInvalid_ShouldThrowBadRequestException(string name, int repetitions, int weight)
    {
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);

        var dto = new WorkoutWriteDto
        {
            StartDate = startDate,
            EndDate = endDate,
            WorkoutsExercise = 
            [
                new WorkoutExerciseWriteDto()
                {
                    Name = name,
                    Repetitions = repetitions,
                    Weight = weight
                }
            ]
        };
        
        const int userId = 1;
        
        var act = async () => await  _workoutServices.AddWorkout(dto, userId);
        
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid WorkoutExercise data");  

    }

    [Fact]
    public async Task AddWorkout_WhenDataIsValid_ShouldAddWorkout()
    {
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);

        var dto = new WorkoutWriteDto
        {
            StartDate = startDate,
            EndDate = endDate,
            WorkoutsExercise = 
            [
                new WorkoutExerciseWriteDto()
                {
                    Name = "Bench Press",
                    Repetitions = 10,
                    Weight = 100
                }
            ]
        };
        
        const int userId = 1;
        
        _workoutRepositoryMoc.Setup(repository => repository.WorkoutTimeCheck(userId, dto.StartDate, dto.EndDate, null)).ReturnsAsync(false);
        
        await  _workoutServices.AddWorkout(dto, userId);
        
        _workoutRepositoryMoc.Verify(repository =>  repository.AddWorkout(It.Is<Workout>(workout => workout.UserId == userId && workout.EndDate == endDate && workout.StartDate == startDate && workout.WorkoutExercise.Count > 0)), Times.Once);
 

    }


    [Theory]
    [InlineData(1,-1, 10)]
    [InlineData(1,1, -1)]
    [InlineData(1,1, 101)]
    public async Task GetMyWorkout_WhenPageOrPageSizeInvalid_ShouldThrowBadRequestException(int userId,int page, int pageSize)
    {
        var act = async () => await _workoutServices.GetMyWorkout(userId, page, pageSize);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid Page or PageSize data");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetMyWorkout(userId), Times.Never);
        
    }

    
 
    [Fact]
    public async Task GetMyWorkout_WhenDataIsValid_ShouldGetMyWorkout()
    {
        int userId = 1, page = 1, pageSize = 10;

        var listWorkout = new List<Workout>
        {
            new Workout
            {
                StartDate = DateTimeOffset.UtcNow.AddDays(1),
                EndDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                Id = 1,
                UserId = 1,
                WorkoutExercise = 
                {
                    new WorkoutExercise
                    {
                        Name = "Bench Press",
                        Repetitions = 10,
                        Weight = 100,
                        WorkoutId = 1
                    },
                    new WorkoutExercise
                    {
                        Name = "Pull Ups",
                        Repetitions = 15,
                        Weight = 10,
                        WorkoutId = 1
                    }
                }
            },
            new Workout
            {
                StartDate = DateTimeOffset.UtcNow.AddDays(2),
                EndDate = DateTimeOffset.UtcNow.AddDays(2).AddHours(1),
                Id = 2,
                UserId = 1,
                WorkoutExercise = 
                {
                    new WorkoutExercise
                    {
                        Name = "Push Ups",
                        Repetitions = 40,
                        Weight = 15,
                        WorkoutId = 2
                    },
                    new WorkoutExercise
                    {
                        Name = "Bench Press",
                        Repetitions = 3,
                        Weight = 130,
                        WorkoutId = 2
                    }
                }
            }

        };
        
        var mockRepository = listWorkout.BuildMock();
        
        
        _workoutRepositoryMoc.Setup(repository => repository.GetMyWorkout(userId)).Returns(mockRepository);
        
        var result = await _workoutServices.GetMyWorkout(userId, page, pageSize);
        
        result.Should().HaveCount(2);
        
        result[0].Id.Should().Be(1);
        result[0].WorkoutExerciseShort.Should().HaveCount(2);
        result[0].WorkoutExerciseShort[0].Name.Should().Be("Bench Press");
        result[0].WorkoutExerciseShort[1].Name.Should().Be("Pull Ups");
        
        result[1].Id.Should().Be(2);
        result[1].WorkoutExerciseShort.Should().HaveCount(2);
        result[1].WorkoutExerciseShort[0].Name.Should().Be("Push Ups");
        result[1].WorkoutExerciseShort[1].Name.Should().Be("Bench Press");
        
        
        _workoutRepositoryMoc.Verify(repository => repository.GetMyWorkout(userId), Times.Once);

    }


    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    public async Task GetAllWorkout_WhenPageOrPageSizeInvalid_ShouldThrowBadRequestException(int page, int pageSize)
    {
        var act =  async () => await _workoutServices.GetAllWorkout(page, pageSize);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid data Page or PageSize");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetAllWorkout(), Times.Never);
        
    }

    
    [Fact]
    public async Task GetAllWorkout_WhenDataIsValid_ShouldGetAllWorkouts()
    {
        int page = 1, pageSize = 10;
        
        var startDate = DateTimeOffset.UtcNow;
        var endDate = startDate.AddHours(1);

        var workout = new List<Workout>
        {
            new Workout
            {
                Id = 1,
                StartDate = startDate,
                EndDate = endDate,
                UserId = 1,
                WorkoutExercise =
                {
                    new WorkoutExercise
                    {
                        Name = "Bench Press",
                        Repetitions = 7,
                        Weight = 85
                    }
                }
            },
            new Workout
            {
                Id = 2,
                StartDate = startDate.AddDays(1),
                EndDate = endDate.AddDays(1).AddHours(1),
                UserId = 2,
                WorkoutExercise = 
                {
                    new WorkoutExercise
                    {
                        Name = "Dead Lift",
                        Repetitions = 5,
                        Weight = 185
                    }
                }
            }, 
            new Workout
            {
                Id = 3,
                StartDate = startDate.AddDays(2),
                EndDate = endDate.AddDays(2).AddHours(1),
                UserId = 3,
                WorkoutExercise = 
                {
                    new WorkoutExercise
                    {
                        Name = "Pull Up", 
                        Weight = 10,
                        Repetitions = 20
                    }
                }
            }
        };
        
        
        var mockQuery = workout.BuildMock();
        
        _workoutRepositoryMoc.Setup(repository => repository.GetAllWorkout()).Returns(mockQuery);

        var result = await _workoutServices.GetAllWorkout(page, pageSize);
        
        result.Should().HaveCount(3);
        
        result[0].Id.Should().Be(1);
        result[0].WorkoutExerciseShort.Should().HaveCount(1);
        result[0].WorkoutExerciseShort[0].Name.Should().Be("Bench Press");
        
        result[1].Id.Should().Be(2);
        result[1].WorkoutExerciseShort.Should().HaveCount(1);
        result[1].WorkoutExerciseShort[0].Name.Should().Be("Dead Lift");
        
        result[2].Id.Should().Be(3);
        result[2].WorkoutExerciseShort.Should().HaveCount(1);
        result[2].WorkoutExerciseShort[0].Name.Should().Be("Pull Up");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetAllWorkout(), Times.Once);



    }


    [Fact]
    public async Task DeleteMyWorkout_WhenWorkoutNotFound_ShouldThrowNotFoundException()
    {
        int workoutId = 1, userId = 1;
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync((Workout?) null);

        var act =  () => _workoutServices.DeleteMyWorkout(workoutId, userId);
        
       await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("Workout not found");
       
       _workoutRepositoryMoc.Verify(repository => repository.DeleteMyWorkout(It.IsAny<Workout>()), Times.Never);
       

    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    public async Task SearchWorkoutByDate_WhenPageOrPageSizeIsInvalid_ShouldThrowBedRequestException(int page, int pageSize)
    {
        int userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);
        
        var act = () => _workoutServices.SearchWorkoutByData(startDate, endDate ,page, pageSize, userId);
        
        await act.Should()
            .ThrowAsync<BadRequestExceptions>()
            .WithMessage("Invalid Page or PageSize data");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetAllWorkout(),  Times.Never);
    }


    [Fact]
    public async Task SearchWorkoutByDate_WhenInvalidDate_ShouldThrowBedRequestException()
    {
        int page = 1, pageSize = 10, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = DateTimeOffset.UtcNow.AddHours(1);

        var act = async () => await _workoutServices.SearchWorkoutByData(startDate, endDate, page, pageSize, userId);
        
        await act.Should()
            .ThrowAsync<BadRequestExceptions>().WithMessage("The start time cannot be greater than the end time");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetAllWorkout(), Times.Never);
    }

    [Fact]
    public async Task SearchWorkoutByDate_WhenDataIsValid_ShouldReturnFilteredWorkouts()
    {
        int userId = 1, page = 1, pageSize = 10;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        
        var endDate = startDate.AddHours(1);

        var workout = new List<Workout>
        {
            new Workout
            {
                Id = 1,
                StartDate = startDate,
                EndDate = endDate,  
                UserId =  1,
                WorkoutExercise = 
                {
                    new WorkoutExercise
                    {
                        Weight = 100,
                        Repetitions = 1,
                        Name = "Bench Press"
                    }
                }
            },
            
            new Workout
            {
                Id = 2,
                StartDate = startDate.AddDays(1),
                EndDate = endDate.AddDays(1).AddHours(1),
                UserId = 2,
                WorkoutExercise = 
                {
                    new WorkoutExercise
                    {
                        Name = "Dead Lift",
                        Repetitions = 10,
                        Weight = 100
                    }
                }
            }

        };


        var queryMock = workout.BuildMock();
        
        
        _workoutRepositoryMoc.Setup(repository => repository.GetAllWorkout()).Returns(queryMock);
        
        var act = await _workoutServices.SearchWorkoutByData(startDate, endDate, page, pageSize, userId);
        
        
        act[0].Id.Should().Be(1);
        act[0].UserId.Should().Be(1);
        act[0].WorkoutExerciseShort.Should().ContainSingle();
        act[0].WorkoutExerciseShort[0].Name.Should().Be("Bench Press");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetAllWorkout(), Times.Once);
        
    }
    
    [Fact]
    public async Task UpdateMyWorkoutDate_WhenStartDateMoreThanOneYear_ShouldThrowBadRequestException()
    {
        int workoutId = 1, userId = 1;
        var startDate = DateTimeOffset.UtcNow;
        var endDate =  startDate.AddYears(2);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };
        
        
        var act = async () => await _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid time Data");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(workout), Times.Never);
        
        
    }

    [Fact]
    public async Task UpdateMyWorkoutDate_WhenStartDateLessThanMinusOneYear_ShouldThrowBadRequest()
    {
        int workoutId = 1, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddYears(-2);
        var endDate = startDate.AddHours(-10);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };
        
        var act = async () => await  _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid time Data");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(workout), Times.Never);

    }

    [Fact]
    public async Task UpdateMyWorkoutDate_WhenEndDateMoreThanOneYear_ShouldThrowBadRequest()
    {
        int  workoutId = 1, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddYears(1);
        
        var endDate = startDate.AddYears(2);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };
        
        var act = async () => await  _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid time Data");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(workout), Times.Never);
        
    }

    [Fact]
    public async Task UpdateMyWorkoutDate_WhenEndDateLessThanMinusOneYear_ShouldThrowBadRequest()
    {
        int  workoutId = 1, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddYears(-2);
        var endDate = startDate.AddHours(-2);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };
        
        var act = async () => await  _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid time Data");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(workout), Times.Never);
        
    }


    [Fact]
    public async Task UpdateMyWorkoutDate_WhenTrainingLongerThetOneDay_ShouldThrowBadRequestException()
    {
        int workoutId = 1, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        
        var endDate = startDate.AddDays(2);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };
        
        var act = async () => await  _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("The training session cannot last longer than one day");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(workout), Times.Never);
    }

    [Fact]
    public async Task UpdateMyWorkoutDate_WhenStartDateLaterThanEndDate_ShouldThrowBadRequestException()
    {
        int  workoutId = 1, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddYears(1);
        var endDate = DateTimeOffset.UtcNow.AddDays(1);
        
        
        var act = async () => await  _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("The start time cannot be later than the end time");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);   
        
    }


    [Fact]
    public async Task UpdateMyWorkoutDate_WhenWorkoutNotFound_ShouldThrowNotFoundException()
    {
        int workoutId = 1, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);
        

        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync((Workout?) null);
        
        var act = async () => await _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("Workout not found");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateMyWorkoutDate_WhenStatusIsCanceled_ShouldThrowBadRequestException()
    {
        int workoutId = 1, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        
        var endDate = startDate.AddHours(1);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId,
            Status = Status.Cancelled
        };
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync(workout);
        
        
        var act = async () => await  _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("You cannot change a canceled workout");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);
        
    }

    [Fact]
    public async Task UpdateMyWorkoutDate_WhenTimeConflict_ShouldThrowBadRequestException()
    {
        int  workoutId = 1, userId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        
        var endDate = startDate.AddHours(1);

        var workout = new Workout
        {   
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };
        
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync(workout);
        
        _workoutRepositoryMoc.Setup(repository => repository.WorkoutTimeCheck(userId, startDate, endDate, workoutId)).ReturnsAsync(true);
        
        var act = async () => await _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("You already have a workout scheduled for that time");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);
        
        
    }

    [Fact]
    public async Task UpdateMyWorkoutDate_WhenDataIsValid_ShouldUpdateMyWorkoutDate()
    {
        int  workoutId = 1, userId = 1;
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };
        
        
        _workoutRepositoryMoc.Setup(repository => repository.WorkoutTimeCheck(userId, startDate,endDate,workoutId)).ReturnsAsync(false);
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync(workout);
        
        var act = async () => await _workoutServices.UpdateMyWorkoutDate(workoutId, userId, startDate, endDate);

        await act.Should().NotThrowAsync();
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Once);
        
    }

    [Fact]
    public async Task UpdateMyWorkoutExercise_WhenWorkoutNotFound_ShouldThrowNotFoundException()
    {
        int workoutId = 1, userId = 1, workoutExerciseId = 1;
        

        var workoutExerciseRequestDto = new WorkoutExerciseRequestDto
        {
            Name = "Bench Press",
            Repetitions = 10,
            Weight = 50,
        };
        
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync((Workout?) null);
        
        var act = async () => await _workoutServices.UpdateMyWorkoutExcercise(workoutId, workoutExerciseId, userId, workoutExerciseRequestDto);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("Workout not found");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);
        
    }

    [Fact]
    public async Task UpdateMyWorkoutExercise_WhenWorkoutExerciseNotFound_ShouldThrowNotFoundException()
    {
        int userId = 1, workoutExerciseId = 1, workoutId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        
        var endDate = startDate.AddHours(1);
        
        var workoutExerciseRequestDto = new WorkoutExerciseRequestDto
        {
            Name = "Bench Press",
            Repetitions = 10,
            Weight = 50,
        };

        var workout = new Workout
        {   
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };

        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutExcerciseById(workoutId, workoutExerciseId))
            .ReturnsAsync((WorkoutExercise?) null);
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync(workout);
        
        var act = async () => await _workoutServices.UpdateMyWorkoutExcercise(workoutId, workoutExerciseId, userId, workoutExerciseRequestDto);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("Workout exercise not found");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);
        
        
    }

    [Theory]
    [InlineData("", 100 , 1)]
    [InlineData("Bench Pres", -1 , 10)]
    [InlineData("Bench Pres", 100 , -10)]
    public async Task UpdateMyWorkoutExercise_WhenWorkoutExerciseDataIsIncorrect_ShouldThrowBadRequestExceptions(string name , int weight, int repetitions)
    {

        int workoutId = 1, userId = 1, workoutExerciseId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        
        var endDate = startDate.AddHours(1);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };

        var workoutExercise = new WorkoutExercise
        {
            Weight = weight,
            Repetitions = repetitions,
            Name = name,
        };

        var workoutExerciseRequestDto = new WorkoutExerciseRequestDto
        {
            Weight = weight,
            Repetitions = repetitions,
            Name = name,
        };

        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutExcerciseById(workoutId, workoutExerciseId)).ReturnsAsync(workoutExercise);
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync(workout);
        

        var act = async () => await _workoutServices.UpdateMyWorkoutExcercise(workoutId,  workoutExerciseId, userId, workoutExerciseRequestDto);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid WorkoutExercise data");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkoutExcercise(It.IsAny<WorkoutExercise>()), Times.Never);


    }

    [Fact]
    public async Task UpdateMyWorkoutExercise_WhenDataIsValid_ShouldUpdateWorkoutExercise()
    {
        int workoutId = 1, userId = 1, workoutExerciseId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        
        var endDate = startDate.AddHours(1);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };

        var workoutExercise = new WorkoutExercise
        {
            Id = workoutExerciseId,
            Name = "Bench Press",
            Repetitions = 10,
            Weight = 50,
            WorkoutId = workoutId
        };

        var workoutExerciseRequestDto = new WorkoutExerciseRequestDto
        {
            Name = "Bench Press",
            Repetitions = 10,
            Weight = 100
        };


        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutExcerciseById(workoutId, workoutExerciseId)).ReturnsAsync(workoutExercise);
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync(workout);
        
        var act = async () => await _workoutServices.UpdateMyWorkoutExcercise(workoutId, workoutExerciseId, userId, workoutExerciseRequestDto);
        
        await act.Should().NotThrowAsync();
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkoutExcercise(It.IsAny<WorkoutExercise>()), Times.Once);
        
    }

    [Fact]
    public async Task DeleteMyWorkoutExercise_WhenWorkoutNotFound_ShouldNotFoundException()
    {
        int workoutId = 1, userId = 1 , workoutExerciseId = 1;
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync((Workout?) null);
        
        
        var act = async () => await _workoutServices.DeleteMyWorkoutExcercise(workoutId, workoutExerciseId,  userId);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("Workout not found");
        
        _workoutRepositoryMoc.Verify(repository => repository.DeleteMyWorkoutExercise(It.IsAny<WorkoutExercise>()), Times.Never );
    }

    [Fact]
    public async Task DeleteMyWorkoutExercise_WhenWorkoutExerciseNotFound_ShouldThrowNotFoundException()
    {
        int workoutId = 1, userId = 1 , workoutExerciseId = 1;

        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);
        
        var workout = new Workout
        {
            Id = workoutId,
            UserId =  userId,
            StartDate = startDate,
            EndDate = endDate
        };
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId) ).ReturnsAsync(workout);
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutExcerciseById(workoutId, workoutExerciseId)).ReturnsAsync((WorkoutExercise?) null);
        
        
        var act = async () => await _workoutServices.DeleteMyWorkoutExcercise(workoutId, workoutExerciseId, userId);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("WorkoutExercise not found");
        
        _workoutRepositoryMoc.Verify(repository => repository.DeleteMyWorkoutExercise(It.IsAny<WorkoutExercise>()), Times.Never);
    }

    [Fact]
    public async Task DeleteMyWorkoutExercise_WhenDataIsValid_ShouldDeleteMyWorkoutExercise()
    {
        int  workoutId = 1, userId = 1, workoutExerciseId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };

        var workoutExercise = new WorkoutExercise
        {
            Id = workoutExerciseId,
            Name = "Bench Press",
            Repetitions = 10,
            Weight = 50,
            WorkoutId = workoutId
        };
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId) ).ReturnsAsync(workout);
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutExcerciseById(workoutId, workoutExerciseId) ).ReturnsAsync(workoutExercise);
        
        var act = async () => await _workoutServices.DeleteMyWorkoutExcercise(workoutId, workoutExerciseId, userId);
        
        await act.Should().NotThrowAsync();
        
        _workoutRepositoryMoc.Verify(repository => repository.DeleteMyWorkoutExercise(It.IsAny<WorkoutExercise>()), Times.Once);

    }

    [Fact]
    public async Task AddMyWorkoutExercise_WhenWorkoutNotFound_ShouldThrowNotFoundException()
    {
        int workoutId = 1, userId = 1;

        var workoutExerciseRequestDto = new WorkoutExerciseRequestDto
        {
            Name = "Bench Press",
            Repetitions = 10,
            Weight = 50,
        };

        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId))
            .ReturnsAsync((Workout?)null);

        var act = async () => await _workoutServices.AddMyWorkoutExercise(workoutId, userId, workoutExerciseRequestDto);

        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("Workout not found");

        _workoutRepositoryMoc.Verify(repository => repository.AddMyWorkoutExercise(It.IsAny<WorkoutExercise>()),
            Times.Never);
    }

    [Theory]
    [InlineData("", 100, 10)]
    [InlineData("Bench Press", -10, 10)]
    [InlineData("Bench Press", 100, -1)]
    public async Task AddMyWorkoutExercise_WhenDataIsInvalid_ShouldThrowBadRequestException(string name, int weight, int repetitions)
    {
        int workoutId = 1, userId = 1;
    
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(1);
        
        var workout = new Workout
        {
            Id = workoutId,
            UserId =  userId,
            StartDate = startDate,
            EndDate = endDate
        };


        var workoutExerciseRequestDto = new WorkoutExerciseRequestDto
        {
            Name = name,
            Repetitions = repetitions,
            Weight = weight,
        };
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync(workout);
        
        
        var act = async () => await _workoutServices.AddMyWorkoutExercise(workoutId, userId, workoutExerciseRequestDto);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid data");
        
        _workoutRepositoryMoc.Verify(repository => repository.AddMyWorkoutExercise(It.IsAny<WorkoutExercise>()), Times.Never);

    }

    [Fact]
    public async Task AddMyWorkoutExercise_WhenDataIsValid_ShouldAddWorkoutExercise()
    {
        int workoutId = 1, userId = 1, workoutExerciseId = 1;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        
        var endDate = startDate.AddHours(1);

        var workout = new Workout
        {
            Id = workoutId,
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId
        };

        var workoutExercise = new WorkoutExercise
        {
            Id = workoutExerciseId,
            Name = "Bench Press",
            Repetitions = 10,
            Weight = 50,
            WorkoutId = workoutId
        };

        var workoutExerciseRequestDto = new WorkoutExerciseRequestDto
        {
            Name = "Bench Press",
            Repetitions = 10,
            Weight = 50,
        };
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync(workout);
        
        _workoutRepositoryMoc.Setup(repository => repository.AddMyWorkoutExercise(workoutExercise)).Returns(Task.CompletedTask);
        
        var act = async () => await _workoutServices.AddMyWorkoutExercise(workoutId, userId, workoutExerciseRequestDto);
        
        await act.Should().NotThrowAsync();
        
        _workoutRepositoryMoc.Verify(repository => repository.AddMyWorkoutExercise(It.IsAny<WorkoutExercise>()), Times.Once);

    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    public async Task SearchMyWorkoutByStatus_WhenInvalidPageOrPageSizeData_ShouldThrowBadRequestException(int page, int pageSize)
    {
        var status = Status.InProgres;

        int userId = 1;
        
        var act = async () => await _workoutServices.SearchMyWorkoutByStatus(status, userId, page, pageSize);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid Page or PageSize data");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetAllWorkout(), Times.Never);

    }

    [Fact]
    public async Task SearchMyWorkoutByStatus_WhenDataIsValid_ShouldSearchWorkouts()
    {
        var  status = Status.InProgres;
        
        int userId = 1, page = 1, pageSize = 10;
        
        
        var workout = new List<Workout>
        {
            new Workout()
            {
                Id = 1,
                UserId = 1,
                StartDate = DateTimeOffset.UtcNow.AddDays(1),
                EndDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                Status = Status.InProgres,
                WorkoutExercise = 
                {
                    new WorkoutExercise()
                    {
                        Name = "Bench Press",
                        Repetitions = 10,
                        Weight = 120,
                    }, 
                    new WorkoutExercise()
                    {
                        Name = "Pull Ups",
                        Repetitions = 15 ,
                        Weight = 10,
                    }
                }
            }, 
            new Workout()
            {
                Id = 2,
                UserId = 1,
                StartDate = DateTimeOffset.UtcNow.AddDays(1),
                EndDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                Status = Status.InProgres,
                WorkoutExercise = 
                {
                    new WorkoutExercise()
                    {
                        Name = "Bench Press",
                        Repetitions = 4,
                        Weight = 90,
                    }, 
                    new WorkoutExercise()
                    {
                        Name = "Push Ups",
                        Repetitions = 30,
                        Weight = 5,
                    }
                }
            },
            new  Workout()
            {
                Id = 3,
                UserId = 2,
                StartDate = DateTimeOffset.UtcNow.AddDays(1),
                EndDate = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                Status = Status.InProgres, 
                WorkoutExercise = 
                {
                    new  WorkoutExercise()
                    {
                        Name = "Bench Press",
                        Repetitions = 10,
                        Weight = 120,
                    },
                    new WorkoutExercise()
                    {
                        Name = "Pull Ups",
                        Repetitions = 30,
                        Weight = 5,
                    }
                }
            }
        };
        
        var queryMock = workout.BuildMock();

        _workoutRepositoryMoc.Setup(repository => repository.GetAllWorkout()).Returns(queryMock);


        var result = await _workoutServices.SearchMyWorkoutByStatus(status, userId, page, pageSize);

        result.Should().HaveCount(2);
        
        result[0].Id.Should().Be(1);
        result[0].UserId.Should().Be(1);
        result[0].WorkoutExerciseShort[0].Name.Should().Be("Bench Press");
        
        result[1].Id.Should().Be(2);
        result[1].UserId.Should().Be(1);
        result[1].WorkoutExerciseShort[0].Name.Should().Be("Bench Press");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetAllWorkout(), Times.Once);

    }

    [Fact]
    public async Task CancelMyWorkout_WhenWorkoutIsNotFound_ShouldThrowNotFoundException()
    {
        int userId = 1, workoutId = 1;
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId)).ReturnsAsync((Workout?) null);
        
        var act = async () => await _workoutServices.CancelMyWorkout(workoutId, userId);

        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("Workout not found");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);
    }
    
    [Fact]
    public async Task CancelMyWorkout_WhenWorkoutIsCompleted_ShouldThrowBadRequestException()
    {
        int userId = 1, workoutId = 1;
        
        var status = Status.Completed;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = DateTimeOffset.UtcNow.AddHours(1);

        var workout = new Workout()
        {   
            Id = 1,
            UserId = 1,
            StartDate = startDate,
            EndDate = endDate,
            Status = status
        };
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId) ).ReturnsAsync(workout);
        
        var act = async () => await _workoutServices.CancelMyWorkout(workoutId, userId);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("You cannot cancel a workout that has already been completed");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);

    }

    [Fact]
    public async Task CancelMyWorkout_WhenWorkoutIsCancelled_ShouldThrowBadRequestException()
    {
        int  userId = 1, workoutId = 1;
        
        var status = Status.Cancelled;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = DateTimeOffset.UtcNow.AddHours(1);

        var workout = new Workout()
        {   
            Id = 1,
            UserId = 1,
            StartDate = startDate,
            EndDate = endDate,
            Status = status
        };
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId) ).ReturnsAsync(workout);
        
        var act = async () => await _workoutServices.CancelMyWorkout(workoutId, userId);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Training has already been canceled");
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Never);
        
    }

    [Fact]
    public async Task CancelMyWorkout_WhenDataIsValid_ShouldUpdateMyWorkoutStatus()
    {
        int  userId = 1, workoutId = 1;
        
        var status = Status.InProgres;
        
        var startDate = DateTimeOffset.UtcNow.AddDays(1);
        var endDate = DateTimeOffset.UtcNow.AddHours(1);

        var workout = new Workout()
        {   
            Id = 1,
            UserId = 1,
            StartDate = startDate,
            EndDate = endDate,
            Status = status
        };
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByIdAndUser(userId, workoutId) ).ReturnsAsync(workout);
        
        var act = async () => await _workoutServices.CancelMyWorkout(workoutId, userId);
        
        await act.Should().NotThrowAsync();
        
        _workoutRepositoryMoc.Verify(repository => repository.UpdateMyWorkout(It.IsAny<Workout>()), Times.Once);
    }

    [Fact]
    public async Task GetMyExerciseByName_WhenNameIsEmpty_ShouldThrowBadRequestException()
    {
        string name = "";
        
        int page = 1, pageSize = 10, userId = 1;
        
        var act = async () => await _workoutServices.GetMyExerciseByName(name, page, pageSize, userId);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("The name cannot be empty");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetMyExerciseByName(name, userId), Times.Never);

    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    public async Task GetMyExerciseByName_WhenPageOrPageSizeInvalid_ShouldThrowBadRequestException(int page, int pageSize)
    {
        int userId = 1;
        
        string name = "Bench Press"; 
        
        var act = async () => await _workoutServices.GetMyExerciseByName(name, page, pageSize, userId);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid Page or PageSize data");
        
        _workoutRepositoryMoc.Verify(repository => repository.GetMyExerciseByName(name, userId), Times.Never);

    }

    [Fact]
         public async Task GetMyExerciseByName_WhenDataIsValid_ShouldGetMyWorkoutExercise()
         {
             int userId = 1, page = 1 , pageSize = 10;
             
             string name = "Push Ups";
     
             var workout = new Workout()
             {
                 Id = 1,
                 UserId = 1,
                 StartDate = DateTimeOffset.UtcNow.AddDays(1),
                 EndDate = DateTimeOffset.UtcNow.AddDays(2),
                 Status = Status.InProgres
             };
     
             var workoutExercise = new List<WorkoutExercise>()
             {
                 new WorkoutExercise()
                 {
                     Name = "Bench Press",
                     Repetitions = 10,
                     Weight = 100,
                     Workout = workout
                 },
                 new  WorkoutExercise()
                 {
                     Name =  "Pull Ups",
                     Repetitions = 15,
                     Weight = 20,
                     Workout = workout
                 },
                 new   WorkoutExercise()
                 {
                     Name =  "Push Ups",
                     Repetitions = 20,
                     Weight = 25,
                     Workout = workout 
                 }
             };
     
             var queryMock = workoutExercise.BuildMock();
     
             _workoutRepositoryMoc
                 .Setup(repo => repo.GetMyExerciseByName(name, userId))
                 .Returns((string n, int u) =>
                 {
                     var filtered = queryMock
                         .Where(x => x.Workout != null &&
                                     x.Workout.UserId == u &&
                                     x.Name != null &&
                                     x.Name.ToLower() == n.ToLower())
                         .AsQueryable();
                     return filtered;
                 });
             
             var result = await _workoutServices.GetMyExerciseByName(name, page, pageSize, userId);
             
             result.Should().HaveCount(1);
             
             result[0].Name.Should().Be("Push Ups");
             result[0].Repetitions.Should().Be(20);
             result[0].Weight.Should().Be(25);
             
             _workoutRepositoryMoc.Verify(repository => repository.GetMyExerciseByName(name, userId), Times.Once);
     
         }

    [Fact]
    public async Task GetWorkoutStats_WhenDataIsValid_ShouldGetWorkoutStats()
    {
        int userId = 1;
        
        var workouts = new List<Workout>
        {
            new Workout { Id = 1, UserId = userId, Status = Status.Completed },
            new Workout { Id = 2, UserId = userId, Status = Status.Completed },
            new Workout { Id = 3, UserId = userId, Status = Status.Cancelled },
            new Workout { Id = 4, UserId = userId, Status = Status.InProgres },
            new Workout { Id = 5, UserId = userId, Status = Status.Planned }
        };
        
        var exercises = new List<WorkoutExercise>
        {
            new WorkoutExercise { Name = "Push Ups", Weight = 50, WorkoutId = 1 },
            new WorkoutExercise { Name = "Push Ups", Weight = 80, WorkoutId = 2 },
            new WorkoutExercise { Name = "Push Ups", Weight = 100, WorkoutId = 3 },
            new WorkoutExercise { Name = "Pull Ups", Weight = 60, WorkoutId = 4 }
        };
        
        var queryWorkoutMock = workouts.BuildMock();
        
        var queryWorkoutExerciseMock = exercises.BuildMock();
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutByStatistics(userId)).Returns(queryWorkoutMock);
        
        _workoutRepositoryMoc.Setup(repository => repository.GetWorkoutExerciseByStatistics(userId)).Returns(queryWorkoutExerciseMock);


        var result = await _workoutServices.GetWorkoutsStats(userId);
        
        result.TotalWorkout.Should().Be(5);
        result.MaxWeight.Should().Be(100);
        result.AvarageExercisPerWorkout.Should().Be(0.8);
        result.MostCommonExercise.Should().Be("Push Ups");
        result.TotalCanceledWorkout.Should().Be(1);
        result.TotalComplateWorkout.Should().Be(2);
        result.TotalInProgresWorkout.Should().Be(1);
        result.TotalPlannedWorkout.Should().Be(1);
        result.TotalWorkoutExercise.Should().Be(4);
        
        _workoutRepositoryMoc.Verify(repository => repository.GetWorkoutByStatistics(userId), Times.Once);
        
        _workoutRepositoryMoc.Verify(repository => repository.GetWorkoutExerciseByStatistics(userId), Times.Once);


    }
    
         
         
    

}