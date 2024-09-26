using Domain.Entities;
using WebAPI.DTO;
using Profile = AutoMapper.Profile;

namespace WebAPI;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        Console.WriteLine("added AutoMapperConfig");
        CreateMap<Domain.Entities.Profile, ProfileDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
    }
}