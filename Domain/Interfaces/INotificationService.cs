namespace Domain.Interfaces;

public interface INotificationService
{
    public Task SendNotification(int userId, string message);
}