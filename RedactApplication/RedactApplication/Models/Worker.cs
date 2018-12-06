using Microsoft.Security.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace RedactApplication.Models
{
    public class Worker
    {
        string filePath = @"d:/test.txt";
        private redactapplicationEntities db = new redactapplicationEntities();

        public void StartProcessing(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);

                using (File.Create(filePath)) { }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
                {
                    for (int index = 1; index <= 20; index++)
                    {
                        //execute when task has been cancel  
                        cancellationToken.ThrowIfCancellationRequested();
                        file.WriteLine("Its Line number : " + index + "\n");
                        Thread.Sleep(1500);   // wait to 1.5 sec every time  
                    }

                    if (Directory.Exists(@"d:/done"))
                        Directory.Delete(@"d:/done");
                    Directory.CreateDirectory(@"d:/done");
                }
            }
            catch (Exception ex)
            {
                ProcessCancellation();
                File.AppendAllText(filePath, "Error Occured : " + ex.GetType().ToString() + " : " + ex.Message);
            }
        }
        private void ProcessCancellation()
        {
            Thread.Sleep(10000);
            File.AppendAllText(filePath, "Process Cancelled");
        }

        public async System.Threading.Tasks.Task GenerateCommandeKeysAsync(CancellationToken cancellationToken = default(CancellationToken))
        {          
                try
                {
                 
                    // Exécute le traitement de la pagination
                    Commandes val = new Commandes();

                    // Récupère la liste des commandes
                    var listeDataCmde = val.GetListCommande();
                    var now = DateTime.Now;
                    var startDate = ConfigurationManager.AppSettings["startDate"].ToString();
                    var startOfMonth = new DateTime(now.Year, now.Month - 1, int.Parse(startDate));
                    var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    var lastDay = new DateTime(now.Year, now.Month, daysInMonth);
                    listeDataCmde = listeDataCmde.Where(x => x.date_livraison >= startOfMonth &&
                                                                  x.date_livraison <= lastDay).ToList();

                    var statut = db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains("Traitement"));
                    listeDataCmde = listeDataCmde.Where(x => x.commandeStatutId == statut.statutCommandeId).ToList();

                    foreach (var cmd in listeDataCmde)
                    {
                        COMMANDE commande = db.COMMANDEs.Find(cmd.commandeId);
                        if (string.IsNullOrEmpty(commande.mot_cle_secondaire))
                        {
                            string mot_cle_pricipal = commande.mot_cle_pricipal;
                            int guide_id = GetGuideID(mot_cle_pricipal);

                            //Lancer une commande de guide              
                            string url = "https://yourtext.guru/api/guide/" + guide_id;

                            var res = "";
                            var request = (HttpWebRequest)WebRequest.Create(url);
                            var grammes = "";

                            string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];
                            //execute when task has been cancel  
                            cancellationToken.ThrowIfCancellationRequested();
                            //Obtenir le guide
                            if (request != null)
                            {
                                request.ContentType = "application/json";
                                request.Method = "GET";
                                UTF8Encoding enc = new UTF8Encoding();
                                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                                request.Headers.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);

                                await Task.Delay(200000); //3mn       


                                var response = (HttpWebResponse)request.GetResponse();
                                await Task.Delay(200000); //3mn    
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    res = streamReader.ReadToEnd();
                                    JArray jsonVal = JArray.Parse(res) as JArray;
                                    Console.WriteLine(string.Join(" ", jsonVal[0]["grammes2"]));
                                    string grammes2 = string.Join(",", jsonVal[0]["grammes2"]);
                                    string grammes3 = string.Join(",", jsonVal[0]["grammes3"]);
                                    grammes = grammes2 + "," + grammes3;
                                }

                                if (!string.IsNullOrEmpty(grammes))
                                {
                                    commande.mot_cle_secondaire = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes));
                                    string etat = "En attente";
                                    var newstatut = db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains(etat));
                                    commande.commandeStatutId = newstatut.statutCommandeId;
                                    int result = db.SaveChanges();
                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    ProcessCancellation();
                    File.AppendAllText(filePath, "Error Occured : " + ex.GetType().ToString() + " : " + ex.Message);
                }
       
         
        }

        private string GetGuideAssetAsync(int guide_id)
        {
            string url = "https://yourtext.guru/api/guide/" + guide_id;
            var request = (HttpWebRequest)WebRequest.Create(url);
            string grammes = "";

            string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];
            //Obtenir le guide
            if (request != null)
            {
                request.ContentType = "application/json";
                request.Method = "GET";
                UTF8Encoding enc = new UTF8Encoding();
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                request.Headers.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);
                var res = "";
                Thread.Sleep(180000);
                var response = (HttpWebResponse)request.GetResponse();
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    res = streamReader.ReadToEnd();
                    JArray jsonVal = JArray.Parse(res) as JArray;
                    Console.WriteLine(jsonVal[0]);

                }
                return grammes;
            }
            return null;
        }

        private int GetGuideID(string mot_cle_pricipal)
        {
            int guide_id = 0;
            string url = "https://yourtext.guru/api/guide/";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];

            if (request != null)
            {
                UTF8Encoding enc = new UTF8Encoding();
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                request.Headers.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        query = mot_cle_pricipal,
                        lang = "fr_fr",
                        type = "premium"
                    });

                    streamWriter.Write(json);
                }

                var response = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(String.Format("Response: {0}", result));

                    JObject obj = JObject.Parse(result);
                    guide_id = (int)obj["guide_id"];
                }


            }

            return guide_id;
        }



        public async Task GetScoringAsync1(string mot_cle_pricipal, string contenu) {

            mot_cle_pricipal = "grille-pain";
            int guide_id = GetGuideID(mot_cle_pricipal);          

            //Where we are posting to:
            Uri theUri = new Uri("https://yourtext.guru/api/check/" + guide_id);
            string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];
            UTF8Encoding enc = new UTF8Encoding();

            var response = string.Empty;
            using (var aClient = new HttpClient())
            {
                aClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                aClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                aClient.DefaultRequestHeaders.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);

                var httpcontenu = "{\"content\":\""+ contenu+"\" }";

                HttpContent cnt = new StringContent(httpcontenu, Encoding.UTF8, "application/json");

               

                // The actual Get method
              using (var result = await aClient.GetAsync($"{theUri}{cnt}"))
                {
                    string content = await result.Content.ReadAsStringAsync();
                   
                }
            }
           
        }


        public async Task<string> GetScoringAsync(string mot_cle_pricipal, string contenu)
        {
            string scores = "";
            try
            {
               
                

                int guide_id = GetGuideID(mot_cle_pricipal);

                string url = "https://yourtext.guru/api/check/" + guide_id;
                

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];

                if (request != null)
                {
                    UTF8Encoding enc = new UTF8Encoding();
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                    request.Headers.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        string json = new JavaScriptSerializer().Serialize(new
                        {
                            content = contenu
                        });

                        streamWriter.Write(json);
                    }

                  
                    var response = (HttpWebResponse)request.GetResponse();
                    //await Task.Delay(200000); //3mn    

                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        Console.WriteLine(String.Format("Response: {0}", result));
                        JArray jsonVal = JArray.Parse(result) as JArray;
                        Console.WriteLine(string.Join(" ", jsonVal[0]["grammes2"]));
                        string score = string.Join(",", jsonVal[0]["score"]);
                        string danger = string.Join(",", jsonVal[0]["danger"]);
                        scores = score + "," + danger;
                    }
                }



            }
            catch (Exception ex)
            {
                ProcessCancellation();
                Console.WriteLine(ex.GetType().ToString() + " : " + ex.Message);
            }
            return scores;
        }



        private int GetOneshotGuideID(string mot_cle_pricipal)
        {
            int guide_id = 0;
            string url = "https://yourtext.guru/api/guide/";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            string usernamePassword = "claude@digital-impulse.be" + ":" + "nreb48t7";

            if (request != null)
            {
                UTF8Encoding enc = new UTF8Encoding();
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                request.Headers.Add("KEY", "$2y$10$GiIWImgbCnujDhIyDXFfcO0nTAUU6UIGem45JA/IBbg/4zaGy7vHm");

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        query = mot_cle_pricipal,
                        lang = "fr_fr",
                        type = "oneshot"
                    });

                    streamWriter.Write(json);
                }

                var response = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(String.Format("Response: {0}", result));
                    JObject obj = JObject.Parse(result);
                    guide_id = (int)obj["guide_id"];
                }
            }

            return guide_id;
        }

        public async Task GenerateCommandeOneShotKeysAsync(string mot_cle_pricipal, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {  
                int guide_id = GetOneshotGuideID(mot_cle_pricipal);

                //Lancer une commande de guide              
                string url = "https://yourtext.guru/api/guide/" + guide_id;

                var res = "";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var grammes = "";

                string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];
                //execute when task has been cancel  
                cancellationToken.ThrowIfCancellationRequested();
                //Obtenir le guide
                if (request != null)
                {
                    request.ContentType = "application/json";
                    request.Method = "GET";
                    UTF8Encoding enc = new UTF8Encoding();
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                    request.Headers.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);

                    await Task.Delay(200000); //3mn       
                  

                    var response = (HttpWebResponse)request.GetResponse();
                    await Task.Delay(200000); //3mn    
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        res = streamReader.ReadToEnd();
                        JArray jsonVal = JArray.Parse(res) as JArray;
                        Console.WriteLine(string.Join(" ", jsonVal[0]["grammes2"]));
                        string grammes2 = string.Join(",", jsonVal[0]["grammes2"]);
                        string grammes3 = string.Join(",", jsonVal[0]["grammes3"]);
                        grammes = grammes2 + "," + grammes3;
                    }

                }

            }
            catch (Exception ex)
            {
                ProcessCancellation();
                File.AppendAllText(filePath, "Error Occured : " + ex.GetType().ToString() + " : " + ex.Message);
            }


        }

    }

    class ContenuSite
    {
        public string Contenu { get; set; }
      
    }

}

