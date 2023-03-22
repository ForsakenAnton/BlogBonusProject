
using Blog.Models.DTO;

namespace Blog.Models.ViewModels.SharedViewModels
{
    public class CategoriesDropDownListVM
    {
        public IEnumerable<CategoryDto> Categories { get; set; } = default!;
        public int CategoryId { get; set; }
    }
}
