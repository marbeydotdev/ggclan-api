using Application.Services;
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
    private readonly UserService _userService;
    private readonly GenericRepository<User> _users;

    public UpdateUserProfileCommandHandler(UserService userService, GenericRepository<User> users)
    {
        _userService = userService;
        _users = users;
    }

    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrCreateUser(request.NameIdentifier);
        user.Profile = request.Profile;
        await _users.UpdateAsync(user);
        return Result.Ok();
    }
}