namespace Domain.Entities;

public class Profile
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "User";
    public string ProfilePicture { get; set; } = "";
    public string BannerPicture { get; set; } = "";
}