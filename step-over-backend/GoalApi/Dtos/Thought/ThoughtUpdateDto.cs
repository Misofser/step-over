using System.ComponentModel.DataAnnotations;
using GoalApi.Enums;

namespace GoalApi.Dtos.Thought;

/// <summary>Data used to partially update an existing thought.</summary>
public class ThoughtUpdateDto
{
    /// <summary>The updated content of the thought.</summary>
    [MinLength(1, ErrorMessage = "Content cannot be empty")]
    public string? Content { get; set; }

    /// <summary>The updated status of the thought.</summary>
    [EnumDataType(typeof(ThoughtStatus), ErrorMessage = "Status is invalid")]
    public ThoughtStatus? Status { get; set; }
}
