using AutoMapper;
using Blog.Data.Entities;
using Blog.Models.DTO;

namespace Blog.AutoMapperProfiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostDto>().ReverseMap();
        }
    }
}
