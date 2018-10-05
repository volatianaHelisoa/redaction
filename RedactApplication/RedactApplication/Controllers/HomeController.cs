
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RedactApplication.Models;
using Microsoft.Security.Application;
using System.Collections;
using System.Configuration;

namespace RedactApplication.Controllers
{
    /// <summary>
    /// Class implémentant le Controlleur de la section User.
    /// </summary>
    [HandleError]
    [MvcApplication.AuthorizeRole]
    public class HomeController : Controller
    {
        /// <summary>
        /// Représente l'identifiant de l'utilisateur courant.
        /// </summary>
        public static Guid _userId;

       
        /// <summary>
        /// Retourne la vue d'Index de la page Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [MvcApplication.CheckSessionOut]
        public ActionResult Index()
        {
           
            return View();
        }

        private void UserDashboard()
        {
            Utilisateurs val = new Utilisateurs();
            var listeDataUser = val.GetListUtilisateur();
            var managerCount = listeDataUser.Count(x => x.userRole == "3");
            ViewBag.managerCount = managerCount;

            var referenceurCount = listeDataUser.Count(x => x.userRole == "1");
            ViewBag.referenceurCount = referenceurCount;

            var redacteurCount = listeDataUser.Count(x => x.userRole == "2");
            ViewBag.redacteurCount = redacteurCount;

        }

        private void CommandeDashboard()
        {
            redactapplicationEntities db = new redactapplicationEntities();
            Utilisateurs val = new Utilisateurs();
            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);

            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            

            var user = db.UTILISATEURs.Find(userId);
            var now = DateTime.Now;
            var startDate = ConfigurationManager.AppSettings["startDate"].ToString();
            var startOfMonth = new DateTime(now.Year, now.Month - 1, int.Parse(startDate));
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var lastDay = new DateTime(now.Year, now.Month, daysInMonth);
            var role = val.GetUtilisateurRoleToString(userId);
           
            var commandes = db.COMMANDEs.Where(x => x.date_livraison >= startOfMonth &&
                                x.date_livraison <= lastDay).ToList();


            if (user != null)
            {
                if (role == "1" ) //referenceur ou manager
                    commandes = commandes.Where(x => x.commandeReferenceurId == userId).ToList();

                if(role =="2") //redacteur
                    commandes = commandes.Where(x => x.commandeRedacteurId == userId).ToList();

                /* Livraisons reçues */
                //var commandesLivrer = commandes.Count(x => x.date_cmde >= startOfMonth &&
                //                                                x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                //                                                                           x.STATUT_COMMANDE.statut_cmde.Contains("Livré")));
                var commandesLivrer = commandes.Count(x => x.STATUT_COMMANDE != null && x.STATUT_COMMANDE.statut_cmde.Contains("Livré"));
                ViewBag.commandesLivrer = commandesLivrer;

                /* Livraisons En Retard */
                //var commandesEnRetard = commandes.Count(x =>  x.date_livraison <= now &&
                //                                               (x.STATUT_COMMANDE != null &&
                //                                                x.STATUT_COMMANDE.statut_cmde.Contains("En cours")));
                var commandesEnRetard = commandes.Count(x => x.date_livraison <= now);
                ViewBag.commandesEnRetard = commandesEnRetard;

                /* Commande refusée */
                //var commandesRefuser = commandes.Count(x => x.date_cmde >= startOfMonth &&
                //                                                x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                //                                                 x.STATUT_COMMANDE.statut_cmde.Contains("Refusé")));

                var commandesRefuser = commandes.Count(x => x.STATUT_COMMANDE != null &&
                                                              x.STATUT_COMMANDE.statut_cmde.Contains("Refusé"));
                ViewBag.commandesRefuser = commandesRefuser;

                /*Commander en attente*/
                //var commandesEnAttente = commandes.Where(x => x.date_cmde >= startOfMonth &&
                //                             x.date_cmde <= lastDay &&
                //                                                 (x.STATUT_COMMANDE != null && x.STATUT_COMMANDE.statut_cmde.Contains("En attente"))).Distinct().ToList().Count;

                var commandesEnAttente = commandes.Where(x => x.STATUT_COMMANDE != null &&
                                                              x.STATUT_COMMANDE.statut_cmde.Contains("En attente")).Distinct().ToList().Count;
                ViewBag.commandesEnAttente = commandesEnAttente;

                /* Redaction en cours */
                //var commandesEnCours = commandes.Count(x => x.date_cmde >= startOfMonth &&
                //               x.date_cmde <= lastDay &&
                //              (x.STATUT_COMMANDE != null && x.STATUT_COMMANDE.statut_cmde.Contains("En cours"))

                var commandesEnCours = commandes.Count(x => x.STATUT_COMMANDE != null && x.STATUT_COMMANDE.statut_cmde.Contains("En cours"));
                ViewBag.commandesEnCours = commandesEnCours;

                /* Commande annulée */

                //var commandesAnnuler = commandes.Count(x => x.date_cmde >= startOfMonth &&
                //                                                x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                //                                                 x.STATUT_COMMANDE.statut_cmde.Contains("Annulé")));

                var commandesAnnuler = commandes.Count(x => x.STATUT_COMMANDE != null &&
                                                                x.STATUT_COMMANDE.statut_cmde.Contains("Annulé"));


                ViewBag.commandesAnnuler = commandesAnnuler;

                //var commandesAFacturer = commandes.Count(x => x.date_cmde >= startOfMonth &&
                //                                                  x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                //                                                  x.STATUT_COMMANDE.statut_cmde.Contains("Validé")));

                var commandesAFacturer = commandes.Count(x => x.STATUT_COMMANDE != null &&
                                                                  x.STATUT_COMMANDE.statut_cmde.Contains("Validé"));

                ViewBag.commandesAFacturer = commandesAFacturer;

            }
            var factures = db.FACTUREs.Where(x => x.dateDebut >= startOfMonth &&
                             x.dateFin <= lastDay).ToList();

            //if (role == "1" || role == "3")
            //    factures = factures.Where(x => x.createurId == userId).ToList();

            if (role == "2")
                factures = factures.Where(x => x.redacteurId == userId).ToList();

            var montantAFacturer = factures.Sum(x => Convert.ToDouble(x.montant));
            ViewBag.montantAFacturer = String.Format("{0:N0}", montantAFacturer) ;
        }

