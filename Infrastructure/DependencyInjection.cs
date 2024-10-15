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
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClanRepository, ClanRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
}