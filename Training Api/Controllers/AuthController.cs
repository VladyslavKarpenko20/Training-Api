using Microsoft.AspNetCore.Mvc;
using Training_Api.DtoModels;
using Training_Api.Interface;

namespace Training_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthServices authServices) : ControllerBase
    {
        [HttpPost("Registr")]
        public async Task<IActionResult> Registr(RegistrDto registrDto)
        {
            await authServices.Register(registrDto);

            return Ok();
        }

        [HttpPut("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await authServices.Login(loginDto);

            return Ok(token);
        }
    }
}
