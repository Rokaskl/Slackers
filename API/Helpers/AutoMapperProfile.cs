using AutoMapper;
using WebApi.Dtos;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Room, RoomDto>();
            CreateMap<RoomDto, Room>();
            CreateMap<TimeMark, TimeMarkDto>();
            CreateMap<TimeMarkDto, TimeMark>();
        }
    }
}