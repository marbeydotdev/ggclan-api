using Application.Clans.Commands;
using Application.Clans.Queries;
using Application.Clans.Services;
using Application.Users.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/clan")]
public class ClanController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly GgDbContext _context;
    private readonly IUserService _userService;
    private readonly IClanService _clanService;
    private readonly IMediator _mediator;

    public ClanController(IMapper mapper, GgDbContext context, IUserService userService, IClanService clanService, IMediator mediator)
    {
        _mapper = mapper;
        _context = context;
        _userService = userService;
        _clanService = clanService;
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("list")]
    public async Task<IActionResult> ListClans(string search = "", int skip = 0, int limit = 10)
    {
        var clans = await _mediator.Send(new GetClansQuery
        {
            Skip = skip,
            Limit = limit,
        });
        
        return clans.IsFailed ? 
            BadRequest(clans.Errors) : 
            Ok(_mapper.Map<IEnumerable<ClanDto>>(clans.Value));
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateClan([FromBody] CreateClanDto clan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var newClan = await _mediator.Send(new CreateClanCommand
        {
            CreateClanDto = clan,
            NameIdentifier = HttpContext.GetNameIdentifier()
        });
        
        return newClan.IsFailed ? 
            BadRequest(newClan.Errors) : 
            Ok(_mapper.Map<ClanDto>(newClan.Value));
    }

    [Authorize]
    [HttpGet("list/me")]
    public async Task<IActionResult> GetParticipatingClans(int skip = 0, int limit = 10)
    {
        var clans = await _mediator.Send(new GetUserClansQuery
        {
            NameIdentifier = HttpContext.GetNameIdentifier(),
            Skip = skip,
            Limit = limit
        });
        
        return clans.IsFailed ? 
            BadRequest(clans.Errors) : 
            Ok(_mapper.Map<IEnumerable<ClanDto>>(clans.Value));
    }
    
    [Authorize]
    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetClan(int id)
    {
        var clan = await _mediator.Send(new GetClanQuery
        {
            ClanId = id,
            NameIdentifier = HttpContext.GetNameIdentifier()
        });

        return clan.IsFailed ? 
            BadRequest(clan.Errors) : 
            Ok(_mapper.Map<ClanDto>(clan.Value));
    }

    [Authorize]
    [HttpPost("join/{id:int}")]
    public async Task<IActionResult> JoinClan(int id)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());

        if (!await _clanService.UserCanSendInvite(user.Id, id))
        {
            return BadRequest("Cannot join this clan.");
        }

        var invite = new ClanInvite
        {
            ClanId = id,
            User = user
        };
        
        _context.ClanInvites.Add(invite);
        
        await _context.SaveChangesAsync();

        return Ok();
    }

    [Authorize]
    [HttpGet("get/{id:int}/invites")]
    public async Task<IActionResult> GetInvites(int id)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        var clan = await _clanService.GetClan(id, user.Id);
        if (clan.IsFailed)
        {
            return BadRequest(clan.Errors);
        }

        if (clan.Value.Members.First(c => c.UserId == user.Id).Role == ClanMemberRole.Member)
        {
            return BadRequest("You are not allowed to view invites.");
        }
        
        var invites = await _context.ClanInvites.Include(u => u.User).Where(c => c.ClanId == clan.Value.Id).ToListAsync();
        
        return Ok(_mapper.Map<IEnumerable<ClanInviteDto>>(invites));
    }

    [Authorize]
    [HttpPost("invite/accept/{id:int}")]
    public async Task<IActionResult> AcceptInvite(int id)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        var invite = await _context.ClanInvites.FirstOrDefaultAsync(i => i.Id == id);
        if (invite == null)
        {
            return BadRequest("Invite not found.");
        }
        var clan  = await _clanService.GetClan(invite.ClanId, user.Id);

        if (clan.IsFailed)
        {
            return BadRequest(clan.Errors);
        }

        if (clan.Value.Members.First(u => u.UserId == user.Id).Role == ClanMemberRole.Member)
        {
            return BadRequest("You are not allowed to accept invites.");
        }
        
        clan.Value.Members.Add(new ClanMember
        {
            UserId = invite.UserId,
            Role = ClanMemberRole.Member
        });

        _context.Clans.Update(clan.Value);
        _context.ClanInvites.Remove(invite);
        
        await _context.SaveChangesAsync();

        return Ok();
    }
    
    [Authorize]
    [HttpPost("invite/deny/{id:int}")]
    public async Task<IActionResult> DenyInvite(int id)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        var invite = await _context.ClanInvites.FirstOrDefaultAsync(i => i.Id == id);
        if (invite == null)
        {
            return BadRequest("Invite not found.");
        }
        var clan  = await _clanService.GetClan(invite.ClanId, user.Id);

        if (clan.IsFailed)
        {
            return BadRequest(clan.Errors);
        }

        if (clan.Value.Members.First(u => u.UserId == user.Id).Role == ClanMemberRole.Member)
        {
            return BadRequest("You are not allowed to accept or deny invites.");
        }
        _context.ClanInvites.Remove(invite);
        
        await _context.SaveChangesAsync();

        return Ok();
    }
    
}