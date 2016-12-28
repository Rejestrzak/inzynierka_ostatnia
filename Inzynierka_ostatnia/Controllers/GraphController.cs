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
    //[Authorize]
    public class GraphController : Controller
    {
        private KaczkaDBCtxt db = new KaczkaDBCtxt();
        private InzynierEntities3 ie = new InzynierEntities3();

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
            //Wypełniamy listę obiektami klasy Gęś, które są już w bazie grafowej
            var dList = session.Run("Match (k:Ges) RETURN  k.UID AS UID, k.MATKA AS MATKA, k.OJCIEC AS OJCIEC").ToList();

            var w33 = from x in ie.W33MNAll
                      select x;
            var w11 = from z in ie.W11MNALL
                      select z;


            //Zbieramy wszystkie UID do osobnych list i konwertujemy na INT, ponieważ w bazue pole UID jest typu varchar
            List<int> w33_UID = new List<int>();
            foreach (var item in w33)
            {
                w33_UID.Add(Convert.ToInt32(item.UID));
            }

            List<int> w11_UID = new List<int>();
            foreach (var item in w11)
            {
                w11_UID.Add(Convert.ToInt32(item.UID));
            }

            List<int> graf_UID = new List<int>();
            foreach (var item in dList)
            {
                graf_UID.Add(Convert.ToInt32(item[0]));
            }

            //Wybieramy tylko nowe rekordy, aby uniknąć redundancji
            var nowe_w33 = w33_UID.Except(graf_UID);
            var nowe_w11 = w11_UID.Except(graf_UID);


            foreach (var item in w33)
            {
                if (nowe_w33.Equals(Convert.ToInt32(item.UID))==true)
                {
                    //Tworzenie obiektu w bazie grafowej
                    session.Run("CREATE (k:Ges{UID:{UID}, MATKA:{MATKA}, OJCIEC:{OJCIEC}})", new Dictionary<string, object> { { "UID", item.UID }, { "MATKA", item.MATKA }, { "OJCIEC", item.OJCIEC } });
                }
            }

            foreach (var item in w11)
            {
                if (nowe_w11.Equals(Convert.ToInt32(item.UID)) == true)
                {
                    //Tworzenie obiektu w bazie grafowej
                    session.Run("CREATE (k:Ges{UID:{UID}, MATKA:{MATKA}, OJCIEC:{OJCIEC}})", new Dictionary<string, object> { { "UID", item.UID }, { "MATKA", item.MATKA }, { "OJCIEC", item.OJCIEC } });
                }
            }

            dList.Clear();
            dList = session.Run("Match (k:Ges) RETURN  k.UID AS UID, k.MATKA AS MATKA, k.OJCIEC AS OJCIEC").ToList();


            return View();
        }
    }
}