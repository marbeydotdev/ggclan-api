using Application.Services;
using AutoMapper;
using Domain.Achievements;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebAPI.DTO;

namespace WebAPI.Controllers;

[ApiController]
[Route("v1/chat")]
[EnableRateLimiting("fixed")]
public class ChatController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ClanService _clanService;
    private readonly GGDbContext _context;
    private readonly AchievementService _achievementService;
    
    private readonly IMapper _mapper;

    public ChatController(UserService userService, ClanService clanService, GGDbContext context, IMapper mapper, AchievementService achievementService)
    {
        _userService = userService;
        _clanService = clanService;
        _context = context;
        _mapper = mapper;
        _achievementService = achievementService;
    }

    [Authorize]
    [HttpGet("messages/{clanId:int}")]
    public async Task<IActionResult> GetMessages(int clanId, int skip = 0, int limit = 10)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        var messages = await _clanService.TryGetClanMessages(user.Id, clanId, skip, limit);
        if (messages.IsFailed)
        {
            return BadRequest(messages.Errors);
        }
        
        return Ok(_mapper.Map<IEnumerable<ClanMessageDto>>(messages.Value));
    }

    [Authorize]
    [HttpPost("messages/{clanId:int}")]
    public async Task<IActionResult> CreateMessage(int clanId, [FromBody]CreateMessageDto messageDto)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        var clan = await _clanService.TryGetClan(clanId, user.Id);

        if (clan.IsFailed)
        {
            return Unauthorized();
        }

        var message = new ClanMessage
        {
            Clan = clan.Value,
            ClanMember = clan.Value.Members.First(c => c.UserId == user.Id),
            Message = messageDto.Message
        };
        
        var added = await _context.ClanMessages.AddAsync(message);
        await _achievementService.AddAchievementIfNotExists(user.Id, new FirstMessageAchievement());
        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<ClanMessageDto>(added.Entity));
    }
}