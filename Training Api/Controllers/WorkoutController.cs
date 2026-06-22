using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Training_Api.DtoModels;
using Training_Api.Interface;

namespace Training_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutServices _services;

        public WorkoutController(IWorkoutServices services)
        {
            _services = services;
        }

        [Authorize]
        [HttpPost("Add/Workout")]
        public async Task<IActionResult> AddWorkout([FromBody] WorkoutWriteDto workoutWrite)
        {
            string userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userid, out var id))
            {
                {
                    await _services.AddWorkout(workoutWrite, id);

                    return Ok();
                }

            }
            else
                return Unauthorized("Failed to identify user from token");
        }

        [Authorize]
        [HttpGet("Get/My/Workout")]
        public async Task<IActionResult> GetMyWorkout()
        {
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out int res))
            {
                var list = await _services.GetMyWorkout(res);

                return Ok(list);
            }
            else
                return Unauthorized("Failed to identify user from token");
        }

        [Authorize(Roles = nameof(Role.Role.Admin))]
        [HttpGet("Get/All/Workout/{Page:int}/{PageSize:int}")]
        public IActionResult GetAllWorkout(int Page = 1, int PageSize = 10)
        {
            var listWorkout = _services.GetAllWorkout(Page, PageSize);

            return Ok(listWorkout);
        }

        [Authorize]
        [HttpDelete("Delete/My/Workout/{workoutId:int}")]
        public async Task<IActionResult> DeleteMyWorkout(int workoutId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out int result))
            {
                await _services.DeleteMyWorkout(workoutId, result);

                return Ok();
            }
            else
                return Unauthorized("Failed to identify user from token");
        }

        [Authorize(Roles = nameof(Role.Role.Admin))]
        [HttpDelete("Delete/Workout/{userId:int}/{workoutId:int}")]
        public async Task<IActionResult> DeleteWorkout(int userId, int workoutId)
        {
            await _services.DeleteMyWorkout(workoutId, userId);

            return Ok();
        }
    }
}
