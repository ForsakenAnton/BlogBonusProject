using Blog.Models.DTO;
using Microsoft.AspNetCore.Mvc;

using Blog.Extentions;

namespace Blog.Components
{
    public class LastViewedPostsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<PostDto> sessionPosts = new List<PostDto>();

            foreach (string key
                in HttpContext.Session.Keys
                    .Where(k => k.Contains("LastViewedPosts")))
            {
                sessionPosts.Add(HttpContext.Session.Get<PostDto>(key)!);
            }

            return View(sessionPosts);
        }
    }
}
