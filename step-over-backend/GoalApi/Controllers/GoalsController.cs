using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Controllers;

[ApiController]
[Route("api/goals")]
public class GoalsController : ControllerBase
{
    private readonly AppDbContext _db;

    public GoalsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<GoalReadDto>>> GetGoals()
    {
        var goals = await _db.Goals
            .Select(g => new GoalReadDto
            {
                Id = g.Id,
                Title = g.Title,
                IsCompleted = g.IsCompleted,
            })
            .ToListAsync();

        return Ok(goals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GoalReadDto>> GetGoal(int id)
    {
        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) return NotFound();

        return new GoalReadDto
        {
            Id = goal.Id,
            Title = goal.Title,
            IsCompleted = goal.IsCompleted,
        };
    }

    [HttpPost]
    public async Task<ActionResult<GoalReadDto>> Create([FromBody] GoalCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var goal = new Goal
        {
            Title = dto.Title,
        };

        _db.Goals.Add(goal);
        await _db.SaveChangesAsync();

        var readDto = new GoalReadDto
        {
            Id = goal.Id,
            Title = goal.Title,
            IsCompleted = goal.IsCompleted,
        };

        return CreatedAtAction(
            nameof(GetGoal),
            new { id = goal.Id },
            readDto
        );
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] GoalUpdateDto updatedGoal, [FromServices] AppDbContext db)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(updatedGoal.Title)) {
            goal.Title = updatedGoal.Title.Trim();
        }

        if (updatedGoal.IsCompleted.HasValue)
            goal.IsCompleted = updatedGoal.IsCompleted.Value;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) return NotFound();

        _db.Goals.Remove(goal);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
