using RedactApplication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace RedactApplication.Controllers
{

    public class TemplateController : Controller
    {

        private redactapplicationEntities db = new redactapplicationEntities();
        public static Guid _userId;
        // GET: Template
        public ActionResult Index()
        {
            var modeleId = Session["modeleId"];
            MODELEViewModel modelVm = new MODELEViewModel();
            modelVm = new Modeles().GetDetailsModele(Guid.Parse(modeleId.ToString()));

            return View(modelVm);
        }

        public ActionResult ListTemplate()
        {
            ViewBag.listeTemplateVm = new Templates().GetListTemplate();

            return View();

        }

        public ActionResult ChoiceTemplate()
        {
            return View();

        }
        

        public ActionResult Theme1(FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["color-select"]))
            {
                string color_select = collection["color-select"];
                Session["Color"] = get_css_link(color_select.Trim());
            }             
            Session["TemplateName"] = "Theme1";

            return View();
        }

        private string get_css_link(string color)
        {
            switch(color){
                case "dark":
                    return "css/dark-theme.css";
                case "light":
                    return "css/theme2.css";
                default:
                    return "css/templates-style.css";

            }
        }

        private void DeleteFiles(string path)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);
            if (Directory.Exists(path))
            {
                
                foreach (FileInfo tmp in di.EnumerateFiles())
                {
                    tmp.Delete();
                }
            }
        }


        private string SavePhoto(HttpPostedFileBase file,string templateName)
        {
            string path = Server.MapPath("~/Themes/"+ templateName + "/img/");
          
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
           
            if (file != null)
            {
                string fileName = Path.GetFileName(file.FileName.ToLower());

                file.SaveAs(path + removeDiacritics(fileName));
                //return "/Themes/" + templateName + "/img/" + fileName;
                return "img/" + removeDiacritics(fileName);
            }
            return "";
        }

        private string AltPhoto(HttpPostedFileBase file, string templateName)
        {
            string path = Server.MapPath("~/Themes/" + templateName + "/img/");           
            if (file != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                return fileName.Replace("-", " ");               
            }
            return "";
        }

        private string SaveFavicon(HttpPostedFileBase file, string templateName)
        {
            string path = Server.MapPath("~/Themes/" + templateName + "/img/");
            
            if (file != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName.ToLower()) + "-favicon";

                WebImage img = new WebImage(file.InputStream);
                if (img.Width > 17)
                    img.Resize(16, 16);

                img.Save(path+ removeDiacritics(fileName) + Path.GetExtension(file.FileName));

                return "img/" + removeDiacritics(fileName) + Path.GetExtension(file.FileName);
            }
            return "";
        }

        public static String removeDiacritics(string str)
        {
            if (null == str) return null;
            var chars = str
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();
            var res = new string(chars).Normalize(NormalizationForm.FormC);
            return res.Replace(" ", "-");
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult SaveTheme(MODELEViewModel  model, HttpPostedFileBase logoUrl, 
            HttpPostedFileBase menu1_paragraphe1_photoUrl, HttpPostedFileBase menu1_paragraphe2_photoUrl,
            HttpPostedFileBase menu2_paragraphe1_photoUrl, HttpPostedFileBase menu2_paragraphe2_photoUrl,
            HttpPostedFileBase menu3_paragraphe1_photoUrl, HttpPostedFileBase menu3_paragraphe2_photoUrl,
           HttpPostedFileBase menu4_paragraphe1_photoUrl, HttpPostedFileBase menu4_paragraphe2_photoUrl,
           HttpPostedFileBase photoALaUneUrl, HttpPostedFileBase favicone, FormCollection collection)
        {
            var templateName = Session["TemplateName"].ToString();
            if (!string.IsNullOrEmpty(collection["nbmenu"]))
            {
                string nbmenu = collection["nbmenu"];
                Session["nbmenu"]  = nbmenu ;
            }         

            string path = Server.MapPath("~/Themes/"+ templateName );
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else DeleteFiles(path);
            string pathimg = Server.MapPath("~/Themes/" + templateName + "/img/");
            DeleteFiles(pathimg);

            MODELE newmodel = new MODELE();
            newmodel.logoUrl = SavePhoto(logoUrl, templateName);
            newmodel.favicone = SaveFavicon(favicone, templateName);

            /*Menu 1 */
            newmodel.menu1_titre = model.menu1_titre;

            newmodel.menu1_paragraphe1_titre = model.menu1_paragraphe1_titre;
            newmodel.menu1_paragraphe1_photoUrl = SavePhoto(menu1_paragraphe1_photoUrl, templateName);
            newmodel.menu1_p1_alt = AltPhoto(menu1_paragraphe1_photoUrl, templateName);
            newmodel.menu1_contenu1 = model.menu1_contenu1;

            newmodel.menu1_paragraphe2_titre = model.menu1_paragraphe2_titre;
            newmodel.menu1_paragraphe2_photoUrl = SavePhoto(menu1_paragraphe2_photoUrl, templateName);
            newmodel.menu1_p2_alt = AltPhoto(menu1_paragraphe2_photoUrl, templateName);
            newmodel.menu1_contenu2 = model.menu1_contenu2;
            newmodel.menu1_meta_description = model.menu1_meta_description;

            /*Menu 2 */
            newmodel.menu2_titre = model.menu2_titre;                     
            
            newmodel.menu2_paragraphe1_titre = model.menu2_paragraphe1_titre;
            newmodel.menu2_paragraphe2_titre = model.menu2_paragraphe2_titre;
            newmodel.menu2_paragraphe1_photoUrl = SavePhoto(menu2_paragraphe1_photoUrl, templateName);
            newmodel.menu2_p1_alt = AltPhoto(menu2_paragraphe1_photoUrl, templateName);
            newmodel.menu2_paragraphe2_photoUrl = SavePhoto(menu2_paragraphe2_photoUrl, templateName);
            newmodel.menu2_p2_alt = AltPhoto(menu2_paragraphe2_photoUrl, templateName);
            newmodel.menu2_contenu1 = model.menu2_contenu1;
            newmodel.menu2_contenu2 = model.menu2_contenu2;
            newmodel.menu2_meta_description = model.menu2_meta_description;

            /*Menu 3 */
            newmodel.menu3_titre = model.menu3_titre;

            newmodel.menu3_paragraphe1_titre = model.menu3_paragraphe1_titre;
            newmodel.menu3_paragraphe2_titre = model.menu3_paragraphe2_titre;
            newmodel.menu3_paragraphe1_photoUrl = SavePhoto(menu3_paragraphe1_photoUrl, templateName);
            newmodel.menu3_p1_alt = AltPhoto(menu3_paragraphe1_photoUrl, templateName);
            newmodel.menu3_paragraphe2_photoUrl = SavePhoto(menu3_paragraphe2_photoUrl, templateName);
            newmodel.menu3_p2_alt = AltPhoto(menu3_paragraphe2_photoUrl, templateName);
            newmodel.menu3_contenu1 = model.menu3_contenu1;
            newmodel.menu3_contenu2 = model.menu3_contenu2;
            newmodel.menu3_meta_description = model.menu3_meta_description;

            /*Menu 4 */

            newmodel.menu4_titre = model.menu4_titre;
                        
            newmodel.menu4_paragraphe1_titre = model.menu4_paragraphe1_titre;
            newmodel.menu4_paragraphe2_titre = model.menu4_paragraphe2_titre;
            newmodel.menu4_paragraphe1_photoUrl = SavePhoto(menu4_paragraphe1_photoUrl, templateName);
            newmodel.menu4_p1_alt = AltPhoto(menu4_paragraphe1_photoUrl, templateName);
            newmodel.menu4_paragraphe2_photoUrl = SavePhoto(menu4_paragraphe2_photoUrl, templateName);
            newmodel.menu4_p2_alt = AltPhoto(menu4_paragraphe2_photoUrl, templateName);
            newmodel.menu4_contenu1 = model.menu4_contenu1;
            newmodel.menu4_contenu2 = model.menu4_contenu2;
            newmodel.menu4_meta_description = model.menu4_meta_description;

            /*A la une*/
            newmodel.photoALaUneUrl = SavePhoto(photoALaUneUrl, templateName);
            newmodel.site_url = model.site_url;

            newmodel.modeleId = Guid.NewGuid();
            Session["modeleId"] = newmodel.modeleId;

            db.MODELEs.Add(newmodel);
            try
            {
                int res = db.SaveChanges();
                if (res > 0)
                {
                    Templates val = new Templates();
                    TEMPLATEViewModel templateVm = new TEMPLATEViewModel();
                    templateVm.ListProjet = val.GetListProjetItem();
                    templateVm.ListTheme = val.GetListThemeItem();

                    return View("CreateTemplate", templateVm);
                }
                  
                else
                    return View("ErrorException");
            }
            catch(Exception ex)
            {
                return View("Theme1");
            }              
        }


        public string RenderViewAsString(string viewName, object model)
        {
            Response.Headers["Content-Type"] = "charset=utf-8";
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
            viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
            // return the HTML code
            return stringWriter.GetStringBuilder().ToString();
        }


        public ActionResult CreateTemplate(Guid? hash)
        {
            Templates val = new Templates();
            TEMPLATEViewModel templateVm = SetTemplateVM();
            //templateVm.ListProjet = val.GetListProjetItem();
            //templateVm.ListTheme = val.GetListThemeItem();
            //var modeleId = Session["modeleId"];
            //MODELEViewModel modelVm = new MODELEViewModel();
            //modelVm = new Modeles().GetDetailsModele(Guid.Parse(modeleId.ToString()));
            //templateVm.url = modelVm.site_url;
        
            return View(templateVm);
        }

        private TEMPLATEViewModel SetTemplateVM()
        {
            Templates val = new Templates();
            TEMPLATEViewModel templateVm = new TEMPLATEViewModel();
            templateVm.ListProjet = val.GetListProjetItem();
            templateVm.ListTheme = val.GetListThemeItem();
            var modeleId = Session["modeleId"];
            MODELEViewModel modelVm = new MODELEViewModel();
            modelVm = new Modeles().GetDetailsModele(Guid.Parse(modeleId.ToString()));
            templateVm.url = modelVm.site_url;
            return templateVm;
        }

        [HttpPost]
        public ActionResult CreateProjet(TEMPLATEViewModel model)
        {
            PROJET projet = new PROJET { projetId = Guid.NewGuid(), projet_name = model.PROJET.projet_name };
            db.PROJETS.Add(projet);
            db.SaveChanges();

            TEMPLATEViewModel templateVm = SetTemplateVM();
            return View("CreateTemplate", templateVm);

        }

        [Authorize]
        [HttpGet]
        public JsonResult AutocompleteThemeSuggestions(string term)
        {
            var suggestions = new Templates().GetListThemeItem(term);
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Home()
        {
            var modeleId = Session["modeleId"];
            MODELEViewModel modelVm = new MODELEViewModel();
            modelVm = new Modeles().GetDetailsModele(Guid.Parse(modeleId.ToString()));

            return View(modelVm);
        }


        public ActionResult Theme2(FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["color-select"]))
            {
                string color_select = collection["color-select"];
                Session["Color"] = get_css_link(color_select.Trim());
            }
            Session["TemplateName"] = "Theme2";
            return View();
        }


        public ActionResult SaveTemplate(TEMPLATEViewModel model, FormCollection collection)
        {
            int res = 0;       
            
            var selectedProjetId = model.listprojetId;
            var selectedThemeId = model.listThemeId;
            string ftpdir = "";

            if (!string.IsNullOrEmpty(collection["ftpdirs"]))
            {
                 ftpdir = collection["ftpdirs"];
               
            }

            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                _userId = Guid.Parse(HttpContext.User.Identity.Name);
            }

            MODELEViewModel modelVm = new MODELEViewModel();
            modelVm = new Modeles().GetDetailsModele((Guid)Session["modeleId"]);
            Session["Menu"] = "1";
            var html = RenderViewAsString("Home", modelVm);        

            TEMPLATE newtemplate = new TEMPLATE();
            newtemplate.dateCreation = DateTime.Now;
            newtemplate.url = modelVm.site_url;
            newtemplate.ftpUser = model.ftpUser;
            newtemplate.ftpPassword = model.ftpPassword;
            newtemplate.modeleId = (Guid)Session["modeleId"];
            newtemplate.ip = model.ip;
            newtemplate.userId = _userId;
            newtemplate.PROJET = db.PROJETS.Find(selectedProjetId);
            newtemplate.projetId = selectedProjetId;
            var selectedTheme = model.THEME.theme_name;
            THEME currentTheme = db.THEMES.FirstOrDefault(x => x.theme_name.Contains(selectedTheme.TrimEnd()));
            if (currentTheme == null)
            {
                currentTheme = new THEME { themeId = Guid.NewGuid(), theme_name = selectedTheme };
                db.THEMES.Add(currentTheme);
                db.SaveChanges();
            }
            
            newtemplate.THEME = currentTheme;
            newtemplate.themeId = currentTheme.themeId;

            newtemplate.html = html;

           
            var results = 
            newtemplate.templateId = Guid.NewGuid();

            db.TEMPLATEs.Add(newtemplate);
            try
            {
                res = db.SaveChanges();
                if (res > 0)
                {
                    var templateName = Session["TemplateName"].ToString();

                    int nb_menu = (Session["nbmenu"] == null) ? 1 : int.Parse(Session["nbmenu"].ToString());                   

                    CreateFiles(nb_menu, html);

                    //Send Ftp
                    string pathParent = Server.MapPath("~/Themes/" + templateName);
                    string pathCss = pathParent + "/css/";
                    string pathImg = pathParent + "/img";
                    string pathJs = pathParent + "/js";
                    int result = SendToFtp(ftpdir,model.url, model.ftpUser, model.ftpPassword, pathCss, pathParent, pathImg,pathJs);

                    //return new FilePathResult(path, "text/html");
                    if (result == 0)
                        return View("CreateTemplateConfirmation");
                    else
                        return View("ErrorException");
                }
                else
                    return View("ErrorException");
            }
            catch (Exception ex)
            {
                return View("ErrorException");
            
            }
        }

        /// <summary>
        /// Charge une liste d'utilisateur à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id d'utilisateur</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelecteAllTemplateToDelete(string hash)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ListTemplateToDelete"] = hash;
                if (Session["ListTemplateToDelete"] != null)
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
        /// Supprime une liste Template dans la base de données.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeleteAllTemplateSelected()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);


            try
            {
                bool unique = true;
                if (Session["ListTemplateToDelete"] != null)
                {
                    string hash = Session["ListTemplateToDelete"].ToString();
                    List<Guid> listIdTemplate = new List<Guid>();
                    if (!string.IsNullOrEmpty(hash))
                    {
                        if (!hash.Contains(','))
                        {
                            listIdTemplate.Add(Guid.Parse(hash));
                        }
                        else
                        {
                            foreach (var id in (hash).Split(','))
                            {
                                listIdTemplate.Add(Guid.Parse(id));
                            }
                            unique = false;
                        }
                    }
                    if (listIdTemplate.Count != 0)
                    {
                        redactapplicationEntities db = new Models.redactapplicationEntities();
                        foreach (var templateId in listIdTemplate)
                        {
                            //suppression des commandes
                            TEMPLATE template = db.TEMPLATEs.SingleOrDefault(x => x.templateId == templateId);
                            if (template != null) db.TEMPLATEs.Remove(template);
                        }
                        db.SaveChanges();

                        return View(unique ? "DeleteTemplateConfirmation" : "DeteleAllTemplateConfirmation");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                { "controller", "Template" },
                { "action", "ListTemplate" }
            });
        }

        public static string ExtractDomainName(string Url)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                Url,
                @"^([a-zA-Z]+:\/\/)?([^\/]+)\/.*?$",
                "$2"
            );
        }

        private string GetMenuTitle(int menu, MODELEViewModel modelVm)
        {
            var result = modelVm.menu1_titre;

            switch (menu)
            {
                case 2:
                    result = modelVm.menu2_titre;
                    break;
                case 3:
                    result = modelVm.menu3_titre;
                    break;
                case 4:
                    result = modelVm.menu4_titre;
                    break;
                default:
                    result = modelVm.menu1_titre;
                    break;
            };

            return result;
        }


        private string GetMetatitle(int menu,MODELEViewModel modelVm)
        {
            var result = modelVm.menu1_paragraphe1_titre;

            switch (menu)
            {
                case 2:
                    result = modelVm.menu2_paragraphe1_titre;
                    break;
                case 3:
                    result = modelVm.menu3_paragraphe1_titre;
                    break;
                case 4:
                    result = modelVm.menu4_paragraphe1_titre;
                    break;
                default:
                    result =  modelVm.menu1_paragraphe1_titre;
                    break;
            };

            return result;
        }

        private string GetMenuClass(int menu, MODELEViewModel modelVm)
        {
            var result = "current";

            switch (menu)
            {
                case 2:
                    result = "current";
                    break;
                case 3:
                    result = "current";
                    break;
                case 4:
                    result = "current";
                    break;
                default:
                    result = "current";
                    break;
            };

            return result;
        }

        private string GetMetaDescription(int menu, MODELEViewModel modelVm)
        {
            var result = modelVm.menu1_meta_description;

            switch (menu)
            {
                case 2:
                    result = modelVm.menu2_meta_description;
                    break;
                case 3:
                    result = modelVm.menu3_meta_description;
                    break;
                case 4:
                    result = modelVm.menu4_meta_description;
                    break;
                default:
                    result = modelVm.menu1_meta_description;
                    break;
            };

            return result;
        }

      

        private void CreateFiles(int nb_menu, string menu1_html)
        {
            var templateName = Session["TemplateName"].ToString();
            string pathHtml = "";
            MODELEViewModel modelVm = new MODELEViewModel();
            modelVm = new Modeles().GetDetailsModele((Guid)Session["modeleId"]);
            var html = menu1_html;

            for (int i = 1; i <= nb_menu; i++)
            {
                pathHtml = "~/Themes/" + templateName;

                if (i == 1)
                    pathHtml = pathHtml + "/index.html";
                else
                {
                    string title = GetMenuTitle(i, modelVm);
                    pathHtml = pathHtml + "/" + removeDiacritics(title.ToLower()).Replace("_", "-") + ".html";
                }

                Session["Menu"] = i;

                modelVm.meta_title = GetMetatitle(i, modelVm);
                modelVm.meta_description = GetMetaDescription(i, modelVm);
                modelVm.menu1_link_class = (i == 1) ? "current" : "";
                modelVm.menu2_link_class = (i == 2) ? "current" : "";
                modelVm.menu3_link_class = (i == 3) ? "current" : "";
                modelVm.menu4_link_class = (i == 4) ? "current" : "";

                modelVm.menu1_link = removeDiacritics(modelVm.menu1_titre.ToLower());
                modelVm.menu2_link = !string.IsNullOrEmpty(modelVm.menu2_titre) ? removeDiacritics(modelVm.menu2_titre.ToLower()) : "";
                modelVm.menu3_link = !string.IsNullOrEmpty(modelVm.menu3_titre) ? removeDiacritics(modelVm.menu3_titre.ToLower()) :"";
                modelVm.menu4_link = !string.IsNullOrEmpty(modelVm.menu4_titre) ? removeDiacritics(modelVm.menu4_titre.ToLower()) : "";

                html = RenderViewAsString("Home", modelVm);

                if (!System.IO.File.Exists(Server.MapPath(pathHtml)))
                {
                    FileInfo info = new FileInfo(Server.MapPath(pathHtml));
                }

                using (StreamWriter w = new StreamWriter(Server.MapPath(pathHtml), false))
                {
                    w.WriteLine(html); // Write the text
                    w.Close();
                }
            }
        }

        private int SendToFtp(string ftpDir, string ftpurl,string username,string password,string pathCss,string pathHtml,string pathImg,string pathJs)
        {
            int res = 0;
            try
            {
                var url = ftpurl ;
                if (!string.IsNullOrEmpty(ftpDir))
                {
                    url = ftpurl + "/" + ftpDir;

                }
                /*Create directory*/
                FTP ftpClient = new FTP(@"ftp://" + url + "/", username, password);
                /* Create a New Directory */
                ftpClient.createDirectory("/css");
                ftpClient.createDirectory("/img");
               

                ftpClient = null;

                string[] htmlPaths = Directory.GetFiles(pathHtml, "*.html");    
                
                var imgPaths = Directory.EnumerateFiles(pathImg, "*.*", SearchOption.AllDirectories)
               .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".svg") || s.EndsWith(".gif") || s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".tiff") || s.EndsWith(".tif"));



                var pathCssfiles = Directory.EnumerateFiles(pathCss, "*.*", SearchOption.AllDirectories);
             
              

                using (WebClient client = new WebClient())
                {
                
                    foreach (var html in htmlPaths)
                    {
                        client.Credentials = new NetworkCredential(username, password);
                        client.UploadFile(
                            "ftp://" + url + "/" + Path.GetFileName(html), html);
                    }

                    foreach (var img in imgPaths)
                    {
                        client.Credentials = new NetworkCredential(username, password);
                        client.UploadFile(
                            "ftp://" + url + "/img/" + Path.GetFileName(img), img);
                    }


                    foreach (var pcss in pathCssfiles)
                    {
                        client.Credentials = new NetworkCredential(username, password);
                        client.UploadFile(
                               "ftp://" + url + "/css/" + Path.GetFileName(pcss), pcss);
                    }



                    if (Session["TemplateName"] != null && Session["TemplateName"].ToString() == "Theme3")
                    {
                        ftpClient = new FTP(@"ftp://" + url + "/", username, password);
                        ftpClient.createDirectory("/js");
                        ftpClient = null;
                        var pathJsfiles = Directory.EnumerateFiles(pathJs, "*.*", SearchOption.AllDirectories)
                          .Where(s => s.EndsWith(".js"));
                        foreach (var pJs in pathJsfiles)
                        {
                            client.Credentials = new NetworkCredential(username, password);
                            client.UploadFile(
                                   "ftp://" + url + "/js/" + Path.GetFileName(pJs), pJs);
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                res = 1;
            }
            return res;
        }

        public int SaveTheme2(MODELEViewModel model, IEnumerable<HttpPostedFileBase> files)
        {
            int res = 0;
               var path = "";
            foreach (var file in files){
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    file.SaveAs(path);
                }
            }
            MODELE newmodel = new  MODELE();
            newmodel.logoUrl = path;
            newmodel.menu1_titre = model.menu1_titre;
            newmodel.menu2_titre = model.menu2_titre;
            newmodel.menu3_titre = model.menu3_titre;
            newmodel.menu4_titre = model.menu4_titre;
            newmodel.menu1_paragraphe1_titre = model.menu1_paragraphe1_titre;
            newmodel.menu1_paragraphe2_titre = model.menu1_paragraphe2_titre;
            newmodel.menu1_paragraphe1_photoUrl = path;
            newmodel.menu1_contenu1 = model.menu1_contenu1;
            newmodel.menu2_paragraphe1_titre = model.menu2_paragraphe1_titre;
            newmodel.menu2_paragraphe2_titre = model.menu2_paragraphe2_titre;
            newmodel.menu2_paragraphe1_photoUrl = path;
            newmodel.menu2_contenu2 = model.menu2_contenu1;
            newmodel.menu3_paragraphe1_titre = model.menu3_paragraphe1_titre;
            newmodel.menu3_paragraphe2_titre = model.menu3_paragraphe2_titre;
            newmodel.menu3_paragraphe1_photoUrl = path;
            newmodel.menu3_contenu1 = model.menu3_contenu1;
            newmodel.menu4_paragraphe1_titre = model.menu4_paragraphe1_titre;
            newmodel.menu4_paragraphe2_titre = model.menu4_paragraphe2_titre;
            newmodel.menu4_paragraphe1_photoUrl = path;
            newmodel.menu4_contenu1 = model.menu4_contenu1;

            db.MODELEs.Add(newmodel);
            try
            {
                res = db.SaveChanges();
               
            }
            catch (Exception ex)
            {
                return res;
            }

            return res;

        }

        [HttpPost]
        public string RefreshFtp(string url, string ftpUser,string ftpPassword)
        {

            try
            {
              
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + url);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(ftpUser, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                var list = names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return string.Join(",", list.ToArray());
            }
            catch (Exception e)
            {
                return e.ToString();
            }

           
        }


        public ActionResult Theme3(FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["color-select"]))
            {
                string color_select = collection["color-select"];
                Session["Color"] = get_css_link(color_select.Trim());
            }
            Session["TemplateName"] = "Theme3";
            return View();
        }

     

        public ActionResult Theme4(FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["color-select"]))
            {
                string color_select = collection["color-select"];
                Session["Color"] = get_css_link(color_select.Trim());
            }
            Session["TemplateName"] = "Theme4";
            return View();
        }

        public ActionResult Theme5(FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["color-select"]))
            {
                string color_select = collection["color-select"];
                Session["Color"] = get_css_link(color_select.Trim());
            }
            Session["TemplateName"] = "Theme5";
            return View();
        }

        public ActionResult Theme6(FormCollection collection)
        {
            if (!string.IsNullOrEmpty(collection["color-select"]))
            {
                string color_select = collection["color-select"];
                Session["Color"] = get_css_link(color_select.Trim());
            }
            Session["TemplateName"] = "Theme6";
            return View();
        }
    }
}