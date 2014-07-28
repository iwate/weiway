using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WeiWay.Models;
using Microsoft.AspNet.Identity;

namespace WeiWay.ApiControllers
{
    public class OnechancesController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private Microsoft.AspNet.SignalR.IHubContext dashboardHubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<HubControllers.DashboardHub>();
        // GET: api/Onechances
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Onechances/5
        public string Get(string id)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var wei = db.WeiWei.Find(id);
            if (wei.OneChanceUsers.Where(x => x.Id == user.Id).Count() == 0)
            {
                wei.OneChanceUsers.Add(user);
                //user.Onechances.Add(wei);
                db.SaveChanges();
                dashboardHubContext.Clients.All.Onechan("wei-" + wei.Id);
            }
            return "";
        }

        // POST: api/Onechances
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Onechances/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Onechances/5
        public void Delete(int id)
        {
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
