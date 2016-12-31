using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inzynierka_ostatnia.Models;


namespace Inzynierka_ostatnia.Controllers
{


    public class HomeController : Controller
    {

        private KaczkaDBCtxt db = new KaczkaDBCtxt();


        public ActionResult Index()
        {


            return View();
        }

        [HttpPost]
        public ActionResult Index(string Login, string Password)
        {

            
            if ((!String.IsNullOrEmpty(Login)) && (!String.IsNullOrEmpty(Password)))
            {
                
               var users = from i in db.Users
                         where i.username.Equals(Login) && i.password.Equals(Password)
                         select i;

                if (users.Count() ==1)
                {
                    return Redirect("/Kaczkas");
                }
            }

            return RedirectToAction("Index");

        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}