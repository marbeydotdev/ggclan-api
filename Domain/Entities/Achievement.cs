using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Achievement : BaseEntity
{
    public Achievement()
    {
        
    }

    public Achievement(string name, string description, Rarity rarity)
    {
        Name = name;
        Description = description;
        Rarity = rarity;
    }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Rarity Rarity { get; set; }
}