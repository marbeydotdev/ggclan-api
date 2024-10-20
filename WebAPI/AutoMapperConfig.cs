using Application.DTO;
using Domain.Entities;
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
        CreateMap<ClanMessage, ClanMessageDto>().ReverseMap();
        CreateMap<ClanInvite, ClanInviteDto>().ReverseMap();
    }
}