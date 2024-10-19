using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Users.Queries;

public class GetFriendsQuery : IRequest<Result<List<User>>>
{
    public string NameIdentifier { get; set; } = null!;
}

public class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, Result<List<User>>>
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;

    public GetFriendsQueryHandler(IUserService userService, IUserRepository userRepository)
    {
        _userService = userService;
        _userRepository = userRepository;
    }

    public async Task<Result<List<User>>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        var friends = await _userRepository.GetFriends(user.Id);
        return friends;
    }
}