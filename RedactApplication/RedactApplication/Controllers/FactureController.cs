using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using RedactApplication.Models;


namespace RedactApplication.Controllers
{
    public class FactureController : Controller
    {
        private redactapplicationEntities db = new redactapplicationEntities();
        private static Guid _userId;
        // GET: Facture
        public ActionResult Index()
        {
            ViewBag.listeFactureVm = new Factures().GetListFacture();

            return View();
        }

        public ActionResult ListFacture()
        {
            ViewBag.listeFactureVm = new Factures().GetListFacture();
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

            var currentrole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);

            if (currentrole != null && currentrole.Contains("2"))
                ViewBag.listeFactureVm = new Factures().GetListFacture().Where(x=>x.redacteurId == _userId);


            return View();
        }

        [HttpPost]
        public string UpdateFacture(string id)
        {
            try
            {

                var facture = db.FACTUREs.Where(x => x.factureId.ToString() == id).SingleOrDefault();

                facture.etat = (facture.etat == true) ? false : true;

                var commandesFacturer = db.COMMANDEs.Where(x => x.factureId.ToString() == id  ).ToList();
                foreach (var commande in commandesFacturer)
                {
                   
                    var status = (facture.etat == true) ? db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("Payé")) : db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("Facturé")); ;
                    commande.STATUT_COMMANDE = status;
                    db.SaveChanges();
                }

