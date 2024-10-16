using Domain.Common;

namespace Application.DTO;

public class UserDto : BaseEntity
{
    public ProfileDto Profile { get; set; } = null!;
}