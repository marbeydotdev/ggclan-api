using Application.Achievements.Services;
using Application.Clans.Commands;
using Application.DTO;
using Application.Users.Services;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests;

public class Tests
{
    private GgDbContext _context;

    public Tests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<GgDbContext>();
        optionsBuilder.UseInMemoryDatabase("test"); 
        _context = new GgDbContext(optionsBuilder.Options);
        
        _userRepository = new UserRepository(_context);
        _userAchievementRepository = new UserAchievementRepository(_context);
        var mockNotificationService = new Mock<INotificationService>();
        var notificationService = mockNotificationService.Object;
        _achievementService = new AchievementService(_userAchievementRepository, notificationService);
        _userService = new UserService(_userRepository, _achievementService);
        _clanRepository = new ClanRepository(_context);
    }

    private readonly UserRepository _userRepository;
    private readonly UserAchievementRepository _userAchievementRepository;
    private readonly AchievementService _achievementService;
    private readonly UserService _userService;
    private readonly ClanRepository _clanRepository;

    [Test]
    public async Task CreateClanCommandHandler_CreatesClan()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
        
        var handler = new CreateClanCommandHandler(_userService, _clanRepository, _achievementService);
        var command = new CreateClanCommand
        {
            NameIdentifier = "test",
            CreateClanDto = new CreateClanDto
            {
                Name = "TestClanName",
                Description = "TestClanDescription",
                Game = "TestClanGame",
                Private = false
            }
        };
        
        // act
        var result = await handler.Handle(command, CancellationToken.None);

        if (!result.IsSuccess)
        {
            Assert.Fail();
        }

        if (result.Value.Name == "TestClanName")
        {
            Assert.Pass();
        }
        
        Assert.Fail();
    }
}