using Domain.Achievements;
using Domain.Entities;
using FluentResults;
using Infrastructure.Repositories;

namespace Application.Services;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetOrCreateUser(string nameIdentifier)
    {
        var getUser = await _userRepository.GetAsync(nameIdentifier);

        if (getUser.IsFailed)
        {
            var user = new User()
            {
                NameIdentifier = nameIdentifier,
                Achievements = [new AccountCreatedAchievement()]
            };

            var newUser = await _userRepository.AddAsync(user);

            return newUser.Value;
        }

        return getUser.Value;

    }
}