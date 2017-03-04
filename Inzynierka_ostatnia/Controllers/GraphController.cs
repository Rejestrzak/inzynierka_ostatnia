using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neo4j.Driver.V1;
using Microsoft.SqlServer.Management.Smo;
using Inzynierka_ostatnia.Models;
using System.IO;

namespace Inzynierka_ostatnia.Controllers
{
    //[Authorize]
    public class GraphController : Controller
    {
        
        private InzynierEntities5 ie = new InzynierEntities5();

        

        

        // GET: Graph
        public ActionResult Index()
        {



            
            //Uzupełnianie bazy grafowej o brakujące obiekty

            
            
           

            var driver = GraphDatabase.Driver("bolt://localhost");
            var session = driver.Session();
            //Wypełniamy listę obiektami klasy Gęś, które są już w bazie grafowej
            var dList = session.Run("Match (k:Ges) RETURN  k.UID AS UID, k.MATKA AS MATKA, k.OJCIEC AS OJCIEC, k.ROD AS ROD, k.MASA8T AS MASA8T, k.MASA11T AS MASA11T, k.PMASY AS PMASY, k.PPMASY AS PPMASY, k.SUMAZNJAJ AS SUMAZNJAJ, k.MASAJAJ AS MASAJAJ, k.DLMOS AS DLMOS, k.DLPR AS DLPR, k.GRMP AS GRMP, k.OMM AS OMM").ToList();

            
            //Tworzymy i wypełniamy listy rekordami z bazy relacyjnej
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
                    session.Run("CREATE (k:Ges{UID:{UID}, MATKA:{MATKA}, OJCIEC:{OJCIEC}, ROD:{ROD}, PLEC:{PLEC}, MASA8T:{MASA8T}, MASA11T:{MASA11T}, PMASY:{PMASY}, PPMASY:{PPMASY}, SUMAZNJAJ:{SUMAZNJAJ}, MASAJAJ:{MASAJAJ}, DLMOS:{DLMOS}, DLPR:{DLPR}, GRMP:{GRMP}, OMM:{OMM}})", new Dictionary<string, object> { { "UID", item.UID }, { "MATKA", item.MATKA }, { "OJCIEC", item.OJCIEC }, { "ROD", item.ROD },{"PLEC", item.PLEC}, { "MASA8T", item.MASA8T }, { "MASA11T", item.MASA11T }, { "PMASY", item.PMASY }, { "PPMASY", item.PPMASY }, { "SUMAZNJAJ", item.SUMAZNJAJ }, { "MASAJAJ", item.MASAJAJ }, { "DLMOS", item.DLMOS }, { "DLPR", item.DLPR }, { "GRMP", item.GRMP }, { "OMM", item.OMM } });
                }
            }

            foreach (var item in w11)
            {
                if (nowe_w11_UID.Contains(Convert.ToInt32(item.UID)) == true)
                {
                    //Tworzenie obiektu w bazie grafowej
                    session.Run("CREATE (k:Ges{UID:{UID}, MATKA:{MATKA}, OJCIEC:{OJCIEC}, ROD:{ROD}, PLEC:{PLEC}, MASA8T:{MASA8T}, MASA11T:{MASA11T}, PMASY:{PMASY}, PPMASY:{PPMASY}, SUMAZNJAJ:{SUMAZNJAJ}, MASAJAJ:{MASAJAJ}, DLMOS:{DLMOS}, DLPR:{DLPR}, GRMP:{GRMP}, OMM:{OMM}})", new Dictionary<string, object> { { "UID", item.UID }, { "MATKA", item.MATKA }, { "OJCIEC", item.OJCIEC }, {"ROD", item.ROD},{"PLEC", item.PLEC}, { "MASA8T", item.MASA8T}, { "MASA11T", item.MASA11T}, { "PMASY", item.PMASY}, { "PPMASY", item.PPMASY}, { "SUMAZNJAJ", item.SUMAZNJAJ}, { "MASAJAJ", item.MASAJAJ}, { "DLMOS", item.DLMOS}, { "DLPR", item.DLPR}, { "GRMP", item.GRMP}, { "OMM", item.OMM}  });
                }
            }

            

