using Application.Clans.Commands;
using Application.Clans.Queries;
using Application.Clans.Services;
using Application.DTO;
using Application.Users.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    private readonly IClanRepository _clanRepository;

    public ClanController(IMapper mapper, GgDbContext context, IUserService userService, IClanService clanService, IMediator mediator, IClanRepository clanRepository)
    {
        _mapper = mapper;
        _context = context;
        _userService = userService;
        _clanService = clanService;
        _mediator = mediator;
        _clanRepository = clanRepository;
    }

    [Authorize]
    [HttpGet("browse")]
    public async Task<IActionResult> BrowseClans(string search = "", int skip = 0, int limit = 10)
    {
        var clans = await _mediator.Send(new GetClansQuery
        {
            NameIdentifier = HttpContext.GetNameIdentifier(),
            SearchTerm = search,
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
    [HttpGet("browse/me")]
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

        var canSendInvite = await _clanService.UserCanSendInvite(user.Id, id);

        if (canSendInvite.IsFailed)
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

        var clanMember = await _clanRepository.GetClanMemberAsync(user.Id, id);

        if (clanMember.IsFailed || clanMember.Value.Role == ClanMemberRole.Member)
        {
            return Forbid();
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

        var clanMember = await _clanRepository.GetClanMemberAsync(user.Id, invite.ClanId);

        if (clanMember.IsFailed || clanMember.Value.Role == ClanMemberRole.Member)
        {
            return Forbid("You need to be an administrator to accept this invite.");
        }

        await _clanRepository.AddClanMemberAsync(invite.UserId, invite.ClanId, ClanMemberRole.Member);

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

        var clanMember = await _clanRepository.GetClanMemberAsync(user.Id, invite.ClanId);

        if (clanMember.IsFailed || clanMember.Value.Role == ClanMemberRole.Member)
        {
            return Forbid();
        }
        
        _context.ClanInvites.Remove(invite);
        
        await _context.SaveChangesAsync();

        return Ok();
    }
    
}