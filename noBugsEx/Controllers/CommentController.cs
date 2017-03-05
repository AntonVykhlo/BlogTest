using noBugsEx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace noBugsEx.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        CoreObjects model = new CoreObjects();
        private const int CommentsPerPage = 4;

        public ActionResult Index(int? id)
        {
            int pageNumber = id ?? 0;
            IEnumerable<Comment> comments = (from comment in model.Comments
                                             where comment.DateTime < DateTime.Now
                                             orderby comment.DateTime descending
                                             select comment).Skip(pageNumber * CommentsPerPage).Take(CommentsPerPage + 1);
            ViewBag.IsPreviousLinkVisible = pageNumber > 0;
            ViewBag.IsNextLinkVisible = comments.Count() > CommentsPerPage;
            ViewBag.pageNumber = pageNumber;



            return View(comments.Take(CommentsPerPage));
        }
        [Authorize(Roles = "Admin, Moderator,Client")]
        public ActionResult Update(int? id, string title, string body)
        {

            Comment com = GetComment(id);
            com.Title = title;
            com.Body = body;
            com.DateTime = DateTime.Now;
            if (!id.HasValue)
            {
                model.Comments.Add(com);
            }
            ViewBag.CanEdit = true;
            model.SaveChanges();
            return RedirectToAction("Details", new { id = com.Id });
        }

        [ValidateInput(false)]
        [Authorize(Roles = "Admin, Moderator, Client")]
        public ActionResult Edit(int? id)
        {
            ViewBag.CanEdit = true;
            Comment com = GetComment(id);
            return View(com);
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            Comment comment = GetComment(id);
            model.Comments.Remove(comment);
            model.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int? id)
        {
            Comment com = GetComment(id);
            return View(com);
        }


        private Comment GetComment(int? id)
        {
            return id.HasValue ? model.Comments.Where(x => x.Id == id).First() : new Comment() { Id = -1 };
        }
    }

}