
using Blog.Models.DTO;

namespace Blog.Models.ViewModels.PostsViewModels
{
    public class PostsIndexVM
    {
        public IEnumerable<PostDto> Posts { get; set; } = default!;
        // public IEnumerable<Category> Categories { get; set; } = default!;
        // public int CategoryId { get; set; }

        public FilterVM? FilterVM { get; set; }
        public SortVM? SortVM { get; set; }
        public PageVM? PageVM { get; set; }

        public PostsIndexVM(
            IEnumerable<PostDto> posts,
            // IEnumerable<Category> categories,
            // int categoryId,
            FilterVM? filterVM,
            SortVM? sortVM,
            PageVM? pageVM)
        {
            Posts = posts;
            // Categories = categories;
            // CategoryId = categoryId;
            FilterVM = filterVM;
            SortVM = sortVM;
            PageVM = pageVM;
        }
    }
}
