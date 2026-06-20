using Training_Api.DtoModels;

namespace Training_Api.Interface
{
    public interface IUserServices
    {
        List<UserReadDto> GetAllUser(int Page, int PageSize);

        Task<UserReadDto> GetUserById(int userId);

        Task DeleteUser(int userId);

        Task GiveRoleAdmin(int userId);

        Task GiveRoleUser(int userId);
    }
}
