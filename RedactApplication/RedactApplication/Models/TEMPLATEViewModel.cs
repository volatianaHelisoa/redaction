using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class TEMPLATEViewModel
    {
        public System.Guid templateId { get; set; }
        public Nullable<System.Guid> projetId { get; set; }
        public Nullable<System.Guid> themeId { get; set; }
        public string url { get; set; }
        public string ftpUser { get; set; }
        public string ftpPassword { get; set; }
        public Nullable<System.DateTime> dateCreation { get; set; }
        public Nullable<System.Guid> userId { get; set; }
        public string html { get; set; }
        public Nullable<System.Guid> modeleId { get; set; }
        public string ip { get; set; }

        public virtual PROJET PROJET { get; set; }
        public virtual THEME THEME { get; set; }
        public virtual UTILISATEUR UTILISATEUR { get; set; }
        public virtual MODELE MODELE { get; set; }

        [Required]
        [Display(Name = "SelectItemProjet")]
        public Guid listprojetId { get; set; }
        public IEnumerable<SelectListItem> ListProjet { get; set; }

        [Required]
        [Display(Name = "SelectItemTheme")]
        public Guid listThemeId { get; set; }
        public IEnumerable<SelectListItem> ListTheme { get; set; }

    }
}