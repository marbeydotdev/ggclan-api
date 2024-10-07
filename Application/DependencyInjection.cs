using System.Reflection;
using Application.Games.Services;
using Application.Services;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<GameCacheService>();
        services.AddScoped<UserService>();
        services.AddScoped<ClanService>();
        services.AddScoped<AchievementService>();
        services.AddMediatR(options => { options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
    }
}