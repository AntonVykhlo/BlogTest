using noBugsEx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace noBugsEx.Controllers
{
    public class SuppliedServicesController : Controller
    {
        CoreObjects model = new CoreObjects();
        private const int PostsPerPage = 4;

        public ActionResult Index(int? id)
        {
            int pageNumber = id ?? 0;
            IEnumerable<SuppliedService> suppServ = (from serv in model.SuppliedServices
                                                     orderby serv.Id descending
                                                     select serv).Skip(pageNumber * PostsPerPage).Take(PostsPerPage + 1);
            ViewBag.IsPreviousLinkVisible = pageNumber > 0;
            ViewBag.IsNextLinkVisible = suppServ.Count() > PostsPerPage;
            ViewBag.pageNumber = pageNumber;

            return View(suppServ.Take(PostsPerPage));
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Update(int? id, string title, string shortDescription, string FullDescription)
        {

            SuppliedService supServ = GetService(id);
            supServ.Title = title;
            supServ.ShortDescription = shortDescription;
            supServ.FullDescription = FullDescription;
            if (!id.HasValue)
            {
                model.SuppliedServices.Add(supServ);
            }
            model.SaveChanges();
            return RedirectToAction("Details", new { id = supServ.Id });
        }

        [ValidateInput(false)]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
        {
            SuppliedService serv = GetService(id);
            return View(serv);
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            SuppliedService supServ = GetService(id);
            model.SuppliedServices.Remove(supServ);
            model.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int? id)
        {
            SuppliedService supServ = GetService(id);
            return View(supServ);
        }


        private SuppliedService GetService(int? id)
        {
            return id.HasValue ? model.SuppliedServices.Where(x => x.Id == id).First() : new SuppliedService() { Id = -1 };
        }
    }
}
