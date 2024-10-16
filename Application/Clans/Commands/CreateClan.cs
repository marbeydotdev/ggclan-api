using Application.Achievements.Services;
using Application.DTO;
using Application.Users.Services;
using Domain.Entities;
using Domain.Enums;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Clans.Commands;

public class CreateClanCommand : IRequest<Result<Clan>>
{
    public CreateClanDto CreateClanDto { get; set; } = null!;
    public string NameIdentifier  { get; set; } = null!;
}

public class CreateClanCommandHandler : IRequestHandler<CreateClanCommand, Result<Clan>>
{
    private readonly IUserService _userService;
    private readonly IClanRepository _clanRepository;
    private readonly IAchievementService _achievementService;

    public CreateClanCommandHandler(IUserService userService, IClanRepository clanRepository, IAchievementService achievementService)
    {
        _userService = userService;
        _clanRepository = clanRepository;
        _achievementService = achievementService;
    }

    public async Task<Result<Clan>> Handle(CreateClanCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);

        var newClan = new Clan
        {
            Name = request.CreateClanDto.Name,
            Description = request.CreateClanDto.Description,
            Game = request.CreateClanDto.Game,
        };
        
        var result = await _clanRepository.AddAsync(newClan, true);
        
        await _clanRepository.AddClanMemberAsync(user.Id, newClan.Id, ClanMemberRole.Owner);
        await _achievementService.AddAchievementIfNotExists(user.Id, (int)EAchievements.ClanCreated);
        
        return result as Result<Clan>; // returnEntity = true, so not null
    }
}