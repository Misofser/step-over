using GoalApi.Dtos.Goal;
using GoalApi.Services.Interfaces;
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
        return Ok(goal);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<GoalReadDto>> Create(GoalCreateDto dto)
    {
        var goal = await _goalService.CreateGoalAsync(dto);
        return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, GoalUpdateDto dto)
    {
        await _goalService.UpdateGoalAsync(id, dto);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _goalService.DeleteGoalAsync(id);
        return NoContent();
    }

}
