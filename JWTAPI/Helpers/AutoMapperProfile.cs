using AutoMapper;
using JWTAPI.Dtos;
using JWTAPI.Entities;

namespace JWTAPI.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}