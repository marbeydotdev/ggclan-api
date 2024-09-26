namespace WebAPI.DTO;

public class ProfileDto
{
    public string DisplayName { get; set; } = "User";
    public string ProfilePicture { get; set; } = null!;
    public string BannerPicture { get; set; } = null!;
}