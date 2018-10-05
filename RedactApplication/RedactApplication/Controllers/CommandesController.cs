using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Security.Application;
using RedactApplication.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Text.RegularExpressions;
using System.Data.Entity.Validation;
using System.Configuration;

namespace RedactApplication.Controllers
{
    public class CommandesController : Controller
    {
        private redactapplicationEntities db = new redactapplicationEntities();
        /// <summary>
        /// Représente l'identifiant de l'utilisateur courant.
        /// </summary>
        public static Guid _userId;

        // GET: COMMANDEs
        public ActionResult Index()
        {
           
            return View();
        }

        public ActionResult Chat()
        {
            return View("ListCommandes");
        }

        [HttpPost]       
        public ActionResult CreateProjet(COMMANDEViewModel model)
        {            
            PROJET projet = new PROJET { projetId = Guid.NewGuid(), projet_name = model.projet };            
            db.PROJETS.Add(projet);
            db.SaveChanges();
            
            Commandes val = new Commandes();
            COMMANDEViewModel commandeVm = new COMMANDEViewModel();
            commandeVm.ListProjet = val.GetListProjetItem();
            commandeVm.ListTheme = val.GetListThemeItem();
            commandeVm.ListRedacteur = val.GetListRedacteurItem();
            commandeVm.ListCommandeType = val.GetListCommandeTypeItem();
            commandeVm.ListTag = val.GetListTagItem();
            commandeVm.ListOtherRedacteur = val.GetListRedacteurItem();
            return View("Create",commandeVm);
            
        }

