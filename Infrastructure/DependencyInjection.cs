using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<GgDbContext>(options =>
        {
            options.UseSqlite("Data Source=GG.db");
        });
        services.AddScoped<UserRepository>();
        services.AddScoped<ClanRepository>();
        services.AddScoped<ChatMessageRepository>();
        services.AddScoped<UserAchievementRepository>();
        services.AddScoped(typeof(GenericRepository<>));
    }
}