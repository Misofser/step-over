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
        return Created($"/api/thoughts/{thought.Id}", thought);
    }
}
