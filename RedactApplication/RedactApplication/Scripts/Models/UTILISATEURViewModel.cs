using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class UTILISATEURViewModel
    {
        public System.Guid userId { get; set; }
        public string userNom { get; set; }
        public string userPrenom { get; set; }
        public string userMail { get; set; }
        public string userAdresse { get; set; }
        public int userIdSecond { get; set; }
        public string logoUrl { get; set; }
        public string redactSkype { get; set; }
        public string redactVolume { get; set; }
        public string redactModePaiement { get; set; }
        public string redactReferenceur { get; set; }
        public string redactThemes { get; set; }
        public string redactNiveau { get; set; }

      
        public string redactPhone { get; set; }
        public string redactTarif { get; set; }
        public string redactVolumeRestant { get; set; }
        [Required]
        public string userRole { get; set; }
        public string userMotdepasse { get; set; }
        public string userMotdepasseConfirme { get; set; }
        public bool saveOnComputer { get; set; }

        [Required]
        [Display(Name = "SelectItemTheme")]
        //public Guid listThemeId { get; set; }
        public IEnumerable<SelectListItem> ListTheme { get; set; }
        public List<Guid?> listThemeId { get; set; }

        public Guid?[] themeId { get; set; }
        public MultiSelectList themeList { get; set; }
    }
}