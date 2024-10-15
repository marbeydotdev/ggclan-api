using Application.Clans.Services;
using Application.Services;
using Application.Users.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
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
    private readonly GgDbContext _context;
    private readonly AchievementService _achievementService;
    private readonly IMediator _mediator;
    private readonly ClanRepository _clanRepository;
    
    private readonly IMapper _mapper;

    public ChatController(UserService userService, ClanService clanService, GgDbContext context, IMapper mapper, AchievementService achievementService, IMediator mediator, ClanRepository clanRepository)
    {
        _userService = userService;
        _clanService = clanService;
        _context = context;
        _mapper = mapper;
        _achievementService = achievementService;
        _mediator = mediator;
        _clanRepository = clanRepository;
    }

    [Authorize]
    [HttpGet("messages/{clanId:int}")]
    public async Task<IActionResult> GetMessages(int clanId, int skip = 0, int limit = 10)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        var messages = await _clanService.GetClanMessages(user.Id, clanId, skip, limit);
        
        return messages.IsFailed ? 
            BadRequest(messages.Errors) : 
            Ok(_mapper.Map<IEnumerable<ClanMessageDto>>(messages.Value));
    }

    [Authorize]
    [HttpPost("messages/{clanId:int}")]
    public async Task<IActionResult> CreateMessage(int clanId, [FromBody]CreateMessageDto messageDto)
    {
        var user = await _userService.GetOrCreateUser(HttpContext.GetNameIdentifier());
        
        var clanMember = await _clanRepository.GetClanMemberAsync(user.Id, clanId);

        if (clanMember.IsFailed)
        {
            return Forbid();
        }

        var message = new ClanMessage
        {
            ClanId = clanId,
            ClanMemberId = clanMember.Value.Id,
            Message = messageDto.Message
        };
        
        var added = await _context.ClanMessages.AddAsync(message);
        await _achievementService.AddAchievementIfNotExists(user.Id, (int)EAchievements.FirstMessage);
        
        await _context.SaveChangesAsync();
        return Ok(_mapper.Map<ClanMessageDto>(added.Entity));
    }
}