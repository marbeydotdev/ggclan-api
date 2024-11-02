using Application.DTO;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Controllers;

namespace WebAPI.Services;

public class RealtimeChatService: IRealtimeChatService
{
    private readonly IHubContext<RealtimeHub> _hubContext;
    private readonly IMapper _mapper;

    public RealtimeChatService(IHubContext<RealtimeHub> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }
    
    public async Task BroadcastChatMessage(ClanMessage message)
    {
        await _hubContext.Clients.Group($"chat/{message.ClanId}").SendAsync("chatmessage", _mapper.Map<ClanMessageDto>(message));
    }
}