using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Training_Api.Interface;

namespace Training_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [Authorize(Roles = nameof(Role.Role.Admin))]
        [HttpGet("Get/All/User/{Page:int}/{PageSize:int}")]
        public async Task<IActionResult> GetAllUser(int Page, int PageSize)
        {
            var listUser = _userServices.GetAllUser(Page, PageSize);

            return Ok(listUser);

        }

        [Authorize(Roles = nameof(Role.Role.Admin))]
        [HttpGet("Get/User/By/Id/{userId:int}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var res = await _userServices.GetUserById(userId);

            return Ok(res);
        }
    }
}
