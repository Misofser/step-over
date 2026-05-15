using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GoalApi.Dtos.Goal;

/// <summary>
/// Query parameters for retrieving goal heatmap data.
/// </summary>
public class GoalHeatmapQuery
{
    /// <summary>
    /// Number of days to include in the heatmap.
    /// Value must be between 1 and 365. Default is 30.
    /// </summary>
    [FromQuery(Name = "days")]
    [Range(1, 365)]
    public int Days { get; set; } = 30;
}
