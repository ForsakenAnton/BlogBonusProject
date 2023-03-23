
//using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.DTO
{
    public class PostDto
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string? Body { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        [Display(Name = "Image")]
        public string? MainPostImagePath { get; set; } //= default!;

        //public bool IsDeleted { get; set; } это лишнее свойство

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        public string? UserId { get; set; }

        public CategoryDto? Category { get; set; }
        public UserDto? User { get; set; }

        public ICollection<CommentDto>? Comments { get; set; }
    }
}
