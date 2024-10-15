using Application.Services;
using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Achievements.Queries;

public class GetAchievementsQuery : IRequest<Result<List<Achievement>>>
{
    public string NameIdentifier  { get; set; } = null!;
}

public class GetAchievementsQueryHandler : IRequestHandler<GetAchievementsQuery, Result<List<Achievement>>>
{
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserAchievementRepository _userAchievementRepository;

    public GetAchievementsQueryHandler(UserService userService, UserRepository userRepository, UserAchievementRepository userAchievementRepository)
    {
        _userService = userService;
        _userRepository = userRepository;
        _userAchievementRepository = userAchievementRepository;
    }

    public async Task<Result<List<Achievement>>> Handle(GetAchievementsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        var achievements = await _userAchievementRepository.GetAllAsync(a => a.UserId == user.Id);
        return achievements.IsFailed ? 
            Result.Fail("Could not get achievements.") : 
            Result.Ok(achievements.Value.Select(u => u.Achievement).ToList());
    }
}