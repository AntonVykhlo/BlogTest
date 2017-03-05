using noBugsEx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace noBugsEx.Controllers
{
    public class BlogController : Controller
    {
        CoreObjects model = new CoreObjects();
        private const int PostsPerPage = 4;

        public ActionResult Index(int? id)
        {
            int pageNumber = id ?? 0;
            IEnumerable<Post> posts = (from post in model.Posts
                                       where post.DateTime < DateTime.Now
                                       orderby post.DateTime descending
                                       select post).Skip(pageNumber * PostsPerPage).Take(PostsPerPage + 1);
            ViewBag.IsPreviousLinkVisible = pageNumber > 0;
            ViewBag.IsNextLinkVisible = posts.Count() > PostsPerPage;
            ViewBag.pageNumber = pageNumber;



            return View(posts.Take(PostsPerPage));
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Update(int? id, string title, string body)
        {

            Post post = GetPost(id);
            post.Title = title;
            post.Body = body;
            post.DateTime = DateTime.Now;
            if (!id.HasValue)
            {
                model.Posts.Add(post);
            }
            ViewBag.CanEdit = true;
            model.SaveChanges();
            return RedirectToAction("Details", new { id = post.Id });
        }

        [ValidateInput(false)]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
        {
            ViewBag.CanEdit = true;
            Post post = GetPost(id);
            return View(post);
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            Post post = GetPost(id);
            model.Posts.Remove(post);
            model.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int? id)
        {
            Post post = GetPost(id);
            return View(post);
        }


        private Post GetPost(int? id)
        {
            return id.HasValue ? model.Posts.Where(x => x.Id == id).First() : new Post() { Id = -1 };
        }
    }
}