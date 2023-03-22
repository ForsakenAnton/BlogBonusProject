// using Blog.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.Models.DTO
{
    public class UserDto : IdentityUser
    {
        //public string Name { get; set; } = default!;
        //public string Surname { get; set; } = default!;
        public ICollection<PostDto>? Posts { get; set; }
        public ICollection<CommentDto>? Comments { get; set; }
    }
}