                var res = db.SaveChanges();
                return res.ToString();
            }
            catch (Exception)
            {
                throw;
            }


        }


        // GET: Facture/Details/5
        public ActionResult Details(Guid? hash)
        {
            if (hash == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var factureVm = new Factures().GetDetailsFacture(hash);
            Session["factureNum"] = factureVm.factureNumero;
            Session["logo"] = Server.MapPath("~/images/logo_mc_large.png");
            Session["redact"] = factureVm.REDACTEUR.userNom.Trim().ToLower();
            return View(factureVm);
        }

        private FACTUREViewModel SetFactureViewModel()
        {
            Factures val = new Factures();
            FACTUREViewModel factureVm = new FACTUREViewModel();
            factureVm.dateDebut = DateTime.Now.AddMonths(-1).AddDays(1);
            factureVm.dateFin = DateTime.Now;
            factureVm.ListRedacteur = val.GetListRedacteurItem();
            return factureVm;
        }

        // GET: Facture/Create
        public ActionResult Create()
        {
            
            FACTUREViewModel factureVm = SetFactureViewModel();
            return View(factureVm);
        }
        public byte[] GetPDF(string pHTML)
        {
            byte[] bPDF = null;
            var cssText = System.IO.File.ReadAllText(Server.MapPath("~/Content/css/facture.css"));
            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);

            // 1: create object of a itextsharp document class
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);

            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            // 3: we create a worker parse the document
            HTMLWorker htmlWorker = new HTMLWorker(doc);
           

            // 4: we open document and start the worker on the document
            doc.Open();
            htmlWorker.StartDocument();

            // 5: parse the html into the document
            htmlWorker.Parse(txtReader);

            // 6: close the document and the worker
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
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
                var numFacture = Session["factureNum"].ToString();
                var redactFolder = Session["redact"].ToString();
                var bytes = BindPdf(htmlContent);
                string filename = "Facture-" + numFacture + "-" + DateTime.Now.Month + ".pdf";
                var filePath = Server.MapPath("~/Pdf/" + redactFolder + "/");

                
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

        // POST: Facture/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFacture(FACTUREViewModel model, FormCollection collection)
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

                var selectedRedacteurId = model.listRedacteurId;
                var newFacture = new FACTURE();
                newFacture.dateDebut = model.dateDebut;
                newFacture.dateFin = model.dateFin;
                newFacture.dateEmission = DateTime.Now;
                var commandesFacturer = db.COMMANDEs.Where(x => x.date_livraison >= model.dateDebut &&
                                                                 x.date_livraison <= model.dateFin && 
                                                                 (x.STATUT_COMMANDE != null &&
                                                                 (x.STATUT_COMMANDE.statut_cmde.Contains("Validé") || x.STATUT_COMMANDE.statut_cmde.Contains("Refusé"))) &&
                                                                 x.factureId == null
                                                                 ).ToList();
                if (commandesFacturer.Count() > 0)
                {
                    var redacteur = db.UTILISATEURs.SingleOrDefault(x => x.userId == model.listRedacteurId);
                    
                    double montant = 0;
                    Guid factureId = Guid.NewGuid();


                    foreach (var commande in commandesFacturer)
                    {
                        if(commande.STATUT_COMMANDE.statut_cmde.Contains("Validé"))
                            montant += Convert.ToDouble(commande.nombre_mots) * (Convert.ToDouble(redacteur.redactTarif));
                        
                        commande.factureId = factureId;
                        commande.REDACTEUR.redactTarif = String.Format("{0:N0}", commande.REDACTEUR.redactTarif);
                        
                    }

                    newFacture.montant = String.Format("{0:N0}", montant);
                    newFacture.etat = false;
                    newFacture.redacteurId = model.listRedacteurId;
                    newFacture.createurId = _userId;
                    newFacture.factureId = factureId;
                    int maxRef = (db.FACTUREs.ToList().Count != 0) ? db.FACTUREs.Max(u => u.factureNumero) : 0;
                    newFacture.factureNumero = maxRef + 1;
                    db.FACTUREs.Add(newFacture);

                    int res = db.SaveChanges();
                    if (res > 0)
                    {
                        foreach (var commande in commandesFacturer)
                        {                            
                            var status = db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("Facturé"));
                            commande.STATUT_COMMANDE = status;
                            db.SaveChanges();
                        }
                        return RedirectToAction("ListFacture");
                    }
                   
                }
                else
                {
                    ViewBag.ErrorMessage = true;
                    FACTUREViewModel factureVm = SetFactureViewModel();
                    return View("Create",factureVm);
                }
                    
                           
                   
            }

            return View("ErrorException");
        }

      
        /// <summary>
        /// Charge une liste des factures à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id de facture</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelecteAllFactureToDelete(string hash)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ListFactureToDelete"] = hash;
                if (Session["ListFactureToDelete"] != null)
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
        public ActionResult DeleteAllFactureSelected()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);

            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(userSession);
            try
            {
                bool unique = true;
                if (Session["ListFactureToDelete"] != null)
                {
                    string hash = Session["ListFactureToDelete"].ToString();
                    List<Guid> listIdFacture = new List<Guid>();
                    if (!string.IsNullOrEmpty(hash))
                    {
                        if (!hash.Contains(','))
                        {
                            listIdFacture.Add(Guid.Parse(hash));
                        }
                        else
                        {
                            foreach (var id in (hash).Split(','))
                            {
                                listIdFacture.Add(Guid.Parse(id));
                            }
                            unique = false;
                        }
                    }
                    if (listIdFacture.Count != 0)
                    {
                        redactapplicationEntities db = new Models.redactapplicationEntities();
                        foreach (var factureId in listIdFacture)
                        {

                            //suppression des relations
                            //var commandes = db.COMMANDEs.Where(x => x.factureId == factureId);
                            //foreach (var cmde in commandes)
                            //{
                            //    cmde.factureId = null;
                            //    db.SaveChanges();
                            //}
                            //suppression des factures
                            FACTURE facture = db.FACTUREs.SingleOrDefault(x => x.factureId == factureId);
                            db.FACTUREs.Remove(facture);
                        }
                        db.SaveChanges();

                        if (unique)
                        {
                            return View("DeletedFactureConfirmation");
                        }
                        return View("DeletedAllFactureConfirmation");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return View("ListFacture");
        }

        /// <summary>
        /// Retourne la vue contenant la recherche d'Utilisateur.
        /// </summary>
        /// <param name="searchValue">Chaine de recherche</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FactureSearch(string searchValue)
        {
            if (searchValue != null && searchValue != "")
            {
                Session["Infosearch"] = searchValue;
            }
            else
            {
                return View("ListFacture");
            }

            redactapplicationEntities bds = new Models.redactapplicationEntities();
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
          
            Factures db = new Factures();
            var answer = db.SearchFacture(searchValue);
            if (answer == null || answer.Count == 0)
            {
                List<FACTUREViewModel> listeFacture = new List<FACTUREViewModel>();
                answer = listeFacture;
                ViewBag.SearchUserNoResultat = 1;
            }

            ViewBag.Search = true;
            redactapplicationEntities e = new redactapplicationEntities();
          
            List<FACTUREViewModel> listeDataFactureFiltered = new List<FACTUREViewModel>();

            ViewBag.listeFactureVm = answer;

            return View("ListFacture");
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
