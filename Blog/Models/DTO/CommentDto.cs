
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = default!;
        public DateTime Created { get; set; }


        public int PostId { get; set; }
        public string UserId { get; set; } = default!;
        public PostDto? Post { get; set; }
        public UserDto? User { get; set; }

        public int? ParentCommentId { get; set; }

        [ForeignKey(nameof(ParentCommentId))]
        public CommentDto? ParentComment { get; set; }
        public ICollection<CommentDto>? ChildComments { get; set; }
    }
}
