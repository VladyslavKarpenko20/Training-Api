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
        public IActionResult GetAllUser(int Page, int PageSize)
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

        [Authorize(Roles = nameof(Role.Role.Admin))]
        [HttpDelete("Delete/User/By/Id/{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _userServices.DeleteUser(userId);

            return Ok();
        }

        [Authorize(Roles = nameof(Role.Role.Admin))]
        [HttpPut("Give/Role/Admin/{userId:int}")]
        public async Task<IActionResult> GiveRoleAdmin(int userId)
        {
            await _userServices.GiveRoleAdmin(userId);

            return Ok();
        }

        [Authorize(Roles = nameof(Role.Role.Admin))]
        [HttpPut("Give/Role/User/{userId:int}")]
        public async Task<IActionResult> GiveRoleUser(int userId)
        {
            await _userServices.GiveRoleUser(userId);

            return Ok();
        }
    }
}
