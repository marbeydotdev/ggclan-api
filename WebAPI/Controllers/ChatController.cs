using Application.ClanChat.Commands;
using Application.ClanChat.Queries;
using AutoMapper;
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
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ChatController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpGet("messages/{clanId:int}")]
    public async Task<IActionResult> GetMessages(int clanId, int skip = 0, int limit = 10)
    {
        var messages = await _mediator.Send(new GetClanMessagesQuery
        {
            ClanId = clanId,
            Limit = limit,
            Skip = skip,
            NameIdentifier = HttpContext.GetNameIdentifier()
        });
        
        return messages.IsFailed ? 
            BadRequest(messages.Errors) : 
            Ok(_mapper.Map<IEnumerable<ClanMessageDto>>(messages.Value));
    }

    [Authorize]
    [HttpPost("messages/{clanId:int}")]
    public async Task<IActionResult> CreateMessage(int clanId, [FromBody]CreateMessageDto messageDto)
    {
        var sentMessage = await _mediator.Send(new SendMessageCommand
        {
            NameIdentifier = HttpContext.GetNameIdentifier(),
            ClanId = clanId,
            CreateMessageDto = messageDto
        });
        
        return sentMessage.IsFailed ? 
            BadRequest(sentMessage.Errors) : 
            Ok(_mapper.Map<ClanMessageDto>(sentMessage.Value));
    }
}