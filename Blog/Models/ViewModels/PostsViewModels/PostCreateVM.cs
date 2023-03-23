using Blog.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models.ViewModels.PostsViewModels
{
    public class PostCreateVM
    {
        public PostDto Post { get; set; } = default!;
        public SelectList? CategoriesSL { get; set; }

        [Required]
        public IFormFile Image { get; set; } = default!;
    }
}
