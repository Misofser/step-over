using GoalApi.Data;
using GoalApi.Dtos.Thought;
using GoalApi.Enums;
using GoalApi.Exceptions;
using GoalApi.Models;
using GoalApi.Services.Interfaces;

namespace GoalApi.Services;

public class ThoughtService(AppDbContext db, IWorkspaceService workspaceService) : IThoughtService
{
    private readonly AppDbContext _db = db;
    private readonly IWorkspaceService _workspaceService = workspaceService;

    public async Task<ThoughtReadDto> CreateThoughtAsync(int userId, ThoughtCreateDto dto)
    {
        var workspaceId = await _workspaceService.GetPersonalWorkspaceIdAsync(userId);
        var thought = new Thought
        {
            Content = dto.Content.Trim(),
            Status = ThoughtStatus.Inbox,
            UserId = userId,
            WorkspaceId = workspaceId
        };

        _db.Thoughts.Add(thought);
        await _db.SaveChangesAsync();

        return new ThoughtReadDto
        {
            Id = thought.Id,
            Content = thought.Content,
            Status = thought.Status
        };
    }
}
