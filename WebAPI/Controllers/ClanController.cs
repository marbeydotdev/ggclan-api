using Application.Clans.Commands;
using Application.Clans.Queries;
using Application.DTO;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/clan")]
public class ClanController(IMapper mapper, IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet("browse")]
    public async Task<IActionResult> BrowseClans(string search = "", int skip = 0, int limit = 10)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var clans = await mediator.Send(new GetClansQuery
        {
            NameIdentifier = HttpContext.GetNameIdentifier(),
            SearchTerm = search,
            Skip = skip,
            Limit = limit,
        });
        
        return clans.IsFailed ? 
            BadRequest(clans.Errors) : 
            Ok(mapper.Map<IEnumerable<ClanDto>>(clans.Value));
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateClan([FromBody] CreateClanDto clan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var newClan = await mediator.Send(new CreateClanCommand
        {
            CreateClanDto = clan,
            NameIdentifier = HttpContext.GetNameIdentifier()
        });
        
        return newClan.IsFailed ? 
            BadRequest(newClan.Errors) : 
            Ok(mapper.Map<ClanDto>(newClan.Value));
    }

    [Authorize]
    [HttpGet("browse/me")]
    public async Task<IActionResult> GetParticipatingClans(int skip = 0, int limit = 10)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var clans = await mediator.Send(new GetUserClansQuery
        {
            NameIdentifier = HttpContext.GetNameIdentifier(),
            Skip = skip,
            Limit = limit
        });
        
        return clans.IsFailed ? 
            BadRequest(clans.Errors) : 
            Ok(mapper.Map<IEnumerable<ClanDto>>(clans.Value));
    }
    
    [Authorize]
    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetClan(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var clan = await mediator.Send(new GetClanQuery
        {
            ClanId = id,
            NameIdentifier = HttpContext.GetNameIdentifier()
        });

        return clan.IsFailed ? 
            BadRequest(clan.Errors) : 
            Ok(mapper.Map<ClanDto>(clan.Value));
    }

    [Authorize]
    [HttpPost("join/{id:int}")]
    public async Task<IActionResult> JoinClan(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await mediator.Send(new SendInviteCommand
        {
            ClanId = id,
            NameIdentifier = HttpContext.GetNameIdentifier()
        });

        return result.IsSuccess ? Ok() : BadRequest(result.Errors);
    }

    [Authorize]
    [HttpGet("get/{id:int}/invites")]
    public async Task<IActionResult> GetInvites(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await mediator.Send(new GetInvitesCommand
        {
            NameIdentifier = HttpContext.GetNameIdentifier(),
            ClanId = id
        });
        
        return result.IsSuccess ? Ok(mapper.Map<IEnumerable<ClanInviteDto>>(result.Value)) : BadRequest(result.Errors);
    }

    [Authorize]
    [HttpPost("invite/accept/{id:int}")]
    public async Task<IActionResult> AcceptInvite(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var success = await mediator.Send(new AcceptInviteCommand
        {
            InviteId = id,
            NameIdentifier = HttpContext.GetNameIdentifier()
        });

        return success.IsFailed ? BadRequest(success.Errors) : Ok();
    }
    
    [Authorize]
    [HttpPost("invite/deny/{id:int}")]
    public async Task<IActionResult> DenyInvite(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
            
        }

        var result = await mediator.Send(new DenyInviteCommand
        {
            InviteId = id,
            NameIdentifier = HttpContext.GetNameIdentifier()
        });

        return result.IsSuccess ? Ok() : BadRequest(result.Errors);
    }
}