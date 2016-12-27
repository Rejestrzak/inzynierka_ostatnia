using Inzynierka_ostatnia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inzynierka_ostatnia.Controllers
{
    public class KaczkiController : Controller
    {

        private KaczkaDBCtxt baza = new KaczkaDBCtxt();
        // GET: Kaczki
        public ActionResult Index()
        {
            return View(baza.Kaczki.ToList());
        }

        // GET: Kaczki/Create

        public ActionResult Create()
        {
            return View();
        }

        // POST: Kaczki/Create

        [HttpPost]

        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "Id,Id_matki,Id_ojca,wyklucie,zyje")] Kaczka kaczka)
        {
            if (ModelState.IsValid)
            {
                baza.Kaczki.Add(kaczka);
                baza.SaveChanges();
                RedirectToAction("Index");
            }
            return View();
        }
    }
}