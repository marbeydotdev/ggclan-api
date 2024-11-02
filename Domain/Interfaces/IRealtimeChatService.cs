using Domain.Entities;

namespace Domain.Interfaces;

public interface IRealtimeChatService
{
    public Task BroadcastChatMessage(ClanMessage message);
}