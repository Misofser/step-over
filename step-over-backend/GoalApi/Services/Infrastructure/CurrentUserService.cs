using GoalApi.Services.Infrastructure.Interfaces;
using System.Security.Claims;

namespace GoalApi.Services.Infrastructure;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public int GetUserId()
    {
        var user = _accessor.HttpContext?.User;
        var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (idClaim == null) throw new UnauthorizedAccessException();

        return int.Parse(idClaim);
    }

    public string GetUsername()
        => _accessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value 
            ?? throw new UnauthorizedAccessException();

    public string GetRole()
        => _accessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value
            ?? throw new UnauthorizedAccessException();
}
