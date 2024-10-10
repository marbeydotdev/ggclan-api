namespace WebAPI.DTO;

public class UserDto
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public ProfileDto Profile { get; set; } = null!;
}