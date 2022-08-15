using HydroServerTools.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
//using WebApplication5.Models;

namespace HydroServerTools.Areas.Admin.Controllers
{
    [Authorize]
    public class ConnectionParametersUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: EntityConnectionstringParametersUsers
        public ActionResult Index()
        {
           
            if(User.IsInRole("PowerUser"))
            {
                var user = User.Identity.Name.ToString();
                //return filtered list
                var l = db.ConnectionParametersUser.ToList().Where(p => p.User.UserName.ToString() == user).OrderBy(p => p.User.UserName).ToList();
                return View(l);
            }
            else
            {
                //return all
                return View(db.ConnectionParametersUser.ToList().OrderBy(p => p.User.UserName));
            }
           
        }

        // GET: EntityConnectionstringParametersUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConnectionParametersUser connectionParametersUser = db.ConnectionParametersUser.Find(id);
            if (connectionParametersUser == null)
            {
                return HttpNotFound();
            }
            return View(connectionParametersUser);
        }

        // GET: EntityConnectionstringParametersUsers/Create
        public ActionResult Create()
        {
            if (User.IsInRole("PowerUser"))
            {
                var user = User.Identity.Name.ToString();
                var userid = db.Users.FirstOrDefault(p => p.UserName == user).Id;
                //return filtered list
               ViewBag.Users = new SelectList(db.Users.Where(p => p.UserName.ToString() == user).OrderBy(p => p.UserName), "Id", "UserName");
              //  ViewBag.ConnectionParameters = new SelectList(db.ConnectionParameters.Where(p => p.UserId.ToString() == userid).OrderBy(p => p.Name), "Id", "Name");
                ViewBag.ConnectionParameters = new SelectList(from cpu in db.ConnectionParametersUser
                                                              join cp in db.ConnectionParameters on cpu.ConnectionParametersId equals cp.Id
                                                              where cpu.UserId == userid
                                                              select cp.Name);


            }
            else
            {
                //return all
                ViewBag.Users = new SelectList(db.Users.OrderBy(p => p.UserName), "Id", "UserName");
                ViewBag.ConnectionParameters = new SelectList(db.ConnectionParameters.OrderBy(p => p.Name), "Id", "Name");
            }


            return View();
        }

        // POST: EntityConnectionstringParametersUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                 
                

                //db.EntityConnectionstringParametersUsers.Add(model);
                var m = new ConnectionParametersUser();
                m.ConnectionParametersId = Convert.ToInt32(collection["ConnectionParameters"]);
               // m.EntityConnectionstringParameters.UserId = model.UserName.ToString();
                m.UserId =collection["UserName"];
                var existingConnectionID = db.ConnectionParametersUser.FirstOrDefault(p => p.UserId == m.UserId && p.ConnectionParametersId == m.ConnectionParametersId);
                var existingConnectionName = db.ConnectionParametersUser.FirstOrDefault(p => p.UserId == m.UserId && p.ConnectionParametersId == m.ConnectionParametersId).ConnectionParameters.Name;
                var isDuplicate = db.ConnectionParametersUser.Where(p => p.UserId == m.UserId && p.ConnectionParametersId == m.ConnectionParametersId).Count() > 0;

                if (!isDuplicate)
                {
                    var userHasConnection = db.ConnectionParametersUser.Where(p => p.UserId == m.UserId).Count() > 0;
                    if (!userHasConnection)
                    {
                        
                        db.ConnectionParametersUser.Add(m);
                        // m.EntityConnectionstringParameters.Id= model.EntityConnectionstringParametersName
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("hasConnection", "User already has a connection assigned. Replacing the current connection");
                        ConnectionParametersUser entityConnectionstringParametersUser = db.ConnectionParametersUser.Find(existingConnectionID);
                        db.ConnectionParametersUser.Remove(entityConnectionstringParametersUser);
                        db.ConnectionParametersUser.Add(m);
                        db.SaveChanges();

                    }
                }
                else
                {
                   // ViewBag.ResultMessage = "is Duplicate";
                   
                    ModelState.AddModelError("duplicate", "Link already exists. No Duplicate entries are allowed");   
                }
                ViewBag.Users = new SelectList(db.Users.OrderBy(p => p.UserName), "Id", "UserName");
                ViewBag.ConnectionParameters = new SelectList(db.ConnectionParameters.OrderBy(p => p.Name), "Id", "Name");
                return View(m);
            }

            return View();
        }

        // GET: EntityConnectionstringParametersUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Users = new SelectList(db.Users.OrderBy(p => p.UserName), "Id", "UserName");
            ViewBag.ConnectionParameters = new SelectList(db.ConnectionParameters.OrderBy(p => p.Name), "Id", "Name");

            ConnectionParametersUser entityConnectionstringParametersUser = db.ConnectionParametersUser.Find(id);
            if (entityConnectionstringParametersUser == null)
            {
                return HttpNotFound();
            }
            return View(entityConnectionstringParametersUser);
        }

        // POST: EntityConnectionstringParametersUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id")] ConnectionParametersUser entityConnectionstringParametersUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entityConnectionstringParametersUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entityConnectionstringParametersUser);
        }

        // GET: EntityConnectionstringParametersUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConnectionParametersUser entityConnectionstringParametersUser = db.ConnectionParametersUser.Find(id);
            if (entityConnectionstringParametersUser == null)
            {
                return HttpNotFound();
            }
            db.ConnectionParametersUser.Remove(entityConnectionstringParametersUser);
            db.SaveChanges();
            return RedirectToAction("Index");
            //return View(entityConnectionstringParametersUser);
        }

        // POST: EntityConnectionstringParametersUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConnectionParametersUser entityConnectionstringParametersUser = db.ConnectionParametersUser.Find(id);
            db.ConnectionParametersUser.Remove(entityConnectionstringParametersUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
