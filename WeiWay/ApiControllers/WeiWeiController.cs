using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WeiWay.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Core.Objects;
using System.Threading.Tasks;

namespace WeiWay.ApiControllers
{
    public class WeiWeiGetQuery
    {
        public string before { get; set; }
    }
    [Authorize]
    public class WeiWeiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Microsoft.AspNet.SignalR.IHubContext dashboardHubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<HubControllers.DashboardHub>();
        private static int LIMIT = 20;
        // GET: api/Weis
        public List<WeiViewModel> GetWeiWei([FromUri]WeiWeiGetQuery query)
        {
            if (query.before == null)
            {
                return db.WeiWei.OrderByDescending(x => x.CreateTime).Take(LIMIT).Select(x => new WeiViewModel {
                    id = x.Id,
                    text = x.Message.Text,
                    imageUrl = x.ImageUrl,
                    postDate = x.CreateTime,
                    userName = x.User.UserName,
                    userIcon = x.User.Icon,
                    onechances = x.OneChanceUsers.Count,
                    comments = x.Comments
                }).ToList();
            }

            return db.WeiWei.OrderByDescending(x => x.CreateTime).ToArray().SkipWhile(x => x.Id != query.before).Take(LIMIT).Select(x => new WeiViewModel
            { 
                id = x.Id,
                text = x.Message.Text,
                imageUrl = x.ImageUrl,
                postDate = x.CreateTime,
                userName = x.User.UserName,
                userIcon = x.User.Icon,
                onechances = x.OneChanceUsers.Count,
                comments = x.Comments
            }).ToList();
        }

        // GET: api/Weis/5
        [ResponseType(typeof(Wei))]
        public IHttpActionResult GetWei(string id)
        {
            Wei wei = db.WeiWei.Find(id);
            if (wei == null)
            {
                return NotFound();
            }

            return Ok(wei);
        }

        // PUT: api/Weis/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutWei(string id, Wei wei)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != wei.Id)
            {
                return BadRequest();
            }

            db.Entry(wei).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeiExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
        
        // POST: api/Weis
        [ResponseType(typeof(Wei))]
        public async Task<IHttpActionResult> PostWei(Wei wei)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            wei.User = db.Users.Find(User.Identity.GetUserId());

            db.WeiWei.Add(wei);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (WeiExists(wei.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            wei.Message = await db.Messages.FindAsync(wei.MessageId);
            dashboardHubContext.Clients.All.Wei(new WeiViewModel {
                id = wei.Id, 
                text = wei.Message.Text,
                imageUrl = wei.ImageUrl,
                userIcon = wei.User.Icon,
                userName = wei.User.UserName,
                postDate = wei.CreateTime,
                onechances = wei.OneChanceUsers.Count,
                comments = wei.Comments
            });

            return CreatedAtRoute("DefaultApi", new { id = wei.Id }, wei);
        }

        // DELETE: api/Weis/5
        [ResponseType(typeof(Wei))]
        public IHttpActionResult DeleteWei(string id)
        {
            Wei wei = db.WeiWei.Find(id);
            if (wei == null)
            {
                return NotFound();
            }

            db.WeiWei.Remove(wei);
            db.SaveChanges();

            return Ok(wei);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WeiExists(string id)
        {
            return db.WeiWei.Count(e => e.Id == id) > 0;
        }
    }
}