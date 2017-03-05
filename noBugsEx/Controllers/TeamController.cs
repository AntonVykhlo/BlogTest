using noBugsEx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace noBugsEx.Controllers
{
    public class TeamController : Controller
    {
        CoreObjects model = new CoreObjects();
        private const int MembersPerPage = 4;

        public ActionResult Index(int? id)
        {
            int pageNumber = id ?? 0;
            IEnumerable<Team> members = (from member in model.Teams
                                         orderby member.Id descending
                                         select member).Skip(pageNumber * MembersPerPage).Take(MembersPerPage + 1);
            ViewBag.IsPreviousLinkVisible = pageNumber > 0;
            ViewBag.IsNextLinkVisible = members.Count() > MembersPerPage;
            ViewBag.pageNumber = pageNumber;
            return View(members.Take(MembersPerPage));
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Update(int? id, string firstName, string lastName, string email, string position, string image)
        {

            Team member = getMember(id);
            member.FirstName = firstName;
            member.LastName = lastName;
            member.Email = email;
            member.Position = position;
            member.Image = image;
            if (!id.HasValue)
            {
                model.Teams.Add(member);
            }
            ViewBag.CanEdit = true;
            model.SaveChanges();
            return RedirectToAction("Details", new { id = member.Id });
        }

        [ValidateInput(false)]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Edit(int? id)
        {
            ViewBag.CanEdit = true;
            Team com = getMember(id);
            return View(com);
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            Team member = getMember(id);
            model.Teams.Remove(member);
            model.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Details(int? id)
        {
            Team member = getMember(id);
            return View(member);
        }


        private Team getMember(int? id)
        {
            return id.HasValue ? model.Teams.Where(x => x.Id == id).First() : new Team() { Id = -1 };
        }
    }

}