using GoalApi.Dtos.Today;
using GoalApi.Services.Interfaces;
using GoalApi.Services.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GoalApi.Controllers;

/// <summary>
/// Returns today dashboard items including pending and completed tasks and habits.
/// </summary>
[ApiController]
[Route("api/today")]
[Authorize]
[Produces("application/json")]
public class TodayController(ITodayService todayService, ICurrentUserService currentUser) : ControllerBase
{
    private readonly ITodayService _todayService = todayService;
    private readonly ICurrentUserService _currentUser = currentUser;

    /// <summary>Gets today's tasks and habits grouped into pending and completed lists</summary>
    /// <returns>The today dashboard grouped result</returns>
    /// <response code="200">Returns the today dashboard data</response>
    /// <response code="401">User is unauthorized</response>
    [HttpGet]
    [ProducesResponseType(typeof(TodayDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TodayDashboardDto>> GetToday()
    {
        var userId = _currentUser.GetUserId();
        var result = await _todayService.GetTodayItemsAsync(userId);
        return Ok(result);
    }
}
