
using Blog.Models.DTO;

namespace Blog.Models.ViewModels.CommentsViewModels
{
    public class CommentVM
    {
        public CommentDto? Comment { get; set; }
        public bool IsReply { get; set; }
        public int CurrentNested { get; set; }

        public const int MaxNested = 5;
        public string BackgroundColor => CurrentNested % 2 == 0 ? "lightgray" : "white";

    }
}
