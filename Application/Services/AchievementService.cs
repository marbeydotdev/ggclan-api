using Domain.Entities;
using FluentResults;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AchievementService
{
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly GGDbContext _dbContext;

    public AchievementService(UserService userService, UserRepository userRepository, GGDbContext dbContext)
    {
        _userService = userService;
        _userRepository = userRepository;
        _dbContext = dbContext;
    }

    public async Task<Result> AddAchievementIfNotExists(int userId, Achievement achievement)
    {
        var user = await _dbContext.Users.Include(u => u.Achievements).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Result.Fail("User not found");
        }
        if (user.Achievements.Any(u => u.Name == achievement.Name))
        {
            return Result.Ok();
        }
        
        user.Achievements.Add(achievement);

        await _dbContext.SaveChangesAsync();
        
        return Result.Ok();
    }
}