using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RedactApplication.Models;

namespace RedactApplication.Models
{
    /// <summary>
    /// Class implémentant les méthodes de recherche d'Utilisateur, de Contact , de Campaign.
    /// </summary>
    public class SearchData
    {
        /// <summary>
        /// Retourne une liste d'utilisateur suivant la recherche.
        /// </summary>
        /// <param name="valeur">chaine de recherche</param>
        /// <returns>List<UTILISATEUR></returns>
        public List<UTILISATEUR> UserSearch(string valeur)
        {
            if (string.IsNullOrEmpty(valeur) || (valeur.Trim()==""))
            {
                return null;
            }
            Func<List<UTILISATEUR>, string, List<UTILISATEUR>> testUserContaints = (userdata, userValue) =>
            {
                if (userdata != null)
                {
                    return (from user in userdata
                            where (user.userNom.ToLower().Contains(userValue.ToLower()) ||
                            user.userPrenom.ToLower().Contains(userValue.ToLower()) ||
                            user.userMail.ToLower().Contains(userValue.ToLower()) ||
                            user.redactSkype.ToLower().Contains(userValue.ToLower()) ||
                            user.redactModePaiement.ToLower().Contains(userValue.ToLower()) ||
                            user.redactNiveau.ToLower().Contains(userValue.ToLower()) ||
                            user.redactPhone.ToLower().Contains(userValue.ToLower()) ||
                            user.redactReferenceur.ToLower().Contains(userValue.ToLower()) ||
                            user.redactThemes.ToLower().Contains(userValue.ToLower()) ||
                            user.redactVolume.ToLower().Contains(userValue.ToLower()) ||
                            user.redactTarif.ToLower().Contains(userValue.ToLower()) ||
                            user.redactVolumeRestant.ToLower().Contains(userValue.ToLower()) 

                                   )
                            select user).Distinct().OrderBy(x => x.userNom).ThenBy(x => x.redactSkype).ToList();
                }
                return null;
            };
            List<string> str = new List<string>();
            int test = 0;
            if (valeur.Contains(" "))
            {
                str = valeur.Split(' ').ToList();
                test = 1;
            }
            redactapplicationEntities db = new redactapplicationEntities();
            List<UTILISATEUR> tempUser = new List<UTILISATEUR>();
            foreach (var u in db.UTILISATEURs.ToList())
            {
                var uRole = db.UserRoles.FirstOrDefault(x => x.idUser == u.userId);
                if (uRole != null && uRole.idRole != 5)
                {
                    tempUser.Add(u);
                }
            }
            switch (test)
            {
                case 0:
                    return testUserContaints(tempUser, valeur);
                case 1:
                    List<UTILISATEUR> data = new List<UTILISATEUR>();
                    List<UTILISATEUR> temp = new List<UTILISATEUR>();
                    foreach (var val in str)
                    {
                        data.AddRange(testUserContaints(tempUser, val));
                        data.AddRange(temp);
                    }
                    return data.Distinct().OrderBy(x => x.userNom).ThenBy(x => x.redactSkype).ToList();             
            }
            return null;
        }


