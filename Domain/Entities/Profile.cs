using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Profile
{
    [MaxLength(30)]
    public string DisplayName { get; set; } = "User";
    [MaxLength(200)]
    public string ProfilePicture { get; set; } = "/favicon.png";
    [MaxLength(5000)]
    public string Biography { get; set; } = string.Empty;
}