using System.Reflection;
using Application.Achievements.Services;
using Application.Clans.Services;
using Application.Games.Services;
using Application.Users.Services;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IGameCacheService, LocalGameCacheService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClanService, ClanService>();
        services.AddScoped<IAchievementService, AchievementService>();
        services.AddMediatR(options => { options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
    }
}