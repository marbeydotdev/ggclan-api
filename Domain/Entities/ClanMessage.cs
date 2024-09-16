using Domain.Common;

namespace Domain.Entities;

public class ClanMessage : BaseEntity
{
    public ClanMember Author { get; set; } = null!;
    public string Message { get; set; } = null!;
}