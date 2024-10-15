using Domain.Common;

namespace Domain.Entities;

public class UserAchievement : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int AchievementId { get; set; }
    public Achievement Achievement { get; set; } = null!;
}