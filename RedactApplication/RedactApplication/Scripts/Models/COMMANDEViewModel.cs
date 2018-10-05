using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class COMMANDEViewModel
    {
        public System.Guid commandeId { get; set; }
        public Nullable<System.Guid> commandeReferenceurId { get; set; }
        public Nullable<System.Guid> commandeRedacteurId { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> date_cmde { get; set; }

        [DataType(DataType.Date)]
        public Nullable<System.DateTime> date_livraison { get; set; }
        public string ordrePriorite { get; set; }
        public Nullable<System.Guid> commandeTypeId { get; set; }
        public Nullable<int> nombre_mots { get; set; }
        public string mot_cle_pricipal { get; set; }
        public string mot_cle_secondaire { get; set; }
        public string texte_ancrage { get; set; }
        public string consigne_references { get; set; }
        public Nullable<System.Guid> tagId { get; set; }
        public Nullable<System.Guid> siteId { get; set; }
        public string consigne_autres { get; set; }
        public string balise_titre { get; set; }

        public string remarques { get; set; }

        public Nullable<int> commandeREF { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> dateLivraisonReel { get; set; }

        [AllowHtml]
        public string contenu_livre { get; set; }

        public Nullable<bool> etat_paiement { get; set; }
        public Nullable<System.Guid> commandeProjetId { get; set; }
        public Nullable<System.Guid> commandeStatutId { get; set; }
        public Nullable<System.Guid> commandeThemeId { get; set; }
        public Nullable<System.Guid> commandeToken { get; set; }
        public Nullable<System.DateTime> dateToken { get; set; }

        public string commandeDemandeur { get; set; }
        public string commandeRedacteur { get; set; }
        public string commandeType { get; set; }
        public string consigneType { get; set; }
        public string projet { get; set; }
        public string thematique { get; set; }
        public string statut_cmde { get; set; }
        public string tag { get; set; }
        public string site { get; set; }

     //   public virtual UTILISATEUR REDACTEUR { get; set; }
      //  public virtual SITE SITE { get; set; }
    //    public virtual UTILISATEUR REFERENCEUR { get; set; }
      //  public virtual TAG TAG { get; set; }

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
        [Display(Name = "SelectItemOtherRedacteur")]
        public Guid listOtherRedacteurId { get; set; }
        public IEnumerable<SelectListItem> ListOtherRedacteur { get; set; }

        public JsonResult JsonRedacteur { get; set; }
        

        [Required]
        [Display(Name = "SelectItemCommandeType")]
        public Guid listCommandeTypeId { get; set; }
        public IEnumerable<SelectListItem> ListCommandeType { get; set; }

        [Required]
        [Display(Name = "SelectItemTag")]
        public Guid listTagId { get; set; }
        public IEnumerable<SelectListItem> ListTag { get; set; }

        [Required]
        [Display(Name = "SelectItemStatut")]
        public Guid listStatutId { get; set; }
        public IEnumerable<SelectListItem> ListStatut { get; set; }

    }
}