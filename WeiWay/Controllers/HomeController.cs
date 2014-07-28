using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeiWay.Models;
using Microsoft.AspNet.Identity;

namespace WeiWay.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db;
        public HomeController()
        {
            this.db = new ApplicationDbContext();
        }
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var viewModel = new DashboardViewModel {
                    WeiWei = user.Messages.OrderBy(x => x.Index).Select(x => new SelectListItem {
                        Value = x.Id,
                        Text = x.Text
                    }).ToList(), UserName = user.UserName, UserIcon = user.Icon
                };
                return View("Dashboard", viewModel);
            }
            else
            {
                return View();  
            }
        }
        [Authorize]
        public ActionResult Public()
        {
            return View();
        }
        public ActionResult Policy()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db = null;
            }
            base.Dispose(disposing);
        }
    }
}