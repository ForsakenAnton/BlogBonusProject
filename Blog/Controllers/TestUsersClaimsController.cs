using Blog.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class TestUsersClaimsController : Controller
    {

        [Authorize(Policy = MyPolicies.PostsWriterAndAboveAccess)]
        public IActionResult PostsWtiter()
        {
            string? user = User.Identity!.Name;
            return Content(user ?? "user is null");
        }

        [Authorize(Policy = MyPolicies.AdminAndAboveAccess)]
        public IActionResult Admin()
        {
            string? user = User.Identity!.Name;
            return Content(user ?? "user is null");
        }

        [Authorize(Policy = MyPolicies.SuperAdminAccessOnly)]
        public IActionResult SuperAdmin()
        {
            string? user = User.Identity!.Name;
            return Content(user ?? "user is null");
        }
    }
}
