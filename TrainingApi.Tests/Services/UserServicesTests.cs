using FluentAssertions;
using MockQueryable;
using Training_Api.Interface;
using Moq;
using Training_Api.Enums;
using Training_Api.Exceptions;
using Training_Api.Models;
using Training_Api.Services;


namespace TrainingApi.Tests.Services;

public class UserServicesTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private  readonly IUserServices _userServices;
    
    public  UserServicesTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userServices = new UserServices(_mockUserRepository.Object);
    }


    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -1)]
    [InlineData(1, 10001)]
    public void GetAllUsers_WhenInvalidPageOrPageSizeData_ShouldThrowBadRequestException(int page, int pageSize)
    {
        
        var act = () =>  _userServices.GetAllUser(page, pageSize);
        
        act.Should().Throw<BadRequestExceptions>().WithMessage("Invalid Page or PageSize data");
        
        _mockUserRepository.Verify(x => x.GetAllUser(), Times.Never);

        
    }

    [Fact]
    public void GetAllUser_WhenDataIsValid_ShouldReturnAllUsers()
    {
        int page = 1,  pageSize = 10;


        var users = new List<User>()
        {
            new User()
            {
                Id = 1,
                Name = "Daniel",
                Email = "Daniel123@gmail.com",
                Role = Role.User,
                Password = "123456",
            },
            new User()
            {
                Id = 2,
                Name = "Josh",
                Email = "Josh@gmail.com",
                Role = Role.User, 
                Password = "Josh123",
            },
            new User()
            {
                Id = 3,
                Name = "Michael",
                Email = "Michel@gmail.com",
                Role = Role.Admin,
                Password = "Michel123",
            },
            new User()
            {
                Id = 4,
                Name = "James",
                Email = "James@gmail.com",
                Role = Role.User,
                Password = "James123",
            }
        };

        var queryMock = users.BuildMock();
        
        _mockUserRepository.Setup(repository => repository.GetAllUser()).Returns(queryMock);
        
        var result = _userServices.GetAllUser(page, pageSize);

        result.Should().HaveCount(4);
        
        result[0].Id.Should().Be(1);
        result[0].Name.Should().Be("Daniel");
        result[0].Email.Should().Be("Daniel123@gmail.com");
        
        result[1].Id.Should().Be(2);
        result[1].Name.Should().Be("Josh");
        result[1].Email.Should().Be("Josh@gmail.com");

        result[2].Id.Should().Be(3);
        result[2].Name.Should().Be("Michael");
        result[2].Email.Should().Be("Michel@gmail.com");
        
        result[3].Id.Should().Be(4);
        result[3].Name.Should().Be("James");
        result[3].Email.Should().Be("James@gmail.com");
        
        
        _mockUserRepository.Verify(x => x.GetAllUser(), Times.Once);
        

    }

    [Fact]
    public async Task GetUserById_WhenInvalidUserId_ShouldThrowBadRequestException()
    {
        int userId = -1;
        
        var act = async () => await _userServices.GetUserById(userId);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Invalid userId data");
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Never);
    }

    [Fact]
    public async Task GetUserById_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        int userId = 1;
        
        _mockUserRepository.Setup(repository => repository.GetUserById(userId)).ReturnsAsync((User?) null);
        
        var act = async () => await _userServices.GetUserById(userId);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("User not found");
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);

    }

    [Fact]
    public async Task GetUserById_WhenDataIsValid_ShouldReturnUser()
    {
        int userId = 1;

        var user = new User()
        {
            Id = 1,
            Email = "Bob@gmail.com",
            Role = Role.User,
            Name = "Bob",
            Password = "Bob123",
        };
        
        _mockUserRepository.Setup(repository => repository.GetUserById(userId)).ReturnsAsync(user);
        
        var act = async () => await _userServices.GetUserById(userId);
        
        await act.Should().NotThrowAsync();
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);

    }

    [Fact]
    public async Task DeleteUser_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        int  userId = 1;
        
        _mockUserRepository.Setup(repository => repository.GetUserById(userId) ).ReturnsAsync((User?) null);
        
        var act = async () => await _userServices.DeleteUser(userId);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("User not found");
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
        
        _mockUserRepository.Verify(repository => repository.DeleteUser(It.IsAny<User>()), Times.Never );
    }

    [Fact]
    public async Task DeleteUser_WhenUserDeleted_ShouldDeleteUser()
    {
        int userId = 1;
        
        
        var user = new User()
        {
            Id = 1,
            Email = "Josh@gmail.com",
            Role = Role.User,
            Name = "Josh",
            Password = "Josh123",
        };
        
        _mockUserRepository.Setup(repository => repository.GetUserById(userId)).ReturnsAsync(user);
        
        _mockUserRepository.Setup(repository => repository.DeleteUser(user)).Returns(Task.CompletedTask);
        
        var act = async () => await _userServices.DeleteUser(userId);
        
        await act.Should().NotThrowAsync();
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
        
        _mockUserRepository.Verify(repository => repository.DeleteUser(user), Times.Once);

    }
    
    [Fact]
    public async Task GiveRoleAdmin_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        int  userId = 1;
        
        _mockUserRepository.Setup(repository => repository.GetUserById(userId)).ReturnsAsync((User?) null);
        
        var act = async () => await _userServices.GiveRoleAdmin(userId);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("User not found");
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
    }

    [Fact]
    public async Task GiveRoleAdmin_WhenDataIsValid_ShouldGiveRoleAdmin()
    {
        int userId = 1;
        
        var user = new User()
        {
            Id = 1,
            Email = "Jon@gmail.com",
            Role = Role.User,
            Name = "Jon",
            Password = "Jon123",
        };
        
        _mockUserRepository.Setup(repository => repository.GetUserById(userId)).ReturnsAsync(user);
        
        _mockUserRepository.Setup(repository => repository.GiveRoleAdmin(user) ).Returns(Task.CompletedTask);
        
        var act = async () => await _userServices.GiveRoleAdmin(userId);
        
        await act.Should().NotThrowAsync();
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
        
        _mockUserRepository.Verify(repository => repository.GiveRoleAdmin(user), Times.Once);
        
    }

    [Fact]
    public async Task GiveRoleUser_WheUserNotFound_ShouldThrowNotFoundException()
    {
        int  userId = 1;
        
        _mockUserRepository.Setup(repository => repository.GetUserById(userId)).ReturnsAsync((User?) null);
        
        var act = async () => await _userServices.GiveRoleUser(userId);
        
        await act.Should().ThrowAsync<NotFoundExceptions>().WithMessage("User not found");
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
    }


    [Fact]
    public async Task GiveRoleUser_WhenDataIsValid_ShouldGiveRoleUser()
    {
        int userId = 1;
        
        var user = new User()
        {
            Id = 1,
            Email = "Ben@gmail.com",
            Role = Role.User,
            Name = "Ben",
            Password = "Ben123",
        };
        
        _mockUserRepository.Setup(repository => repository.GetUserById(userId)).ReturnsAsync(user);
        
        _mockUserRepository.Setup(repository => repository.GiveRoleUser(user) ).Returns(Task.CompletedTask);
        
        var act = async () => await _userServices.GiveRoleUser(userId);
        
        await act.Should().NotThrowAsync();
        
        _mockUserRepository.Verify(x => x.GetUserById(userId), Times.Once);
        
        _mockUserRepository.Verify(repository => repository.GiveRoleUser(user), Times.Once);
    }
    
}