using Microsoft.AspNetCore.Identity;

namespace Blog.Data.Entities
{
    public class User : IdentityUser
    {
        //public string Name { get; set; } = default!;
        //public string Surname { get; set; } = default!;
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
