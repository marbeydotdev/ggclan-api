using Application.Achievements.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;

namespace Application.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAchievementService _achievementService;

    public UserService(IUserRepository userRepository, IAchievementService achievementService)
    {
        _userRepository = userRepository;
        _achievementService = achievementService;
    }

    public async Task<User> GetOrCreateUser(string nameIdentifier)
    {
        var getUser = await _userRepository.GetAsync(u => u.NameIdentifier == nameIdentifier);

        if (getUser.IsFailed)
        {
            var newUser = await CreateUser(nameIdentifier);
            await _achievementService.AddAchievementIfNotExists(newUser.Id, (int)EAchievements.NewAccount);
            return newUser;
        }

        return getUser.Value;
    }

    private async Task<User> CreateUser(string nameIdentifier)
    {
        var user = new User()
        {
            NameIdentifier = nameIdentifier,
        };

        var newUser = await _userRepository.AddAsync(user, true);

        return newUser.Value!;
    }
}