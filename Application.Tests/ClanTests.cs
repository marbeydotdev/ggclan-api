using Application.Achievements.Services;
using Application.Clans.Commands;
using Application.Clans.Services;
using Application.DTO;
using Application.Users.Services;
using Domain.Entities;
using Domain.Interfaces;
using FluentResults;
using Infrastructure;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Application.Tests;

[TestFixture]
public class ClanTests
{
    [SetUp]
    public async Task Setup()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
        _context.ChangeTracker.Clear();
        SetupServices();
    }

    private const string TestClanName = "TestClan";
    private const string TestNameIdentifier = "TestNameIdentifier";
    private const int TestClanId = 1;

    private readonly GgDbContext _context;
    private AchievementService _achievementService;
    private ClanRepository _clanRepository;
    private IClanService _clanService;
    private INotificationService _notificationService;
    private IUserAchievementRepository _userAchievementRepository;
    private IUserRepository _userRepository;
    private UserService _userService;
    private IGenericRepository<ClanInvite> _clanInviteRepository;

    public ClanTests()
    {
        var optionsBuilder = new DbContextOptionsBuilder<GgDbContext>();
        optionsBuilder.UseInMemoryDatabase("test");
        _context = new GgDbContext(optionsBuilder.Options);

        SetupServices();
    }


    private void SetupServices()
    {
        _userRepository = new UserRepository(_context);
        _userAchievementRepository = new UserAchievementRepository(_context);

        var mockNotificationService = new Mock<INotificationService>();
        _notificationService = mockNotificationService.Object;

        _achievementService = new AchievementService(_userAchievementRepository, _notificationService);
        _userService = new UserService(_userRepository, _achievementService);
        _clanInviteRepository = new GenericRepository<ClanInvite>(_context);

        _achievementService = new AchievementService(_userAchievementRepository, _notificationService);
        _clanRepository = new ClanRepository(_context);

        _clanService = new ClanService(new ChatMessageRepository(_context), _clanRepository,
            new GenericRepository<ClanInvite>(_context), new GenericRepository<ClanMember>(_context));
    }

    private async Task<Result<Clan>> CreateClan(bool privateClan = false)
    {
        var handler = new CreateClanCommandHandler(_userService, _clanRepository, _achievementService);
        var command = new CreateClanCommand
        {
            NameIdentifier = TestNameIdentifier,
            CreateClanDto = new CreateClanDto
            {
                Name = TestClanName,
                Description = string.Empty,
                Game = string.Empty,
                Private = privateClan
            }
        };

        // act
        var result = await handler.Handle(command, CancellationToken.None);

        return result;
    }

    private async Task<Result> SendInvite()
    {
        var handler = new SendInviteCommandHandler(_clanService, _context, _userService);
        var command = new SendInviteCommand
        {
            NameIdentifier = "testuser",
            ClanId = TestClanId
        };

        var result = await handler.Handle(command, CancellationToken.None);

        return result;
    }

    private async Task<Result> AcceptInvite()
    {
        var handler = new AcceptInviteCommandHandler(_clanInviteRepository, _clanRepository, _userService);
        var command = new AcceptInviteCommand
        {
            NameIdentifier = TestNameIdentifier,
            InviteId = 1
        };

        var result = await handler.Handle(command, CancellationToken.None);

        return result;
    }

    [Test]
    public async Task GivenValidCommand_CreateClanCommandHandler_CreatesClan()
    {
        var result = await CreateClan();

        if (!result.IsSuccess)
        {
            Assert.Fail();
        }

        if (result.Value.Name == TestClanName)
        {
            Assert.Pass();
        }

        Assert.Fail();
    }

    [Test]
    public async Task GivenValidCommand_SendInviteCommandHandler_SendsInvite()
    {
        await CreateClan();

        var result = await SendInvite();

        if (!result.IsSuccess)
        {
            Assert.Fail(string.Join(" | ", result.Errors));
        }

        Assert.Pass();
    }

    [Test]
    public async Task GivenValidCommand_AcceptInviteCommandHandler_AcceptsInvite()
    {
        await CreateClan();
        await SendInvite();

        var result = await AcceptInvite();

        if (!result.IsSuccess)
        {
            Assert.Fail(string.Join(" | ", result.Errors));
        }

        Assert.Pass();
    }

    [Test]
    public async Task GivenAlreadyParticipatingUser_SendInviteCommandHandler_Fails()
    {
        await CreateClan();
        await SendInvite();
        await AcceptInvite();

        var result = await SendInvite();

        if (result.IsSuccess)
        {
            Assert.Fail("Already participating user can send invite to same clan.");
        }

        Assert.Pass();
    }

    [Test]
    public async Task GivenPrivateClan_SendInviteCommandHandler_Fails()
    {
        await CreateClan(true);

        var result = await SendInvite();

        if (!result.IsSuccess)
        {
            Assert.Fail("User can send invite to private clan.");
        }

        Assert.Pass();
    }
}