using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

// ReSharper disable once InconsistentNaming
public class GGDbContext : DbContext
{
    public GGDbContext(DbContextOptions<GGDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>().ToTable("Achievements");
        modelBuilder.Entity<User>().OwnsOne(u => u.Profile);
        modelBuilder.Entity<User>().HasMany(u => u.Achievements).WithMany();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Clan> Clans { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<ClanMessage> ClanMessages { get; set; }
    public DbSet<ClanInvite> ClanInvites { get; set; }
}