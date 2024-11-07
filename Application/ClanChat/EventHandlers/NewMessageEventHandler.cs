using Application.Achievements.Services;
using Application.ClanChat.Events;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.ClanChat.EventHandlers;

public class NewMessageEventHandler : INotificationHandler<NewMessageEvent>
{
    private readonly IAchievementService _achievementService;
    private readonly IRealtimeChatService _chatService;

    public NewMessageEventHandler(IAchievementService achievementService, IRealtimeChatService chatService)
    {
        _achievementService = achievementService;
        _chatService = chatService;
    }

    public async Task Handle(NewMessageEvent notification, CancellationToken cancellationToken)
    {
        await _achievementService.AddAchievementIfNotExists(notification.Message.ClanMember.UserId, (int)EAchievements.FirstMessage);
        await _chatService.BroadcastChatMessage(notification.Message);
    }
}