using GoalApi.Data;
using GoalApi.Models;
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Goal updatedGoal)
    {
        var goal = await _db.Goals.FindAsync(id);
        if (goal == null) return NotFound();

        goal.Title = updatedGoal.Title;
        goal.IsCompleted = updatedGoal.IsCompleted;
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
