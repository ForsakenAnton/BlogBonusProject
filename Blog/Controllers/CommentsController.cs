using Blog.Data;
using Blog.Data.Entities;
using Blog.Models.DTO;
using Blog.Models.ViewModels.CommentsViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Blog.Authorization;

namespace Blog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<CommentsController> _logger;
        private readonly UserManager<User> _userManager;

        private readonly IMapper _mapper;

        public CommentsController(
            ApplicationContext context,
            ILogger<CommentsController> logger,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
        }


        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentAjaxModel commentAjaxModel)
        {
            User currentUser = await _userManager.GetUserAsync(User); // it is HttpContext.User
            if (currentUser == null)
            {
                return Unauthorized();
            }

            string userId = currentUser.Id;

            Comment commentToAdd = new Comment
            {
                Message = commentAjaxModel.Message,
                ParentCommentId = commentAjaxModel.ParentCommentId,
                PostId = commentAjaxModel.PostId,
                Created = DateTime.Now,
                UserId = userId,
            };

            await _context.Comments.AddAsync(commentToAdd);
            await _context.SaveChangesAsync();

            // подгружаем сущность Post
            await _context
                .Entry(commentToAdd)
                .Reference(c => c.Post)
                .LoadAsync();

            CommentVM commentVM = new CommentVM
            {
                Comment = _mapper.Map<CommentDto>(commentToAdd),
                CurrentNested = commentAjaxModel.CurrentNested,
                IsReply = commentAjaxModel.IsReply,
            };

            return PartialView("_WriteParentCommentAndChildrenPartial", commentVM);
        }


        [Authorize]
        public async Task<IActionResult> EditComment([FromBody] CommentAjaxModel commentAjaxModel)
        {
            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            Comment? commentToEdit = await _context.Comments
                .FindAsync(commentAjaxModel.Id);

            if (commentToEdit is null)
            {
                return NotFound();
            }

            commentToEdit.Message = commentAjaxModel.Message;

            _context.Comments.Update(commentToEdit);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [Authorize(MyPolicies.AdminAndAboveAccess)]
        public async Task<IActionResult> DeleteComment([FromBody] int? commentId)
        {
            Comment? comment = await _context.Comments.FindAsync(commentId);
            if (comment is null)
            {
                return BadRequest();
            }

            await _context.Entry(comment).Collection(c => c.ChildComments).LoadAsync();
            await _context.Entry(comment).Reference(c => c.ParentComment).LoadAsync();

            _context.Comments.Remove(comment);

            int deletedCount = 1;
            await RemoveChildComments(comment); // recursion

            async Task RemoveChildComments(Comment comment)
            {
                foreach (var childComment in comment.ChildComments)
                {
                    _context.Comments.Remove(childComment);

                    deletedCount++;

                    await _context.Entry(childComment).Collection(c => c.ChildComments).LoadAsync();
                    // await _context.Entry(childComment).Reference(c => c.ParentComment).LoadAsync();

                    if (childComment.ChildComments!.Count > 0)
                    {
                        await RemoveChildComments(childComment);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(deletedCount);
        }
    }
}
