using Application.Services;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/achievements")]
public class AchievementController : ControllerBase
{
    private readonly UserService _userService;
    private readonly GGDbContext _context;

    public AchievementController(UserService userService, GGDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    [Authorize]
    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<Achievement>>> GetAchievements()
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        var userWithAchievements = await _context.Users.Include(u => u.Achievements).FirstAsync(u => u.Id == user.Id);
        return Ok(userWithAchievements.Achievements);
    }
}