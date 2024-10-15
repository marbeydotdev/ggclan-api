using Application.Services;
using Application.Users.Services;
using Domain.Entities;
using MediatR;

namespace Application.Users.Commands;

public class GetUserCommand : IRequest<User>
{
    public string NameIdentifier { get; set; } = null!;
}

public class GetUserCommandHandler : IRequestHandler<GetUserCommand, User>
{
    private readonly UserService _userService;

    public GetUserCommandHandler(UserService userService)
    {
        _userService = userService;
    }

    public async Task<User> Handle(GetUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.GetOrCreateUser(request.NameIdentifier);
    }
}