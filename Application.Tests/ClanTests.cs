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
        
        var userRepository = new UserRepository(_context);
        var userAchievementRepository = new UserAchievementRepository(_context);
        var mockNotificationService = new Mock<INotificationService>();
        var notificationService = mockNotificationService.Object;
        _achievementService = new AchievementService(userAchievementRepository, notificationService);
        _userService = new UserService(userRepository, _achievementService);
        _clanRepository = new ClanRepository(_context);
    }

    private readonly AchievementService _achievementService;
    private readonly UserService _userService;
    private readonly ClanRepository _clanRepository;

    [SetUp]
    public async Task Setup()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task CreateClanCommandHandler_CreatesClan()
    {
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