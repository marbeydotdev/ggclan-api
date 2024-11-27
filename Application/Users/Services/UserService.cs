using Application.Achievements.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;

namespace Application.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAchievementService _achievementService;
    
    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public UserService(IUserRepository userRepository, IAchievementService achievementService)
    {
        _userRepository = userRepository;
        _achievementService = achievementService;
    }

    public async Task<User> GetOrCreateUser(string nameIdentifier)
    {
        await _semaphore.WaitAsync();
        try
        {
            var getUser = await _userRepository.GetAsync(u => u.NameIdentifier == nameIdentifier);

            if (!getUser.IsFailed) return getUser.Value;
            
            var newUser = await CreateUser(nameIdentifier);
            await _achievementService.AddAchievementIfNotExists(newUser.Id, (int)EAchievements.NewAccount);
            return newUser;

        }
        finally
        {
            _semaphore.Release();
        }
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