        private STATUT_COMMANDE GetStatut(string statut)
        {                      
            switch (statut)
            {
                case "livrer":
                    return db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains("Livré"));                    
                case "retard":
                    return db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains("En cours"));
                case "refuser":
                    return db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains("Refusé"));
                case "attente":
                    return db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains("En attente"));
                case "encours":
                    return db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains("En cours"));
                case "annuler":
                    return db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains("Annulé"));                  
                case "facturer":
                    return db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains("Validé"));
                default:                   
                    break;
            }
            return null;
        }

        public ActionResult ListCommandes(string statut)
        {
            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

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
                if (statut != null)
                {
                    var currentStatus = GetStatut(statut);

                    if (statut.Contains("retard"))                      
                        listeDataCmde = listeDataCmde.Where(x => x.date_livraison <= now ).ToList();

                    //listeDataCmde = listeDataCmde.Where(x => x.date_cmde >= startOfMonth &&
                    //                                       x.date_cmde <= lastDay &&
                    //                                       x.date_livraison <= now &&
                    //                                       x.commandeStatutId == currentStatus.statutCommandeId).ToList();
                    //if (statut.Contains("all"))
                    //    listeDataCmde = listeDataCmde.Where(x => x.date_cmde >= startOfMonth &&
                    //                                         x.date_cmde <= lastDay).ToList();
                    //else
                    //    listeDataCmde = listeDataCmde.Where(x => x.date_cmde >= startOfMonth &&
                    //                                            x.date_cmde <= lastDay && x.commandeStatutId == currentStatus.statutCommandeId).ToList();

                    else if (!statut.Contains("all") && !statut.Contains("retard"))                        
                        listeDataCmde = listeDataCmde.Where(x => x.commandeStatutId == currentStatus.statutCommandeId).ToList();
                  
                }

                ViewBag.listeCommandeVms = listeDataCmde.Distinct().ToList();

                var currentrole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);
                if (currentrole != null)
                {
                    GetRedacteurInformations(_userId);
                    if (currentrole.Contains("2"))
                    {
                        ViewBag.listeCommandeVms = listeDataCmde.Where(x => x.commandeRedacteurId == _userId).ToList();

                    }
                    if (currentrole.Contains("1"))
                    {
                        ViewBag.listeCommandeVms = listeDataCmde.Where(x => x.commandeReferenceurId == _userId).ToList();

                    }
                }
                return View();
            }

            return RedirectToRoute("Home", new RouteValueDictionary {
                    { "controller", "Login" },
                    { "action", "Accueil" }
                });
        }


        public ActionResult ListCommandeAValider()
        {
            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
                Session["currentid"] = Request.QueryString["currentid"];
            }
            else if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                _userId = Guid.Parse(HttpContext.User.Identity.Name);

                // Exécute le traitement de la pagination
                Commandes val = new Commandes();

                // Récupère la liste des commandes
                var listeDataCmde = val.GetListCommande();
                ViewBag.listeCommandeVms = listeDataCmde.Distinct().ToList();

                var currentrole = (new Utilisateurs()).GetUtilisateurRoleToString(_userId);
                if (currentrole != null)
                {
                    string etat = "En attente";
                    var statut = db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains(etat));
                    if (currentrole.Contains("2"))
                    {
                        ViewBag.listeCommandeVms = listeDataCmde
                            .Where(x => x.commandeRedacteurId == _userId && x.commandeStatutId == statut.statutCommandeId).ToList();
                        GetRedacteurInformations(_userId);
                    }

                    else
                    {
                        ViewBag.listeCommandeVms = listeDataCmde.Where(x => x.commandeStatutId == statut.statutCommandeId).ToList();
                    }

                }

                return View();
            }

            return View("ErrorException");
        }


        [Authorize]
        [HttpGet]
        public JsonResult GetNotifications()
        {
            // Exécute le suivi de session utilisateur
            if (!string.IsNullOrEmpty(Request.QueryString["currentid"]))
            {
                _userId = Guid.Parse(Request.QueryString["currentid"]);
            }
            else if (!string.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                _userId = Guid.Parse(HttpContext.User.Identity.Name);
            }

            return Json(new Notifications().GetAllMessages(_userId), JsonRequestBehavior.AllowGet);
            
        }
        private int GetVolumeEnCours(Guid? redactId, DateTime? date_livraison)
        {
            var redact = db.UTILISATEURs.Find(redactId);
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var lastDay = new DateTime(now.Year, now.Month, daysInMonth);
            var commandes = db.COMMANDEs.Where(x => x.commandeRedacteurId == redactId && x.date_livraison >= now &&
            x.date_livraison <= date_livraison && !x.STATUT_COMMANDE.statut_cmde.Contains("Annulé")).ToList();
            int volume = 0;
            foreach (var commande in commandes)
            {
                volume += Convert.ToInt32(commande.nombre_mots);
            }

            return volume;
        }


        private bool IsLimiteVolumeEnCours(Guid? redactId,DateTime? date_livraison,int? nb_mots)
        {
            var redact = db.UTILISATEURs.Find(redactId);
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var lastDay = new DateTime(now.Year, now.Month, daysInMonth);
            var commandes = db.COMMANDEs.Where(x => x.commandeRedacteurId == redactId && x.date_livraison >= now &&
            x.date_livraison <= date_livraison &&  !x.STATUT_COMMANDE.statut_cmde.Contains("Annulé")).ToList();
            bool limite = true ;
            if (commandes.Count() == 0)
                return false;
            int? volume = 0;
            foreach (var commande in commandes)
            {
                volume += Convert.ToInt32(commande.nombre_mots);
            }
            volume += nb_mots;
            int maxVolume = Convert.ToInt32(redact.redactVolume) * commandes.Count();
            if (volume < maxVolume)
                return false;

            return limite;
        }

        private int? GetVolumeRestant(Guid? redactId)
        {
            var redact = db.UTILISATEURs.Find(redactId);
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var lastDay = new DateTime(now.Year, now.Month, daysInMonth);
            var commandes = db.COMMANDEs.Where(x => x.commandeRedacteurId == redactId && x.date_cmde >= startOfMonth &&
                                                    x.date_cmde <= lastDay).ToList();
            int volume = 0;
            foreach (var commande in commandes)
            {
                volume += Convert.ToInt32(commande.nombre_mots);
            }
            int? redactVolume = !string.IsNullOrEmpty(redact.redactVolume) ? int.Parse(redact.redactVolume) :0;
            int? redactVolumeRestant = redactVolume - volume;
            return redactVolumeRestant;
        }


        private void GetRedacteurInformations(Guid redactId)
        {
            var redact = db.UTILISATEURs.Find(redactId);
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var lastDay = new DateTime(now.Year, now.Month, daysInMonth);

            if (redact != null)
            {
                var commandes = db.COMMANDEs.Where(x=>x.commandeRedacteurId == redactId).ToList();

                var commandesEnCours = commandes.Count(x => x.date_cmde >= startOfMonth &&
                                x.date_cmde <= lastDay && 
                               (x.STATUT_COMMANDE != null && x.STATUT_COMMANDE.statut_cmde.Contains("En cours"))
                                                            );

                ViewBag.commandesEnCours = commandesEnCours;

                var commandesEnAttente = commandes.Where(x => x.date_cmde >= startOfMonth &&
                                             x.date_cmde <= lastDay &&
                                                                 (x.STATUT_COMMANDE != null && x.STATUT_COMMANDE.statut_cmde.Contains("En attente"))).Distinct().ToList().Count;
                ViewBag.commandesEnAttente = commandesEnAttente;

                var commandesRefuser = commandes.Count(x => x.date_cmde >= startOfMonth &&
                                                                 x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                                                                  x.STATUT_COMMANDE.statut_cmde.Contains("Refusé"))); 

                ViewBag.commandesRefuser = commandesRefuser;


                var commandesAnnuler = commandes.Count(x => x.date_cmde >= startOfMonth &&
                                                                x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                                                                 x.STATUT_COMMANDE.statut_cmde.Contains("Annulé")));

                ViewBag.commandesAnnuler = commandesAnnuler;


                var commandesEnRetard = commandes.Count(x => x.date_cmde >= startOfMonth &&
                                                                x.date_cmde <= lastDay &&
                                                                x.date_livraison <= now &&
                                                                (x.STATUT_COMMANDE != null &&
                                                                 x.STATUT_COMMANDE.statut_cmde.Contains("En cours")));
                ViewBag.commandesEnRetard = commandesEnRetard;
                var commandesFacturer = commandes.Count(x => x.date_cmde >= startOfMonth &&
                                                                  x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                                                                  x.STATUT_COMMANDE.statut_cmde.Contains("Validé")));
                ViewBag.commandesFacturer = commandesFacturer;

                var commandesLivrer = commandes.Count(x => x.date_cmde >= startOfMonth &&
                                                                x.date_cmde <= lastDay && (x.STATUT_COMMANDE != null &&
                                                                                           x.STATUT_COMMANDE.statut_cmde.Contains("Livré")));
                ViewBag.commandesLivrer = commandesLivrer;

            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CommandesSearch(string searchValue)
        {
            if (StatePageSingleton.nullInstance())
            {
                StatePageSingleton.getInstance(1, 10);
            }
            StatePageSingleton q = StatePageSingleton.getInstance();
            ViewBag.numpage = q.Numpage;
            ViewBag.nbrow = q.Nbrow;
            if (!string.IsNullOrEmpty(searchValue))
            {
                Session["Infosearch"] = searchValue;
            }
            else
            {
                return RedirectToRoute("Home", new RouteValueDictionary {
                    { "controller", "Commandes" },
                    { "action", "ListCommandes" }
                });
            }

            redactapplicationEntities bds = new Models.redactapplicationEntities();
            searchValue = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(searchValue));

            Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.userRole = (new Utilisateurs()).GetUtilisateurRole(user);

            Commandes db = new Commandes();
            var answer = db.SearchCommandes(searchValue);

            if (answer == null || answer.Count == 0)
            {
                List<COMMANDEViewModel> listcCommande = new List<COMMANDEViewModel>();
                answer = listcCommande;
                ViewBag.SearchContactNoResultat = 1;
            }
            ViewBag.Search = true;
            ViewBag.listeCommandeVms = answer.Distinct().ToList();

            COMMANDEViewModel cmdeVm = new COMMANDEViewModel();
            if ((new Commandes()).GetCommandeType(userId) != null)
            {
                cmdeVm.commandeType = (new Commandes()).GetCommandeType(userId).Type;
            }

            return View("ListCommandes", cmdeVm);
        }

        /// <summary>
        /// Charge une liste d'utilisateur à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id d'utilisateur</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelecteAllCommandeToDelete(string hash)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ListCommandeToDelete"] = hash;
                if (Session["ListCommandeToDelete"] != null)
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
        public ActionResult DeleteAllCommandeSelected()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);

           
            try
            {
                bool unique = true;
                if (Session["ListCommandeToDelete"] != null)
                {
                    string hash = Session["ListCommandeToDelete"].ToString();
                    List<Guid> listIdCmde = new List<Guid>();
                    if (!string.IsNullOrEmpty(hash))
                    {
                        if (!hash.Contains(','))
                        {
                            listIdCmde.Add(Guid.Parse(hash));
                        }
                        else
                        {
                            foreach (var id in (hash).Split(','))
                            {
                                listIdCmde.Add(Guid.Parse(id));
                            }
                            unique = false;
                        }
                    }
                    if (listIdCmde.Count != 0)
                    {
                        redactapplicationEntities db = new Models.redactapplicationEntities();
                        foreach (var cmdeId in listIdCmde)
                        {
                            //suppression des commandes
                            COMMANDE cmde = db.COMMANDEs.SingleOrDefault(x => x.commandeId == cmdeId);
                            if (cmde != null) db.COMMANDEs.Remove(cmde);
                        }
                        db.SaveChanges();

                        return View(unique ? "DeletedCommandeConfirmation" : "DeteleAllCommandeConfirmation");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                { "controller", "Commandes" },
                { "action", "ListCommandes" }
            });
        }

        /// <summary>
        /// Retourne la vue de validation de suppression d'Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        public ActionResult DeletedUserWaitting(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.hashCmde = hash;
            return View("DeletedCommandeWaitting");
        }

        /// <summary>
        /// Retourne la vue de confirmation de suppression d'un Utilisateur.
        /// </summary>
        /// <param name="hash">id de l'Utilisateur</param>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeletedCommandeConfirmation(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
        
            if (hash != Guid.Empty)
            {
                try
                {
                    redactapplicationEntities db = new redactapplicationEntities();
                    COMMANDE cmde = db.COMMANDEs.SingleOrDefault(x => x.commandeId == hash);
                    if (cmde != null)
                    {
                        var notifications = db.NOTIFICATIONs.Where(x => x.commandeId == hash).ToList();
                        foreach(var notif in notifications)
                        {
                            db.NOTIFICATIONs.Remove(notif);
                        }
                       
                        db.COMMANDEs.Remove(cmde);
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
                        { "controller", "Commandes" },
                        { "action", "ListCommandes" }
                    });
        }

        /// <summary>
        /// Retourne la vue de confirmation de suppression d'une liste d'Utilisateur.
        /// </summary>
        /// <returns>View</returns>
        [Authorize]
        public ActionResult DeleteAllCommandeConfirmation()
        {
            Guid userSession = new Guid(HttpContext.User.Identity.Name);
           

            try
            {
                return View("DeletedAllCommandeWaitting");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return RedirectToRoute("Home", new RouteValueDictionary {
                        { "controller", "Commandes" },
                        { "action", "ListCommandes" }
                    });
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
        /// Charge une liste d'utilisateur à supprimer dans la base de données.
        /// </summary>
        /// <param name="hash">List d'id d'utilisateur</param>
        /// <returns>bool</returns>
        [Authorize]
        [HttpPost]
        public bool SelectedTheme(string theme)
        {
            try
            {
                // Récupère la liste des id d'utilisateur                
                Session["ThemeSelected"] = theme;
                if (Session["ThemeSelected"] != null)
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

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadRedacteurByTheme(string theme)
        {
            THEME selectedTheme = db.THEMES.SingleOrDefault(x=>x.themeId.ToString() == theme);
            //Your Code For Getting Physicans Goes Here
            var redactList = (selectedTheme != null)?(new Commandes().GetListRedacteurItem(selectedTheme.theme_name)): null;
    
            return Json(redactList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadOtherRedacteur(string redact)
        {            
            //Your Code For Getting Physicans Goes Here
            var redactList = (redact != null) ? (new Commandes().GetListRedacteurItem().Where(x=>x.Value != redact)) : null;

            return Json(redactList, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public JsonResult AutocompleteTagSuggestions(string term)
        {
            var suggestions = new Commandes().GetListTagItem(term);
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpGet]
        public JsonResult AutocompleteSiteSuggestions(string term)
        {
            var suggestions = new Commandes().GetListSitetem(term);
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        private COMMANDEViewModel SetCommandeViewModelDetails(COMMANDE commande)
        {
            Commandes val = new Commandes();
            COMMANDEViewModel commandeVm = new COMMANDEViewModel();
            if (commande != null)
            {
                commandeVm.ListProjet = val.GetListProjetItem();
                commandeVm.ListTheme = val.GetListThemeItem();
                commandeVm.ListRedacteur = val.GetListRedacteurItem();
                commandeVm.ListCommandeType = val.GetListCommandeTypeItem();
                commandeVm.ListTag = val.GetListTagItem();
                commandeVm.ListStatut = val.GetListStatutItem();
                commandeVm.ListOtherRedacteur = val.GetListRedacteurItem();
                string theme = "";
                if (commande.commandeProjetId != null)
                    commandeVm.listprojetId = (Guid)commande.commandeProjetId;

                if (commande.commandeThemeId != null)
                {
                    commandeVm.listThemeId = (Guid)commande.commandeThemeId;
                    theme = val.GetTheme(commande.commandeThemeId).theme_name;
                }

                if (commande.commandeTypeId != null)
                    commandeVm.listCommandeTypeId = (Guid)commande.commandeTypeId;

                if (commande.commandeRedacteurId != null)
                {
                    commandeVm.listRedacteurId = (Guid)commande.commandeRedacteurId;
                    commandeVm.ListOtherRedacteur = val.GetListRedacteurItem().Where(x => x.Value != commande.commandeRedacteurId.ToString());
                }

                if (commande.tagId != null)
                    commandeVm.tag = commande.TAG.type;

                if (commande.commandeStatutId != null)
                    commandeVm.listStatutId = (Guid)commande.commandeStatutId;

                string referenceur = val.GetUtilisateurReferenceur(commande.commandeReferenceurId).userNom;
                string cmdeType = val.GetCommandeType(commande.commandeTypeId).Type;

                string redacteur = (commande.commandeRedacteurId != Guid.Empty) ? val.GetUtilisateurReferenceur(commande.commandeRedacteurId).userPrenom +" "+ val.GetUtilisateurReferenceur(commande.commandeRedacteurId).userNom : "";
                //string priorite = commande.ordrePriorite == "0" ? "Moyen" : "Haut";
                string projet = val.GetProjet(commande.commandeProjetId).projet_name;

                string statutcmde = (commande.commandeStatutId != Guid.Empty) ? val.GetStatutCommande(commande.commandeStatutId).statut_cmde : "";

                commandeVm.commandeId = commande.commandeId;
                commandeVm.commandeDemandeur = referenceur;
                commandeVm.date_cmde = commande.date_cmde;
                commandeVm.date_livraison = commande.date_livraison;
                commandeVm.commandeType = cmdeType;
                commandeVm.nombre_mots = commande.nombre_mots;
                commandeVm.mot_cle_pricipal = commande.mot_cle_pricipal;
                commandeVm.mot_cle_secondaire = commande.mot_cle_secondaire;
                commandeVm.consigne_references = commande.consigne_references;
                commandeVm.texte_ancrage = commande.texte_ancrage;

                commandeVm.consigne_autres = commande.consigne_autres;
                commandeVm.etat_paiement = commande.etat_paiement;
                commandeVm.commandeRedacteur = redacteur;
                commandeVm.ordrePriorite = commande.ordrePriorite;
                commandeVm.balise_titre = commande.balise_titre;
                commandeVm.contenu_livre = commande.contenu_livre;
                commandeVm.projet = projet;
                commandeVm.thematique = theme;

                if (commande.SITE != null)
                    commandeVm.site = commande.SITE.site_name;

                if (commande.TAG != null)
                    commandeVm.tag = commande.TAG.type;
                commandeVm.statut_cmde = statutcmde;
                Session["cmdeEditModif"] = null;
                commandeVm.commandeREF = commande.commandeREF;
                commandeVm.remarques = commande.remarques;

                if (commandeVm.contenu_livre != null) ViewBag.ComptMetaContenu = commandeVm.contenu_livre.Length;
                string contenu = (!string.IsNullOrEmpty(commande.contenu_livre)) ? Regex.Replace(commande.contenu_livre, "<.*?>", string.Empty) : string.Empty;

                if (CountWords(contenu) > 0)
                    ViewBag.ContentLength = CountWords(contenu);
                return commandeVm;
            }

            return null;
        }

        /// <summary>
        /// Count words with Regex.
        /// </summary>
        private int CountWords(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }


      

        public ActionResult DetailsCommande(Guid? hash, string not ="")
        {
            var commande = db.COMMANDEs.Find(hash);
            var notifications = db.NOTIFICATIONs.Where(x => x.commandeId == hash).ToList();
            foreach (var notif in notifications)
            {
                notif.statut = false;
                db.SaveChanges();
            }
            COMMANDEViewModel commandeVm = SetCommandeViewModelDetails(commande);
            if (commandeVm != null)
            {
              
                return View(commandeVm);
            }
               
            return View("ErrorException");
        }


        // GET: COMMANDEs/Details/5
        public ActionResult DetailsCommandeAValider(Guid? hash, string not = "")
        {
            var commande = db.COMMANDEs.Find(hash);
            var notifications = db.NOTIFICATIONs.Where(x => x.commandeId == hash).ToList();
            foreach(var notif in notifications)
            {
                notif.statut = false;
                db.SaveChanges();
            }
            COMMANDEViewModel commandeVm = SetCommandeViewModelDetails(commande);
            if (commandeVm != null)
                return View(commandeVm);
            return View("ErrorException");
        }


        // GET: COMMANDEs/Create
        public ActionResult Create()
        {
           
            Commandes val = new Commandes();
            COMMANDEViewModel commandeVm = new COMMANDEViewModel();
            commandeVm.ListProjet = val.GetListProjetItem();
            commandeVm.ListTheme = val.GetListThemeItem();
            commandeVm.ListRedacteur = val.GetListRedacteurItem();
            commandeVm.ListCommandeType = val.GetListCommandeTypeItem();
            commandeVm.ListTag = val.GetListTagItem();
            commandeVm.ListOtherRedacteur = val.GetListRedacteurItem();
            return View(commandeVm);
        }

        // POST: COMMANDEs/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCommande(COMMANDEViewModel model, FormCollection collection)
        {
            if (Session["cmdeEditModif"] != null)
                model = (COMMANDEViewModel)Session["cmdeEditModif"];

            var selectedProjetId = model.listprojetId;
            var selectedThemeId = model.listThemeId;
            var selectedRedacteurId = model.listRedacteurId;
            var selectedCommandeTypeId = model.listCommandeTypeId;
            var selectedTag = model.tag;
            var selectedReferenceurId = Guid.Parse(HttpContext.User.Identity.Name);
            var selectedSite = model.site;

            model.mot_cle_secondaire =
                StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_secondaire));
            model.texte_ancrage = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.texte_ancrage));
            model.consigne_references =
                StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_references));
            model.consigne_autres =
                StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_autres));
            bool notifSms = false;
            if (!string.IsNullOrEmpty(collection["checkResp"]))
            {
                string checkResp = collection["checkResp"];
                notifSms = checkResp == "on";
            }

            COMMANDE newcommande = new COMMANDE();
         

            var commande = db.COMMANDEs.Count(x => x.commandeProjetId == selectedProjetId && x.commandeThemeId == selectedThemeId && x.commandeTypeId == model.listCommandeTypeId && x.date_livraison == model.date_livraison) ;
            
            if (commande <= 0)
            {
                newcommande.PROJET = db.PROJETS.Find(selectedProjetId);
                newcommande.commandeProjetId = selectedProjetId;
                newcommande.THEME = db.THEMES.Find(selectedThemeId);
                newcommande.commandeThemeId = selectedThemeId;
                newcommande.REFERENCEUR = db.UTILISATEURs.Find(selectedReferenceurId);
                newcommande.commandeReferenceurId = selectedReferenceurId;
                newcommande.REDACTEUR = db.UTILISATEURs.Find(selectedRedacteurId);
                newcommande.commandeRedacteurId = selectedRedacteurId;

                string etat = "En attente";
                var statut = db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains(etat));
                newcommande.commandeStatutId = statut.statutCommandeId;

                newcommande.COMMANDE_TYPE = db.COMMANDE_TYPE.Find(selectedCommandeTypeId);
                newcommande.commandeTypeId = selectedCommandeTypeId;
                
               
                newcommande.mot_cle_pricipal =
                    StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_pricipal));
                newcommande.mot_cle_secondaire =
                    StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_secondaire));
                newcommande.texte_ancrage =
                    StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.texte_ancrage));
                newcommande.nombre_mots = model.nombre_mots;
                newcommande.consigne_references = model.consigne_references;
                if (!string.IsNullOrEmpty(selectedTag))
                {
                    TAG currentTag = db.TAGS.FirstOrDefault(x => x.type.Contains(selectedTag.TrimEnd()));
                    if (currentTag == null)
                    {
                        currentTag = new TAG { tagId = Guid.NewGuid(), type = selectedTag };
                        db.TAGS.Add(currentTag);
                        db.SaveChanges();
                    }
                    newcommande.tagId = currentTag.tagId;
                    newcommande.TAG = currentTag;
                }

                if (!string.IsNullOrEmpty(selectedSite))
                {
                    SITE currentSite = db.SITES.FirstOrDefault(x => x.site_name.Contains(selectedSite.TrimEnd()));
                    if (currentSite == null)
                    {
                        currentSite = new SITE { siteId = Guid.NewGuid(), site_name = selectedSite };
                        db.SITES.Add(currentSite);
                        db.SaveChanges();
                    }
                    newcommande.siteId = currentSite.siteId;
                    newcommande.SITE = currentSite;
                }

                newcommande.consigne_autres = StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_autres));
                newcommande.ordrePriorite = model.ordrePriorite;
                 newcommande.date_livraison = model.date_livraison;
                newcommande.date_cmde = DateTime.Now;
                int? maxRef = db.COMMANDEs.Max(u => u.commandeREF);
                newcommande.commandeREF = (maxRef != null) ? maxRef + 1 : 1;
                newcommande.commandeId = Guid.NewGuid();

                COMMANDEViewModel cmd = SetCommandeViewModelDetails(newcommande);

                bool limit = IsLimiteVolumeEnCours(newcommande.commandeRedacteurId,model.date_livraison,newcommande.nombre_mots) ; //total volume journalier en cours
               

                if (limit && Session["VolumeInfo"] == null)
                {
                    Session["VolumeInfo"] = "Le volume journalier pour le rédacteur " + newcommande.REDACTEUR.userNom + " est atteint. Vous confirmez l'envoi de la commande ? ";
                    Session["cmdeEditModif"] = cmd;
                    return View("Create", cmd);
                }
                if (model.listRedacteurId == Guid.Empty)
                {
                    ViewBag.ErrorRedacteur = true;
                    return View("Create", cmd);
                }

                Session["cmdeEditModif"] = null;
                Session["VolumeInfo"] = null;

                db.COMMANDEs.Add(newcommande);
                db.SaveChanges();
                /*update volume redacteur*/
                var redact = newcommande.REDACTEUR;
                int? volumeRestant = 0;
                if (redact != null)
                     volumeRestant = GetVolumeRestant(newcommande.commandeRedacteurId);
                redact.redactVolumeRestant = volumeRestant.ToString();

                try
                {
                    int res = db.SaveChanges();
                    if(res > 0)
                    {
                        if (notifSms)
                        {
                            string msgBody = "Vous avez une nouvelle commande.";
                            var accountSid =
                                System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"];
                            var authToken = System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"];
                            var phonenumber = System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"];

                            TwilioClient.Init(accountSid, authToken);
                            if (newcommande.REDACTEUR != null)
                            {
                                string redactNumber = newcommande.REDACTEUR.redactPhone;
                                redactNumber = redactNumber.TrimStart(new char[] { '0' });
                                redactNumber = "+261" + redactNumber;
                              
                                var to = new PhoneNumber(redactNumber);
                                var message = MessageResource.Create(
                                    to,
                                    @from: new PhoneNumber(phonenumber),
                                    body: msgBody);

                                if (!string.IsNullOrEmpty(message.Sid))
                                {
                                    Console.WriteLine(message.Sid);
                                    //return View("Create");
                                }
                            }
                        }

                        if (Request.Url != null)
                        {
                            var url = Request.Url.Scheme;
                            if (Request.Url != null)
                            {

                                string callbackurl = Request.Url.Host != "localhost"
                                    ? Request.Url.Host
                                    : Request.Url.Authority;
                                var port = Request.Url.Port;
                                if (!string.IsNullOrEmpty(port.ToString()) && Request.Url.Host != "localhost")
                                    callbackurl += ":" + port;

                                url += "://" + callbackurl;
                            }


                            StringBuilder mailBody = new StringBuilder();
                            if (newcommande.REDACTEUR != null)
                            {
                                mailBody.AppendFormat(
                                    "Monsieur / Madame " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newcommande.REDACTEUR.userNom
                                        .ToLower()));
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat(
                                    "<p>Vous venez de recevoir une de commande de " + newcommande.REFERENCEUR.userNom + " " + newcommande.REFERENCEUR.userPrenom + " le " + DateTime.Now + ".Veuillez cliquer sur le lien suivant pour accepter ou refuser la commande.</p>");
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat(url + "/Commandes/CommandeWaitting?token=" + newcommande.commandeId);
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat("Cordialement,");
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat("Media click App .");

                                bool isSendMail = MailClient.SendMail(newcommande.REDACTEUR.userMail,
                                    mailBody.ToString(), "Media click App - nouvelle commande.");
                                if (isSendMail)
                                {
                                    newcommande.commandeToken = newcommande.commandeId;
                                    newcommande.dateToken = DateTime.Now;
                                    int result = db.SaveChanges();
                                    if (result > 0)
                                    {
                                        string notif = "Vous venez de recevoir une de commande de " + newcommande.REFERENCEUR.userNom + " " + newcommande.REFERENCEUR.userPrenom + " le " + DateTime.Now + ".Veuillez accepter ou refuser la commande.";

                                        if (SendNotification(newcommande, newcommande.commandeReferenceurId, newcommande.commandeRedacteurId, notif) > 0)
                                            return View("CreateCommandeConfirmation");

                                        return RedirectToRoute("Home", new RouteValueDictionary
                                    {
                                        {"controller", "Commandes"},
                                        {"action", "Create"}
                                    });

                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        return View("ErrorException");
                    }
                }
                catch (Exception ex)
                {

                    return RedirectToRoute("Home", new RouteValueDictionary
                    {
                        {"controller", "Commandes"},
                        {"action", "Create"}
                    });
                }
            }

            return View("ErrorException");
        }

        public ActionResult CommandeConfirmationVolume()
        {
            return View("CreateCommandeConfirmation");
        }


        /*pour aller a la page de modification du mot de passe*/
        public ActionResult CommandeWaitting(Guid? token)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            COMMANDE commande = db.COMMANDEs.SingleOrDefault(x => x.commandeToken == token);
            COMMANDEViewModel commandeVm = new Commandes().GetDetailsCommande(token);
            /*l'utilisateur est null si le token n'existe pas/plus dans la base de donnees*/
            if (commande == null)
            {
                return View("ExpiredLink");
            }

            DateTime now = DateTime.Now;
            if (commande.dateToken != null)
            {
                DateTime dateToken = (DateTime)commande.dateToken;
                double nbrTime = (now - dateToken).TotalMinutes;
                if (nbrTime > 60.0)
                {
                    return View("ExpiredLink");
                }
            }
            Session["tokenCmde"] = token;
            ViewBag.hashCmde = token;
            return View(commandeVm);
        }

      

        /*pour aller a la page de modification du mot de passe*/
        public ActionResult AcceptCommande(Guid? hash)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            var tokenCmde = !string.IsNullOrEmpty(hash.ToString())?hash: Session["tokenCmde"];
            COMMANDE commande;
            if (tokenCmde != null)
            {
                commande = db.COMMANDEs.SingleOrDefault(x =>x.commandeToken == (Guid) tokenCmde);


                /*l'utilisateur est null si le token n'existe pas/plus dans la base de donnees*/
                if (commande == null)
                {
                    return View("ExpiredLink");
                }

                commande.commandeToken = null;
                commande.dateToken = null;
                var status = db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("En cours"));
                commande.STATUT_COMMANDE = status;
                if (status != null) commande.commandeStatutId = status.statutCommandeId;
                db.SaveChanges();
                string mailbody = "<p> Votre  commande " + commande.commandeREF + " a été bien acceptée par "+commande.REDACTEUR.userNom+ " le " + DateTime.Now.ToString("dd/MM/yyyy") + ".</p>";
                SendNotification(commande,commande.commandeRedacteurId, commande.commandeReferenceurId, Regex.Replace(mailbody, "<.*?>", String.Empty));
            }

            return RedirectToRoute("Home", new RouteValueDictionary
            {
                {"controller", "Commandes"},
                {"action", "ListCommandes"}
            });
        }

       
        public ActionResult ValidCommande(Guid? hash)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
          
            COMMANDE commande;
            commande = db.COMMANDEs.SingleOrDefault(x => x.commandeId == hash);


               
                var status = db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("Validé"));
                commande.STATUT_COMMANDE = status;
                if (status != null) commande.commandeStatutId = status.statutCommandeId;
                db.SaveChanges();
               
            string mailbody = "<p> Votre livraison "+commande.commandeREF + " a été bien validée le " + DateTime.Now.ToString("dd/MM/yyyy") + ", nous vous remercions de votre collaboration.</p>";
                string mailobject = "Media click App - Validation de la commande";
            bool isSendMail = SendeMailNotification(commande, mailbody, mailobject);

            if (isSendMail)
            {
                SendNotification(commande, commande.commandeReferenceurId, commande.commandeRedacteurId, Regex.Replace(mailbody, "<.*?>", String.Empty));
                return RedirectToRoute("Home", new RouteValueDictionary{
                {"controller", "Commandes"},
                {"action", "ListCommandes"}
            });
            }
                
            
            else
                return View("ErrorException");
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        [MvcApplication.CheckSessionOut]
        public ActionResult CancelCommande(Guid idCommande, COMMANDEViewModel model)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();

            COMMANDE commande;
            commande = db.COMMANDEs.SingleOrDefault(x => x.commandeId == idCommande);
            
            var status = db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("Annulé"));

            if (status != null) {
                commande.commandeStatutId = status.statutCommandeId;
                commande.remarques = model.remarques;
                int? wordsCount = commande.nombre_mots;
                var redact = commande.REDACTEUR;
                if (redact != null)
                {
                  
                    int? volume = GetVolumeEnCours(commande.commandeRedacteurId,model.date_livraison) - wordsCount;
                    int? redactVolume = !string.IsNullOrEmpty(redact.redactVolume) ? int.Parse(redact.redactVolume) : 0;
                    int? redactVolumeRestant = redactVolume - volume;
                    redact.redactVolumeRestant = redactVolumeRestant.ToString();
                }
            }
            db.SaveChanges();
          
            string mailbody = "<p> La commande " + commande.commandeREF + " a été annulée par "+commande.REFERENCEUR.userNom +" "+commande.REFERENCEUR.userPrenom+" le "+ DateTime.Now.ToString("dd/MM/yyyy")+" , vous pouvez contacter le responsable pour plus de détails.</p>";
            string mailobject = "Media click App - Annulation de la commande";
            bool isSendMail = SendeMailNotification(commande, mailbody, mailobject);

            if (isSendMail)
            {
                SendNotification(commande, commande.commandeReferenceurId, commande.commandeRedacteurId, Regex.Replace(mailbody, "<.*?>", String.Empty));
                return RedirectToRoute("Home", new RouteValueDictionary
            {
                {"controller", "Commandes"},
                {"action", "ListCommandes"}
            });
            }
              
            else
                return View("ErrorException");
        }

        public ActionResult CommandeCancel(Guid? hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.hashCmde = hash;
            var commande = db.COMMANDEs.Find(hash);
            COMMANDEViewModel commandeVm = SetCommandeViewModelDetails(commande);
            if (commandeVm != null)
                return View(commandeVm);
            return View("ErrorException");
        }

       

    public ActionResult CommandeRefuser(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.hashCmde = hash;
            var commande = db.COMMANDEs.Find(hash);
            COMMANDEViewModel commandeVm = SetCommandeViewModelDetails(commande);
            if (commandeVm != null)
                return View(commandeVm);
            return View("ErrorException");
        }


        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        [MvcApplication.CheckSessionOut]
        public ActionResult RefuserCommande(Guid idCommande, COMMANDEViewModel model)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();

            COMMANDE commande;
            commande = db.COMMANDEs.SingleOrDefault(x => x.commandeId == idCommande);



            var status = db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("Refusé"));
            commande.STATUT_COMMANDE = status;
            bool isSendMail = false;
            string mailbody = "";

            if (Session["role"] != null && Session["role"].ToString() == "2")
            {
                if (status != null)
                {
                    commande.commandeStatutId = status.statutCommandeId;
                    commande.remarques = "Refus du redacteur :" + model.remarques;
                }
                isSendMail = db.SaveChanges() > 0 ;
             
                 mailbody = "Votre commande " + commande.commandeREF + " a été refusé par le rédacteur le " + DateTime.Now.ToString("dd/MM/yyyy") + ", vous pouvez  contacter " + commande.REDACTEUR.userNom.ToUpper() +" pour plus de détails.";
                if (isSendMail)               
                        SendNotification(commande, commande.commandeRedacteurId, commande.commandeReferenceurId, Regex.Replace(mailbody, "<.*?>", String.Empty));
                else
                    return View("ErrorException");
            }
            else
            {               
                    if (status != null)
                    {
                        commande.commandeStatutId = status.statutCommandeId;
                        commande.remarques = model.remarques;
                    }
                    db.SaveChanges();
                  
                     mailbody = "<p> Votre commande " + commande.commandeREF + " a été refusé, vous pouvez contacter le responsable pour plus de détails.</p>";
                    string mailobject = "Media click App - Refus de la commande";
                    isSendMail = SendeMailNotification(commande, mailbody, mailobject);
                if (isSendMail)
                    SendNotification(commande, commande.commandeReferenceurId, commande.commandeRedacteurId, Regex.Replace(mailbody, "<.*?>", String.Empty));
                else
                    return View("ErrorException");
            }         
               
                return RedirectToRoute("Home", new RouteValueDictionary
            {
                {"controller", "Commandes"},
                {"action", "ListCommandes"}
            });
       
               
            
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        [MvcApplication.CheckSessionOut]      
        public ActionResult CorrectCommande(Guid idCommande, COMMANDEViewModel model)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();

            COMMANDE commande;
            commande = db.COMMANDEs.SingleOrDefault(x => x.commandeId == idCommande);



            var status = db.STATUT_COMMANDE.SingleOrDefault(s => s.statut_cmde.Contains("A corriger"));
            commande.STATUT_COMMANDE = status;
            if (status != null) commande.commandeStatutId = status.statutCommandeId;
            commande.remarques = model.remarques;
            db.SaveChanges();
           
            string mailbody = "<p> "+commande.REFERENCEUR.userNom + " " + commande.REFERENCEUR.userPrenom + " a demandé un correctif sur votre livraison " + commande.commandeREF + ".veuillez revoir la commande s'il vous plait. Nous vous remercions de votre collaboration.</p>";
            string mailobject = "Media click App - Demande de correction de la commande";

            string notif = commande.REFERENCEUR.userNom + " " + commande.REFERENCEUR.userPrenom + " a demandé un correctif sur votre livraison " + commande.commandeREF + ".veuillez revoir la commande s'il vous plait. Nous vous remercions de votre collaboration.";
            bool isSendMail = SendeMailNotification(commande, mailbody, mailobject);
            
            if (isSendMail)
            {
                SendNotification(commande, commande.commandeReferenceurId, commande.commandeRedacteurId, notif);

                return RedirectToRoute("Home", new RouteValueDictionary
                {
                    {"controller", "Commandes"},
                    {"action", "ListCommandes"}
                });
            }

            else
                return View("ErrorException");
        }

        public ActionResult CommandeUpdateNote(Guid hash)
        {
            Guid user = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.hashCmde = hash;
            var commande = db.COMMANDEs.Find(hash);
            COMMANDEViewModel commandeVm = SetCommandeViewModelDetails(commande);
            if (commandeVm != null)
                return View(commandeVm);
            return View("ErrorException");
        }

        public int SendNotification( COMMANDE commande,Guid? fromId, Guid? toId,string message )
        {
            var notif = new  NOTIFICATION();
            notif.commandeId = commande.commandeId;
            notif.fromId = fromId;
            notif.toId = toId;
            notif.statut = true;
            notif.datenotif = DateTime.Now;
            notif.message = message;
            notif.notificationId = Guid.NewGuid();
            db.NOTIFICATIONs.Add(notif);
            var res = db.SaveChanges();

            return res;
        }


        public ActionResult RelanceSMS(Guid hash)
        {
            var commande = db.COMMANDEs.Find(hash);
            string msgBody = "Nous voulons vous relancer sur la commande numéro " + commande.commandeREF;
            var accountSid =
                System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"];
            var authToken = System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"];
            var phonenumber = System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"];
            int res = 0;
            TwilioClient.Init(accountSid, authToken);
            if (commande.REDACTEUR != null)
            {
                string redactNumber = "+" + commande.REDACTEUR.redactPhone.Trim();
                var to = new PhoneNumber(redactNumber);
                var message = MessageResource.Create(
                    to,
                    @from: new PhoneNumber(phonenumber),
                    body: msgBody);

                if (!string.IsNullOrEmpty(message.Sid))
                {
                    Console.WriteLine(message.Sid);
                    res = 1;
                }
            }
            if(res == 1)
            {

                return View("RelanceCommandeConfirmation");
            }
            else
            {
                return View("ErrorException"); 
            }
          
        }

        public ActionResult RelanceMail(Guid hash)
        {
            var newcommande = db.COMMANDEs.Find(hash);

            if (Request.Url != null)
            {
                var url = Request.Url.Scheme;
                if (Request.Url != null)
                {

                    string callbackurl = Request.Url.Host != "localhost"
                        ? Request.Url.Host
                        : Request.Url.Authority;
                    var port = Request.Url.Port;
                    if (!string.IsNullOrEmpty(port.ToString()) && Request.Url.Host != "localhost")
                        callbackurl += ":" + port;

                    url += "://" + callbackurl;
                }


                StringBuilder mailBody = new StringBuilder();
                if (newcommande.REDACTEUR != null)
                {
                    mailBody.AppendFormat(
                        "Monsieur / Madame " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newcommande.REDACTEUR.userNom
                            .ToLower()));
                    mailBody.AppendFormat("<br />");
                    mailBody.AppendFormat(
                        "<p>Vous venez de recevoir une commande de " + newcommande.REFERENCEUR.userNom + " " + newcommande.REFERENCEUR.userPrenom + " le " + DateTime.Now.ToString("dd/MM/yyyy") + ".Veuillez cliquer sur le lien suivant pour accepter ou refuser la commande.</p>");
                    mailBody.AppendFormat("<br />");
                    mailBody.AppendFormat(url + "/Commandes/CommandeWaitting?token=" + newcommande.commandeId);
                    mailBody.AppendFormat("<br />");
                    mailBody.AppendFormat("Cordialement,");
                    mailBody.AppendFormat("<br />");
                    mailBody.AppendFormat("Media click App .");

                    bool isSendMail = MailClient.SendMail(newcommande.REDACTEUR.userMail,
                        mailBody.ToString(), "Media click App  - nouvelle commande.");
                    if (isSendMail)
                    {
                        newcommande.commandeToken = newcommande.commandeId;
                        newcommande.dateToken = DateTime.Now;
                        int result = db.SaveChanges();
                        if (result > 0)
                        {
                            string notif = "Vous venez de recevoir une de commande de " + newcommande.REFERENCEUR.userNom + " " + newcommande.REFERENCEUR.userPrenom + " le " + DateTime.Now + ".Veuillez accepter ou refuser la commande.";
                            if (SendNotification(newcommande, newcommande.commandeReferenceurId, newcommande.commandeRedacteurId, notif) > 0)
                                 return View("RelanceCommandeConfirmation");                           

                        }
                    }
                }
            }
            return View("ErrorException");


        }


        private bool SendeMailNotification(COMMANDE newcommande,string mailbody,string mailobject)
        {
            bool isSendMail = false;

              var url = Request.Url.Scheme;
            if (Request.Url != null)
            {

                string callbackurl = Request.Url.Host != "localhost"
                    ? Request.Url.Host
                    : Request.Url.Authority;
                var port = Request.Url.Port;
                if (!string.IsNullOrEmpty(port.ToString()) && Request.Url.Host != "localhost")
                    callbackurl += ":" + port;

                url += "://" + callbackurl;
            }


            StringBuilder mailBody = new StringBuilder();
            if (newcommande.REDACTEUR != null)
            {
                mailBody.AppendFormat(
                    "Monsieur / Madame " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(newcommande.REDACTEUR.userNom
                        .ToLower()));
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat(mailbody);
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat(url + "/Commandes/ListCommandes");
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat("Cordialement,");
                mailBody.AppendFormat("<br />");
                mailBody.AppendFormat("Media click App .");

                isSendMail = MailClient.SendMail(newcommande.REDACTEUR.userMail,
                    mailBody.ToString(), mailobject);
            }
            return isSendMail;
            }

        public int UpdateStatutNotification(Guid? notificationId)
        {
            var notif =  db.NOTIFICATIONs.SingleOrDefault(x=>x.notificationId == notificationId);
            
            notif.statut = false;
            
            
            var res = db.SaveChanges();

            return res;

        }


        // GET: COMMANDEs/Edit/5
        public ActionResult Edit(Guid? hash)
        {
            if (hash == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            Commandes val = new Commandes();
            var currentCommande = val.GetDetailsCommande(hash);

            if (Session["cmdeEditModif"] != null)
                currentCommande = (COMMANDEViewModel)Session["cmdeEditModif"];

            currentCommande.ListProjet = val.GetListProjetItem();
            currentCommande.ListProjet = val.GetListProjetItem();
            currentCommande.ListTheme = val.GetListThemeItem();
            currentCommande.ListRedacteur = val.GetListRedacteurItem();
            currentCommande.ListCommandeType = val.GetListCommandeTypeItem();
            currentCommande.ListTag = val.GetListTagItem();
            currentCommande.ListStatut = val.GetListStatutItem();
            if (currentCommande.commandeProjetId != null)
                currentCommande.listprojetId = (Guid) currentCommande.commandeProjetId;

            if (currentCommande.commandeThemeId != null)
                currentCommande.listThemeId = (Guid)currentCommande.commandeThemeId;

            if (currentCommande.commandeTypeId != null)
                currentCommande.listCommandeTypeId = (Guid)currentCommande.commandeTypeId;

            if (currentCommande.commandeRedacteurId != null)
                currentCommande.listRedacteurId = (Guid)currentCommande.commandeRedacteurId;

            if (currentCommande.tagId != null)
                currentCommande.listTagId = (Guid)currentCommande.tagId;


            if (currentCommande.commandeStatutId != null)
                currentCommande.listStatutId = (Guid)currentCommande.commandeStatutId;

            Session["cmdeEditModif"] = null;
            return View(currentCommande);
           
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        [MvcApplication.CheckSessionOut]
        public ActionResult EditCommande(Guid idCommande, COMMANDEViewModel model, FormCollection collection)
        {
            var hash = idCommande;
            COMMANDE commande = db.COMMANDEs.Find(hash);
            Guid? fromId =_userId;
            Guid? toId;
            if (commande != null)
            {
                bool notifSms = false;
                if (Session["role"] != null && Session["role"].ToString() == "2")
                {
                    string etat = "Livré";
                    var statut = db.STATUT_COMMANDE.SingleOrDefault(x => x.statut_cmde.Contains(etat));
                    commande.commandeStatutId = statut.statutCommandeId;

                    commande.balise_titre = model.balise_titre;
                    commande.contenu_livre = model.contenu_livre;

                    commande.dateLivraisonReel = DateTime.Now;
                    toId = commande.commandeReferenceurId;
                    string contenu = Regex.Replace(commande.contenu_livre, "<.*?>", string.Empty);


                    if (contenu.Split(' ').ToList().Count < commande.nombre_mots)
                    {
                        ViewBag.ErrorMessage = "Le nombre de mots à livrer est inférieur à celui demandé ("+ commande.nombre_mots + "), veuillez revoir votre contenu.";
                        COMMANDEViewModel cmd = SetCommandeViewModelDetails(commande);
                        return View("Edit", cmd);
                    }
                    
                }

                else
                {
                    toId = commande.commandeRedacteurId;
                    var selectedProjetId = model.listprojetId;
                    var selectedThemeId = model.listThemeId;
                    var selectedRedacteurId = model.listRedacteurId;
                    var selectedCommandeTypeId = model.listCommandeTypeId;
                    var selectedTag = model.tag;
                    var selectedReferenceurId = Guid.Parse(HttpContext.User.Identity.Name);
                    var selectedSite = model.site;
                    
                    var selectedStatut = model.listStatutId;

                    model.mot_cle_secondaire =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_secondaire));
                    model.texte_ancrage =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.texte_ancrage));
                    model.consigne_references =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_references));
                    model.consigne_autres =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_autres));
                   
                    if (!string.IsNullOrEmpty(collection["checkResp"]))
                    {
                        string checkResp = collection["checkResp"];
                        notifSms = checkResp == "on";
                    }
                    
                    commande.PROJET = db.PROJETS.Find(selectedProjetId);
                    commande.commandeProjetId = selectedProjetId;
                    commande.THEME = db.THEMES.Find(selectedThemeId);
                    commande.commandeThemeId = selectedThemeId;
                    commande.REFERENCEUR = db.UTILISATEURs.Find(selectedReferenceurId);
                    commande.commandeReferenceurId = selectedReferenceurId;
                    commande.REDACTEUR = db.UTILISATEURs.Find(selectedRedacteurId);
                    commande.commandeRedacteurId = selectedRedacteurId;

                    commande.COMMANDE_TYPE = db.COMMANDE_TYPE.Find(selectedCommandeTypeId);
                    commande.commandeTypeId = selectedCommandeTypeId;

                    if (!string.IsNullOrEmpty(selectedTag))
                    {
                        TAG currentTag = db.TAGS.SingleOrDefault(x => x.type.Contains(selectedTag.TrimEnd()));
                        if (currentTag == null)
                        {
                            currentTag = new TAG { tagId = Guid.NewGuid(), type = selectedTag };
                            db.TAGS.Add(currentTag);
                            db.SaveChanges();
                        }
                        commande.tagId = currentTag.tagId;
                        commande.TAG = currentTag;
                    }

                    if (!string.IsNullOrEmpty(selectedSite))
                    {
                        SITE currentSite = db.SITES.SingleOrDefault(x => x.site_name.Contains(selectedSite.TrimEnd()));
                        if (currentSite == null)
                        {
                            currentSite = new SITE { siteId = Guid.NewGuid(), site_name = selectedSite };
                            db.SITES.Add(currentSite);
                            db.SaveChanges();
                        }
                        commande.siteId = currentSite.siteId;
                        commande.SITE = currentSite;
                    }



                    commande.commandeStatutId = selectedStatut;
                    commande.mot_cle_pricipal =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_pricipal));
                    commande.mot_cle_secondaire =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.mot_cle_secondaire));
                    commande.texte_ancrage =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.texte_ancrage));
                    commande.nombre_mots = model.nombre_mots;
                    commande.consigne_references = model.consigne_references;
                    commande.tagId = model.listTagId;
                    commande.consigne_autres =
                        StatePageSingleton.SanitizeString(Sanitizer.GetSafeHtmlFragment(model.consigne_autres));
                    commande.ordrePriorite = model.ordrePriorite;
                    commande.date_livraison = model.date_livraison;
                    bool limit = IsLimiteVolumeEnCours(commande.commandeRedacteurId, model.date_livraison, commande.nombre_mots); //total volume journalier en cours


                    if (limit && Session["VolumeInfo"] == null)
                    {
                        Session["VolumeInfo"] = "Le volume journalier pour le rédacteur " + commande.REDACTEUR.userNom + " est atteint. Vous confirmez l'envoi de la commande ? ";
                        COMMANDEViewModel cmd = SetCommandeViewModelDetails(commande);
                        return View("Edit", cmd);
                    }                    
                }
            
            try
                {
                    Session["VolumeInfo"] = null;
                    int result = db.SaveChanges();
                    if (result > 0)
                    {
                        if (notifSms)
                        {
                            string msgBody = "Vous avez une commande mise à jour.";
                            var accountSid =
                                System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"];
                            var authToken = System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"];
                            var phonenumber = System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"];

                            TwilioClient.Init(accountSid, authToken);
                            if (commande.REDACTEUR != null)
                            {
                                string redactNumber = commande.REDACTEUR.redactPhone;
                                redactNumber = redactNumber.TrimStart(new char[] { '0' });
                                redactNumber = "+261" + redactNumber;
                                var to = new PhoneNumber(redactNumber);
                                var message = MessageResource.Create(
                                    to,
                                    @from: new PhoneNumber(phonenumber),
                                    body: msgBody);

                                if (!string.IsNullOrEmpty(message.Sid))
                                {
                                    Console.WriteLine(message.Sid);

                                }
                            }
                        }

                        if (Request.Url != null)
                        {
                            var url = Request.Url.Scheme;
                            if (Request.Url != null)
                            {

                                string callbackurl = Request.Url.Host != "localhost"
                                    ? Request.Url.Host
                                    : Request.Url.Authority;
                                var port = Request.Url.Port;
                                if (!string.IsNullOrEmpty(port.ToString()) && Request.Url.Host != "localhost")
                                    callbackurl += ":" + port;

                                url += "://" + callbackurl;
                            }


                            StringBuilder mailBody = new StringBuilder();
                            if (commande.REDACTEUR != null)
                            {
                                mailBody.AppendFormat(
                                    "Monsieur / Madame " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(commande.REDACTEUR.userNom
                                        .ToLower()));
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat(
                                    "<p>Vous avez une commande à corriger. Veuillez cliquer sur le lien suivant pour voir les détails de la commande.</p>");
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat(url + "/Commandes/DetailsCommande?hash=" + commande.commandeId);
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat("Cordialement,");
                                mailBody.AppendFormat("<br />");
                                mailBody.AppendFormat("Media click App .");

                                bool isSendMail = MailClient.SendMail(commande.REDACTEUR.userMail,
                                    mailBody.ToString(), "Media click App  - nouvelle commande.");
                                if (isSendMail)
                                {
                                    string notif = "Vous avez une commande à corriger de " + commande.REFERENCEUR.userNom + " " + commande.REFERENCEUR.userPrenom + " le " + DateTime.Now + ".Veuillez regarder les détails de la commande.";
                                    if (Session["role"] != null && Session["role"].ToString() == "2")
                                         notif = "La commande "+commande.commandeREF + " a été livrée  par " + commande.REDACTEUR.userNom +"le " + DateTime.Now.ToShortDateString() ;

                                    if (SendNotification(commande,fromId,toId, notif) > 0)
                                        return View("EditCommandeConfirmation");

                                    return RedirectToRoute("Home", new RouteValueDictionary
                                    {
                                        {"controller", "Commandes"},
                                        {"action", "Edit"}
                                    });

                                }
                            }
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    Session["cmdeEditModif"] = model;
                    return RedirectToRoute("Home", new RouteValueDictionary
                    {
                        {"controller", "Commandes"},
                        {"action", "Edit"}
                    });
                }
            }

            return View("ErrorException");

        }

        // GET: COMMANDEs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            COMMANDE cOMMANDE = db.COMMANDEs.Find(id);
            if (cOMMANDE == null)
            {
                return HttpNotFound();
            }
            return View(cOMMANDE);
        }

        // POST: COMMANDEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            COMMANDE cOMMANDE = db.COMMANDEs.Find(id);
            db.COMMANDEs.Remove(cOMMANDE);
            db.SaveChanges();
            return RedirectToAction("Index");
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
