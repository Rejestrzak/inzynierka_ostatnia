using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Inzynierka_ostatnia.Models;
using Neo4j.Driver.V1;
using Microsoft.SqlServer.Management.Smo;

namespace Inzynierka_ostatnia.Controllers
{

    [Authorize]
    public class KaczkasController : Controller
    {
        private KaczkaDBCtxt db = new KaczkaDBCtxt();
        
        
        

        

        // GET: Kaczkas
        public ActionResult Index()
        {


            
            return View(db.Kaczki.ToList());
        }


        [HttpPost]
        public ActionResult Index(string Znajdz_ID)
        {

            
            var kaczki = from i in db.Kaczki
                         select i;
            if (!String.IsNullOrEmpty(Znajdz_ID))
            {
                int x = Convert.ToInt32(Znajdz_ID);
                kaczki = from i in db.Kaczki
                         where i.id.Equals(x)
                         select i;
            }


            return View(kaczki.ToList());
        }

        // GET: Kaczkas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kaczka kaczka = db.Kaczki.Find(id);
            if (kaczka == null)
            {
                return HttpNotFound();
            }
            return View(kaczka);
        }

        // GET: Kaczkas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Kaczkas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_matki,id_ojca,wyklucie,zyje,zmiana")] Kaczka kaczka)
        {
            if (ModelState.IsValid)
            {
                db.Kaczki.Add(kaczka);
                db.SaveChanges();


                //Połączenie z bazą grafową
                var driver = GraphDatabase.Driver("bolt://localhost");
                var session = driver.Session();

                //Tworzenie obiektu w bazie grafowej
                session.Run("CREATE (k:Kaczka{id:{id}, id_matki:{id_matki}, id_ojca:{id_ojca}, zyje:{zyje}, wyklucie:{wyklucie}})", new Dictionary<string, object> { { "id", kaczka.id}, {"id_matki",kaczka.id_matki}, {"id_ojca",kaczka.id_ojca}, {"zyje",kaczka.zyje}, {"wyklucie",Convert.ToString(kaczka.wyklucie)} });

                session.Dispose();
                driver.Dispose();


                

                return RedirectToAction("Index");
            }
            return View(kaczka);
        }

        // GET: Kaczkas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kaczka kaczka = db.Kaczki.Find(id);
            if (kaczka == null)
            {
                return HttpNotFound();
            }
            return View(kaczka);
        }

        // POST: Kaczkas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,id_matki,id_ojca,wyklucie,zyje,zmiana")] Kaczka kaczka)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kaczka).State = EntityState.Modified;
                db.SaveChanges();

                //Połączenie z bazą grafową
                var driver = GraphDatabase.Driver("bolt://localhost");
                var session = driver.Session();



                session.Dispose();
                driver.Dispose();

                return RedirectToAction("Index");
            }
            return View(kaczka);
        }

        // GET: Kaczkas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kaczka kaczka = db.Kaczki.Find(id);
            if (kaczka == null)
            {
                return HttpNotFound();
            }
            return View(kaczka);
        }

        // POST: Kaczkas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kaczka kaczka = db.Kaczki.Find(id);
            db.Kaczki.Remove(kaczka);
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
