using Microsoft.EntityFrameworkCore;
using Training_Api.DtoModels;
using Training_Api.Exceptions;
using Training_Api.Interface;

namespace Training_Api.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;

        public UserServices(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public List<UserReadDto> GetAllUser(int Page , int PageSize)
        {
            if (Page < 1 || PageSize < 1 || PageSize > 10000)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            var listUser = _userRepository.GetAllUser();

            var res  = listUser.Select(u => new UserReadDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Role = u.Role
            }).Skip((Page - 1) * PageSize).Take(PageSize).ToList();

            return res;
        }

        public async Task<UserReadDto> GetUserById(int userId)
        {
            if (userId < 1)
                throw new BadRequestExceptions("Invalid userId data");

            var res = await _userRepository.GetUserById(userId);

            if (res == null)
                throw new NotFoundExceptions("User not found");

            var user = new UserReadDto
            {
                Id = res.Id,
                Name = res.Name,
                Email = res.Email,
                Role = res.Role,
                Workouts = res.Workouts.Select(w => new WorkoutReadDto
                {
                    Id= w.Id,
                    Date = w.Date,
                    User = w.User,
                    WorkoutExerciseShort = w.WorkoutExercise.Select(we => new WorkoutExerciseShortDto
                    {
                        Id = we.Id,
                        Name = we.Name,
                        Repetitions = we.Repetitions,
                        Weight = we.Weight
                    }).ToList()
                }).ToList()
                
            };

            return user;

        }

        public async Task DeleteUser(int userId)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
                throw new NotFoundExceptions("User not found");

            await _userRepository.DeleteUser(user);
        }

        public async Task GiveRoleAdmin(int userId)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
                throw new NotFoundExceptions("User not found");

            user.Role = Role.Role.Admin;

            await _userRepository.GiveRoleAdmin(user);
        }

        public async Task GiveRoleUser(int userId)
        {
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
                throw new NotFoundExceptions("User not found");

            user.Role = Role.Role.User;

            await _userRepository.GiveRoleUser(user);
        }
    }
}
