using AutoMapper;
using Blog.Data.Entities;
using Blog.Models.DTO;

namespace Blog.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
