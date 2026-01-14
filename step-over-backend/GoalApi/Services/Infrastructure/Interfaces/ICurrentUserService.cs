namespace GoalApi.Services.Infrastructure.Interfaces;

public interface ICurrentUserService
{
    int GetUserId();
    string GetUsername();
    string GetRole();
}
