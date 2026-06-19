using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Training_Api.DtoModels;
using Training_Api.Interface;
using Training_Api.Services;

namespace Training_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;


        public AuthController(IAuthServices authServices )
        {
            _authServices = authServices;
        }

        [HttpPost("Registr")]
        public async Task<IActionResult> Registr(RegistrDto registrDto)
        {
            await _authServices.Registr(registrDto);

            return Ok();
        }

        [HttpPut("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _authServices.Login(loginDto);

            return Ok(token);
        }
    }
}
