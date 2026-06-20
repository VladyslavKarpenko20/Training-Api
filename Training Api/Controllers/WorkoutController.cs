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
    }
}
