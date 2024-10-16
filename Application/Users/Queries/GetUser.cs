using Application.Users.Services;
using Domain.Entities;
using MediatR;

namespace Application.Users.Queries;

public class GetUserCommand : IRequest<User>
{
    public string NameIdentifier { get; set; } = null!;
}

public class GetUserCommandHandler : IRequestHandler<GetUserCommand, User>
{
    private readonly IUserService _userService;

    public GetUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<User> Handle(GetUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.GetOrCreateUser(request.NameIdentifier);
    }
}