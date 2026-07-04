using Training_Api.DtoModels;
using Training_Api.Exceptions;
using Training_Api.Interface;
using Training_Api.Enums; 

namespace Training_Api.Services
{
    public class UserServices(IUserRepository userRepository) : IUserServices
    {
        public List<UserReadDto> GetAllUser(int page , int pageSize)
        {
            if (page < 1 || pageSize < 1 || pageSize > 10000)
                throw new BadRequestExceptions("Invalid Page or PageSize data");

            var listUser = userRepository.GetAllUser();

            var res  = listUser.Select(u => new UserReadDto
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                Role = u.Role
            }).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return res;
        }

        public async Task<UserReadDto> GetUserById(int userId)
        {
            if (userId < 1)
                throw new BadRequestExceptions("Invalid userId data");

            var res = await userRepository.GetUserById(userId);

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
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    UserId = w.UserId,
                    WorkoutExerciseShort = w.WorkoutExercise.Select(we => new WorkoutExerciseShortDto
                    {
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
            var user = await userRepository.GetUserById(userId);

            if (user == null)
                throw new NotFoundExceptions("User not found");

            await userRepository.DeleteUser(user);
        }

        public async Task GiveRoleAdmin(int userId)
        {
            var user = await userRepository.GetUserById(userId);

            if (user == null)
                throw new NotFoundExceptions("User not found");

            user.Role = Role.Admin;

            await userRepository.GiveRoleAdmin(user);
        }

        public async Task GiveRoleUser(int userId)
        {
            var user = await userRepository.GetUserById(userId);

            if (user == null)
                throw new NotFoundExceptions("User not found");

            user.Role = Role.User;

            await userRepository.GiveRoleUser(user);
        }
    }
}
