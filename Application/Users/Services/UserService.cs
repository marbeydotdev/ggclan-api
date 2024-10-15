using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories;

namespace Application.Users.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly AchievementService _achievementService;

    public UserService(UserRepository userRepository, AchievementService achievementService)
    {
        _userRepository = userRepository;
        _achievementService = achievementService;
    }

    public async Task<User> GetOrCreateUser(string nameIdentifier)
    {
        var getUser = await _userRepository.GetAsync(nameIdentifier);

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

        return newUser.Value;
    }
}