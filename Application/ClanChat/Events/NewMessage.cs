using Domain.Entities;
using MediatR;

namespace Application.ClanChat.Events;

public class NewMessageEvent : INotification
{
    public ClanMessage Message { get; set; } = null!;
    
    public NewMessageEvent(ClanMessage message)
    {
        Message = message;
    }

    public NewMessageEvent()
    {
        
    }
}