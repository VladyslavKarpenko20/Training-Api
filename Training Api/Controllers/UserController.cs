using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training_Api.Interface;
using Training_Api.Enums;

namespace Training_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserServices userServices) : ControllerBase
    {
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpGet("Get/All/User/{page:int}/{pageSize:int}")]
        public IActionResult GetAllUser(int page = 1, int pageSize = 10)
        {
            var listUser = userServices.GetAllUser(page, pageSize);

            return Ok(listUser);

        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpGet("Get/User/By/Id/{userId:int}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var res = await userServices.GetUserById(userId);

            return Ok(res);
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("Delete/User/By/Id/{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await userServices.DeleteUser(userId);

            return Ok();
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("Give/Role/Admin/{userId:int}")]
        public async Task<IActionResult> GiveRoleAdmin(int userId)
        {
            await userServices.GiveRoleAdmin(userId);

            return Ok();
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("Give/Role/User/{userId:int}")]
        public async Task<IActionResult> GiveRoleUser(int userId)
        {
            await userServices.GiveRoleUser(userId);

            return Ok();
        }
    }
}
