using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Training_Api.DtoModels;
using Training_Api.Exceptions;
using Training_Api.Interface;
using Training_Api.Models;
using Microsoft.IdentityModel.Tokens;
using Training_Api.Enums;

namespace Training_Api.Services
{
    public class AuthServices(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
        : IAuthServices
    {
        public async Task Register(RegistrDto registrDto)
        {
            if (await userRepository.SearchUserByEmail(registrDto.Email) != null)
                throw new ConflictExceptions("This email already exists");


            if (await userRepository.SearchUserByName(registrDto.Name) != null)
                throw new ConflictExceptions("This name already exists");


            var user = new User
            {
                Email = registrDto.Email,
                Name = registrDto.Name,
                Role = Role.User
            };

            user.Password = passwordHasher.HashPassword(user, registrDto.Password);


            await userRepository.AddUser(user);
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            if (loginDto.Email == null || loginDto.Password == null)
                throw new BadRequestExceptions("Email or password is missing");
            
            var user = await userRepository.SearchUserByEmail(loginDto.Email);

            if (user == null || user.Password == null )
                throw new BadRequestExceptions("User or password is missing"); 

            if (passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password) == PasswordVerificationResult.Failed)
                throw new UnAuthorizeExceptions("Incorrect password");

            return GenerateAcsessToken(user);

        }

        private string GenerateAcsessToken(User user)
        {
            if (user.Email == null || user.Name == null)
                throw new BadRequestExceptions("Email or Name is missing");
                
            var secretKey = configuration["JWTSetting:SecretKey"] ?? throw new  BadRequestExceptions("JWTSetting:SecretKey is missing") ;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));


            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken
            (
               issuer: "MyApp",
               audience: "User",
               claims: claims,
               expires: DateTime.UtcNow.AddHours(1),
               signingCredentials: credential

            );

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();


            return tokenHandler.WriteToken(token);


        }


    }
}
