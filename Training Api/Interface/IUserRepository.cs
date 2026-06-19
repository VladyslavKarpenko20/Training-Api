using Training_Api.Models;

namespace Training_Api.Interface
{
    public interface IUserRepository
    {

        Task<User?> SearchUserByEmail(string email);

        Task<User?> SearchUserByName(string name);

        Task AddUser(User user);
    }
}
