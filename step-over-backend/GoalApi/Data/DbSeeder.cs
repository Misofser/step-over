using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GoalApi.Models;
using GoalApi.Data;

namespace GoalApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        if (!env.IsDevelopment()) return;

        await db.Database.MigrateAsync();

        if (await db.Users.AnyAsync()) return;

        var adminUsername = config["DevAdmin:Username"];
        var adminPassword = config["DevAdmin:Password"];

        if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new Exception("DevAdmin credentials are missing in configuration");
        }

        var adminUser = new User
        {
            Username = adminUsername,
            Role = "Admin"
        };

        var hasher = new PasswordHasher<User>();
        adminUser.PasswordHash = hasher.HashPassword(adminUser, adminPassword);

        db.Users.Add(adminUser);
        await db.SaveChangesAsync();
    }
}
