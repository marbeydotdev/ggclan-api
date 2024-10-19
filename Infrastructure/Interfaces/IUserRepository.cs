using Domain.Entities;
using FluentResults;

namespace Infrastructure.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    public Task<Result<List<User>>> GetFriends(int userId);
}