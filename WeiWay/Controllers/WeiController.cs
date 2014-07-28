using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeiWay.Models;
using Microsoft.AspNet.Identity;

namespace WeiWay.Controllers
{
    [Authorize]
    public class WeiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Wei
        public ActionResult Index()
        {
            return Redirect("Store");
        }
        
        public ActionResult Store()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var message = db.Messages.ToList().Where(x => !user.Messages.Contains(x)).ToList();
            return View(new StoreViewModel { UserName = user.UserName, UserIcon = user.Icon, Messages =  message});
        }
        public ActionResult Mine()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            return View(new StoreViewModel { UserName = user.UserName, UserIcon = user.Icon, Messages = user.Messages.ToList() });
        }

        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            return View(new StoreViewModel { UserName = user.UserName, UserIcon = user.Icon});
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing){
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}