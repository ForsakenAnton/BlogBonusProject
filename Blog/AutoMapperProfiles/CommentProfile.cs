using AutoMapper;
using Blog.Data.Entities;
using Blog.Models.DTO;

namespace Blog.AutoMapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDto>().ReverseMap();
        }
    }
}
