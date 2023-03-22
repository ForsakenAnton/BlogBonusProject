using AutoMapper;
using Blog.Data.Entities;
using Blog.Models.DTO;

namespace Blog.AutoMapperProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
