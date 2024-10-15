using Domain.Entities;

namespace Application.Users.Services;

public interface IUserService
{
    public Task<User> GetOrCreateUser(string nameIdentifier);
}