using Blog.Authorization;
using Blog.Data.Entities;
using Blog.Models.ViewModels.SuperAdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blog.Controllers
{
    [Authorize(Policy = MyPolicies.SuperAdminAccessOnly)]
    public class SuperAdminController : Controller
    {
        private readonly UserManager<User> _userManager;

        public SuperAdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> ManageUsersClaims()
        {
            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            IQueryable<User> users = _userManager.Users
                .Where(u => u.EmailConfirmed == true && // email подтвержден и так же
                              u.Id != currentUser.Id); // самого себя не выбираем


            IList<UserAccessVM> userAccessModels = new List<UserAccessVM>();

            foreach (User user in users)
            {
                UserAccessVM userAccessModel = new UserAccessVM();

                IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);

                if (userClaims.Any(c => c.Type == MyClaims.Admin))
                {
                    userAccessModel.Access = Access.Admin;
                }
                else if (userClaims.Any(c => c.Type == MyClaims.PostsWriter))
                {
                    userAccessModel.Access = Access.PostsWriter;
                }
                else
                {
                    userAccessModel.Access = Access.None;
                }

                userAccessModel.Email = user.Email;
                userAccessModels.Add(userAccessModel);
            }

            return View(userAccessModels);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUsersClaims(List<UserAccessVM> userAccessModels)
        {
            Claim postWriterClaim = new Claim(MyClaims.PostsWriter, MyClaims.PostsWriter);
            Claim adminClaim = new Claim(MyClaims.Admin, MyClaims.Admin);

            ViewBag.Message = null;

            foreach (var userAccessModel in userAccessModels)
            {
                User? user = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == userAccessModel.Email);

                if (user is not null)
                {
                    IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
                    await _userManager.RemoveClaimsAsync(user, userClaims); // удаляем все утверждения

                    if (userAccessModel.Access == Access.PostsWriter)
                    {
                        await _userManager.AddClaimAsync(user, postWriterClaim);
                    }
                    else if (userAccessModel.Access == Access.Admin)
                    {
                        await _userManager.AddClaimAsync(user, postWriterClaim);
                        await _userManager.AddClaimAsync(user, adminClaim);
                    }
                }
            }
            ViewBag.Message = "Claims was managed successfully!";
            return View(userAccessModels);
        }
    }
}
