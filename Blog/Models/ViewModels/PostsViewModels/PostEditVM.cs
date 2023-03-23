using Blog.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Models.ViewModels.PostsViewModels
{
    public class PostEditVM
    {
        public PostDto Post { get; set; } = default!;
        public SelectList? CategoriesSL { get; set; }

        public IFormFile? Image { get; set; }
    }
}
