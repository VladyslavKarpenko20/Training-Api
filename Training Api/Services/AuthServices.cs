using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Training_Api.DtoModels;
using Training_Api.Exceptions;
using Training_Api.Interface;
using Training_Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace Training_Api.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IUserRepository _userRepository;

        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly IConfiguration _configuration;

        public AuthServices(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task Registr(RegistrDto registrDto)
        {
            if (await _userRepository.SearchUserByEmail(registrDto.Email) != null)
                throw new ConflictExceptions("This email already exists");


            if (await _userRepository.SearchUserByName(registrDto.Name) != null)
                throw new ConflictExceptions("This name already exists");


            var user = new User
            {
                Email = registrDto.Email,
                Name = registrDto.Name,
                Role = Role.Role.User
            };

            user.Password = _passwordHasher.HashPassword(user, registrDto.Password);


            await _userRepository.AddUser(user);
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _userRepository.SearchUserByEmail(loginDto.Email);

            if (user == null)
                throw new NotFoundExceptions("User not found");

            if (_passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password) == PasswordVerificationResult.Failed)
                throw new UnAuthorizeExceptions("Uncorrect Password");

            return GenerateAcsesToken(user);

        }

        public string GenerateAcsesToken(User user)
        {

            var secretKey = _configuration["JWTSetting:SecretKey"] ?? "1234";

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
