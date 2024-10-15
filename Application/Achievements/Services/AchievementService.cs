using Domain.Entities;
using FluentResults;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AchievementService
{
    private readonly GenericRepository<UserAchievement> _userAchievementRepository;

    public AchievementService(GenericRepository<UserAchievement> userAchievementRepository)
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