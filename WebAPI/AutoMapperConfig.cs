using Domain.Entities;
using WebAPI.DTO;
using Profile = AutoMapper.Profile;

namespace WebAPI;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Domain.Entities.Profile, ProfileDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Clan, CreateClanDto>().ReverseMap();
        CreateMap<Clan, ClanDto>().ReverseMap();
        CreateMap<ClanMember, ClanMemberDto>().ReverseMap();
    }
}