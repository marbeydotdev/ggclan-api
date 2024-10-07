using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Achievements;

public class ClanCreatedAchievement() : Achievement("Clan Created", "You created a clan.", Rarity.Common);