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
        modelBuilder.Entity<User>()
            .HasIndex(u => u.NameIdentifier)
            .IsUnique();
        modelBuilder.Entity<User>().OwnsOne(u => u.Profile);
        
        modelBuilder.Entity<Achievement>().HasData(
            new Achievement { Id = (int)EAchievements.NewAccount, Name = "Welcome!", Description = "You have created an account." },
            new Achievement { Id = (int)EAchievements.FirstMessage, Name = "Chatter", Description = "You have sent your first message." },
            new Achievement { Id = (int)EAchievements.ClanCreated, Name = "Founder", Description = "You created your first clan." },
            new Achievement { Id = (int)EAchievements.ClanJoined, Name = "Member", Description = "You joined your fist clan." }
        );
    }

    public IQueryable<ClanWithMember> ClanWithMembers =>
        from a in Clans
        join b in ClanMembers on a.Id equals b.ClanId into clanMemberGroup
        select new ClanWithMember
        {
            Clan = a,
            Members = clanMemberGroup.ToList()
        };

    public IQueryable<User> Friends(int userId) =>
        from m1 in ClanMembers
        where m1.UserId == userId
        join m2 in ClanMembers on m1.ClanId equals m2.ClanId
        join u in Users on m2.UserId equals u.Id
        where u.Id != userId
        select u;
    
    public IQueryable<Clan> AvailableClans(int userId) =>
        from c in Clans
        where !(from m in ClanMembers
            where m.UserId == userId
            select m.ClanId).Contains(c.Id) && !c.Private && 
              !(from i in ClanInvites
                where i.UserId == userId
                select i.ClanId).Contains(c.Id)
        select c;
    
    public DbSet<User> Users { get; set; }
    public DbSet<Clan> Clans { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<ClanMessage> ClanMessages { get; set; }
    public DbSet<ClanInvite> ClanInvites { get; set; }
    public DbSet<ClanMember> ClanMembers { get; set; }
}