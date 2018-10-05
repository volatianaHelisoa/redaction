using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;


namespace RedactApplication.Models
{
    class Factures
    {
        public List<FACTUREViewModel> GetListFacture()
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var req = db.FACTUREs.ToList();
            List<FACTUREViewModel> listeFacture = new List<FACTUREViewModel>();
           
            foreach (var facture in req)
            {

                listeFacture.Add(new FACTUREViewModel()
                {
                    factureId = facture.factureId,
                    factureNumero = facture.factureNumero,
                    dateEmission = facture.dateEmission,
                    COMMANDEs = facture.COMMANDEs,
                    dateDebut = facture.dateDebut,
                    dateFin = facture.dateFin,
                    montant = facture.montant,
                    etat = facture.etat,
                    periode = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(facture.dateFin.Month),
                    redacteurId = facture.redacteurId,
                    REDACTEUR = db.UTILISATEURs.Find(facture.redacteurId)
            });

            }
            return listeFacture.OrderBy(x => x.dateEmission).ToList();
            
        }



        public FACTUREViewModel GetDetailsFacture(Guid? factureId)
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var facture = db.FACTUREs.Find(factureId);
                       
            var factureVm = new FACTUREViewModel();
            factureVm.factureId = facture.factureId;
            factureVm.factureNumero = facture.factureNumero;
            factureVm.dateEmission = facture.dateEmission;
            factureVm.COMMANDEs = facture.COMMANDEs;
            factureVm.dateDebut = facture.dateDebut;
            factureVm.dateFin = facture.dateFin;
            factureVm.montant = facture.montant;
            factureVm.etat = facture.etat;
            factureVm.redacteurId = facture.redacteurId;
            factureVm.REDACTEUR = db.UTILISATEURs.Find(facture.redacteurId);
            factureVm.createurId = facture.createurId;
            factureVm.periode = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(facture.dateFin.Month);
            

            return factureVm;

        }
        
        public IEnumerable<SelectListItem> GetListRedacteurItem()
        {
            using (var context = new redactapplicationEntities())
            {
                var redacteurs = from c in context.UTILISATEURs
                                 from p in context.UserRoles
                                 where p.idUser == c.userId && p.idRole == 2
                                 orderby c.userNom
                                 select c;

                List<SelectListItem> listredacteur = redacteurs
                     .Select(n =>
                        new SelectListItem
                        {
                            Value = n.userId.ToString(),
                            Text = n.userNom +" "+ n.userPrenom
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

        public List<FACTUREViewModel> SearchFacture(string facture)
        {
            var temp = (new SearchData()).FactureSearch(facture);
           
            return temp;
        }


    }
}