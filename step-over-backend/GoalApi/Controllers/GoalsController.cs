using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GoalsController : ControllerBase
{
    private readonly AppDbContext _db;

    public GoalsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<List<Goal>> Get()
    {
        return await _db.Goals.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Goal>> Create(Goal goal)
    {
        _db.Goals.Add(goal);
        await _db.SaveChangesAsync();
        return Ok(goal);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] GoalUpdateDto updatedGoal, [FromServices] AppDbContext db)
    {
        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) return NotFound();

        if (updatedGoal.Title != null)
            goal.Title = updatedGoal.Title;

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
