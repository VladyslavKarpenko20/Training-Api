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
    }
}