        /// <summary>
        /// Retourne une liste des commandes suivant la recherche.
        /// </summary>
        /// <param name="valeur">chaine de recherche</param>
        /// <returns>List<COMMANDE></returns>
        public List<COMMANDE> CommandesSearch(string valeur)
        {
            if (string.IsNullOrEmpty(valeur) || (valeur.Trim() == ""))
            {
                return null;
            }
            Func<List<COMMANDE>, string, List<COMMANDE>> testCommandeContaints = (cmdedata, userValue) =>
            {
                if (cmdedata != null)
                {
                    return (from cmde in cmdedata
                            where (
                            cmde.date_cmde.ToString().ToLower().Contains(userValue.ToLower()) ||
                            cmde.date_livraison.ToString().ToLower().Contains(userValue.ToLower()) ||
                            cmde.ordrePriorite.ToString().ToLower().Contains(userValue.ToLower()))
                            select cmde).Distinct().OrderBy(x => x.date_cmde).ThenBy(x => x.date_livraison).ToList();
                }
                return null;
            };
            List<string> str = new List<string>();
            int test = 0;
            if (valeur.Contains(" "))
            {
                str = valeur.Split(' ').ToList();
                test = 1;
            }
            redactapplicationEntities db = new redactapplicationEntities();
            List<COMMANDE> tempCmde = new List<COMMANDE>();
            foreach (var u in db.COMMANDEs.ToList())
            {
                var uDEmandeur = db.UTILISATEURs.FirstOrDefault(x => x.userId == u.commandeReferenceurId);
                if (uDEmandeur != null)
                {
                    tempCmde.Add(u);
                }

                var uRedacteur= db.UTILISATEURs.FirstOrDefault(x => x.userId == u.commandeRedacteurId);
                if (uRedacteur != null)
                {
                    tempCmde.Add(u);
                }

                var uProjet = db.PROJETS.FirstOrDefault(x => x.projetId == u.commandeProjetId);
                if (uProjet != null)
                {
                    tempCmde.Add(u);
                }
                var uTheme = db.THEMES.FirstOrDefault(x => x.themeId == u.commandeThemeId);
                if (uTheme != null)
                {
                    tempCmde.Add(u);
                }

                var uCommandeType = db.COMMANDE_TYPE.FirstOrDefault(x => x.commandeTypeId == u.commandeTypeId);
                if (uCommandeType != null)
                {
                    tempCmde.Add(u);
                }

                var uStatutCmde = db.STATUT_COMMANDE.FirstOrDefault(x => x.statutCommandeId == u.commandeStatutId);
                if (uStatutCmde != null)
                {
                    tempCmde.Add(u);
                }

              
            }
           
            switch (test)
            {
                case 0:
                    return testCommandeContaints(tempCmde, valeur);
                case 1:
                    List<COMMANDE> data = new List<COMMANDE>();
                    List<COMMANDE> tempcmd = new List<COMMANDE>();
                    foreach (var val in str)
                    {
                        data.AddRange(testCommandeContaints(tempCmde, val));
                        data.AddRange(tempcmd);
                    }
                    return data.Distinct().OrderBy(x => x.date_cmde).ThenBy(x => x.date_livraison).ToList();
            }
            return null;
        }


        /// <summary>
        /// Retourne une liste des commandes suivant la recherche.
        /// </summary>
        /// <param name="valeur">chaine de recherche</param>
        /// <returns>List<COMMANDE></returns>
        public List<FACTUREViewModel> FactureSearch(string valeur)
        {
            if (string.IsNullOrEmpty(valeur) || (valeur.Trim() == ""))
            {
                return null;
            }
            Func<List<FACTUREViewModel>, string, List<FACTUREViewModel>> testFactureContaints = (facturedata, factureValue) =>
            {
                if (facturedata != null)
                {
                    return (from facture in facturedata
                            where (
                            facture.factureNumero.ToString().ToLower().Contains(factureValue.ToLower()) ||
                            facture.dateEmission.ToString().ToLower().Contains(factureValue.ToLower()) ||
                             facture.montant.ToString().ToLower().Contains(factureValue.ToLower()) ||
                               facture.etat.ToString().ToLower().Contains(factureValue.ToLower()) ||
                            facture.periode.ToString().ToLower().Contains(factureValue.ToLower()))
                            select facture).Distinct().OrderBy(x => x.dateEmission).ToList();
                }
                return null;
            };
            List<string> str = new List<string>();
            int test = 0;
            if (valeur.Contains(" "))
            {
                str = valeur.Split(' ').ToList();
                test = 1;
            }
            redactapplicationEntities db = new redactapplicationEntities();
            List<FACTUREViewModel> tempFacture = new List<FACTUREViewModel>();
            foreach (var u in new Factures().GetListFacture())
            {
                var uCommande = db.FACTUREs.FirstOrDefault(x => x.factureId == u.factureId);
                if (uCommande != null)
                {
                    tempFacture.Add(u);
                }
            }

            switch (test)
            {
                case 0:
                    return testFactureContaints(tempFacture, valeur);
                case 1:
                    List<FACTUREViewModel> data = new List<FACTUREViewModel>();
                    List<FACTUREViewModel> tempcmd = new List<FACTUREViewModel>();
                    foreach (var val in str)
                    {
                        data.AddRange(testFactureContaints(tempFacture, val));
                        data.AddRange(tempcmd);
                    }
                    return data.Distinct().OrderBy(x => x.dateEmission).ToList();
            }
            return null;
        }




    }
}