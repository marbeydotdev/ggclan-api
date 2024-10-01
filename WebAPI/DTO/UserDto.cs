namespace WebAPI.DTO;

public class UserDto
{
    public int Id { get; set; }
    public ProfileDto Profile { get; set; } = null;
}