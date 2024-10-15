using FluentResults;

namespace Application.Achievements.Services;

public interface IAchievementService
{
    public Task<Result> AddAchievementIfNotExists(int userId, int achievementId);
}