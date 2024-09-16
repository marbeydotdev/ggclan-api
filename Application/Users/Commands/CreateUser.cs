using MediatR;

namespace Application.Users.Commands;

public class CreateUserCommand : IRequest
{
    
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);
    }
}