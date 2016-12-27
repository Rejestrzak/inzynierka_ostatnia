using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neo4j.Driver.V1;
using Microsoft.SqlServer.Management.Smo;
using Inzynierka_ostatnia.Models;


namespace Inzynierka_ostatnia.Controllers
{
    [Authorize]
    public class GraphController : Controller
    {
        private KaczkaDBCtxt db = new KaczkaDBCtxt();

        Server myServer = new Server("tcp:inzynier.database.windows.net,1433");

        //Lista klucze do przechowywania kolumn klucza głównego
        List<string> klucze = new List<string>();


        public void conn_smo()
        {
            //Łączenie z serwerem dzięki SMO
            myServer.ConnectionContext.LoginSecure = false;
            myServer.ConnectionContext.Login = "inzynier@inzynier";
            myServer.ConnectionContext.Password = "admin1@3";
            myServer.ConnectionContext.Connect();
            
        }

        

        // GET: Graph
        public ActionResult Index()
        {
            
            conn_smo();

            Database smo_base = myServer.Databases["inzynier"];
            Table smo_tab = smo_base.Tables["Kaczkas"];
            //Znajdowanie kluczy tabeli
            foreach (Column item in smo_tab.Columns)
            {
                if (item.InPrimaryKey == true)
                {
                    klucze.Add(item.Name);
                }
            }


            
            //Uzupełnianie bazy grafowej o brakujące obiekty


            var driver = GraphDatabase.Driver("bolt://localhost");
            var session = driver.Session();
            //Wypełniamy listę obiektami klasy Kaczka, które są już w bazie grafowej
            var dList = session.Run("Match (k:Kaczka) RETURN  k.id AS id, k.id_matki AS id_matki, k.id_ojca AS id_ojca, k.wyklucie AS wyklucie, k.zyje AS zyje").ToList();

            


            return View();
        }
    }
}