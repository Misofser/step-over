using Microsoft.EntityFrameworkCore;
using GoalApi.Models;

namespace GoalApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Goal> Goals => Set<Goal>();
}
