using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace RedactApplication.Models
{
    class Commandes
    {       
        public List<COMMANDEViewModel> GetListCommande()
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var req = db.COMMANDEs.ToList();
            List<COMMANDEViewModel> listeCmde = new List<COMMANDEViewModel>();
            foreach (var commande in req)
            {
                var referenceur = this.GetUtilisateurReferenceur(commande.commandeReferenceurId);
                var cmdeType = this.GetCommandeType(commande.commandeTypeId);
                var tags = this.GetCommandeTag(commande.tagId);
                var redacteur = this.GetUtilisateurReferenceur(commande.commandeRedacteurId);
                string priorite = commande.ordrePriorite == "0" ? "Moyen" : "Haut";
                var projet = GetProjet(commande.commandeProjetId);
                var theme = GetTheme(commande.commandeThemeId);
                var statut =  GetStatutCommande(commande.commandeStatutId);
                var site = this.GetCommandeSite(commande.siteId);

                listeCmde.Add(new COMMANDEViewModel()
                {
                    commandeId = commande.commandeId,
                    commandeDemandeur = referenceur.userNom,
                    commandeReferenceurId = referenceur.userId,
                    commandeRedacteur = redacteur.userNom,
                    commandeRedacteurId = redacteur.userId,
                    date_cmde = commande.date_cmde,
                    date_livraison = commande.date_livraison,
                    commandeType = cmdeType.Type,
                    nombre_mots = commande.nombre_mots,
                    mot_cle_pricipal = commande.mot_cle_pricipal,
                    mot_cle_secondaire = commande.mot_cle_secondaire,
                    consigne_references = commande.consigne_references,
                    texte_ancrage = commande.texte_ancrage,
                    consigne_autres = commande.consigne_autres,
                    etat_paiement = commande.etat_paiement,
                    commandeREF = commande.commandeREF,
                    ordrePriorite = priorite,
                    balise_titre = commande.balise_titre,
                    contenu_livre = commande.contenu_livre,
                    projet = projet.projet_name,
                    thematique = theme.theme_name,
                    tag = (tags != null) ? tags.type : "",
                    site = (site != null) ? site.site_name : "",
                    statut_cmde = (statut != null) ? statut.statut_cmde : "",
                    commandeThemeId = commande.commandeThemeId,
                    commandeStatutId = commande.commandeStatutId,
                    commandeTypeId = commande.commandeTypeId,

                    dateLivraisonReel = commande.dateLivraisonReel,
                    remarques = commande.remarques

                });
                
            }
            return listeCmde.OrderBy(x => x.date_livraison).ToList();
           
        }



        public List<string> GetThemes(Guid? redactGuid)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            var themes = from c in db.THEMES
                         from p in db.REDACT_THEME
                         where p.themeId == c.themeId && p.redactId == redactGuid
                         select c.theme_name;
            return themes.ToList();
        }


        public List<COMMANDEViewModel> GetListCommande(int? numpage, int? nbrow)
        {
            try
            {
                List<COMMANDEViewModel> data = GetListCommande();
                int offset = 0;

                if (nbrow != 10 && nbrow != 50)
                {
                    return data.Distinct().ToList();
                }
                else
                {
                    if (numpage != null) offset = (int) ((numpage - 1) * nbrow);
                    if (offset != 0)
                    {
                        return data.Skip(offset).Take((int)nbrow).Distinct().ToList();
                    }
                    else
                    {
                        return data.Take((int)nbrow).Distinct().ToList();
                    }
                }
            }
            catch
            {
            }
            return new List<COMMANDEViewModel>();
        }

        public COMMANDEViewModel GetDetailsCommande(Guid? commandeId)
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var commande = db.COMMANDEs.Find(commandeId);


            var referenceur = commande.commandeReferenceurId != null ? this.GetUtilisateurReferenceur(commande.commandeReferenceurId) : null;
            var cmdeType = this.GetCommandeType(commande.commandeTypeId);
            //var consigneType = this.GetCommandeContenuType(commande.consigne_type_contenuId);
            var redacteur = this.GetUtilisateurReferenceur(commande.commandeRedacteurId);
            string priorite = commande.ordrePriorite == "0" ? "Moyen" : "Haut";
            var projet = GetProjet(commande.commandeProjetId);
            var theme = GetTheme(commande.commandeThemeId);
            var statut = (!string.IsNullOrEmpty(commande.commandeStatutId.ToString())) ? GetStatutCommande(commande.commandeStatutId) : GetStatutCommande(new Guid());
            string statutcmde = (statut != null) ? statut.statut_cmde : "En attente";
            var tags = this.GetCommandeTag(commande.tagId);
            var site = this.GetCommandeSite(commande.siteId);
            var commandeVm = new COMMANDEViewModel();

            commandeVm.commandeId = commande.commandeId;
            commandeVm.commandeDemandeur = referenceur.userNom;
            commandeVm.commandeReferenceurId = referenceur.userId;
            commandeVm.commandeRedacteur = redacteur.userNom;
            commandeVm.commandeRedacteurId = redacteur.userId;
            commandeVm.date_cmde = commande.date_cmde;
            commandeVm.date_livraison = commande.date_livraison;
            commandeVm.commandeType = cmdeType.Type;
            commandeVm.nombre_mots = commande.nombre_mots;
            commandeVm.mot_cle_pricipal = commande.mot_cle_pricipal;
            commandeVm.mot_cle_secondaire = commande.mot_cle_secondaire;
            commandeVm.consigne_references = commande.consigne_references;
            commandeVm.texte_ancrage = commande.texte_ancrage;
            commandeVm.consigne_autres = commande.consigne_autres;
            commandeVm.etat_paiement = commande.etat_paiement;
            commandeVm.commandeREF = commande.commandeREF;
            commandeVm.ordrePriorite = priorite;
            commandeVm.balise_titre = commande.balise_titre;
            commandeVm.contenu_livre = commande.contenu_livre;
            commandeVm.projet = projet.projet_name;
            commandeVm.commandeProjetId = projet.projetId;
            commandeVm.thematique = theme.theme_name;
            commandeVm.commandeThemeId = theme.themeId;
            commandeVm.statut_cmde = statutcmde;
            commandeVm.tag = (tags != null) ? tags.type : "";
            commandeVm.site = (site != null) ? site.site_name : "";

            commandeVm.commandeStatutId = commande.commandeStatutId;
            commandeVm.commandeTypeId = commande.commandeTypeId;

            commandeVm.dateLivraisonReel = commande.dateLivraisonReel;
            commandeVm.remarques = commande.remarques;

            return commandeVm;

        }

        public UTILISATEUR GetUtilisateurReferenceur(Guid? id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == id);
            return utilisateur;
        }

        public List<Guid> GetRelatedTheme(string theme)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            var themes = db.THEMES.Where(x => x.theme_name.Contains(theme)).Select(t=>t.themeId).ToList();
            return themes;
        }

       
        public List<UTILISATEUR> GetRedateurOrderByTheme(string theme)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            var themesids = GetRelatedTheme(theme);
            var redactids = new List<Guid?>();
            foreach (var themeId in themesids)
            {
                var redactTheme = db.REDACT_THEME.FirstOrDefault(t => t.themeId == themeId);
                if (redactTheme != null)
                    redactids.Add(redactTheme.redactId);
            }
           
           
           // var redacteurs = db.UTILISATEURs.Where(u => redactids.Contains(u.userId)).OrderByDescending(n=>n.redactNiveau).ToList();


            var redacteurs = from c in db.UTILISATEURs
                             from p in db.UserRoles
                where p.idUser == c.userId && p.idRole == 2
                select c;

            var allredacteurs = redacteurs.OrderByDescending(n => n.redactNiveau).ToList();
            var redacteurSpec = db.UTILISATEURs.Where(u => redactids.Contains(u.userId)).OrderByDescending(n => n.redactNiveau).ToList();

            redacteurSpec.AddRange(allredacteurs);
            return redacteurSpec.Distinct().ToList();
        }

        public COMMANDE_TYPE GetCommandeType(Guid? id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            COMMANDE_TYPE commandeType = db.COMMANDE_TYPE.SingleOrDefault(x => x.commandeTypeId == id);
            return commandeType;
        }

        public TAG GetCommandeTag(Guid? id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            TAG tag = db.TAGS.SingleOrDefault(x => x.tagId == id);
            
            return tag;
        }

        public SITE GetCommandeSite(Guid? id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            SITE site = db.SITES.SingleOrDefault(x => x.siteId == id);

            return site;
        }

        public PROJET GetProjet(Guid? id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            PROJET projet = db.PROJETS.SingleOrDefault(x => x.projetId == id);
            return projet;
        }

        

        public IEnumerable<SelectListItem> GetListProjetItem()
        {
            using (var context = new redactapplicationEntities())
            {
                List<SelectListItem> listprojet = context.PROJETS.AsNoTracking()
                    .OrderBy(n => n.projet_name)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.projetId.ToString(),
                            Text = n.projet_name
                        }).ToList();
                var projetItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- selectionner projet ---"
                };
                listprojet.Insert(0, projetItem);
                return new SelectList(listprojet, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListThemeItem()
        {
            using (var context = new redactapplicationEntities())
            {
                List<SelectListItem> listtheme = context.THEMES.AsNoTracking()
                    .OrderBy(n => n.theme_name)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.themeId.ToString(),
                            Text = n.theme_name
                        }).ToList();
                var themeItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- selectionner thématique ---"
                };
                listtheme.Insert(0, themeItem);
                return new SelectList(listtheme, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListRedacteurItem()
        {
            using (var context = new redactapplicationEntities())
            {
                var redacteurs = from c in context.UTILISATEURs
                                 from p in context.UserRoles
                                 where p.idUser == c.userId && p.idRole == 2
                                 select c;

                List<SelectListItem> listredacteur = redacteurs
                    .OrderBy(n => n.userNom)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.userId.ToString(),
                            Text = n.userNom
                        }).ToList();
                var redacteurItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- selectionner rédacteur ---"
                };
                listredacteur.Insert(0, redacteurItem);
                return new SelectList(listredacteur, "Value", "Text");
            }
        }

       

        public IEnumerable<SelectListItem> GetListRedacteurItem(string themes)
        {
            using (new redactapplicationEntities())
            {

                List<SelectListItem> listredacteur = GetRedateurOrderByTheme(themes)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.userId.ToString(),
                            Text = n.userNom + " " +n.userPrenom
                        }).ToList();
                //var redacteurItem = new SelectListItem()
                //{
                //    Value = null,
                //    Text = "--- selectionner rédacteur ---"
                //};
                //listredacteur.Insert(0, redacteurItem);
                return new SelectList(listredacteur, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListCommandeTypeItem()
        {
            using (var context = new redactapplicationEntities())
            {
                List<SelectListItem> listcmdetype = context.COMMANDE_TYPE.AsNoTracking()
                    .OrderBy(n => n.Type)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.commandeTypeId.ToString(),
                            Text = n.Type
                        }).ToList();
                var typeItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- selectionner type de texte ---"
                };
                listcmdetype.Insert(0, typeItem);
                return new SelectList(listcmdetype, "Value", "Text");
            }
        }
        public IEnumerable<SelectListItem> GetListTagItem()
        {
            using (var context = new redactapplicationEntities())
            {
                List<SelectListItem> listTag= context.TAGS.AsNoTracking()
                    .OrderBy(n => n.type)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.tagId.ToString(),
                            Text = n.type
                        }).ToList();
                var typeItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- selectionner type de contenu ---"
                };
                listTag.Insert(0, typeItem);
                return new SelectList(listTag, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListTagItem(string term)
        {
            using (var context = new redactapplicationEntities())
            {
                var tags = context.TAGS.Where(x => x.type.StartsWith(term));
                List<SelectListItem> listTag = tags
                    .OrderBy(n => n.type)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.tagId.ToString(),
                            Text = n.type
                        }).ToList();
                
                return new SelectList(listTag, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListSitetem(string term)
        {
            using (var context = new redactapplicationEntities())
            {
                var sites = context.SITES.Where(x => x.site_name.StartsWith(term));
                List<SelectListItem> listSite = sites
                    .OrderBy(n => n.site_name)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.siteId.ToString(),
                            Text = n.site_name
                        }).ToList();

                return new SelectList(listSite, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListStatutItem()
        {
            using (var context = new redactapplicationEntities())
            {
                List<SelectListItem> listcontenutype = context.STATUT_COMMANDE.AsNoTracking()
                    .OrderBy(n => n.statut_cmde)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.statutCommandeId.ToString(),
                            Text = n.statut_cmde
                        }).ToList();
                var typeItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- selectionner type de statut ---"
                };
                listcontenutype.Insert(0, typeItem);
                return new SelectList(listcontenutype, "Value", "Text");
            }
        }

        public THEME GetTheme(Nullable<System.Guid> id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            THEME theme = db.THEMES.SingleOrDefault(x => x.themeId == id);
            return theme;
        }

        public STATUT_COMMANDE GetStatutCommande(Nullable<System.Guid> id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            STATUT_COMMANDE statutCommande = db.STATUT_COMMANDE.SingleOrDefault(x => x.statutCommandeId == id);
            return statutCommande;
        }

        public List<COMMANDEViewModel> SearchCommandes(string sites)
        {
            var temp = (new SearchData()).CommandesSearch(sites);

            List<COMMANDEViewModel> listCmde = temp?.Select(x => new COMMANDEViewModel
            {
                commandeId = x.commandeId,
                commandeDemandeur = GetUtilisateurReferenceur(x.commandeReferenceurId).userNom,
                date_cmde = x.date_cmde,
                date_livraison = x.date_livraison,
                commandeType = GetCommandeType(x.commandeTypeId).Type,
                nombre_mots = x.nombre_mots,
                mot_cle_pricipal = x.mot_cle_pricipal,
                mot_cle_secondaire = x.mot_cle_secondaire,
                consigne_references = x.consigne_references,
                consigneType = GetCommandeTag(x.tagId).type,
                consigne_autres = x.consigne_autres,
                etat_paiement = x.etat_paiement,
                commandeRedacteur = GetUtilisateurReferenceur(x.commandeRedacteurId).userNom,
                ordrePriorite = x.ordrePriorite == "0" ? "Moyen" : "Haut",
                balise_titre = x.balise_titre,
                contenu_livre = x.contenu_livre,
                projet = GetProjet(x.commandeProjetId).projet_name,
                thematique = GetTheme(x.commandeThemeId).theme_name,
                statut_cmde = GetStatutCommande(x.commandeStatutId).statut_cmde
            }).ToList();
            return listCmde?.OrderBy(x => x.date_cmde).ThenBy(x => x.date_livraison).ToList();
        }
    }
}