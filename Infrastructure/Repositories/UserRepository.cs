using Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Infrastructure.Repositories;

public class UserRepository
{
    private readonly GGDbContext _context;
    public UserRepository(GGDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> AddAsync(User user)
    {
        if (await UserExists(user.NameIdentifier))
        {
            return Result.Fail("Username is taken.");
        }
        
        var add = await _context.Users.AddAsync(user);
        
        await _context.SaveChangesAsync();
        
        return Result.Ok(add.Entity);
    }

    public async Task<Result<User>> GetAsync(string nameIdentifier)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.NameIdentifier == nameIdentifier);
        if (user == null)
        {
            return Result.Fail<User>("User not found");
        }

        return Result.Ok(user);
    }

    public async Task<Result> UpdateAsync(User user)
    {
        if (!await UserExists(user.NameIdentifier))
        {
            return Result.Fail("User does not exist.");
        }
        
        _context.Users.Update(user);
        
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(User user)
    {
        if (!await UserExists(user.NameIdentifier))
        {
            return Result.Fail("User does not exist.");
        }
        
        _context.Users.Remove(user);
        
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    private async Task<bool> UserExists(string userIdentifier)
    {
        return await _context.Users.AnyAsync(u => u.NameIdentifier == userIdentifier);
    }
}