            //Tworzenie relacji dla nowych obiektów w bazie - dodany zapis w Web.config aby mogły działać dwa aktywne DataReader-y
            foreach (var item in w33)
            {
                if (nowe_w33_UID.Contains(Convert.ToInt32(item.UID)))
                {
                    foreach (var item_1 in w33)
                    {
                        if (item.MATKA == Convert.ToInt32(item_1.UID))
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE (a)-[r:MATKA]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE (a)-[r:GASIE]->(b)", new Dictionary<string, object> { { "UID_2", item_1.UID }, { "UID_1", item.UID } });
                            
                        }

                        if (item.OJCIEC == Convert.ToInt32(item_1.UID))
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE (a)-[r:OJCIEC]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE (a)-[r:GASIE]->(b)", new Dictionary<string, object> { { "UID_2", item_1.UID }, { "UID_1", item.UID } });
                        }
                    }                    
                }
            }



            foreach (var item in w11)
            {
                if (nowe_w11_UID.Contains(Convert.ToInt32(item.UID)) == true)
                {
                    
                    foreach (var item_1 in w11)
                    {
                        if (item.MATKA == item_1.UID)
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE (a)-[r:MATKA]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE (a)-[r:GASIE]->(b)", new Dictionary<string, object> { { "UID_2", item_1.UID }, { "UID_1", item.UID } });
                        }

                        if (item.OJCIEC == item_1.UID)
                        {
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE (a)-[r:OJCIEC]->(b)", new Dictionary<string, object> { { "UID_1", item_1.UID }, { "UID_2", item.UID } });
                            session.Run("MATCH (a:Ges{UID:{UID_1}}),(b:Ges{UID:{UID_2}}) CREATE (a)-[r:GASIE]->(b)", new Dictionary<string, object> { { "UID_2", item_1.UID }, { "UID_1", item.UID } });
                        }
                    }
                }
            }

            


            dList.Clear();
            

            

            

            return View();
            
        }

        [HttpPost]
        public ActionResult Index(string rod, string masa8t_z, string masa11t_z, string pmasy_z, string ppmasy_z, string dlmos_z, string dlpr_z, string sumaznjaj_z, string masajaj_z, string grmp_z, string omm_z, string masa8t_m, string masa11t_m, string pmasy_m, string ppmasy_m, string dlmos_m, string dlpr_m, string grmp_m, string omm_m)
        {

            
            masa8t_m = masa8t_m.Replace(",",".");
            masa8t_z = masa8t_z.Replace(",", ".");
            masa11t_m = masa11t_m.Replace(",", ".");
            masa11t_z = masa11t_z.Replace(",", ".");
            pmasy_m = pmasy_m.Replace(",", ".");
            pmasy_z = pmasy_z.Replace(",", ".");
            ppmasy_m = ppmasy_m.Replace(",", ".");
            ppmasy_z = ppmasy_z.Replace(",", ".");
            dlmos_m = dlmos_m.Replace(",", ".");
            dlmos_z = dlmos_z.Replace(",", ".");
            dlpr_m = dlpr_m.Replace(",", ".");
            dlpr_z = dlpr_z.Replace(",", ".");
            sumaznjaj_z = sumaznjaj_z.Replace(",", ".");
            masajaj_z = masajaj_z.Replace(",", ".");
            grmp_m = grmp_m.Replace(",", ".");
            grmp_z = grmp_z.Replace(",", ".");
            omm_m = omm_m.Replace(",", ".");
            omm_z = omm_z.Replace(",", ".");




            var driver = GraphDatabase.Driver("bolt://localhost");
            var session = driver.Session();

            string nodes_json = "";
            string edges_json = "";

            if (rod == "W11")
            {
                var w11_1_list = session.Run("MATCH (k:Ges) WHERE k.PLEC=1 AND k.ROD=~'.*W11.*' AND k.MASA8T >= " + masa8t_m + " AND k.MASA11T >= " + masa11t_m + " AND k.PMASY >= " + pmasy_m + " AND k.PPMASY >= " + ppmasy_m + " RETURN k.UID, k.ROD, k.MASA8T, k.MASA11T, k.PMASY, k.PPMASY LIMIT 30").ToList();
                var w11_2_list = session.Run("MATCH (k:Ges) WHERE k.PLEC=1 AND k.ROD=~'.*W11.*' AND k.MASA8T >= " + masa8t_m + " AND k.MASA11T >= " + masa11t_m + " AND k.PMASY >= " + pmasy_m + " AND k.PPMASY >= " + ppmasy_m + " RETURN k.UID, k.ROD, k.MASA8T, k.MASA11T, k.PMASY, k.PPMASY LIMIT 30").ToList();


                foreach (var item_w11_1 in w11_1_list)
                {
                    int licznik = 0;
                    nodes_json += @"{""id"":" + item_w11_1[0] + @", ""ROD"":""" + item_w11_1[1] + @""", ""MASA8T"":" + item_w11_1[2] + @", ""MASA11T"":" + item_w11_1[3] + @", ""PMASY"":" + item_w11_1[4] + @", ""PPMASY"":" + item_w11_1[5] + @", ""role"":""gasior"", ""caption"":""Gąsior:" + item_w11_1[0] + ", ROD:W11, MASA8T:" + item_w11_1[2] + ",MASA11T:" + item_w11_1[3] + ",PMASY:" + item_w11_1[4] + ", PPMASY:" + item_w11_1[5] + @"""},";

                    foreach (var item_w11_2 in w11_2_list)
                    {
                        var list_rel_deg = session.Run("MATCH (a:Ges {UID:{UID_1}})-[r]-(b:Ges{UID:{UID_2}}) RETURN TYPE(r), Count(*)", new Dictionary<string, object> { { "UID_1", item_w11_1[0] }, { "UID_2", item_w11_2[0] } }).ToList();

                        int x = list_rel_deg.Count;
                        switch (x)
                        {
                            case 0:
                                if (nodes_json.Contains(Convert.ToString(item_w11_2[0])) == false)
                                {
                                    nodes_json += @"{""id"":" + item_w11_2[0] + @", ""ROD"":""" + item_w11_2[1] + @""", ""MASA8T"":" + item_w11_2[2] + @", ""MASA11T"":" + item_w11_2[3] + @", ""PMASY"":" + item_w11_2[4] + @", ""PPMASY"":" + item_w11_2[5] + @", ""role"":""gaska"", ""caption"":""Gąska:" + item_w11_2[0] + ", ROD:W11, MASA8T:" + item_w11_2[2] + ",MASA11T:" + item_w11_2[3] + ",PMASY:" + item_w11_2[4] + ", PPMASY:" + item_w11_2[5] + @"""},";
                                }


                                edges_json += @"{""source"": " + item_w11_1[0] + @", ""target"": " + item_w11_2[0] + @", ""caption"": ""DOPASOWANE""},";
                                licznik++;
                                break;


                            case 2:
                                if ((Convert.ToString(list_rel_deg[0][0]) == "GASIE" && Convert.ToInt32(list_rel_deg[0][1]) >= 3) && (Convert.ToInt32(list_rel_deg[1][1]) >= 3))
                                {
                                    if (nodes_json.Contains(Convert.ToString(item_w11_2[0])) == false)
                                    {
                                        nodes_json += @"{""id"":" + item_w11_2[0] + @", ""ROD"":""" + item_w11_2[1] + @""", ""MASA8T"":" + item_w11_2[2] + @", ""MASA11T"":" + item_w11_2[3] + @", ""PMASY"":" + item_w11_2[4] + @", ""PPMASY"":" + item_w11_2[5] + @", ""role"":""gaska"", ""caption"":""Gąska:" + item_w11_2[0] + ", ROD:W11, MASA8T:" + item_w11_2[2] + ",MASA11T:" + item_w11_2[3] + ",PMASY:" + item_w11_2[4] + ", PPMASY:" + item_w11_2[5] + @"""},";
                                    }


                                    edges_json += @"{""source"": " + item_w11_1[0] + @", ""target"": " + item_w11_2[0] + @", ""caption"": ""DOPASOWANE""},";
                                    licznik++;
                                    break;
                                }
                                else if (Convert.ToString(list_rel_deg[0][0]) == "OJCIEC" && Convert.ToString(list_rel_deg[1][0]) == "GASIE")
                                {
                                    if (nodes_json.Contains(Convert.ToString(item_w11_2[0])) == false)
                                    {
                                        nodes_json += @"{""id"":" + item_w11_2[0] + @", ""ROD"":""" + item_w11_2[1] + @""", ""MASA8T"":" + item_w11_2[2] + @", ""MASA11T"":" + item_w11_2[3] + @", ""PMASY"":" + item_w11_2[4] + @", ""PPMASY"":" + item_w11_2[5] + @", ""role"":""gaska"", ""caption"":""Gąska:" + item_w11_2[0] + ", ROD:W11, MASA8T:" + item_w11_2[2] + ",MASA11T:" + item_w11_2[3] + ",PMASY:" + item_w11_2[4] + ", PPMASY:" + item_w11_2[5] + @"""},";
                                    }


                                    edges_json += @"{""source"": " + item_w11_1[0] + @", ""target"": " + item_w11_2[0] + @", ""caption"": ""DOPASOWANE""},";
                                    licznik++;
                                    break;
                                }
                                break;


                            case 3:
                                if ((Convert.ToString(list_rel_deg[0][0]) == "GASIE" && Convert.ToInt32(list_rel_deg[0][1]) >= 3) && ((Convert.ToInt32(list_rel_deg[1][1]) + (Convert.ToInt32(list_rel_deg[2][1]))) >= 3))
                                {
                                    if (nodes_json.Contains(Convert.ToString(item_w11_2[0])) == false)
                                    {
                                        nodes_json += @"{""id"":" + item_w11_2[0] + @", ""ROD"":""" + item_w11_2[1] + @""", ""MASA8T"":" + item_w11_2[2] + @", ""MASA11T"":" + item_w11_2[3] + @", ""PMASY"":" + item_w11_2[4] + @", ""PPMASY"":" + item_w11_2[5] + @", ""role"":""gaska"", ""caption"":""Gąska:" + item_w11_2[0] + ", ROD:W11, MASA8T:" + item_w11_2[2] + ",MASA11T:" + item_w11_2[3] + ",PMASY:" + item_w11_2[4] + ", PPMASY:" + item_w11_2[5] + @"""},";
                                    }


                                    edges_json += @"{""source"": " + item_w11_1[0] + @", ""target"": " + item_w11_2[0] + @", ""caption"": ""DOPASOWANE""},";
                                    licznik++;
                                    break;
                                }
                                break;
                        }
                        if (licznik == 6)
                        {
                            break;
                        }
                    }
                }

                if (w11_1_list.Count != 0 && w11_2_list.Count != 0)
                {
                    System.IO.File.WriteAllText(@"C:\Users\Jakub Krysiński\Desktop\Inzyernika_ostatnia\Inzynierka_ostatnia\Inzynierka_ostatnia\data\dane2.json", string.Empty);
                    System.IO.StreamWriter file_json = new System.IO.StreamWriter(@"C:\Users\Jakub Krysiński\Desktop\Inzyernika_ostatnia\Inzynierka_ostatnia\Inzynierka_ostatnia\data\dane2.json");
                    file_json.WriteLine(@"{""nodes"": [");
                    file_json.WriteLine(nodes_json.Substring(0, nodes_json.Length - 1));
                    file_json.WriteLine(@"], ""edges"": [");
                    file_json.WriteLine(edges_json.Substring(0, edges_json.Length - 1));
                    file_json.WriteLine("]}");
                    file_json.Close();

                }
            }
            else if (rod == "W33")
            {
                var w33_1_list = session.Run("MATCH (k:Ges) WHERE k.PLEC=1 AND k.ROD=~'.*W33.*' AND k.MASA8T >= " + masa8t_m + " AND k.MASA11T >= " + masa11t_m + " AND k.DLMOS >= " + dlmos_m + " AND k.DLPR >= " + dlpr_m + " AND k.GRMP >= " + grmp_m + " AND (k.OMM >= " + omm_m + " or k.OMM is null) RETURN k.UID, k.ROD, k.MASA8T, k.MASA11T, k.DLMOS, k.DLPR, k.GRMP, k.OMM LIMIT 30").ToList();
                var w33_2_list = session.Run("MATCH (k:Ges) WHERE k.PLEC=2 AND k.ROD=~'.*W33.*' AND k.MASA8T >= " + masa8t_z + " AND k.MASA11T >= " + masa11t_z + " AND (k.DLMOS >= " + dlmos_z + " OR k.DLMOS IS NULL) AND (k.DLPR >= " + dlpr_z + " OR k.DLPR IS NULL) AND k.GRMP >= " + grmp_z + " AND ( k.OMM >= " + omm_z + " OR k.OMM IS NULL) RETURN k.UID, k.ROD, k.MASA8T, k.MASA11T, k.DLMOS, k.DLPR, k.GRMP, k.OMM LIMIT 30").ToList();


                foreach (var item_w33_1 in w33_1_list)
                {
                    int licznik = 0;
                    nodes_json += @"{""id"":" + item_w33_1[0] + @", ""ROD"":""" + item_w33_1[1] + @""", ""MASA8T"":" + item_w33_1[2] + @", ""MASA11T"":" + item_w33_1[3] + @", ""DLMOS"":" + item_w33_1[4] + @", ""DLPR"":" + item_w33_1[5] + @", ""GRMP"":" + item_w33_1[6] + /*@", ""OMM"": " + item_w33_1[7] + */@", ""role"":""gasior"", ""caption"":""Gąsior:" + item_w33_1[0] + ", ROD:W33, MASA8T:" + item_w33_1[2] + ",MASA11T:" + item_w33_1[3] + ",DLMOS:" + item_w33_1[4] + ", DLPR:" + item_w33_1[5] + ", GRMP:" + item_w33_1[6] + ", OMM:" + item_w33_1[7] + @"""},";
                    foreach (var item_w33_2 in w33_2_list)
                    {
                        var list_rel_deg = session.Run("MATCH (a:Ges {UID:{UID_1}})-[r]-(b:Ges{UID:{UID_2}}) RETURN TYPE(r), Count(*)", new Dictionary<string, object> { { "UID_1", item_w33_1[0] }, { "UID_2", item_w33_2[0] } }).ToList();


                        int x = list_rel_deg.Count;
                        switch (x)
                        {
                            case 0:
                                if (nodes_json.Contains(Convert.ToString(item_w33_2[0])) == false)
                                {
                                    nodes_json += @"{""id"":" + item_w33_2[0] + @", ""ROD"":""" + item_w33_2[1] + @""", ""MASA8T"":" + item_w33_2[2] + @", ""MASA11T"":" + item_w33_2[3] +/* @", ""DLMOS"":" + item_w33_2[4] + @", ""DLPR"":" + item_w33_2[5] + */@", ""GRMP"":" + item_w33_2[6] +/* @", ""OMM"":" + item_w33_2[7] + */@", ""role"":""gaska"", ""caption"":""Gąska:" + item_w33_2[0] + ", ROD:W33, MASA8T:" + item_w33_2[2] + ",MASA11T:" + item_w33_2[3] + ",DLMOS:" + item_w33_2[4] + ", DLPR:" + item_w33_2[5] + ", GRMP:" + item_w33_2[6] + ", OMM:" + item_w33_2[7] + @"""},";
                                }


                                edges_json += @"{""source"": " + item_w33_1[0] + @", ""target"": " + item_w33_2[0] + @", ""caption"": ""DOPASOWANE""},";
                                licznik++;
                                break;


                            case 2:
                                if ((Convert.ToString(list_rel_deg[0][0]) == "GASIE" && Convert.ToInt32(list_rel_deg[0][1]) >= 3) && (Convert.ToInt32(list_rel_deg[1][1]) >= 3))
                                {
                                    if (nodes_json.Contains(Convert.ToString(item_w33_2[0])) == false)
                                    {
                                        nodes_json += @"{""id"":" + item_w33_2[0] + @", ""ROD"":""" + item_w33_2[1] + @""", ""MASA8T"":" + item_w33_2[2] + @", ""MASA11T"":" + item_w33_2[3] +/* @", ""DLMOS"":" + item_w33_2[4] + @", ""DLPR"":" + item_w33_2[5] + */@", ""GRMP"":" + item_w33_2[6] +/* @", ""OMM"":" + item_w33_2[7] + */@", ""role"":""gaska"", ""caption"":""Gąska:" + item_w33_2[0] + ", ROD:W33, MASA8T:" + item_w33_2[2] + ",MASA11T:" + item_w33_2[3] + ",DLMOS:" + item_w33_2[4] + ", DLPR:" + item_w33_2[5] + ", GRMP:" + item_w33_2[6] + ", OMM:" + item_w33_2[7] + @"""},";
                                    }

                                    edges_json += @"{""source"": " + item_w33_1[0] + @", ""target"": " + item_w33_2[0] + @", ""caption"": ""DOPASOWANE""},";
                                    licznik++;
                                    break;
                                }
                                else if (Convert.ToString(list_rel_deg[0][0]) == "OJCIEC" && Convert.ToString(list_rel_deg[1][0]) == "GASIE")
                                {
                                    if (nodes_json.Contains(Convert.ToString(item_w33_2[0])) == false)
                                    {
                                        nodes_json += @"{""id"":" + item_w33_2[0] + @", ""ROD"":""" + item_w33_2[1] + @""", ""MASA8T"":" + item_w33_2[2] + @", ""MASA11T"":" + item_w33_2[3] +/* @", ""DLMOS"":" + item_w33_2[4] + @", ""DLPR"":" + item_w33_2[5] + */@", ""GRMP"":" + item_w33_2[6] +/* @", ""OMM"":" + item_w33_2[7] + */@", ""role"":""gaska"", ""caption"":""Gąska:" + item_w33_2[0] + ", ROD:W33, MASA8T:" + item_w33_2[2] + ",MASA11T:" + item_w33_2[3] + ",DLMOS:" + item_w33_2[4] + ", DLPR:" + item_w33_2[5] + ", GRMP:" + item_w33_2[6] + ", OMM:" + item_w33_2[7] + @"""},";
                                    }

                                    edges_json += @"{""source"": " + item_w33_1[0] + @", ""target"": " + item_w33_2[0] + @", ""caption"": ""DOPASOWANE""},";
                                    licznik++;
                                    break;
                                }
                                break;


                            case 3:
                                if ((Convert.ToString(list_rel_deg[0][0]) == "GASIE" && Convert.ToInt32(list_rel_deg[0][1]) >= 3) && ((Convert.ToInt32(list_rel_deg[1][1]) + (Convert.ToInt32(list_rel_deg[2][1]))) >= 3))
                                {
                                    if (nodes_json.Contains(Convert.ToString(item_w33_2[0])) == false)
                                    {
                                        nodes_json += @"{""id"":" + item_w33_2[0] + @", ""ROD"":""" + item_w33_2[1] + @""", ""MASA8T"":" + item_w33_2[2] + @", ""MASA11T"":" + item_w33_2[3] +/* @", ""DLMOS"":" + item_w33_2[4] + @", ""DLPR"":" + item_w33_2[5] + */@", ""GRMP"":" + item_w33_2[6] +/* @", ""OMM"":" + item_w33_2[7] + */@", ""role"":""gaska"", ""caption"":""Gąska:" + item_w33_2[0] + ", ROD:W33, MASA8T:" + item_w33_2[2] + ",MASA11T:" + item_w33_2[3] + ",DLMOS:" + item_w33_2[4] + ", DLPR:" + item_w33_2[5] + ", GRMP:" + item_w33_2[6] + ", OMM:" + item_w33_2[7] + @"""},";
                                    }

                                    edges_json += @"{""source"": " + item_w33_1[0] + @", ""target"": " + item_w33_2[0] + @", ""caption"": ""DOPASOWANE""},";
                                    licznik++;
                                    break;
                                }
                                break;
                        }
                        if (licznik == 6)
                        {
                            break;
                        }
                    }

                }


                if (w33_1_list.Count != 0 && w33_2_list.Count != 0)
                {
                    System.IO.File.WriteAllText(@"C:\Users\Jakub Krysiński\Desktop\Inzyernika_ostatnia\Inzynierka_ostatnia\Inzynierka_ostatnia\data\dane2.json", string.Empty);
                    System.IO.StreamWriter file_json = new System.IO.StreamWriter(@"C:\Users\Jakub Krysiński\Desktop\Inzyernika_ostatnia\Inzynierka_ostatnia\Inzynierka_ostatnia\data\dane2.json");
                    file_json.WriteLine(@"{""nodes"": [");
                    file_json.WriteLine(nodes_json.Substring(0, nodes_json.Length - 1));
                    file_json.WriteLine(@"], ""edges"": [");
                    file_json.WriteLine(edges_json.Substring(0, edges_json.Length - 1));
                    file_json.WriteLine("]}");
                    file_json.Close();

                }


            }

            return View();
        }
    }
}