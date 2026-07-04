using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Training_Api.DtoModels;
using Training_Api.Enums;
using Training_Api.Interface;

namespace Training_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutController(IWorkoutServices services) : ControllerBase
    {
        [Authorize]
        [HttpPost("Add/Workout")]
        public async Task<IActionResult> AddWorkout([FromBody] WorkoutWriteDto workoutWrite)
        {
            var userid = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userid, out var id))
            {
                {
                    await services.AddWorkout(workoutWrite, id);

                    return Ok();
                }

            }
            else
                return Unauthorized("Failed to identify user from token");
        }

        [Authorize]
        [HttpGet("Get/My/Workout/{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetMyWorkout(int page = 1, int pageSize = 10)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out int res))
            {
                var list = await services.GetMyWorkout(res, page, pageSize);

                return Ok(list);
            }
            else
                return Unauthorized("Failed to identify user from token");
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpGet("Get/All/Workout/{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetAllWorkout(int page = 1, int pageSize = 10)
        {
            var listWorkout = await services.GetAllWorkout(page, pageSize);

            return Ok(listWorkout);
        }

        [Authorize]
        [HttpDelete("Delete/My/Workout/{workoutId:int}")]
        public async Task<IActionResult> DeleteMyWorkout(int workoutId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out int result))
            {
                await services.DeleteMyWorkout(workoutId, result);

                return Ok();
            }
            else
                return Unauthorized("Failed to identify user from token");
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("Delete/Workout/{userId:int}/{workoutId:int}")]
        public async Task<IActionResult> DeleteMyWorkout(int userId, int workoutId)
        {
            await services.DeleteMyWorkout(workoutId, userId);

            return Ok();
        }

        [Authorize]
        [HttpGet("Get/Workout/By/Date")]
        public async Task<IActionResult> GetWorkoutByDate([FromQuery] DateTimeOffset? startDate, [FromQuery] DateTimeOffset? endDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 10 )
        {
            var listWorkout = await services.SearchWorkoutByData(startDate, endDate, page, pageSize);

            return Ok(listWorkout);

        }

        [Authorize]
        [HttpPut("Update/My/Workout/Date")]
        public async Task<IActionResult> UpdateMyWorkoutDate([FromQuery] int workoutId,[FromQuery] DateTimeOffset startDate, [FromQuery] DateTimeOffset endDate)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out int result))
                return Unauthorized("Failed to identify user from token");


            await services.UpdateMyWorkoutDate(workoutId, result, startDate, endDate);

            return Ok();
        }

        [Authorize]
        [HttpPut("Update/Workout/Excercise/{workoutId:int}/{workoutExcerciseId:int}")]
        public async Task<IActionResult> UpdateMyWorkoutExcercise(int workoutId, int workoutExcerciseId , WorkoutExerciseRequestDto updateWorkout)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out int result))
                return Unauthorized("Failed to identify user from token");

            await services.UpdateMyWorkoutExcercise(workoutId, workoutExcerciseId,result,updateWorkout);

            return Ok();
        }

        [Authorize]
        [HttpDelete("Delete/My/WorkoutExercise/{workoutId:int}/{workoutExerciseId:int}")]
        public async Task<IActionResult> DeleteMyWorkoutExercise(int workoutId, int workoutExerciseId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out int result))
                return Unauthorized("Failed to identify user from token");

            await services.DeleteMyWorkoutExcercise(workoutId,workoutExerciseId, result);

            return Ok();
        }

        [Authorize]
        [HttpPost("Add/My/WorkoutExercise/{workoutId:int}")]
        public async Task<IActionResult> AddMyWorkoutExercise(int workoutId, WorkoutExerciseRequestDto addWorkoutExercise)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out int result))
                return Unauthorized("Failed to identify user from token");

            await services.AddMyWorkoutExercise(workoutId,result, addWorkoutExercise);

            return Ok();
        }

        [Authorize]
        [HttpGet("Search/Workout/By/Status/{page:int}/{pageSize:int}")]
        public async Task<IActionResult> SearchWorkoutByStatus(Status status,int page = 1, int pageSize = 10)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out int result))
                return Unauthorized("Failed to identify user from token");

            var listWorkout = await services.SearchMyWorkoutByStatus(status, result, page, pageSize);
            
            return Ok(listWorkout);
        }

        [Authorize]
        [HttpPut("Cancell/My/Workout/{workoutId:int}")]
        public async Task<IActionResult> CancellMyWorkout(int workoutId)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out int result))
                return Unauthorized("Failed to identify user from token");

            await services.CancelMyWorkout(workoutId, result);

            return Ok();
        }

        [Authorize]
        [HttpGet("Get/My/Exercise/By/Name/{nameExercise}/{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetMyExerciseByName(string nameExercise, int page = 1, int pageSize = 10)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out int result))
                return Unauthorized("Failed to identify user from token");

            var listExercise = await services.GetMyExerciseByName(nameExercise, page, pageSize, result);

            return Ok(listExercise);
        }

        [Authorize]
        [HttpGet("Get/Workouts/Stats")]
        public async Task<IActionResult> GetWorkoutsStats()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userId, out int result))
                return Unauthorized("Failed to identify user from token");

            var stats = await services.GetWorkoutsStats(result);

            return Ok(stats);
        }

    }
    
}
