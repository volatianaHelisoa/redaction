using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class BaseviewModelCommande
    {
        public BaseviewModelCommande(Guid commandeId)
        {
            this.commandeId = commandeId;
        }

        public System.Guid commandeId { get; set; }
        public Nullable<System.Guid> commandeReferenceurId { get; set; }
        public Nullable<System.Guid> commandeRedacteurId { get; set; }
        public Nullable<System.DateTime> date_cmde { get; set; }
        public Nullable<System.DateTime> date_livraison { get; set; }
        public string ordrePriorite { get; set; }
        public Nullable<System.Guid> commandeTypeId { get; set; }
        public Nullable<int> nombre_mots { get; set; }
        public string mot_cle_pricipal { get; set; }
        public string mot_cle_secondaire { get; set; }
        public string texte_ancrage { get; set; }
        public string consigne_references { get; set; }
        public Nullable<System.Guid> consigne_type_contenuId { get; set; }
        public string consigne_autres { get; set; }
        public string balise_titre { get; set; }
        public string contenu_livre { get; set; }
        public Nullable<bool> etat_paiement { get; set; }
        public Nullable<System.Guid> commandeProjetId { get; set; }
        public Nullable<System.Guid> commandeStatutId { get; set; }
        public Nullable<System.Guid> commandeThemeId { get; set; }

        public string commandeDemandeur { get; set; }
        public string commandeRedacteur { get; set; }
        public string commandeType { get; set; }
        public string consigneType { get; set; }
        public string projet { get; set; }
        public string thematique { get; set; }
        public string statut_cmde { get; set; }

     

        [Required]
        [Display(Name = "SelectItemProjet")]
        public Guid listprojetId { get; set; }
        public IEnumerable<SelectListItem> ListProjet { get; set; }

        [Required]
        [Display(Name = "SelectItemTheme")]
        public Guid listThemeId { get; set; }
        public IEnumerable<SelectListItem> ListTheme { get; set; }

        [Required]
        [Display(Name = "SelectItemRedacteur")]
        public Guid listRedacteurId { get; set; }
        public IEnumerable<SelectListItem> ListRedacteur { get; set; }

        [Required]
        [Display(Name = "SelectItemCommandeType")]
        public Guid listCommandeTypeId { get; set; }
        public IEnumerable<SelectListItem> ListCommandeType { get; set; }

        [Required]
        [Display(Name = "SelectItemContenuType")]
        public Guid listContenuTypeId { get; set; }
        public IEnumerable<SelectListItem> ListContenuType { get; set; }

       
        //public virtual COMMANDE COMMANDE1 { get; set; }
        //public virtual COMMANDE COMMANDE2 { get; set; }
        //public virtual COMMANDE_TYPE COMMANDE_TYPE { get; set; }
        //public virtual CONTENU_TYPE CONTENU_TYPE { get; set; }
        //public virtual PROJET PROJET { get; set; }
        //public virtual UTILISATEUR UTILISATEUR { get; set; }
        //public virtual STATUT_COMMANDE STATUT_COMMANDE { get; set; }
        //public virtual THEME THEME { get; set; }
        //public virtual UTILISATEUR UTILISATEUR1 { get; set; }

    }
}