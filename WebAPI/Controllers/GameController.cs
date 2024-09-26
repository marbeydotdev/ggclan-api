using Application.Games.Queries;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/game")]

public class GameController: ControllerBase
{
    private readonly ISender _sender;

    public GameController(ISender sender)
    {
        _sender = sender;
    }

    
    [HttpGet("list")]
    [EnableCors("dev")]
    public async Task<IActionResult> GetGames([FromQuery]string query)
    {
        var games = await _sender.Send(new GameSearchRequest
        {
            Query = query
        });
        
        return Ok(games);
    }
}