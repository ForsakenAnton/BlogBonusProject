
namespace Blog.Models.DTO
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string? Body { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string MainPostImagePath { get; set; } = default!;

        //public bool IsDeleted { get; set; } это лишнее свойство

        public int? CategoryId { get; set; }
        public string? UserId { get; set; }

        public CategoryDto? Category { get; set; }
        public UserDto? User { get; set; }

        public ICollection<CommentDto>? Comments { get; set; }
    }
}
