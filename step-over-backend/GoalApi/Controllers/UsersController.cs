using GoalApi.Data;
using GoalApi.Models;
using GoalApi.Dtos.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GoalApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UsersController(
        AppDbContext db,
        IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserReadDto>>> GetUsers()
    {
        var users = await _db.Users
            .Select(user => new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserReadDto>> GetUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        return new UserReadDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
        };
    }

    [HttpPost]
    public async Task<ActionResult<UserReadDto>> Create([FromBody] UserCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (_db.Users.Any(u => u.Username == dto.Username))
            return Conflict("User with this username already exists");

        var user = new User
        {
            Username = dto.Username,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var readDto = new UserReadDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
        };

        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            readDto
        );
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto updatedUser, [FromServices] AppDbContext db)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (_db.Users.Any(u => u.Username == updatedUser.Username))
            return Conflict("User with this username already exists");

        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(updatedUser.Username)) {
            user.Username = updatedUser.Username.Trim();
        }

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
