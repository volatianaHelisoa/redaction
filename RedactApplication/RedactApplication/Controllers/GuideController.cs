using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Microsoft.Security.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedactApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace RedactApplication.Controllers
{
    public class GuideController : Controller
    {
        private redactapplicationEntities db = new redactapplicationEntities();
        private static Guid _userId;
        // GET: Guide
        public ActionResult ListGuide()
        {
            string guides_in_progress = "0";

            try
            {
                JObject status = get_status();
                guides_in_progress = GetJArrayValue(status, "guides_in_progress");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("thread error: " + ex);
            }

            if (guides_in_progress == "0")
                 UpdateGuide();

            ViewBag.listeGuideVm = new Guides().GetListGuide();           
            return View();
        }

        private string GetJArrayValue(JObject yourJArray, string key)
        {
            string res = "";
            foreach (KeyValuePair<string, JToken> keyValuePair in yourJArray)
            {
                if (key == keyValuePair.Key)
                {
                    return keyValuePair.Value.ToString();
                }
            }

            return res;
        }

        private JObject get_status()
        {
            string url = "https://yourtext.guru/api/status/";
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.ContentType = "application/json";
            request.Method = "GET";
            UTF8Encoding enc = new UTF8Encoding();
            string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
            request.Headers.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);

            var response = (HttpWebResponse)request.GetResponse();

            JObject jsonVal;

            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                var res = streamReader.ReadToEnd();
                jsonVal = JObject.Parse(res) as JObject;
            }

            return jsonVal;
        }

        // GET: Guide/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Guide/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGuide(GUIDEViewModel model, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                // Exécute le suivi de session utilisateur
                if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
                {
                    _userId = Guid.Parse(Request.QueryString["currentid"]);
                    Session["currentid"] = Request.QueryString["currentid"];
                }
                else
                    _userId = Guid.Parse(HttpContext.User.Identity.Name);


                var typeguide = "1";

                var newGuide = new GUIDE();
                newGuide.date_creation = DateTime.Now;
                newGuide.redacteur = model.redacteur;
                var currentuser = (new Utilisateurs()).GetUtilisateur(_userId);
                newGuide.demandeur = currentuser.userMail;
                newGuide.type = int.Parse(typeguide);
                newGuide.mot_cle_pricipal = model.mot_cle_pricipal;
                newGuide.consigne_autres = model.consigne_autres;
                newGuide.guideId = Guid.NewGuid();

                newGuide.mot_cle_secondaire = model.mot_cle_secondaire;
                newGuide.lien_pdf = model.lien_pdf;
                db.GUIDEs.Add(newGuide);
                int res = db.SaveChanges();
                if (res > 0)
                {
                    return View("CreateGuideConfirmation");
                }

            }

            return View("ErrorException");
        }

        private void UpdateGuide()
        {
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => GenerateCommandeKeysAsync(cancellationToken));           
        }

        private async Task GenerateCommandeKeysAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Exécute le traitement 
                Guides val = new Guides();

                // Récupère la liste des commandes
                var listeDataCmde = val.GetListGuide();

                var db = new redactapplicationEntities();

                if (listeDataCmde.Count() > 0)
                { 
                    listeDataCmde = listeDataCmde.Where(x => x.mot_cle_secondaire == "" || x.mot_cle_secondaire == null).ToList();

                    foreach (var cmd in listeDataCmde)
                    {
                        GUIDE guide = db.GUIDEs.Find(cmd.guideId);
                        string mot_cle_pricipal = guide.mot_cle_pricipal;
                        if (string.IsNullOrEmpty(guide.guide_id))
                        {
                            int guide_id = GetGuideID(mot_cle_pricipal, "premium");
                            guide.guide_id = guide_id.ToString();
                            int save = db.SaveChanges();
                        }
                        else
                        {
                            /*if (guide.type == 0) //oneshot
                            {
                                guide_id = GetGuideID(mot_cle_pricipal, "oneshot");
                            }
                            else //premium
                            {
                                guide_id = GetGuideID(mot_cle_pricipal, "premium");
                            }*/

                            //Lancer une commande de guide              
                            string url = "https://yourtext.guru/api/guide/" + guide.guide_id;

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
                                //await Task.Delay(200000); //3mn       

                                var response = (HttpWebResponse)request.GetResponse();
                                //await Task.Delay(200000); //3mn    
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    res = streamReader.ReadToEnd();
                                    JArray jsonVal = JArray.Parse(res) as JArray;
                                    /*if (guide.type == 0) //oneshot
                                    {
                                        string titre = (jsonVal[0]["items"][0]["tokens"] != null) ? string.Join(",", jsonVal[0]["items"][0]["tokens"]) : "";
                                        string chapo = (jsonVal[0]["items"][1]["tokens"] != null) ? string.Join(",", jsonVal[0]["items"][1]["tokens"]) : "";
                                        string sous_titre_1 = (jsonVal[0]["items"][2]["tokens"] != null) ? string.Join(",", jsonVal[0]["items"][2]["tokens"]) : "";
                                        var paragraphe_1 = string.Join(",", jsonVal[0]["items"][3]["lines"]) ?? null;
                                        string sous_titre_2 = (jsonVal[0]["items"][4]["tokens"] != null) ? string.Join(",", jsonVal[0]["items"][4]["tokens"]) : "";
                                        var paragraphe_2 = string.Join(",", jsonVal[0]["items"][5]["lines"]) ?? null;

                                        guide.titre = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(titre));
                                        guide.chapo = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(chapo));
                                        guide.sous_titre_1 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(sous_titre_1));
                                        guide.paragraphe_1 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(paragraphe_1.Replace("[","").Replace("]","")));
                                        guide.sous_titre_2 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(sous_titre_2));
                                        guide.paragraphe_2 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(paragraphe_2.Replace("[", "").Replace("]", "")));

                                        guide.mot_cle_secondaire = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(titre));
                                    }
                                    else //premium
                                    {
                                        string grammes1 = (jsonVal[0]["grammes1"] != null) ? string.Join(",", jsonVal[0]["grammes1"]) : "";
                                        string grammes2 = (jsonVal[0]["grammes2"] != null) ? string.Join(",", jsonVal[0]["grammes2"]) : "";
                                        string grammes3 = (jsonVal[0]["grammes3"] != null) ? string.Join(",", jsonVal[0]["grammes3"]) : "";
                                        string entities = (jsonVal[0]["entities"] != null) ? string.Join(",", jsonVal[0]["entities"]) : "";
                                        grammes = grammes2 + "," + grammes3;

                                        guide.grammes1 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes1));
                                        guide.grammes2 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes2));
                                        guide.grammes3 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes3));
                                        guide.entities = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(entities));

                                        if (!string.IsNullOrEmpty(grammes))
                                            guide.mot_cle_secondaire = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes));
                                    }*/

                                    string grammes1 = (jsonVal[0]["grammes1"] != null) ? string.Join(",", jsonVal[0]["grammes1"]) : "";
                                    string grammes2 = (jsonVal[0]["grammes2"] != null) ? string.Join(",", jsonVal[0]["grammes2"]) : "";
                                    string grammes3 = (jsonVal[0]["grammes3"] != null) ? string.Join(",", jsonVal[0]["grammes3"]) : "";
                                    string entities = (jsonVal[0]["entities"] != null) ? string.Join(",", jsonVal[0]["entities"]) : "";
                                    grammes = grammes2 + "," + grammes3;

                                    guide.grammes1 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes1));
                                    guide.grammes2 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes2));
                                    guide.grammes3 = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes3));
                                    guide.entities = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(entities));

                                    if (!string.IsNullOrEmpty(grammes))
                                        guide.mot_cle_secondaire = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(grammes));
                                }


                            }

                            int result = db.SaveChanges();

                            if (result > 0)
                            {
                                if (Request.Url != null)
                                {
                                    var url_req = Request.Url.Scheme;
                                    if (Request.Url != null)
                                    {
                                        string callbackurl = Request.Url.Host != "localhost"
                                            ? Request.Url.Host
                                            : Request.Url.Authority;
                                        var port = Request.Url.Port;
                                        if (!string.IsNullOrEmpty(port.ToString()) && Request.Url.Host != "localhost")
                                            callbackurl += ":" + port;

                                        url_req += "://" + callbackurl;
                                    }


                                    StringBuilder mailBody = new StringBuilder();
                                    string filename = "";

                                    if (guide.type == 0) //oneshot
                                    {
                                        filename = "GuideOneShot-" + mot_cle_pricipal + ".pdf";
                                        GeneratOneshotPDF(guide, filename);
                                    }

                                    else
                                    {
                                        filename = "GuidePremium" + mot_cle_pricipal + ".pdf";
                                        GeneratePremiumPDF(guide, filename);
                                    }

                                    var filePath = Server.MapPath("~/Pdf/Guides/" + filename);

                                    if (guide.redacteur != null)
                                    {
                                        mailBody.AppendFormat(
                                            "Monsieur / Madame, ");
                                        mailBody.AppendFormat("<br />");
                                        mailBody.AppendFormat(
                                            "<p>Vous avez reçu un nouveau guide à la rédaction le " + DateTime.Now + ".<p>");
                                        mailBody.AppendFormat("<br />");
                                        mailBody.AppendFormat(
                                           "<p>Consignes : " + guide.consigne_autres);
                                        mailBody.AppendFormat("<br />");
                                        mailBody.AppendFormat("Cordialement,");
                                        mailBody.AppendFormat("<br />");
                                        mailBody.AppendFormat("Media click App .");

                                        bool isSendMail = MailClient.SendMail(guide.redacteur, mailBody.ToString(), "Media click App - nouvelle commande.", filePath);
                                        if (isSendMail)
                                        {
                                            guide.lien_pdf = "~/Pdf/Guides/" + filename;
                                            db.SaveChanges();
                                            Debug.WriteLine("CreateCommandeConfirmation");
                                        }



                                    }
                                }
                            }
                            else
                            {
                                Debug.WriteLine("thread error");
                            }
                        }
                    }
                    //Session["UpdateGuide"] = null;
                }

            }
            catch (Exception ex)
            {
                Session["UpdateGuide"] = null;
                Debug.WriteLine("thread error: " + ex);
            }

            Thread.Sleep(200000);
        }

        private int GetGuideID(string mot_cle_pricipal,string type)
        {
            try
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
                            type = type
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
            catch (Exception ex)
            {
                Debug.WriteLine("thread error: " + ex);
                return 0;

            }
        }

        public ActionResult GuideConfirmationVolume()
        {
            return View("CreateGuideConfirmation");
        }


        // POST: Guide/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Guide/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Guide/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Guide/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Guide/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Charge une liste des Guides à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id de Guide</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelecteAllGuideToDelete(string hash)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ListGuideToDelete"] = hash;
                if (Session["ListGuideToDelete"] != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Supprime une liste d'Utilisateur dans la base de données.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeleteAllGuideSelected()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);

            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(userSession);
            try
            {
                bool unique = true;
                if (Session["ListGuideToDelete"] != null)
                {
                    string hash = Session["ListGuideToDelete"].ToString();
                    List<Guid> listIdGuide = new List<Guid>();
                    if (!string.IsNullOrEmpty(hash))
                    {
                        if (!hash.Contains(','))
                        {
                            listIdGuide.Add(Guid.Parse(hash));
                        }
                        else
                        {
                            foreach (var id in (hash).Split(','))
                            {
                                listIdGuide.Add(Guid.Parse(id));
                            }
                            unique = false;
                        }
                    }
                    if (listIdGuide.Count != 0)
                    {
                        redactapplicationEntities db = new Models.redactapplicationEntities();
                        foreach (var GuideId in listIdGuide)
                        {
                            //suppression des Guides
                            GUIDE Guide = db.GUIDEs.Find(GuideId);
                            db.GUIDEs.Remove(Guide);
                        }
                        db.SaveChanges();

                        if (unique)
                        {
                            return View("DeletedGuideConfirmation");
                        }
                        return View("DeletedAllGuideConfirmation");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return View("ListGuide");
        }

        private byte[] BindPdf(string pHTML)
        {
            var cssText = "~/Content/css/facture.css";
            byte[] bPDF = null;

            var memoryStream = new MemoryStream();

            var input = new MemoryStream(Encoding.UTF8.GetBytes(pHTML));
            var document = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            writer.CloseStream = false;

            document.Open();
            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(iTextSharp.tool.xml.html.Tags.GetHtmlTagProcessorFactory());

            ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(false);
            cssResolver.AddCssFile(System.Web.HttpContext.Current.Server.MapPath(cssText), true);

            var pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer)));
            var worker = new XMLWorker(pipeline, true);
            var p = new XMLParser(worker);
            p.Parse(input);
            document.Close();

            bPDF = memoryStream.ToArray();
            return bPDF;

        }

        [HttpPost]
        [Authorize]
        public ActionResult DownloadPDFIText(string htmlContent)
        {
            try
            {
                var mot_cle_pricipal = Session["mot_cle_pricipal"].ToString();
             
                var bytes = BindPdf(htmlContent);
                string filename = "Guide-" + mot_cle_pricipal + "-" + DateTime.Now.Month + ".pdf";
                var filePath = Server.MapPath("~/Pdf/Guides/");

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                System.IO.File.WriteAllBytes(filePath + filename, bytes);

                return Json(new
                {
                    Valid = true,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Valid = false,
                });
            }
        }
        
        public string RenderViewAsString(string viewName, object model)
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();

            // get the view to render
            ViewEngineResult viewResult = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    new ViewDataDictionary(model),
                    new TempDataDictionary(),
                    stringWriter
                    );

            // render the view to a HTML code
            viewResult.View.Render(viewContext, stringWriter);

            // return the HTML code
            return stringWriter.ToString();
        }

        // GET: Facture/DetailsOneshot/5
        public ActionResult DetailsOneshot(Guid? hash)
        {
            if (hash == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var doc1 = new iTextSharp.text.Document();
            //var filePath = Server.MapPath("~/Pdf/Guides/");
            //PdfWriter.GetInstance(doc1, new FileStream(filePath + "/Doc1.pdf", FileMode.Create));
            //doc1.Open();
            //doc1.Add(new Paragraph("My sample text goes here."));
            //doc1.Close();
            
            var guideVm = new Guides().GetDetailsGuide(hash);
            
             Session["mot_cle_pricipal"] = guideVm.mot_cle_pricipal;           
            Session["redact"] = guideVm.redacteur.Trim().ToLower();
            return View(guideVm);
        }

        private void GeneratePremiumPDF(GUIDE guide,string filename)
        {
            //Creating iTextSharp Table from the DataTable data
            PdfPTable pdfTable = new PdfPTable(4);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 30;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTable.DefaultCell.BorderWidth = 1;

            //Exporting to PDF
            var folderPath = Server.MapPath("~/Pdf/Guides/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (FileStream stream = new FileStream(folderPath + filename, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(pdfTable);
                pdfDoc.Add(new Paragraph("TOP TERMES"));
                pdfDoc.Add(new Paragraph(guide.grammes1));
                pdfDoc.Add(new Paragraph("AUTRES TERMES"));
                pdfDoc.Add(new Paragraph(guide.grammes2 + guide.grammes3));
                pdfDoc.Add(new Paragraph("ENTITES"));
                pdfDoc.Add(new Paragraph(guide.entities));
                pdfDoc.Close();
                stream.Close();
            }
        }

        private void GeneratOneshotPDF(GUIDE guide,string filename)
        {
            //Creating iTextSharp Table from the DataTable data
            PdfPTable pdfTable = new PdfPTable(4);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 30;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTable.DefaultCell.BorderWidth = 1;            

            //Exporting to PDF
            var folderPath = Server.MapPath("~/Pdf/Guides/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (FileStream stream = new FileStream(folderPath + filename, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(pdfTable);
                pdfDoc.Add(new Paragraph("Titre"));
                pdfDoc.Add(new Paragraph(guide.titre));
                pdfDoc.Add(new Paragraph("Chapo : Rédigez un texte court contenant au moins :"));
                pdfDoc.Add(new Paragraph(guide.chapo ));
                pdfDoc.Add(new Paragraph("Sous - titre : Rédigez votre sous - titre avec ces termes."));
                pdfDoc.Add(new Paragraph(guide.sous_titre_1));               
                pdfDoc.Add(new Paragraph("Paragraphe : Rédigez une séries de phrases contenant au moins ces termes."));
                pdfDoc.Add(new Paragraph(guide.paragraphe_1));
                pdfDoc.Add(new Paragraph("Sous - titre : Rédigez votre sous - titre avec ces termes."));
                pdfDoc.Add(new Paragraph(guide.sous_titre_2));
                pdfDoc.Add(new Paragraph("Paragraphe : Rédigez une séries de phrases contenant au moins ces termes."));
                pdfDoc.Add(new Paragraph(guide.paragraphe_2));
                pdfDoc.Close();
                stream.Close();
            }
        }

        // GET: Facture/DetailsPremium/5
        public ActionResult DetailsPremium(Guid? hash)
        {
            if (hash == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var guideVm = new Guides().GetDetailsGuide(hash);
          
            Session["mot_cle_pricipal"] = guideVm.mot_cle_pricipal;
            Session["redact"] = guideVm.redacteur.Trim().ToLower();
            return View(guideVm);
        }
      
        public ActionResult Scoring()
        {            
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]     
        public async Task<ActionResult> ScoringGuide(GUIDEViewModel model, FormCollection collection, CancellationToken cancellationToken)
        {
            try
            {

                var mot_cle_pricipal = model.mot_cle_pricipal;
                var CONTENT = model.contenu;

                int guide_id = 47395;

                string type = Request.Form["typeguide"] == "0" ? "oneshot" : "premium";


                /*if (guide_id == 0) //oneshot
                {
                    guide_id = GetGuideID(mot_cle_pricipal, "oneshot");
                }
                else //premium
                {
                    guide_id = GetGuideID(mot_cle_pricipal, "premium");
                }*/
                //guide_id = GetGuideID(mot_cle_pricipal, "premium");
                UTF8Encoding enc = new UTF8Encoding();
                string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];
                
                //Lancer une commande de guide              
                /*string url_guide = "https://yourtext.guru/api/guide/" + guide_id;
                var res = "";
                var request_guide = (HttpWebRequest)WebRequest.Create(url_guide);
          
                
                //execute when task has been cancel  
                cancellationToken.ThrowIfCancellationRequested();
                //Obtenir le guide
                if (request_guide != null)
                {
                    request_guide.ContentType = "application/json";
                    request_guide.Method = "GET";
               
                    request_guide.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                    request_guide.Headers.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);

                    await Task.Delay(200000); //3mn   
                    var response_guide = (HttpWebResponse)request_guide.GetResponse();
                   // await Task.Delay(200000); //3mn    
                    using (StreamReader streamReader = new StreamReader(response_guide.GetResponseStream()))
                    {
                        res = streamReader.ReadToEnd();

                    }
                }*/

                //scoring guide      
                string url = "https://yourtext.guru/api/check/" + guide_id;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                //string usernamePassword = ConfigurationManager.AppSettings["yourtext_usr"] + ":" + ConfigurationManager.AppSettings["yourtext_pwd"];

                if (request != null)
                {
                    //UTF8Encoding enc = new UTF8Encoding();
                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(enc.GetBytes(usernamePassword)));
                    request.Headers.Add("KEY", ConfigurationManager.AppSettings["yourtext_api"]);

                    //using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    //{
                    //    //string json = new JavaScriptSerializer().Serialize(new
                    //    //{
                    //    //    content = "une technologie grille-pain présente de nombreux avantages"
                    //    //});

                    //    string json = "{ \"content\" : \" un grille-pain présente de nombreux avantages \" }";
                    //    // string json = "{\"content\":\"" + CONTENT + "\"}";
                    //    streamWriter.Write(json);
                    //}
                    //var postData = "{ \"content\" : \" un grille-pain présente de nombreux avantages \" }";
                    var postData = "{\"content\":\"" + CONTENT + "\"}";


                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = byteArray.Length;

                    // Get the request stream.
                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    // Close the Stream object.
                    dataStream.Close();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //Thread.Sleep(190000);
                    var response = (HttpWebResponse)request.GetResponse();

                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        Console.WriteLine(String.Format("Response: {0}", result));
                        //JArray jsonVal = JArray.Parse(result) as JArray;
                        JObject jsonVal = JObject.Parse(result);
                        ViewBag.optimisation = jsonVal;
                    }
                }


                return View("Scoring");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("thread error: " + ex);
                return View();

            }
        }
        
    }
}
