using Application.Games.Queries;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/game")]

public class GameController: ControllerBase
{
    private readonly IMediator _mediator;

    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpGet("list")]
    [EnableCors("dev")]
    public async Task<IActionResult> GetGames([FromQuery]string query)
    {
        var games = await _mediator.Send(new GetGamesQuery
        {
            Query = query
        });
        
        return Ok(games);
    }
}