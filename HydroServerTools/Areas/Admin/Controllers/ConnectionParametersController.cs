using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HydroServerTools.Models;

namespace HydroServerTools.Areas.Admin.Controllers
{
    public class ConnectionParametersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin/ConnectionParameters
        public ActionResult Index()
        {
            return View(db.ConnectionParameters.ToList());
        }

        // GET: Admin/ConnectionParameters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConnectionParameters connectionParameters = db.ConnectionParameters.Find(id);
            if (connectionParameters == null)
            {
                return HttpNotFound();
            }
            return View(connectionParameters);
        }

        // GET: Admin/ConnectionParameters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/ConnectionParameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,DataSource,InitialCatalog,UserId,Password")] ConnectionParameters connectionParameters)
        {
            if (ModelState.IsValid)
            {
                db.ConnectionParameters.Add(connectionParameters);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(connectionParameters);
        }

        // GET: Admin/ConnectionParameters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConnectionParameters connectionParameters = db.ConnectionParameters.Find(id);
            if (connectionParameters == null)
            {
                return HttpNotFound();
            }
            return View(connectionParameters);
        }

        // POST: Admin/ConnectionParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,DataSource,InitialCatalog,UserId,Password")] ConnectionParameters connectionParameters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(connectionParameters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(connectionParameters);
        }

        // GET: Admin/ConnectionParameters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConnectionParameters connectionParameters = db.ConnectionParameters.Find(id);
            if (connectionParameters == null)
            {
                return HttpNotFound();
            }
            return View(connectionParameters);
        }

        // POST: Admin/ConnectionParameters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConnectionParameters connectionParameters = db.ConnectionParameters.Find(id);
            db.ConnectionParameters.Remove(connectionParameters);
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
