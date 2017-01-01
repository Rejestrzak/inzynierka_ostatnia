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
        private InzynierEntities5 ie = new InzynierEntities5();

        //Server myServer = new Server("tcp:inzynier.database.windows.net,1433");

        ////Lista klucze do przechowywania kolumn klucza głównego
        //List<string> klucze = new List<string>();


        //public void conn_smo()
        //{
        //    //Łączenie z serwerem dzięki SMO
        //    myServer.ConnectionContext.LoginSecure = false;
        //    myServer.ConnectionContext.Login = "inzynier@inzynier";
        //    myServer.ConnectionContext.Password = "admin1@3";
        //    myServer.ConnectionContext.Connect();
            
        //}

        

        // GET: Graph
        public ActionResult Index()
        {
            
            //conn_smo();

            //Database smo_base = myServer.Databases["inzynier"];
            //Table smo_tab = smo_base.Tables["Kaczkas"];
            ////Znajdowanie kluczy tabeli
            //foreach (Column item in smo_tab.Columns)
            //{
            //    if (item.InPrimaryKey == true)
            //    {
            //        klucze.Add(item.Name);
            //    }
            //}
            
            //Uzupełnianie bazy grafowej o brakujące obiekty


            var driver = GraphDatabase.Driver("bolt://localhost");
            var session = driver.Session();
            //Wypełniamy listę obiektami klasy Gęś, które są już w bazie grafowej
            var dList = session.Run("Match (k:Ges) RETURN  k.UID AS UID, k.MATKA AS MATKA, k.OJCIEC AS OJCIEC").ToList();

            


            var w33 = from x in ie.W33MNAll
                      select x;
            var w11 = from z in ie.W11MNALL
                      select z;


            

            //Zbieramy wszystkie UID do osobnych list i konwertujemy na INT, ponieważ w bazie pole UID jest typu varchar
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
            var nowe_w33_UID = w33_UID.Except(graf_UID);
            var nowe_w11_UID = w11_UID.Except(graf_UID);


            foreach (var item in w33)
            {
                if (nowe_w33_UID.Contains(Convert.ToInt32(item.UID))==true)
                {
                    //Tworzenie obiektu w bazie grafowej
                    session.Run("CREATE (k:Ges{UID:{UID}, MATKA:{MATKA}, OJCIEC:{OJCIEC}})", new Dictionary<string, object> { { "UID", item.UID }, { "MATKA", item.MATKA }, { "OJCIEC", item.OJCIEC } });
                }
            }

            foreach (var item in w11)
            {
                if (nowe_w11_UID.Contains(Convert.ToInt32(item.UID)) == true)
                {
                    //Tworzenie obiektu w bazie grafowej
                    session.Run("CREATE (k:Ges{UID:{UID}, MATKA:{MATKA}, OJCIEC:{OJCIEC}})", new Dictionary<string, object> { { "UID", item.UID }, { "MATKA", item.MATKA }, { "OJCIEC", item.OJCIEC } });
                }
            }

            //Tworzenie relacji dla nowych obiektów w bazie
            foreach (var item in w33)
            {
                if (nowe_w33_UID.Contains(Convert.ToInt32(item.UID))==true)
                {
                    foreach (var item_1 in w33)
                    {
                        if (item.MATKA==Convert.ToInt32(item_1.UID))
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE UNIQUE (a)-[r:MATKA]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_2}}),(b:Ges{UID:{UID_1}}) CREATE UNIQUE (a)-[r:DZIECKO]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                        }

                        if (item.OJCIEC == Convert.ToInt32(item_1.UID))
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE UNIQUE (a)-[r:OJCIEC]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_2}}),(b:Ges{UID:{UID_1}}) CREATE UNIQUE (a)-[r:DZIECKO]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                        }
                    }

                    foreach (var item_1 in w11)
                    {
                        if (item.MATKA == Convert.ToInt32(item_1.UID))
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE UNIQUE (a)-[r:MATKA]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_2}}),(b:Ges{UID:{UID_1}}) CREATE UNIQUE (a)-[r:DZIECKO]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                        }

                        if (item.OJCIEC == Convert.ToInt32(item_1.UID))
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE UNIQUE (a)-[r:OJCIEC]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_2}}),(b:Ges{UID:{UID_1}}) CREATE UNIQUE (a)-[r:DZIECKO]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                        }
                    }
                }
            }



            foreach (var item in w11)
            {
                if (nowe_w11_UID.Contains(Convert.ToInt32(item.UID)) == true)
                {
                    foreach (var item_1 in w33)
                    {
                        if (item.MATKA == item_1.UID)
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE UNIQUE (a)-[r:MATKA]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_2}}),(b:Ges{UID:{UID_1}}) CREATE UNIQUE (a)-[r:DZIECKO]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                        }

                        if (item.OJCIEC == item_1.UID)
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE UNIQUE (a)-[r:OJCIEC]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_2}}),(b:Ges{UID:{UID_1}}) CREATE UNIQUE (a)-[r:DZIECKO]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                        }
                    }

                    foreach (var item_1 in w11)
                    {
                        if (item.MATKA == item_1.UID)
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE UNIQUE (a)-[r:MATKA]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_2}}),(b:Ges{UID:{UID_1}}) CREATE UNIQUE (a)-[r:DZIECKO]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                        }

                        if (item.OJCIEC == item_1.UID)
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE UNIQUE (a)-[r:OJCIEC]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_2}}),(b:Ges{UID:{UID_1}}) CREATE UNIQUE (a)-[r:DZIECKO]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                        }
                    }
                }
            }



            dList.Clear();
            dList = session.Run("Match (k:Ges) RETURN  k.UID AS UID, k.MATKA AS MATKA, k.OJCIEC AS OJCIEC").ToList();



            

            return View();
            
        }

        [HttpPost]
        public ActionResult Index(string pochodzenie_z, string rod_z, string leg_z, int masa8t_z, int masa11t_z, string pochodzenie_m, string rod_m, string leg_m, int masa8t_m, int masa11t_m)
        {
            var w33 = from i in ie.W33MNAll
                      select i;
            var w11 = from i in ie.W11MNALL
                      select i;

            w33 = from i in ie.W33MNAll
                  where i.POCHODZENIE.Equals(pochodzenie_z) && i.ROD.Equals(rod_z)
                  select i;


            return View();
        }
    }
}