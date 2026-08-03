using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Training_Api.Interface;
using Training_Api.Services;
using Microsoft.Extensions.Configuration;
using Training_Api.DtoModels;
using Training_Api.Enums;
using Training_Api.Exceptions;
using Training_Api.Models;

namespace TrainingApi.Tests.Services;

public class AuthServicesTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    
    private readonly IAuthServices _authServices;
    
    
    
    public  AuthServicesTests()
    {
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JWTSetting:SecretKey"] = "SuperSecretKey478392019384422342134454",
        })
            .Build();

        var passwordHasher = new PasswordHasher<User>();

        
        _userRepositoryMock = new Mock<IUserRepository>();

        _authServices = new AuthServices(_userRepositoryMock.Object, passwordHasher, configuration);

    }

    [Fact]
    public async Task Register_WhenEmailIsExist_ShouldThrowConflictExceptions()
    {
        string email = "Jon@gmail.com";

        var regDto = new RegistrDto()
        {
            Email = email,
            Password = "Password",
            Name =  "Jon",
        };
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByEmail(email)).ReturnsAsync(new  User());
        
        var act = async () => await _authServices.Register(regDto);
     
        await act.Should().ThrowAsync<ConflictExceptions>().WithMessage("This email already exists");
        
        _userRepositoryMock.Verify(repository => repository.SearchUserByEmail(email), Times.Once);
        
    }

    [Fact]
    public async Task Register_WhenNameIsExist_ShouldThrowConflictExceptions()
    {
        string name = "Jon";
        var regDto = new RegistrDto()
        {
            Name = name,
            Password = "Password",
            Email = "Jon@gmail.com"
        };
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByName(name)).ReturnsAsync(new  User());
        
        var act = async () => await _authServices.Register(regDto);
        
        await act.Should().ThrowAsync<ConflictExceptions>().WithMessage("This name already exists");
        
        _userRepositoryMock.Verify(repository => repository.SearchUserByName(name), Times.Once);
    }

    [Fact]
    public async Task Register_WhenDataIsValid_ShouldRegisterUser()
    {
        var regDto = new RegistrDto()
        {
            Email = "Steve@gmail.com",
            Name = "Steve",
            Password = "Password",
        };
        
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByEmail(regDto.Email)).ReturnsAsync((User?) null);
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByName(regDto.Name)).ReturnsAsync((User?) null);
        
        _userRepositoryMock.Setup(repository => repository.AddUser(It.IsAny<User>())).Returns(Task.CompletedTask);
        
        var act = async () => await _authServices.Register(regDto);
        
        await act.Should().NotThrowAsync();
        
        _userRepositoryMock.Verify(repository => repository.AddUser(It.IsAny<User>()), Times.Once);
        
        _userRepositoryMock.Verify(repository => repository.SearchUserByEmail(regDto.Email), Times.Once);
        
        _userRepositoryMock.Verify(repository => repository.SearchUserByName(regDto.Name), Times.Once);
        
    }


    [Theory]
    [InlineData(null, "Password")]
    [InlineData("Jon", null)]
    [InlineData(null, null)]
    public async Task Login_WhenEmailOrPasswordIsMissing_ShouldBedRequestExceptions(string? email, string? password)
    {

        var loginDto = new LoginDto()
        {
            Email = email,
            Password = password
        };

        var act = async () => await _authServices.Login(loginDto);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Email or password is missing");
        
    }


    [Fact]
    public async Task Login_WhenUserNotFound_ShouldBedRequestExceptions()
    {
        
        var loginDto = new LoginDto()
        {
            Email = "Jon@gmail.com",
            Password = "password123"
        };
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByEmail(loginDto.Email)).ReturnsAsync((User?) null);
        
        var act = async () => await _authServices.Login(loginDto);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("User or password is missing");

    }

    [Fact]
    public async Task Login_WhenPasswordIsMissing_ShouldBedRequestExceptions()
    {
        var loginDto = new LoginDto()
        {
            Email = "Steve@gmail.com",
            Password = "password123"
        };

        var user = new User()
        {
            Id = 1,
            Name = "Jon",
            Email = "Jon@gmail.com",
            Password = null
        };
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByEmail(loginDto.Email)).ReturnsAsync(user);
        
        var act = async () => await _authServices.Login(loginDto);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("User or password is missing");

    }


    [Fact]
    public async Task Login_WhenIncorrectPassword_ShouldAnUnauthorizedException()
    {
        var passwordHasher = new PasswordHasher<User>();
        
        var loginDto = new LoginDto()
        {
            Email = "Steve@gmail.com",
            Password = "password1234"
        };

        var user = new User()
        {
            Id = 1,
            Name = "Jon",
            Email = "Jon@gmail.com"
        };
        
        user.Password = passwordHasher.HashPassword(user, "password123");
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByEmail(loginDto.Email)).ReturnsAsync(user);
        
        var act = async () => await _authServices.Login(loginDto);
        
        await act.Should().ThrowAsync<UnAuthorizeExceptions>().WithMessage("Incorrect password");
    }

    [Fact]
    public async Task Login_WhenDataIsValid_ShouldGetToken()
    {
        var passwordHasher = new PasswordHasher<User>();
        
        var loginDto = new LoginDto()
        {
            Email = "Steve@gmail.com",
            Password = "password123"
        };

        var user = new User()
        {
            Id = 1,
            Name = "Jon",
            Email = "Steve@gmail.com"
        };
        
        user.Password = passwordHasher.HashPassword(user, "password123");
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByEmail(loginDto.Email)).ReturnsAsync(user);
        
        var token = await _authServices.Login(loginDto);
        
        token.Should().NotBeNull();
        
        _userRepositoryMock.Verify(repository => repository.SearchUserByEmail(loginDto.Email), Times.Once);
        
    }

    [Theory]
    [InlineData(null, "Password")]
    [InlineData("Jon@gmail.com", null)]
    [InlineData(null, null)]
    public async Task Login_WhenEmailOrNameIsMissing_ShouldBedRequestExceptions(string? email, string? name)
    {
        var loginDto = new LoginDto()
        {
            Email = "Jon@gmail.com",
            Password = "password123"
        };

        var passwordHasher = new PasswordHasher<User>();
        
        var user = new User()
        {
            Id = 1,
            Name = name,
            Email = email,
            Role = Role.User
        };
        
        user.Password = passwordHasher.HashPassword(user, loginDto.Password);
        
        _userRepositoryMock.Setup(repository => repository.SearchUserByEmail(loginDto.Email)).ReturnsAsync(user);
        
        
        var act = async () => await _authServices.Login(loginDto);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("Email or Name is missing");
        
        _userRepositoryMock.Verify(repository => repository.SearchUserByEmail(loginDto.Email), Times.Once);
    }

    [Fact]
    public async Task Login_WhenSecretKeyIsMissing_ShouldBedRequestExceptions()
    {
        var loginDto = new LoginDto()
        {
            Email = "Jon@gmail.com",
            Password = "password123"
        };

        var passwordHasher = new PasswordHasher<User>();
        
        var user = new User()
        {
            Id = 1,
            Name = "Jon",
            Email = "Jon@gmail.com",
            Role = Role.User
        };
        
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JWTSetting:SecretKey"] = null
        })
            .Build();
        
        
        var authServices = new AuthServices(_userRepositoryMock.Object, passwordHasher, config);
        
        user.Password = passwordHasher.HashPassword(user,  loginDto.Password);

        
        _userRepositoryMock.Setup(repository => repository.SearchUserByEmail(loginDto.Email)).ReturnsAsync(user);
        
        
        var act = async () => await authServices.Login(loginDto);
        
        await act.Should().ThrowAsync<BadRequestExceptions>().WithMessage("JWTSetting:SecretKey is missing");
        
        _userRepositoryMock.Verify(repository => repository.SearchUserByEmail(loginDto.Email), Times.Once);
        
    }
    
    
}