using Training_Api.DtoModels;

namespace Training_Api.Interface
{
    public interface IAuthServices
    {
        Task Registr(RegistrDto registrDto);

        Task<string> Login(LoginDto loginDto);
    }
}
