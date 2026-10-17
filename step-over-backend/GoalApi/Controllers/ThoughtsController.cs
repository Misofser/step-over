using GoalApi.Dtos.Thought;
using GoalApi.Services.Infrastructure.Interfaces;
using GoalApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoalApi.Controllers;

/// <summary>Manages thoughts belonging to the authenticated user.</summary>
[ApiController]
[Route("api/thoughts")]
[Authorize]
[Produces("application/json")]
public class ThoughtsController(IThoughtService thoughtService, ICurrentUserService currentUser) : ControllerBase
{
    private readonly IThoughtService _thoughtService = thoughtService;
    private readonly ICurrentUserService _currentUser = currentUser;

    /// <summary>Retrieves all thoughts.</summary>
    /// <returns>A list of thoughts.</returns>
    /// <response code="200">Returns the list of thoughts.</response>
    /// <response code="401">User is unauthorized.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ThoughtReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ThoughtReadDto>>> GetThoughts()
    {
        var userId = _currentUser.GetUserId();
        var thoughts = await _thoughtService.GetAllThoughtsAsync(userId);
        return Ok(thoughts);
    }

    /// <summary>Retrieves a specific thought by its ID.</summary>
    /// <param name="id">The ID of the thought to retrieve.</param>
    /// <returns>The requested thought.</returns>
    /// <response code="200">Returns the requested thought.</response>
    /// <response code="401">User is unauthorized.</response>
    /// <response code="404">Thought not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ThoughtReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ThoughtReadDto>> GetThought(int id)
    {
        var userId = _currentUser.GetUserId();
        var thought = await _thoughtService.GetThoughtByIdAsync(userId, id);
        return Ok(thought);
    }

    /// <summary>Creates a new thought.</summary>
    /// <param name="dto">Data required to create a thought.</param>
    /// <returns>The created thought.</returns>
    /// <response code="201">Thought successfully created.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is unauthorized.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ThoughtReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ThoughtReadDto>> Create(ThoughtCreateDto dto)
    {
        var userId = _currentUser.GetUserId();
        var thought = await _thoughtService.CreateThoughtAsync(userId, dto);
        return CreatedAtAction(nameof(GetThought), new { id = thought.Id }, thought);
    }

    /// <summary>Updates an existing thought.</summary>
    /// <param name="id">The ID of the thought to update.</param>
    /// <param name="dto">Data for updating the thought.</param>
    /// <response code="204">Thought successfully updated.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is unauthorized.</response>
    /// <response code="404">Thought not found.</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, ThoughtUpdateDto dto)
    {
        var userId = _currentUser.GetUserId();
        await _thoughtService.UpdateThoughtAsync(userId, id, dto);
        return NoContent();
    }
}
