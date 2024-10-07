using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Achievements;

public class AccountCreatedAchievement() : Achievement("Account Created", "You created an account.", Rarity.Common);