using GoalApi.Dtos.Goal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GoalApi.Controllers;

[ApiController]
[Route("api/goals")]
[Authorize]
public class GoalsController(IGoalService goalService) : ControllerBase
{
    private readonly IGoalService _goalService = goalService;

    [HttpGet]
    public async Task<ActionResult<List<GoalReadDto>>> GetGoals()
    {
        var goals = await _goalService.GetAllGoalsAsync();
        return Ok(goals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GoalReadDto>> GetGoal(int id)
    {
        var goal = await _goalService.GetGoalByIdAsync(id);
        if (goal == null) return NotFound();
        return Ok(goal);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<GoalReadDto>> Create([FromBody] GoalCreateDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        int userId = int.Parse(userIdClaim);
        var goal = await _goalService.CreateGoalAsync(userId, dto);

        return CreatedAtAction(
            nameof(GetGoal),
            new { id = goal.Id },
            goal
        );
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, GoalUpdateDto dto)
    {
        bool updated = await _goalService.UpdateGoalAsync(id, dto);
        if (!updated) return NotFound();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _goalService.DeleteGoalAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }

}
