using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HydroServerTools.Models;

namespace HydroServerTools.Controllers
{
    public class TrackUpdatesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TrackUpdates
        public ActionResult Index()
        {
            return View(db.TrackUpdates.ToList());
        }

        // GET: TrackUpdates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrackUpdates trackUpdates = db.TrackUpdates.Find(id);
            if (trackUpdates == null)
            {
                return HttpNotFound();
            }
            return View(trackUpdates);
        }

        // GET: TrackUpdates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrackUpdates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ConnectionId,UserId,IsUpdated,UpdateDateTime,IsSynchronized,SynchronizedDateTime")] TrackUpdates trackUpdates)
        {
            if (ModelState.IsValid)
            {
                db.TrackUpdates.Add(trackUpdates);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trackUpdates);
        }

        // GET: TrackUpdates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrackUpdates trackUpdates = db.TrackUpdates.Find(id);
            if (trackUpdates == null)
            {
                return HttpNotFound();
            }
            return View(trackUpdates);
        }

        // POST: TrackUpdates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConnectionId,UserId,IsUpdated,UpdateDateTime,IsSynchronized,SynchronizedDateTime")] TrackUpdates trackUpdates)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trackUpdates).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trackUpdates);
        }

        // GET: TrackUpdates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrackUpdates trackUpdates = db.TrackUpdates.Find(id);
            if (trackUpdates == null)
            {
                return HttpNotFound();
            }
            return View(trackUpdates);
        }

        // POST: TrackUpdates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrackUpdates trackUpdates = db.TrackUpdates.Find(id);
            db.TrackUpdates.Remove(trackUpdates);
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
