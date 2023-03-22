using Blog.Data.Entities;
using Blog.Data;
using Blog.Models.ViewModels.SharedViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Blog.Models.DTO;

namespace Blog.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public CategoriesViewComponent(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<Category> categories = _context.Categories;
            int categoryId = 0;

            if (Request.Query.ContainsKey("categoryId"))
            {
                int.TryParse(Request.Query["categoryId"].ToString(), out categoryId);
            }

            CategoriesDropDownListVM viewModel = new CategoriesDropDownListVM
            {
                Categories = _mapper.Map<IEnumerable<CategoryDto>>(await categories.ToListAsync()),
                CategoryId = categoryId
            };

            return View(viewModel);
        }
    }
}
