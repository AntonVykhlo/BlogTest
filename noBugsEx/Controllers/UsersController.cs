using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using noBugsEx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace noBugsEx.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Index()
        {
            using (var context = new ApplicationDbContext())
            {

                ViewBag.Users = context.Users.Select(u => u).ToList();
                ViewBag.RoleNames = new string[] { "Admin", "Moderator", "Client", "User" };
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditAccountRole(string email, string role)
        {
            using (var context = new ApplicationDbContext())
            {
                string[] roles = { "Admin", "Moderator", "Client", "User" };
                var user = new ApplicationUser() { };
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                user = userManager.FindByEmail(email);
                userManager.RemoveFromRoles(user.Id, roles);
                userManager.AddToRole(user.Id, role);

            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult GetClientList()
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var roleUserIdsQuery = from role in context.Roles
                                       where role.Name == "Client"
                                       from user in role.Users
                                       select user.UserId;
                ViewBag.Users = context.Users.Where(u => roleUserIdsQuery.Contains(u.Id)).ToList();
            }
            return View();
        }

    }
}