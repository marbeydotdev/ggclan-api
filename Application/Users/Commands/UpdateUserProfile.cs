using Application.Users.Services;
using Domain.Entities;
using FluentResults;
using Infrastructure.Repositories;
using MediatR;

namespace Application.Users.Commands;

public class UpdateUserProfileCommand : IRequest<Result>
{
    public string NameIdentifier { get; set; } = null!;
    public Profile Profile { get; set; } = null!;
}

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result>
{
    private readonly IUserService _userService;
    private readonly IUserRepository _users;

    public UpdateUserProfileCommandHandler(IUserRepository users, IUserService userService)
    {
        _users = users;
        _userService = userService;
    }

    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        user.Profile = request.Profile;
        await _users.UpdateAsync(user);
        return Result.Ok();
    }
}