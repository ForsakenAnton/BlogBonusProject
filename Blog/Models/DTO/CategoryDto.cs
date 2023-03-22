
namespace Blog.Models.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public ICollection<PostDto>? Posts { get; set; }
    }
}
