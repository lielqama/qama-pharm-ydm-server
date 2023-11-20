using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PharmYdm.Context;
using PharmYdm.Models;

namespace PharmYdm.Controllers
{
    public class BarQamaClientsController : Controller
    {
        private PharmYdmContext db = new PharmYdmContext();

        // GET: BarQamaClients
        public ActionResult Index()
        {
            return View(db.BarQamaClients.ToList());
        }

        // GET: BarQamaClients/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BarQamaClient barQamaClient = db.BarQamaClients.Find(id);
            if (barQamaClient == null)
            {
                return HttpNotFound();
            }
            return View(barQamaClient);
        }

        // GET: BarQamaClients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BarQamaClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ShipCode,Username,Password,CustCode")] BarQamaClient barQamaClient)
        {
            if (ModelState.IsValid)
            {
                barQamaClient.ID = Guid.NewGuid();
                db.BarQamaClients.Add(barQamaClient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(barQamaClient);
        }

        // GET: BarQamaClients/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BarQamaClient barQamaClient = db.BarQamaClients.Find(id);
            if (barQamaClient == null)
            {
                return HttpNotFound();
            }
            return View(barQamaClient);
        }

        // POST: BarQamaClients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ShipCode,Username,Password,CustCode")] BarQamaClient barQamaClient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(barQamaClient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(barQamaClient);
        }

        // GET: BarQamaClients/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BarQamaClient barQamaClient = db.BarQamaClients.Find(id);
            if (barQamaClient == null)
            {
                return HttpNotFound();
            }
            return View(barQamaClient);
        }

        // POST: BarQamaClients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            BarQamaClient barQamaClient = db.BarQamaClients.Find(id);
            db.BarQamaClients.Remove(barQamaClient);
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
