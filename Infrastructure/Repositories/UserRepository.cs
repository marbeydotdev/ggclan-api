using Domain.Entities;
using Domain.Enums;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(GgDbContext context) : base(context)
    {
    }

    public new async Task<Result<User?>> AddAsync(User user, bool returnEntities = false)
    {
        if (await UserExists(user.NameIdentifier))
        {
            return Result.Fail("User exists.");
        }
        
        var add = await Context.Users.AddAsync(user);
        
        await Context.SaveChangesAsync();

        return returnEntities ? Result.Ok(add?.Entity) : Result.Ok();
    }

    public async Task<Result<User>> GetAsync(string nameIdentifier)
    {
        var user = await GetAsync(u => u.NameIdentifier == nameIdentifier);
        return user;
    }

    private async Task<bool> UserExists(string nameIdentifier)
    {
        return await AnyAsync(u => u.NameIdentifier == nameIdentifier);
    }
}