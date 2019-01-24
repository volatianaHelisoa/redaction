using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class GUIDEViewModel
    {
        public System.Guid guideId { get; set; }
        public Nullable<System.DateTime> date_creation { get; set; }
        public string demandeur { get; set; }
        public string redacteur { get; set; }
        public Nullable<int> type { get; set; }
        public string mot_cle_pricipal { get; set; }
        public string mot_cle_secondaire { get; set; }
        public string consigne_autres { get; set; }
        public string lien_pdf { get; set; }
        public string grammes1 { get; set; }
        public string grammes2 { get; set; }
        public string grammes3 { get; set; }
        public string entities { get; set; }
        public string titre { get; set; }
        public string chapo { get; set; }
        public string sous_titre_1 { get; set; }
        public string paragraphe_1 { get; set; }
        public string sous_titre_2 { get; set; }
        public string paragraphe_2 { get; set; }
        public string contenu { get; set; }
        public string guide_id { get; set; }
    }
}