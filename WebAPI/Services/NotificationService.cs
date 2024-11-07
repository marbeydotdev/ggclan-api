using Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Controllers;

namespace WebAPI.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<RealtimeHub> _hubContext;

    public NotificationService(IHubContext<RealtimeHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotification(int userId, string message)
    {
        await _hubContext.Clients.Group(userId.ToString()).SendAsync("notification", message);
    }
}