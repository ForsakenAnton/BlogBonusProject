namespace Blog.Models.ViewModels.PostsViewModels
{
    public class SortVM
    {
        public SortState TitleSort { get; set; }
        public SortState DescriptionSort { get; set; }
        public SortState CategorySort { get; set; }
        public SortState CreatedSort { get; set; }
        public SortState Current { get; set; }

        public SortVM(SortState sortOrder)
        {
            TitleSort = sortOrder == SortState.TitleAsc ?
                SortState.TitleDesc :
                SortState.TitleAsc;

            DescriptionSort = sortOrder == SortState.DescriptionAsc ?
                SortState.DescriptionDesc :
                SortState.DescriptionAsc;

            CategorySort = sortOrder == SortState.CategoryAsc ?
                SortState.CategoryDesc :
                SortState.CategoryAsc;

            CreatedSort = sortOrder == SortState.CreatedAsc ?
                SortState.CreatedDesc :
                SortState.CreatedAsc;

            Current = sortOrder;
        }
    }

    public enum SortState
    {
        TitleAsc,
        TitleDesc,
        DescriptionAsc,
        DescriptionDesc,
        CategoryAsc,
        CategoryDesc,
        CreatedAsc,
        CreatedDesc
    }
}
