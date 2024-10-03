using System.Reflection;
using Application.Games.Services;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<GameCacheService>();
        services.AddScoped<UserService>();
        services.AddScoped<ClanService>();
        services.AddMediatR(options => { options.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
    }
}