using Domain.Entities;
using FluentResults;

namespace Infrastructure.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    public Task<Result<List<User>>> GetFriends(int userId);
}