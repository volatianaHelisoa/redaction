using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class FACTUREViewModel
    {
        public FACTUREViewModel()
        {
            this.COMMANDEs = new HashSet<COMMANDE>();
        }

        public System.Guid factureId { get; set; }
        public int factureNumero { get; set; }
        public System.DateTime dateEmission { get; set; }
        public System.DateTime dateDebut { get; set; }
        public System.DateTime dateFin { get; set; }
        public string montant { get; set; }
        public Nullable<bool> etat { get; set; }
        public string periode { get; set; }
        public Nullable<System.Guid> redacteurId { get; set; }
        public Nullable<System.Guid> createurId { get; set; }
        public double total_commande { get; set; }

        public virtual ICollection<COMMANDE> COMMANDEs { get; set; }
        public virtual UTILISATEUR REDACTEUR { get; set; }
        public virtual UTILISATEUR UTILISATEUR { get; set; }

        [Required]
        [Display(Name = "SelectItemRedacteur")]
        public Guid listRedacteurId { get; set; }
        public IEnumerable<SelectListItem> ListRedacteur { get; set; }

       

        
    }
}