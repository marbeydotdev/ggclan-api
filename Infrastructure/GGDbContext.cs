using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class GgDbContext : DbContext
{
    public GgDbContext(DbContextOptions<GgDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().OwnsOne(u => u.Profile);
        
        modelBuilder.Entity<Achievement>().HasData(
            new Achievement { Id = (int)EAchievements.NewAccount, Name = "Welcome!", Description = "You have created an account." },
            new Achievement { Id = (int)EAchievements.FirstMessage, Name = "Chatter", Description = "You have sent your first message." },
            new Achievement { Id = (int)EAchievements.ClanCreated, Name = "Founder", Description = "You created your fist clan." },
            new Achievement { Id = (int)EAchievements.ClanJoined, Name = "Member", Description = "You joined your fist clan." }
        );
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Clan> Clans { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<ClanMessage> ClanMessages { get; set; }
    public DbSet<ClanInvite> ClanInvites { get; set; }
}