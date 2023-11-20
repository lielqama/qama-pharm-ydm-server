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
    public class HFDQamaClientsController : Controller
    {
        private PharmYdmContext db = new PharmYdmContext();

        // GET: HFDQamaClients
        public ActionResult Index()
        {
            return View(db.HFDQamaClients.ToList());
        }

        // GET: HFDQamaClients/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PharmYdmQamaClient hfdQamaClients = db.HFDQamaClients.Find(id);
            if (hfdQamaClients == null)
            {
                return HttpNotFound();
            }
            return View(hfdQamaClients);
        }

        // GET: HFDQamaClients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HFDQamaClients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CustName,ApiKey")] PharmYdmQamaClient hfdQamaClients)
        {
            if (ModelState.IsValid)
            {
                hfdQamaClients.ID = Guid.NewGuid();
                db.HFDQamaClients.Add(hfdQamaClients);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hfdQamaClients);
        }

        // GET: HFDQamaClients/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PharmYdmQamaClient hfdQamaClients = db.HFDQamaClients.Find(id);
            if (hfdQamaClients == null)
            {
                return HttpNotFound();
            }
            return View(hfdQamaClients);
        }

        // POST: HFDQamaClients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CustName,ApiKey")] PharmYdmQamaClient hfdQamaClients)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hfdQamaClients).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hfdQamaClients);
        }

        // GET: HFDQamaClients/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PharmYdmQamaClient hfdQamaClients = db.HFDQamaClients.Find(id);
            if (hfdQamaClients == null)
            {
                return HttpNotFound();
            }
            return View(hfdQamaClients);
        }

        // POST: HFDQamaClients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            PharmYdmQamaClient hfdQamaClients = db.HFDQamaClients.Find(id);
            db.HFDQamaClients.Remove(hfdQamaClients);
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