        public ActionResult Dashboard()
        {

            UserDashboard();
            CommandeDashboard();
            return View();
        }

        [HttpPost]
        public JsonResult ReInitPagination(string hash)
        {
            var val = new { Page = true };
            if (Session["pagination"] == null)
            {
                Session["pagination"] = 1;
            }
            else
            {
                Session["pagination"] = 1;
            }
            return Json(val, JsonRequestBehavior.AllowGet);
        }

        

        /// <summary>
        /// Retourne une liste de réference d'utilisateur.
        /// </summary>
        /// <param name="prefix">chaine à rechercher</param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult IndexUser(string prefix)
        {
            var use = (new SearchData()).UserSearch(prefix);
            try
            {
                var UserIdentity = (from N in use
                                    select new { Value = (N.userNom + " " + N.userPrenom) }).Distinct().ToList();
                return Json(UserIdentity, JsonRequestBehavior.AllowGet);
            }
            catch
            {
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
       

        /// <summary>
        /// Retourne la vue à propos de la page Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        /// <summary>
        /// Retourne la vue d'une liste d'Utilisateur.
        /// </summary>
        /// <param name="role">role</param>

        /// <returns>View</returns>       
        [Authorize]
        public ActionResult ListeUser(string role)
        {
          
            // Exécute le traitement de la pagination
            Utilisateurs val = new Utilisateurs();
           
            // Récupère la liste des utilisateurs
            var listeDataUser = val.GetListUtilisateur();
            if (role != null)
            {
                listeDataUser = listeDataUser.Where(x => x.userRole == role.ToString()).ToList();
            }
                

            ViewBag.listeUserVm = listeDataUser.Distinct().ToList();

            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

            if (_userId != Guid.Empty)
            {
                ViewBag.CurrentUser = val.GetUtilisateur(_userId);
                if (val.GetUtilisateur(_userId) != null)
                {
                    UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                    var currentuser = val.GetUtilisateur(_userId);
                    userVm.userNom = currentuser.userNom;
                    userVm.userPrenom = currentuser.userPrenom;
                    userVm.userId = currentuser.userId;

                    ViewBag.userRole = val.GetUtilisateurRoleToString(userVm.userId);
                    userVm.redactSkype = currentuser.redactSkype;
                    userVm.redactModePaiement= currentuser.redactModePaiement;
                    userVm.redactNiveau = currentuser.redactNiveau;
                    userVm.redactPhone = currentuser.redactPhone;
                    userVm.redactReferenceur = currentuser.redactReferenceur;
                    userVm.redactThemes = currentuser.redactThemes;
                    userVm.redactVolume = currentuser.redactVolume;
                    userVm.logoUrl = currentuser.logoUrl;
                   
                    userVm.ListTheme = val.GetListThemeItem();
                    Session["logoUrl"] = currentuser.logoUrl;
                    return View(userVm);
                }

                
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                    { "controller", "Login" },
                    { "action", "Accueil" }
                });
        }

        /// <summary>
        /// Retourne la vue d'une liste d'Utilisateur.
        /// </summary>
        /// <param name="role">role</param>

        /// <returns>View</returns>       
        [Authorize]
        public ActionResult ListeUserTemplate(string role)
        {

            // Exécute le traitement de la pagination
            Utilisateurs val = new Utilisateurs();

            // Récupère la liste des utilisateurs
            var listeDataUser = val.GetListUtilisateur();
            if (role != null)
            {
                listeDataUser = listeDataUser.Where(x => x.userRole == role.ToString()).ToList();
            }


            ViewBag.listeUserVm = listeDataUser.Distinct().ToList();

            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

            if (_userId != Guid.Empty)
            {
                ViewBag.CurrentUser = val.GetUtilisateur(_userId);
                if (val.GetUtilisateur(_userId) != null)
                {
                    UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                    var currentuser = val.GetUtilisateur(_userId);
                    userVm.userNom = currentuser.userNom;
                    userVm.userPrenom = currentuser.userPrenom;
                    userVm.userId = currentuser.userId;

                    ViewBag.userRole = val.GetUtilisateurRoleToString(userVm.userId);
                    userVm.redactSkype = currentuser.redactSkype;
                
                    userVm.logoUrl = currentuser.logoUrl;

                    userVm.ListTheme = val.GetListThemeItem();
                    Session["logoUrl"] = currentuser.logoUrl;
                    return View(userVm);
                }


            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                    { "controller", "Login" },
                    { "action", "Accueil" }
                });
        }

