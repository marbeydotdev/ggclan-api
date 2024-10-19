using Domain.Entities;
using FluentResults;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Application.Achievements.Services;

public class AchievementService : IAchievementService
{
    private readonly IUserAchievementRepository _userAchievementRepository;

    public AchievementService(IUserAchievementRepository userAchievementRepository)
    {
        _userAchievementRepository = userAchievementRepository;
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
        
        return Result.Ok();
    }
}