using Application.Services;
using AutoMapper;
using Domain.Achievements;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
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
    private readonly GGDbContext _context;
    private readonly UserService _userService;
    private readonly ClanService _clanService;
    private readonly AchievementService _achievementService;

    private const int ResultsPerPage = 10;

    public ClanController(IMapper mapper, GGDbContext context, UserService userService, ClanService clanService, AchievementService achievementService)
    {
        _mapper = mapper;
        _context = context;
        _userService = userService;
        _clanService = clanService;
        _achievementService = achievementService;
    }

    [Authorize]
    [HttpGet("list")]
    public async Task<IActionResult> ListClans(int page = 0, string search = "")
    {
        var clans = await _context.Clans
            .Where(clan => !clan.Private)
            .Where(clan => clan.Name.Contains(search))
            .Skip(page * ResultsPerPage)
            .Take(ResultsPerPage)
            .Include(c => c.Members)
            .ThenInclude(m => m.User)
            .ToListAsync();
        
        return Ok(_mapper.Map<IEnumerable<ClanDto>>(clans));
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateClan([FromBody] CreateClanDto clan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());

        var newClan = _mapper.Map<Clan>(clan);

        var ent = _context.Clans.Add(newClan);
        ent.Entity.Members.Add(new ClanMember
        {
            UserId = user.Id,
            Role = ClanMemberRole.Owner
        });
        await _context.SaveChangesAsync();
        
        await _achievementService.AddAchievementIfNotExists(user.Id, new ClanCreatedAchievement());
        
        return Ok(_mapper.Map<ClanDto>(ent.Entity as Clan));
    }

    [Authorize]
    [HttpGet("list/me")]
    public async Task<IActionResult> GetParticipatingClans(int page = 0)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());

        var clans = await _context.Clans
            .Where(clan => clan.Members.Any(member => member.UserId == user.Id))
            .Skip(page * ResultsPerPage)
            .Take(ResultsPerPage)
            .Include(c => c.Members)
            .ThenInclude(m => m.User)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<ClanDto>>(clans));
    }

    [Authorize]
    [HttpGet("get/{id:int}")]
    public async Task<IActionResult> GetClan(int id)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());

        var clan = await _clanService.TryGetClan(id, user.Id);

        if (clan.IsFailed)
        {
            return BadRequest(clan.Errors);
        }

        return Ok(clan.Value);
    }
    
}