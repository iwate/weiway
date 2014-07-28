using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WeiWay.Models;
using Microsoft.AspNet.Identity;

namespace WeiWay.ApiControllers
{
    public class MessageQuery
    {
        public int Page { get; set; }
    }
    [Authorize]
    public class MessagesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private int LIMIT = 100;
        // GET: api/Messages
        public IList<Message> GetMessages([FromUri] MessageQuery query)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            return db.Messages.ToList().Where(x => !user.Messages.Contains(x)).OrderByDescending(x => x.CreateTime).ToList();
            //return db.Messages.OrderByDescending(x => x.CreateTime).Skip(LIMIT * query.Page).Take(LIMIT).ToList();
        }

        // GET: api/Messages/5
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> GetMessage(string id)
        {
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Messages/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMessage(string id, Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != message.Id)
            {
                return BadRequest();
            }

            db.Entry(message).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
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

        // POST: api/Messages
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> PostMessage(Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //db.Messages.Add(message);
            var model = await db.Messages.FindAsync(message.Id);
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.Messages.Contains(model))
            {
                return BadRequest("This wei is already having.");
            }
            user.Messages.Add(model);
            model.Users.Add(user);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MessageExists(message.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> DeleteMessage(string id)
        {
            Message message = await db.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            db.Messages.Remove(message);
            await db.SaveChangesAsync();

            return Ok(message);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MessageExists(string id)
        {
            return db.Messages.Count(e => e.Id == id) > 0;
        }
    }
}