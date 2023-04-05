using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Data.Entities;
using AutoMapper;
using Blog.Models.ViewModels.PostsViewModels;
using Blog.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Blog.Authorization;
using Blog.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly UserManager<User> _userManager;

        public PostsController(
            ApplicationContext context, 
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(
            int categoryId,
            string? search,
            int page = 1,
            SortState sortOrder = SortState.TitleAsc)
        {
            int pageSize = 9;

            IQueryable<Post> posts = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Where(p => p.IsDeleted == false)
                .AsNoTracking<Post>();


            // filter
            if (categoryId != 0)
            {
                posts = posts.Where(p => p.Category!.Id == categoryId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                posts = posts.Where(p => p.Title.Contains(search));
            }

            // sort
            switch (sortOrder)
            {
                case SortState.TitleDesc:
                    posts = posts.OrderByDescending(p => p.Title);
                    break;

                case SortState.DescriptionAsc:
                    posts = posts.OrderBy(p => p.Description);
                    break;
                case SortState.DescriptionDesc:
                    posts = posts.OrderByDescending(p => p.Description);
                    break;

                case SortState.CategoryAsc:
                    posts = posts.OrderBy(p => p.Category!.Name);
                    break;
                case SortState.CategoryDesc:
                    posts = posts.OrderByDescending(p => p.Category!.Name);
                    break;

                case SortState.CreatedAsc:
                    posts = posts.OrderBy(p => p.Created);
                    break;
                case SortState.CreatedDesc:
                    posts = posts.OrderByDescending(p => p.Created);
                    break;

                default:
                    posts = posts.OrderBy(p => p.Title);
                    break;
            }


            // pagination
            int postsCount = await posts.CountAsync();

            //List<Post> postItems = await posts
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToListAsync();

            IQueryable<Post> postItems = posts
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            //foreach (Post postItem in postItems)
            //{
            //    await _context.Entry(postItem).Reference(p => p.Category!).LoadAsync();
            //    await _context.Entry(postItem).Reference(p => p.User!).LoadAsync();
            //}

            List<Category> categories = await _context.Categories.ToListAsync();

            PostsIndexVM postsIndexVM = new PostsIndexVM(
                _mapper.Map<IEnumerable<PostDto>>(postItems),
                // categories,
                // categoryId,
                new FilterVM(_mapper.Map<List<CategoryDto>>(categories), categoryId, search),
                new SortVM(sortOrder),
                new PageVM(postsCount, page, pageSize)
                );

            return View(postsIndexVM);
        }


        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            Post? post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Comments)!
                    .ThenInclude(c => c.User)
                // .ThenInclude(c => c.ChildComments)
                .AsSplitQuery<Post>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null || post.IsDeleted == true)
            {
                return NotFound();
            }

            //Post? post = await _context.Posts
            //    .Include(p => p.Category)
            //    .Include(p => p.User)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            //if (post == null || post.IsDeleted == true)
            //{
            //    return NotFound();
            //}

            //await _context.Entry(post).Collection(p => p.Comments!).LoadAsync();
            //foreach (var comment in post.Comments!)
            //{
            //    await _context.Entry(comment).Reference(c => c.User).LoadAsync();
            //}

            HttpContext.Session.Set<PostDto>(
                "LastViewedPosts" + post.Id,
                _mapper.Map<PostDto>(post));

            PostDetailVM model = new PostDetailVM
            {
                Post = _mapper.Map<PostDto>(post)
            };

            return View(model);
        }


        // GET: Posts/Create
        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public IActionResult Create()
        {
            SelectList categoriesSL = new SelectList(
                _mapper.Map<IEnumerable<CategoryDto>>(_context.Categories.AsNoTracking()),
                "Id",
                "Name");

            PostCreateVM model = new PostCreateVM
            {
                CategoriesSL = categoriesSL,
            };

            return View(model);
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public async Task<IActionResult> Create(PostCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                SelectList categoriesSL = new SelectList(
                    _mapper.Map<IEnumerable<CategoryDto>>(_context.Categories.AsNoTracking()),
                    "Id",
                    "Name",
                    model.Post.CategoryId);

                model.CategoriesSL = categoriesSL;

                return View(model);
            }

            string path = "/images/" + model.Image.FileName;
            string webRootPath = _webHostEnvironment.WebRootPath;
            string fullPath = webRootPath + path;

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await model.Image.CopyToAsync(fileStream);
            }

            model.Post.MainPostImagePath = path;

            UserDto currentUser = _mapper
                .Map<UserDto>(await _userManager.GetUserAsync(HttpContext.User));

            model.Post.UserId = currentUser.Id;

            Post postToCreate = _mapper.Map<Post>(model.Post);

            _context.Add(postToCreate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: Posts/Edit/5
        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.IsDeleted == true)
            {
                return NotFound();
            }

            SelectList categoriesSL = new SelectList(
                _mapper.Map<IEnumerable<CategoryDto>>(_context.Categories.AsNoTracking()),
                "Id",
                "Name",
                post.Id);

            PostEditVM model = new PostEditVM
            {
                Post = _mapper.Map<PostDto>(post),
                CategoriesSL = categoriesSL
            };

            return View(model);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public async Task<IActionResult> Edit(int id, PostEditVM model)
        {
            if (id != model.Post.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                SelectList categoriesSL = new SelectList(
                    _mapper.Map<IEnumerable<CategoryDto>>(_context.Categories.AsNoTracking()),
                    "Id",
                    "Name",
                    model.Post.Id);

                model.CategoriesSL = categoriesSL;

                return View(model);
            }

            if (model.Image is not null)
            {
                string path = "/images/" + model.Image.FileName;
                string webRootPath = _webHostEnvironment.WebRootPath;
                string fullPath = webRootPath + path;

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(fileStream);
                }

                model.Post.MainPostImagePath = path;
            }

            Post postToEdit = _mapper.Map<Post>(model.Post);

            try
            {
                _context.Update(postToEdit);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(postToEdit.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }



        // GET: Posts/Delete/5
        [Authorize(Policy = MyPolicies.AdminAndAboveAccess)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            Post? post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Comments)!
                // .ThenInclude(c => c.ChildComments)
                .AsNoTracking<Post>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null || post.IsDeleted == true)
            {
                return NotFound();
            }

            PostDeleteVM model = new PostDeleteVM
            {
                Post = _mapper.Map<PostDto>(post)
            };

            return View(model);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = MyPolicies.AdminAndAboveAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationContext.Posts'  is null.");
            }

            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                // _context.Posts.Remove(post);
                post.IsDeleted = true;
                HttpContext.Session.Remove<Post>("LastViewedPosts" + post.Id);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
