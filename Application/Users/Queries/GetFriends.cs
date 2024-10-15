using Application.Services;
using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Users.Commands;

public class GetFriendsQuery : IRequest<Result<List<User>>>
{
    public string NameIdentifier { get; set; } = null!;
}

public class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, Result<List<User>>>
{
    private readonly UserService _userService;
    private readonly GenericRepository<Clan> _clans;

    public GetFriendsQueryHandler(UserService userService, GenericRepository<Clan> clans)
    {
        _userService = userService;
        _clans = clans;
    }

    public async Task<Result<List<User>>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        
        var clans = await _clans
            .GetAllAsync(clan => clan.Members
                .Any(member => member.UserId == user.Id));

        if (clans.IsFailed)
        {
            return Result.Fail(clans.Errors);
        }

        var friends = clans.Value
            .SelectMany(clan => clan.Members)
            .Where(member => member.UserId != user.Id)
            .Select(member => member.User)
            .Distinct().ToList();
        
        return Result.Ok(friends);
    }
}