        /// <summary>
        /// Redirige vers la vue d'affichage d'une liste d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>        
        [Authorize]
        public ActionResult gotoListUser()
        {
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        /// <summary>
        /// Gère la vue de création d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult GererUtilisateur()
        {
           
            redactapplicationEntities db = new Models.redactapplicationEntities();
            

            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);

            if (!string.IsNullOrEmpty(_userId.ToString()))
            {

                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == _userId);
                UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                var val = new Utilisateurs();
                if (utilisateur != null)
                {
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.userId = utilisateur.userId;

                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = utilisateur.redactThemes;
                    userVm.redactVolume = utilisateur.redactVolume;
                    userVm.ListTheme = val.GetListThemeItem(utilisateur.userId);
                }

                return View("EditUserConfirmation", userVm);
            }
            return View();
        }

        /// <summary>
        /// Retourne la vue de création d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult CreateUser()
        {

            _userId = Guid.Parse(HttpContext.User.Identity.Name);

            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);

            Guid editUserId;

            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                editUserId = Guid.Parse(Request.QueryString["currentid"]);
            }
            else
            {
                editUserId = Guid.NewGuid();
            }
            var val = new Utilisateurs();
            UTILISATEURViewModel userVm = new UTILISATEURViewModel();
          

            if (_userId != Guid.Empty)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == _userId);
                ViewBag.CurrentUser = utilisateur;


              

                if (utilisateur != null)
                {

                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.userId = editUserId;
                  

                    return View(userVm);
                }
            }
            return RedirectToAction("CreateUser", "Home");
        }

        /// <summary>
        /// Retourne la vue de création d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult GotoCreateUser()
        {

            _userId = Guid.Parse(HttpContext.User.Identity.Name);

            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);

            Guid editUserId;

            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                editUserId = Guid.Parse(Request.QueryString["currentid"]);
            }
            else
            {
                editUserId = Guid.NewGuid();
            }
            var val = new Utilisateurs();
            UTILISATEURViewModel userVm = new UTILISATEURViewModel();
            userVm.ListTheme = val.GetListThemeItem();

            if (_userId != Guid.Empty)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == _userId);
                ViewBag.CurrentUser = utilisateur;


                userVm.ListTheme = val.GetListThemeItem(editUserId).Count() > 1 ? val.GetListThemeItem(editUserId) : val.GetListThemeItem();

                if (utilisateur != null)
                {
                  
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.userId = editUserId;
                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = utilisateur.redactThemes;
                    userVm.redactVolume = utilisateur.redactVolume;
                   

                    return View("GererUtilisateur", userVm);
                }
            }
            return RedirectToAction("GererUtilisateur", "Home");
        }

        /// <summary>
        /// Retourne la vue de confirmation de création d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult CreatedUserConfirmation()
        {
            if (StatePageSingleton.nullInstance())
            {
                StatePageSingleton.getInstance(1, 10);
                ViewBag.numpage = 1;
                ViewBag.nbrow = 10;
            }
            else
            {
                ViewBag.numpage = StatePageSingleton.getInstance().Numpage;
                ViewBag.nbrow = StatePageSingleton.getInstance().Nbrow;
            }

            UTILISATEURViewModel currentUser = GetCurrentUserModel();
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);
            return View(currentUser);
        }

        private void SaveRedactThemes(Guid[] themes,Guid redactGuid, List<REDACT_THEME> listeUserThemes = null)
        {
            var listids = new List<Guid>();
            var val = new Utilisateurs();
            if (themes.ToList().Count > 0)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                if (listeUserThemes != null)
                {
                    foreach (var redactTheme in listeUserThemes)
                    {
                        redactTheme.themeId = null;
                        redactTheme.redactId = null;
                        db.SaveChanges();
                      
                    }
                    
                }
            
        
                foreach (var theme in themes)
                {
                    var _theme = db.THEMES.Find(theme);
                    if (_theme != null)
                    {
                        
                        REDACT_THEME redactTheme = new REDACT_THEME {redactThemeId = Guid.NewGuid() , themeId = _theme.themeId, redactId = redactGuid };                       
                        db.REDACT_THEME.Add(redactTheme);                       
                    }
                }
               
              db.SaveChanges();

            }
            
        }

        /// <summary>
        /// Retourne la vue d'édition d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <param name="error">message d'erreur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult EditUserTemplate(Guid? hash, string error)
        {
            switch (error)
            {
                case "ErrorMessage":
                    ViewBag.ErrorMessage = "role null";
                    break;
                case "ErrorMail":
                    ViewBag.ErrorMail = "mail invalid";
                    break;
                case "ErrorRole":
                    ViewBag.ErrorRole = "role null";
                    break;
                case "ErrorUserMailValidation":
                    ViewBag.ErrorUserValidation = "mail is not valid";
                    break;
                case "ErrorPhoneValidation":
                    ViewBag.ErrorPhoneValidation = "phone is not valid";
                    break;
            }

            Guid userId = (Guid)hash;
            ViewBag.currentid = hash;

            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRoleEdit = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            var val = new Utilisateurs();

            if (userId != Guid.Empty)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == userId);

                UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                if (utilisateur != null)
                {
                    userVm.userId = utilisateur.userId;
                    userVm.userMail = utilisateur.userMail;
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = val.RedactThemes(utilisateur.userId);
                    userVm.redactVolume = utilisateur.redactVolume;
                    userVm.redactTarif = utilisateur.redactTarif;
                    userVm.logoUrl = utilisateur.logoUrl;


                    if (Session["userEditModif"] != null)
                    {
                        UTILISATEURViewModel model = (UTILISATEURViewModel)Session["userEditModif"];
                        userVm.userMail = model.userMail;
                        userVm.userNom = model.userNom;
                        userVm.userPrenom = model.userPrenom;
                        userVm.redactSkype = model.redactSkype;
                        userVm.redactModePaiement = model.redactModePaiement;
                        userVm.redactNiveau = model.redactNiveau;
                        userVm.redactPhone = model.redactPhone;
                        userVm.redactReferenceur = model.redactReferenceur;
                        userVm.redactThemes = model.redactThemes;

                        userVm.redactVolume = model.redactVolume;
                        userVm.redactTarif = model.redactTarif;
                        Session["userEditModif"] = null;
                    }

                    userVm.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(utilisateur.userId);
                    List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == utilisateur.userId).ToList();
                    if (listeUserRole.Any())
                    {
                        switch (listeUserRole[0].idRole)
                        {
                            case 1:
                                ViewBag.userRole = 1; /*referenceur manager*/
                                break;
                            case 2:
                                ViewBag.userRole = 2; /*redacteur manager*/
                                break;
                            case 3:
                                ViewBag.userRole = 3; /*manager utilisateur*/
                                break;
                            case 4:
                                ViewBag.userRole = 4; /*administrateur*/
                                break;
                            case 5:
                                ViewBag.userRole = 5; /*super administrateur*/
                                break;
                            default:
                                ViewBag.userRole = 0;
                                break;
                        }

                        ViewBag.listeUserRole = listeUserRole;
                    }
                    userVm.ListTheme = val.GetListThemeItem();
                    List<REDACT_THEME> listeUserThemes = db.REDACT_THEME.Where(x => x.redactId == utilisateur.userId).ToList();

                    if (listeUserThemes.Count() > 0)
                    {
                        var listeThemes = db.REDACT_THEME.Where(x => x.redactId == utilisateur.userId).Select(x => x.themeId).ToList();
                        userVm.listThemeId = listeThemes;

                        var themes = val.GetListThemeItem(utilisateur.userId);
                        userVm.themeId = listeThemes.ToArray();
                        userVm.themeList = new MultiSelectList(themes, "Value", "Text", listeThemes);
                    }

                }

                return View(userVm);
            }
            return View();
        }

        /// <summary>
        /// Retourne la vue d'édition d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <param name="error">message d'erreur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult EditUser(Guid? hash, string error)
        {
            switch (error)
            {
                case "ErrorMessage":
                    ViewBag.ErrorMessage = "role null";
                    break;
                case "ErrorMail":
                    ViewBag.ErrorMail = "mail invalid";
                    break;
                case "ErrorRole":
                    ViewBag.ErrorRole = "role null";
                    break;
                case "ErrorUserMailValidation":
                    ViewBag.ErrorUserValidation = "mail is not valid";
                    break;
                case "ErrorPhoneValidation":
                    ViewBag.ErrorPhoneValidation = "phone is not valid";
                    break;
            }

            Guid userId = (Guid)hash;
            ViewBag.currentid = hash;

            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRoleEdit = (new Utilisateurs()).GetUtilisateurRoleToString(user);                   

            var val = new Utilisateurs();

            if (userId != Guid.Empty)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == userId);
                
                UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                if (utilisateur != null)
                {
                    userVm.userId = utilisateur.userId;
                    userVm.userMail = utilisateur.userMail;
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = val.RedactThemes(utilisateur.userId);
                    userVm.redactVolume = utilisateur.redactVolume;
                    userVm.redactTarif = utilisateur.redactTarif;
                    userVm.logoUrl =  utilisateur.logoUrl;
                  

                    if (Session["userEditModif"] != null)
                    {
                        UTILISATEURViewModel model = (UTILISATEURViewModel) Session["userEditModif"];
                        userVm.userMail = model.userMail;
                        userVm.userNom = model.userNom;
                        userVm.userPrenom = model.userPrenom;
                        userVm.redactSkype = model.redactSkype;
                        userVm.redactModePaiement = model.redactModePaiement;
                        userVm.redactNiveau = model.redactNiveau;
                        userVm.redactPhone = model.redactPhone;
                        userVm.redactReferenceur = model.redactReferenceur;
                        userVm.redactThemes = model.redactThemes;
                       
                        userVm.redactVolume = model.redactVolume;
                        userVm.redactTarif = model.redactTarif;
                        Session["userEditModif"] = null;
                    }

                    userVm.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(utilisateur.userId);
                    List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == utilisateur.userId).ToList();
                     if (listeUserRole.Any())
                    {
                        switch (listeUserRole[0].idRole)
                        {
                            case 1:
                                ViewBag.userRole = 1; /*referenceur manager*/
                                break;
                            case 2:
                                ViewBag.userRole = 2; /*redacteur manager*/
                                break;
                            case 3:
                                ViewBag.userRole = 3; /*manager utilisateur*/
                                break;
                            case 4:
                                ViewBag.userRole = 4; /*administrateur*/
                                break;
                            case 5:
                                ViewBag.userRole = 5; /*super administrateur*/
                                break;
                            default:
                                ViewBag.userRole = 0;
                                break;
                        }

                        ViewBag.listeUserRole = listeUserRole;
                    }
                    userVm.ListTheme = val.GetListThemeItem();
                    List<REDACT_THEME> listeUserThemes = db.REDACT_THEME.Where(x => x.redactId == utilisateur.userId).ToList();
                 
                    if (listeUserThemes.Count() > 0)
                    {
                        var listeThemes = db.REDACT_THEME.Where(x => x.redactId == utilisateur.userId).Select(x => x.themeId).ToList();
                        userVm.listThemeId = listeThemes;

                        var themes = val.GetListThemeItem(utilisateur.userId);
                        userVm.themeId = listeThemes.ToArray();
                        userVm.themeList = new MultiSelectList(themes, "Value", "Text", listeThemes);
                    }
                  
                }

                return View(userVm);
            }
            return View();
        }


        /// <summary>
        /// Retourne la vue d'édition d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <param name="error">message d'erreur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DetailsRedacteurs(Guid? hash, string error)
        {
            switch (error)
            {
                case "ErrorMessage":
                    ViewBag.ErrorMessage = "role null";
                    break;
                case "ErrorMail":
                    ViewBag.ErrorMail = "mail invalid";
                    break;
                case "ErrorRole":
                    ViewBag.ErrorRole = "role null";
                    break;
                case "ErrorUserMailValidation":
                    ViewBag.ErrorUserValidation = "mail is not valid";
                    break;
            }

            Guid userId = (Guid)hash;
            ViewBag.currentid = hash;

            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRoleEdit = (new Utilisateurs()).GetUtilisateurRoleToString(user);
            var val = new Utilisateurs();
            if (userId != Guid.Empty)
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == userId);
                UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                if (utilisateur != null)
                {
                    userVm.userId = utilisateur.userId;
                    userVm.userMail = utilisateur.userMail;
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    var themes = "";
                    var listThemes = val.GetThemes(utilisateur.userId);
                    if (listThemes.Count > 0)
                    {
                         themes = String.Join(", ", listThemes.ToArray());

                    }
                    userVm.redactThemes = themes;
                    userVm.redactVolume = utilisateur.redactVolume;
                    userVm.redactTarif = utilisateur.redactTarif;
                    userVm.logoUrl = utilisateur.logoUrl;

                    if (Session["userEditModif"] != null)
                    {
                        UTILISATEURViewModel model = (UTILISATEURViewModel)Session["userEditModif"];
                        userVm.userMail = model.userMail;
                        userVm.userNom = model.userNom;
                        userVm.userPrenom = model.userPrenom;
                        userVm.redactSkype = model.redactSkype;
                        userVm.redactModePaiement = model.redactModePaiement;
                        userVm.redactNiveau = model.redactNiveau;
                        userVm.redactPhone = model.redactPhone;
                        userVm.redactReferenceur = model.redactReferenceur;
                        userVm.redactThemes = model.redactThemes;
                        userVm.redactVolume = model.redactVolume;
                        userVm.redactTarif = model.redactTarif;
                        userVm.logoUrl = model.logoUrl;
                        Session["userEditModif"] = null;
                    }

                    userVm.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(utilisateur.userId);
                    List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == utilisateur.userId).ToList();
                    if (listeUserRole.Any())
                    {
                        switch (listeUserRole[0].idRole)
                        {
                            case 1:
                                ViewBag.userRole = 1; /*referenceur manager*/
                                break;
                            case 2:
                                ViewBag.userRole = 2; /*redacteur manager*/
                                break;
                            case 3:
                                ViewBag.userRole = 3; /*manager utilisateur*/
                                break;
                            case 4:
                                ViewBag.userRole = 4; /*administrateur*/
                                break;
                            case 5:
                                ViewBag.userRole = 5; /*super administrateur*/
                                break;
                            default:
                                ViewBag.userRole = 0;
                                break;
                        }

                        ViewBag.listeUserRole = listeUserRole;
                    }
                }

                return View(userVm);
            }
            return View();
        }


        /// <summary>
        /// Retourne la vue de validation de suppression d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        public ActionResult DeletedUserWaitting(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            ViewBag.hashUser = hash;
            return View("DeletedUserWaitting");
        }

        /// <summary>
        /// Retourne la vue de confirmation de suppression d'un Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeletedUserConfirmation(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            if (hash != Guid.Empty)
            {
                try
                {
                    redactapplicationEntities db = new Models.redactapplicationEntities();
                    UTILISATEUR users = db.UTILISATEURs.SingleOrDefault(x => x.userId == hash);
                    if (users != null)
                    {
                       
                        List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == users.userId).ToList();
                        for (int indice = 0; indice < listeUserRole.Count(); indice++)
                        {
                            db.UserRoles.Remove(listeUserRole[indice]);
                        }
                        db.UTILISATEURs.Remove(users);
                        db.SaveChanges();
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        /// <summary>
        /// Retourne la vue de confirmation de suppression d'une liste d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeleteAllUserConfirmation()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(userSession);

            try
            {
                return View("DeletedAllUserWaitting");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        [HttpPost]
        public JsonResult InfoSearch()
        {
            var val = new { value = "" };
            if (Session["Infosearch"] != null)
            {
                val = new { value = Session["Infosearch"].ToString() };
                Session["Infosearch"] = null;
            }
            return Json(val, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Retourne la vue contenant la recherche d'Utilisateur.
        /// </summary>
        /// <param name="searchValue">Chaine de recherche</param>
        /// <returns>View</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UserSearch(string searchValue)
        {
            if (searchValue != null && searchValue != "")
            {
                Session["Infosearch"] = searchValue;
            }
            else
            {
                return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
            }

            redactapplicationEntities bds = new Models.redactapplicationEntities();
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            Utilisateurs db = new Utilisateurs();
            var answer = db.SearchUtilisateur(searchValue);
            if (answer == null || answer.Count == 0)
            {                
                List<UTILISATEURViewModel> listeUser = new List<UTILISATEURViewModel>();
                answer = listeUser;
                ViewBag.SearchUserNoResultat = 1;
            }

            ViewBag.Search = true;
            redactapplicationEntities e = new redactapplicationEntities();
            var req = e.UserRoles.Where(x => x.idRole == 1 || x.idRole == 2 || x.idRole == 3 || x.idRole == 4);
            List<UTILISATEURViewModel> listeDataUserFiltered = new List<UTILISATEURViewModel>();
            foreach (var userModel in answer)
            {
                foreach (var q in req)
                {
                    if (userModel.userId == q.idUser)
                    {
                        listeDataUserFiltered.Add(userModel);
                    }
                }
            }
            ViewBag.listeUserVm = listeDataUserFiltered.Distinct().ToList();
            ViewBag.listeUserRole = e.UserRoles.ToList();
            if (user != Guid.Empty)
            {
                ViewBag.CurrentUser = db.GetUtilisateur(user);
                var utilisateur = db.GetUtilisateur(user);
                if (utilisateur != null)
                {
                    UTILISATEURViewModel userVm = new UTILISATEURViewModel();
                   
                    userVm.userNom = utilisateur.userNom;
                    userVm.userPrenom = utilisateur.userPrenom;
                    userVm.userId = utilisateur.userId;

                    userVm.redactSkype = utilisateur.redactSkype;
                    userVm.redactModePaiement = utilisateur.redactModePaiement;
                    userVm.redactNiveau = utilisateur.redactNiveau;
                    userVm.redactPhone = utilisateur.redactPhone;
                    userVm.redactReferenceur = utilisateur.redactReferenceur;
                    userVm.redactThemes = utilisateur.redactThemes;
                    userVm.redactVolume = utilisateur.redactVolume;


                    ViewBag.userRole = db.GetUtilisateurRoleToString(userVm.userId);
                    return View("ListeUser", userVm);
                }
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        /// <summary>
        /// Ajoute un nouvel Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>

        /// <param name="selectedRole">liste des roles de l'utilisateur à créer</param>
        /// <returns>View</returns>
        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EnregistrerTemplateUtilisateur(UTILISATEURViewModel model, HttpPostedFileBase logoUrl)
        {

            model.userNom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userNom));
            model.userPrenom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userPrenom));
            model.userMail = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMail));

            var val = new Utilisateurs();
            UTILISATEURViewModel userVm = new UTILISATEURViewModel();
           


            if (string.IsNullOrEmpty(model.userNom) || string.IsNullOrEmpty(model.userPrenom) || string.IsNullOrEmpty(model.userMail))
            {
                ViewBag.succes = 3;

                model.ListTheme = val.GetListThemeItem();
                Session["userEditModif"] = model;
                return View("CreateUser", model);
            }

            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            


            string path = Server.MapPath("~/images/Logo/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (logoUrl != null)
            {
                string fileName = Path.GetFileName(logoUrl.FileName);
                logoUrl.SaveAs(path + fileName);
                model.logoUrl = "/images/Logo/" + fileName;
            }

            redactapplicationEntities db = new Models.redactapplicationEntities();

            bool isRoleValid = true;
          

          
            if (Regex.IsMatch(model.userMail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
            {
                ViewBag.ErrorUserValidation = "E-mail non valide.";
                model.ListTheme = val.GetListThemeItem();
                Session["userEditModif"] = model;

                return View("CreateUser", model);
            }
           
            try
            {
                UTILISATEUR utilisateur = new UTILISATEUR();
                var users = db.UTILISATEURs.Count(x => x.userMail == model.userMail);
                if (users <= 0)
                {
                    utilisateur.userNom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userNom.ToLower());
                    utilisateur.userPrenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userPrenom.ToLower());
                    utilisateur.userMail = model.userMail;
                    utilisateur.logoUrl = model.logoUrl;


                    var newId = Guid.NewGuid();
                    utilisateur.userId = newId;
                    db.UTILISATEURs.Add(utilisateur);


                    UserRole userRole = new UserRole();

                    userRole.idRole = 6; //role CEO
                    userRole.idUser = newId;
                    db.UserRoles.Add(userRole);


                    db.SaveChanges();

                 

                    return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "sendMailRecovery" },
                        { "hash",newId }
                    });
                }
                else
                {

                    ViewBag.ErrorUserCreation = true;
                    return View("CreateUser");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ViewBag.ErrorMessage = "role null";
                return View("ErrorException");
            }
        }


        /// <summary>
        /// Ajoute un nouvel Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>

        /// <param name="selectedRole">liste des roles de l'utilisateur à créer</param>
        /// <returns>View</returns>
        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EnregistrerUtilisateur(UTILISATEURViewModel model, Guid[] listThemeId, string[] selectedRole, HttpPostedFileBase logoUrl)
        {

            model.userNom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userNom));
            model.userPrenom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userPrenom));
            model.userMail = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMail));

            var val = new Utilisateurs();
            UTILISATEURViewModel userVm = new UTILISATEURViewModel();
            userVm.ListTheme = val.GetListThemeItem();
           

            if (string.IsNullOrEmpty(model.userNom) || string.IsNullOrEmpty(model.userPrenom) || string.IsNullOrEmpty(model.userMail))
            {
                ViewBag.succes = 3;
               
                model.ListTheme = val.GetListThemeItem();
                Session["userEditModif"] = model;
                return View("GererUtilisateur", model);
            }            

            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);
            

            string path = Server.MapPath("~/images/Logo/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (logoUrl != null)
            {
                string fileName = Path.GetFileName(logoUrl.FileName);
                logoUrl.SaveAs(path + fileName);
                model.logoUrl = "/images/Logo/" + fileName;
            }

            redactapplicationEntities db = new Models.redactapplicationEntities();

            bool isRoleValid = true;
            if (selectedRole == null) isRoleValid = false;
            else if (selectedRole.Length == 0) isRoleValid = false;
            if (!isRoleValid && ViewBag.userRole !="2")
            {
                ViewBag.ErrorMessage = "role is null";
                model.ListTheme = val.GetListThemeItem();
                Session["userEditModif"] = model;

                return View("GererUtilisateur", model);
            }
            if (Regex.IsMatch(model.userMail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
            {             
                ViewBag.ErrorUserValidation = "E-mail non valide.";
                model.ListTheme = val.GetListThemeItem();
                Session["userEditModif"] = model;

                return View("GererUtilisateur", model);
            }
            if (Regex.IsMatch(model.redactPhone, @"^\d{9}$", RegexOptions.IgnoreCase) == false)
            {
                ViewBag.ErrorPhoneValidation = "Numéro non valide.";
                model.ListTheme = val.GetListThemeItem();
                Session["userEditModif"] = model;

                return View("GererUtilisateur", model);
            }

            try
            {
                UTILISATEUR utilisateur = new UTILISATEUR();
                var users = db.UTILISATEURs.Count(x => x.userMail == model.userMail);
                if (users <= 0)
                {
                    utilisateur.userNom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userNom.ToLower());
                    utilisateur.userPrenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userPrenom.ToLower());
                    utilisateur.userMail = model.userMail;
                    utilisateur.logoUrl = model.logoUrl;

                    utilisateur.redactSkype = model.redactSkype;
                    utilisateur.redactModePaiement = model.redactModePaiement;
                    utilisateur.redactNiveau = model.redactNiveau;
                  
                    utilisateur.redactThemes = new Utilisateurs().RedactThemes(utilisateur.userId);

                    utilisateur.redactVolume = string.IsNullOrEmpty(model.redactTarif) ? utilisateur.redactVolume : model.redactVolume; 
                    utilisateur.redactTarif = string.IsNullOrEmpty(model.redactTarif) ? utilisateur.redactTarif : model.redactTarif ;

                    utilisateur.redactPhone = model.redactPhone;
                    
                    var newId = Guid.NewGuid();
                    utilisateur.userId = newId;
                    db.UTILISATEURs.Add(utilisateur);
                  

                    UserRole userRole = new UserRole();
                    userRole.idRole = int.Parse(selectedRole[0]);
                    userRole.idUser = utilisateur.userId;
                    db.UserRoles.Add(userRole);
                    
                    
                    db.SaveChanges();

                    if (listThemeId != null)
                        SaveRedactThemes(listThemeId, newId);

                    return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "sendMailRecovery" },
                        { "hash",newId }
                    });
                }
                else
                {
                   
                    ViewBag.ErrorUserCreation = true;
                    return View("GererUtilisateur");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                ViewBag.ErrorMessage = "role null";
                return View("ErrorException");
            }
        }

        /// <summary>
        /// Enregistre les modifications d'un Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>
        /// <param name="selectedDiv">liste des divisions de l'utilisateur à créer</param>
        /// <param name="selectedRole">liste des roles de l'utilisateur à créer</param>
        /// <param name="idUser">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ModifierUtilisateur(UTILISATEURViewModel model, Guid[] listThemeId, string[] selectedRole, Guid idUser, HttpPostedFileBase logoUrl)
        {
            _userId = idUser;
            Guid userID = Guid.Parse(HttpContext.User.Identity.Name);
            var currentrole = (new Utilisateurs()).GetUtilisateurRoleToString(userID);
            ViewBag.userRole = currentrole;
            if(currentrole != "2")
            {
                model.userNom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userNom));
                model.userPrenom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userPrenom));
                model.userMail = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMail));

                model.redactSkype = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactSkype));
                model.redactModePaiement = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactModePaiement));
                model.redactNiveau = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactNiveau));
                model.redactPhone = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactPhone));
                model.redactReferenceur = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactReferenceur));
                model.redactThemes = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactThemes));
                model.redactVolume = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactVolume));
                model.redactTarif = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactTarif));

            }
            else
            {
                model.userNom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userNom));
                model.userPrenom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userPrenom));
                model.userMail = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMail));
                model.redactSkype = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactSkype));
                model.redactPhone = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.redactPhone));
            }
           


            string path = Server.MapPath("~/images/Logo/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (logoUrl != null)
            {
                string fileName = Path.GetFileName(logoUrl.FileName);
                logoUrl.SaveAs(path + fileName);
                model.logoUrl = "/images/Logo/" + fileName;
            }

            if (string.IsNullOrEmpty(model.userNom) || string.IsNullOrEmpty(model.userPrenom) || string.IsNullOrEmpty(model.userMail))
            {
                return View("ErrorEditUser");
            }
            if (StatePageSingleton.nullInstance())
            {
                StatePageSingleton.getInstance(1, 10);
                ViewBag.numpage = 1;
                ViewBag.nbrow = 10;
            }
            else
            {
                ViewBag.numpage = StatePageSingleton.getInstance().Numpage;
                ViewBag.nbrow = StatePageSingleton.getInstance().Nbrow;
            }
           

            
            redactapplicationEntities db = new Models.redactapplicationEntities();
            //Recuperation de l'utilisateur
            UTILISATEUR user = db.UTILISATEURs.SingleOrDefault(x => x.userId == idUser);

            if (selectedRole == null && currentrole != "2" && currentrole != "4" && currentrole != "3")                
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorRole" });
            }
            if (Regex.IsMatch(model.userMail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorUserMailValidation" });
            }
            if (Regex.IsMatch(model.redactPhone, @"^\d{9}$", RegexOptions.IgnoreCase) == false)
            {
                Session["userEditModif"] = model;
                ViewBag.ErrorPhoneValidation = "Numéro non valide.";
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorPhoneValidation" });
            }
            //Verify the user email
            bool userMailValid = true;
            UTILISATEUR userByMail = db.UTILISATEURs.SingleOrDefault(x => x.userMail == model.userMail);
            if (userByMail != null)
            {
                if (userByMail.userId != user.userId) userMailValid = false;
            }
            if (userMailValid)
            {
                try
                {
                    // mise à jour user -> Role
                    if (selectedRole != null && currentrole != "2")
                    {
                        List<UserRole> listeUserRole = db.UserRoles.Where(x => x.idUser == user.userId).ToList();
                        for (int i = 0; i < listeUserRole.Count; i++)
                        {
                            db.UserRoles.Remove(listeUserRole[i]);
                        }

                        UserRole userRole = new UserRole();
                        userRole.idRole = int.Parse(selectedRole[0]);
                        userRole.idUser = user.userId;
                        db.UserRoles.Add(userRole);
                    }
                   
                    // mise à jour de user
                    user.userNom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userNom.ToLower());
                    user.userPrenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userPrenom.ToLower());
                    user.userMail = model.userMail;

                    user.redactSkype = model.redactSkype;
                    user.redactModePaiement = model.redactModePaiement;
                    user.redactNiveau = model.redactNiveau;
                    user.redactPhone = model.redactPhone;
                    user.redactReferenceur = model.redactReferenceur;
                    if (listThemeId != null)
                    {
                        List<REDACT_THEME> listeUserThemes = db.REDACT_THEME.Where(x => x.redactId == user.userId).ToList();
                                                   
                            SaveRedactThemes(listThemeId, user.userId, listeUserThemes);
                    }
                    if (currentrole != "2")
                    {
                        user.redactVolume = model.redactVolume;
                        user.redactTarif = model.redactTarif;
                    }

                    if (!string.IsNullOrEmpty(model.logoUrl))
                    {
                        user.logoUrl = model.logoUrl;
                      
                    } 

                    db.SaveChanges();
                    _userId = Guid.Parse(HttpContext.User.Identity.Name);
                   if(user.userId == _userId)
                     UpdateInfoProfilSession(user.userNom, user.userPrenom, user.userMail, user.UserRoles.FirstOrDefault(x => x.idUser == user.userId).idRole.ToString(), user.logoUrl);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Session["userEditModif"] = model;
                    return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorMessage" });
                }
            }
            else
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUser", new { hash = idUser, error = "ErrorMail" });
            }
            ViewBag.hashUser = user.userId;
            return View("EditUserConfirmation");
        }


        /// <summary>
        /// Enregistre les modifications d'un Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>
        /// <param name="selectedDiv">liste des divisions de l'utilisateur à créer</param>
        /// <param name="selectedRole">liste des roles de l'utilisateur à créer</param>
        /// <param name="idUser">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ModifierUserTemplate(UTILISATEURViewModel model, Guid idUser, HttpPostedFileBase logoUrl)
        {
            _userId = idUser;
            Guid userID = Guid.Parse(HttpContext.User.Identity.Name);
            var currentrole = (new Utilisateurs()).GetUtilisateurRoleToString(userID);
            ViewBag.userRole = currentrole;
           
          
                model.userNom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userNom));
                model.userPrenom = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userPrenom));
                model.userMail = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMail));
                model.userMotdepasse = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.userMotdepasse));
            
            



            string path = Server.MapPath("~/images/Logo/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (logoUrl != null)
            {
                string fileName = Path.GetFileName(logoUrl.FileName);
                logoUrl.SaveAs(path + fileName);
                model.logoUrl = "/images/Logo/" + fileName;
            }

            if (string.IsNullOrEmpty(model.userNom) || string.IsNullOrEmpty(model.userPrenom) || string.IsNullOrEmpty(model.userMail))
            {
                return View("ErrorEditUser");
            }
           



            redactapplicationEntities db = new Models.redactapplicationEntities();
            //Recuperation de l'utilisateur
            UTILISATEUR user = db.UTILISATEURs.SingleOrDefault(x => x.userId == idUser);

           
            if (Regex.IsMatch(model.userMail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase) == false)
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUserTemplate", new { hash = idUser, error = "ErrorUserMailValidation" });
            }
           
            //Verify the user email
            bool userMailValid = true;
            UTILISATEUR userByMail = db.UTILISATEURs.SingleOrDefault(x => x.userMail == model.userMail);
            if (userByMail != null)
            {
                if (userByMail.userId != user.userId) userMailValid = false;
            }
            if (userMailValid)
            {
                try
                {
                    

                    // mise à jour de user
                    user.userNom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userNom.ToLower());
                    user.userPrenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.userPrenom.ToLower());
                    user.userMail = model.userMail;

                   
                    
                    if (model.userMotdepasse != "")
                    {
                        user.userMotdepasse = model.userMotdepasse;
                       
                    }

                    if (!string.IsNullOrEmpty(model.logoUrl))
                    {
                        user.logoUrl = model.logoUrl;

                    }

                    db.SaveChanges();
                    _userId = Guid.Parse(HttpContext.User.Identity.Name);
                    if (user.userId == _userId)
                        UpdateInfoProfilSession(user.userNom, user.userPrenom, user.userMail, user.UserRoles.FirstOrDefault(x => x.idUser == user.userId).idRole.ToString(), user.logoUrl);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    Session["userEditModif"] = model;
                    return RedirectToAction("EditUserTemplate", new { hash = idUser, error = "ErrorMessage" });
                }
            }
            else
            {
                Session["userEditModif"] = model;
                return RedirectToAction("EditUserTemplate", new { hash = idUser, error = "ErrorMail" });
            }
            ViewBag.hashUser = user.userId;
            return View("EditUserConfirmationTemplate");
        }



        private void UpdateInfoProfilSession(string nom, string prenom,string mail,string role, string logourl)
        {
            if(!string.IsNullOrEmpty(nom))
                Session["name"] = nom;

            if (!string.IsNullOrEmpty(prenom))
                Session["surname"] = prenom;

           
            if (!string.IsNullOrEmpty(mail))
                Session["mail"] = mail;

            if (!string.IsNullOrEmpty(role))
                Session["role"] = role;

            if (!string.IsNullOrEmpty(logourl))
                Session["logoUrl"] = logourl;

        }


        /// <summary>
        /// Annule l'enregistrement d'un Utilisateur dans la base de données.
        /// </summary>
        /// <param name="model">information Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult annulerEnregUtilisateur(UTILISATEURViewModel model)
        {
            return RedirectToAction("GererUtilisateur");
        }

        /// <summary>
        /// Supprime un Utilisateur dans la base de données.
        /// </summary>
        /// <param name="userId">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult supprUtilisateur(Guid userId)
        {
            try
            {
                redactapplicationEntities db = new Models.redactapplicationEntities();
                UTILISATEUR utilisateur = db.UTILISATEURs.Where(x => x.userId == userId).FirstOrDefault();

                if (utilisateur != null)
                {
                    db.UTILISATEURs.Remove(utilisateur);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return RedirectToAction("GererUtilisateur");
        }

        /// <summary>
        /// Envoi un mail d'édition de mot de passe Utilisateur et retourne une vue de confirmation.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult sendMailRecovery(Guid? hash)
        {
           
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(user);

            Guid userId = (Guid)hash;
            if (userId != Guid.Empty)
            {
                try
                {
                    Guid userIds = userId;
                    redactapplicationEntities db = new Models.redactapplicationEntities();
                    UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == userIds);
                    Guid TemporaryIdUser = Guid.NewGuid();
                    if (utilisateur != null)
                    {
                        ViewBag.mail = utilisateur.userMail;
                        var url = Request.Url.Scheme;
                        if (Request.Url != null)
                        {

                            string callbackurl = Request.Url.Host != "localhost"
                                ? Request.Url.Host : Request.Url.Authority;
                            var port = Request.Url.Port;
                            if (!string.IsNullOrEmpty(port.ToString()) && Request.Url.Host != "localhost")
                                callbackurl += ":" + port;

                            url += "://" + callbackurl;
                        }

                        StringBuilder mailBody = new StringBuilder();

                        mailBody.AppendFormat(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.userNom.ToLower()) + ",");
                        mailBody.AppendFormat("<br />");
                        mailBody.AppendFormat("<p>Votre compte a été crée .Veuillez réinitialiser votre mot de passe en cliquant sur lien suivant : </p>");
                        mailBody.AppendFormat("<br />");
                        mailBody.AppendFormat(url + "/Login/UpdatePassword?token=" + TemporaryIdUser);
                        mailBody.AppendFormat("<br />");
                        mailBody.AppendFormat("<p>Si vous n'avez pas demandé la création / réinitialisation de ce compte, ignorez cet e-mail.</p>");
                        mailBody.AppendFormat("<br />");
                        mailBody.AppendFormat("Cordialement.");
                        mailBody.AppendFormat("<br />");
                        mailBody.AppendFormat("Media click App.");

                        bool isSendMail = MailClient.SendMail(utilisateur.userMail, mailBody.ToString(), "Media click App - modification mot de passe.");
                        if (isSendMail)
                        {
                            utilisateur.token = TemporaryIdUser;
                            utilisateur.dateToken = DateTime.Now;
                            int result = db.SaveChanges();
                            if (result > 0)
                            {
                                return View();
                            }
                        }
                    }
                    else {
                        
                        return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUserTemplate" }
                        });
                    }
                }
                catch
                {
                    return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUserTemplate" }
                    });
                }
            }
            return View();
        }

        /// <summary>
        /// Charge une liste d'utilisateur à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id d'utilisateur</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelecteAllUserToDelete(string hash)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ListUserToDelete"] = hash;
                if (Session["ListUserToDelete"] != null)
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
        public ActionResult DeleteAllUserSelected()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);
            
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRoleToString(userSession);
            try
            {
                bool unique = true;
                if (Session["ListUserToDelete"] != null)
                {
                    string hash = Session["ListUserToDelete"].ToString();
                    List<Guid> listIdUser = new List<Guid>();
                    if (!string.IsNullOrEmpty(hash))
                    {
                        if (!hash.Contains(','))
                        {
                            listIdUser.Add(Guid.Parse(hash));
                        }
                        else
                        {
                            foreach (var id in (hash).Split(','))
                            {
                                listIdUser.Add(Guid.Parse(id));
                            }
                            unique = false;
                        }                        
                    }
                    if (listIdUser.Count != 0)
                    {
                        redactapplicationEntities db = new Models.redactapplicationEntities();
                        foreach (var userId in listIdUser)
                        {
                           
                            //suppression des roles
                            var userRoleDeleted = db.UserRoles.Where(x => x.idUser == userId);
                            foreach (var role in userRoleDeleted)
                            {
                                db.UserRoles.Remove(role);
                            }
                            var userCommandes = db.COMMANDEs.Where(x => x.commandeReferenceurId == userId);
                            foreach (var cmde in userCommandes)
                            {
                                db.COMMANDEs.Remove(cmde);
                            }

                            //suppression des utilisateurs
                            UTILISATEUR user = db.UTILISATEURs.SingleOrDefault(x => x.userId == userId);
                            db.UTILISATEURs.Remove(user);
                        }
                        db.SaveChanges();
                       
                        if (unique)
                        {
                            return View("DeletedUserConfirmation");
                        }
                        return View("DeletedAllUserConfirmation");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Home" },
                        { "action", "ListeUser" }
                    });
        }

        /// <summary>
        /// Retourne le model de l'Utilisateur courant.
        /// </summary>
        /// <returns>UTILISATEURViewModel</returns>
        private UTILISATEURViewModel GetCurrentUserModel()
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            _userId = Guid.Parse(HttpContext.User.Identity.Name);
            UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == _userId);
            UTILISATEURViewModel userVm = new UTILISATEURViewModel();

            if (utilisateur != null)
            {
                userVm.userNom = utilisateur.userNom;
                userVm.userPrenom = utilisateur.userPrenom;
                userVm.userId = utilisateur.userId;
            }
            return userVm;
        }

      }
    
}