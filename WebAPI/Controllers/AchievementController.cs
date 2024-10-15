using Application.Achievements.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/achievements")]
public class AchievementController : ControllerBase
{
    private readonly IMediator _mediator;

    public AchievementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("get")]
    public async Task<ActionResult<IEnumerable<Achievement>>> GetAchievements()
    {
        var achievements = await _mediator.Send(new GetAchievementsQuery
        {
            NameIdentifier = HttpContext.GetNameIdentifier()
        });
        
        return achievements.IsFailed ? 
            BadRequest(achievements) : 
            Ok(achievements.Value);
    }
}