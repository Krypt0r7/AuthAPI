using AutoMapper;
using AuthAPI.Dtos;
using AuthAPI.Entities;

namespace AuthAPI.Helpers
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