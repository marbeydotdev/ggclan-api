using Domain.Entities;
using Domain.Interfaces;
using FluentResults;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Application.Achievements.Services;

public class AchievementService : IAchievementService
{
    private readonly IUserAchievementRepository _userAchievementRepository;
    private readonly INotificationService _notificationService;
    public AchievementService(IUserAchievementRepository userAchievementRepository, INotificationService notificationService)
    {
        _userAchievementRepository = userAchievementRepository;
        _notificationService = notificationService;
    }

    public async Task<Result> AddAchievementIfNotExists(int userId, int achievementId)
    {
        var hasAchievement = await _userAchievementRepository.AnyAsync(x => x.UserId == userId && x.AchievementId == achievementId);

        if (hasAchievement)
        {
            return Result.Ok();
        }

        await _userAchievementRepository.AddAsync(new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId
        });

        await _notificationService.SendNotification(userId, "You got a new achievement!");
        
        return Result.Ok();
    }
}