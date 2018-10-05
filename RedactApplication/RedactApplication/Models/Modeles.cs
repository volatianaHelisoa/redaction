using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace RedactApplication.Models
{
    public class Modeles
    {
        public MODELEViewModel GetDetailsModele(Guid? modeleId)
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var modele = db.MODELEs.Find(modeleId);
           
            var modeleVm = new MODELEViewModel();

            modeleVm.modeleId = modele.modeleId;
            modeleVm.logoUrl = modele.logoUrl;
            modeleVm.menu1_titre = modele.menu1_titre;
            modeleVm.menu2_titre = modele.menu2_titre;
            modeleVm.menu3_titre = modele.menu3_titre;
            modeleVm.menu4_titre = modele.menu4_titre;
            modeleVm.menu1_paragraphe1_titre = modele.menu1_paragraphe1_titre;
            modeleVm.menu1_paragraphe2_titre = modele.menu1_paragraphe2_titre;
            modeleVm.menu1_contenu1 = modele.menu1_contenu1;
            modeleVm.menu1_contenu2 = modele.menu1_contenu2;
            modeleVm.menu1_paragraphe1_photoUrl = modele.menu1_paragraphe1_photoUrl;
            modeleVm.menu1_paragraphe2_photoUrl = modele.menu1_paragraphe2_photoUrl;
            modeleVm.menu2_paragraphe1_titre = modele.menu2_paragraphe1_titre;
            modeleVm.menu2_paragraphe2_titre = modele.menu2_paragraphe2_titre;
            modeleVm.menu2_contenu1 = modele.menu2_contenu1;
            modeleVm.menu2_contenu2 = modele.menu2_contenu2;
            modeleVm.menu2_paragraphe1_photoUrl = modele.menu2_paragraphe1_photoUrl;
            modeleVm.menu2_paragraphe2_photoUrl = modele.menu2_paragraphe2_photoUrl;
            modeleVm.menu3_paragraphe1_titre = modele.menu3_paragraphe1_titre;
            modeleVm.menu3_paragraphe2_titre = modele.menu3_paragraphe2_titre;
            modeleVm.menu3_contenu1 = modele.menu3_contenu1;
            modeleVm.menu3_contenu2 = modele.menu3_contenu2;
            modeleVm.menu3_paragraphe1_photoUrl = modele.menu3_paragraphe1_photoUrl;
            modeleVm.menu3_paragraphe2_photoUrl = modele.menu3_paragraphe2_photoUrl;
            modeleVm.menu4_paragraphe1_titre = modele.menu4_paragraphe1_titre;
            modeleVm.menu4_paragraphe2_titre = modele.menu4_paragraphe2_titre;
            modeleVm.menu4_contenu1 = modele.menu4_contenu1;
            modeleVm.menu4_contenu2 = modele.menu4_contenu2;
            modeleVm.menu4_paragraphe1_photoUrl = modele.menu4_paragraphe1_photoUrl;
            modeleVm.menu4_paragraphe2_photoUrl = modele.menu4_paragraphe2_photoUrl;

            modeleVm.photoALaUneUrl = modele.photoALaUneUrl;
            modeleVm.site_url = modele.site_url;
            modeleVm.domaine = ExtractDomainName(modele.site_url);

            modeleVm.menu1_meta_description = modele.menu1_meta_description;
            modeleVm.menu2_meta_description = modele.menu2_meta_description;
            modeleVm.menu3_meta_description = modele.menu3_meta_description;
            modeleVm.menu4_meta_description = modele.menu4_meta_description;

            modeleVm.menu1_paragraphe1_alt = modele.menu1_p1_alt;
            modeleVm.menu1_paragraphe2_alt = modele.menu1_p2_alt;
            modeleVm.menu2_paragraphe1_alt = modele.menu2_p1_alt;
            modeleVm.menu2_paragraphe2_alt = modele.menu2_p2_alt;
            modeleVm.menu3_paragraphe1_alt = modele.menu3_p1_alt;
            modeleVm.menu3_paragraphe2_alt = modele.menu3_p2_alt;
            modeleVm.menu4_paragraphe1_alt = modele.menu4_p1_alt;
            modeleVm.menu4_paragraphe2_alt = modele.menu4_p2_alt;

            modeleVm.menu1_meta_description = modele.menu1_meta_description;
            modeleVm.menu2_meta_description = modele.menu2_meta_description;
            modeleVm.menu3_meta_description = modele.menu3_meta_description;
            modeleVm.menu4_meta_description = modele.menu4_meta_description;
            modeleVm.favicone = modele.favicone;

            return modeleVm;

        }

      
        public static String removeDiacritics(string str)
        {
            if (null == str) return null;
            var chars = str
                .Normalize(NormalizationForm.FormD)
                .ToCharArray()
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();
            var res = new string(chars).Normalize(NormalizationForm.FormC);
            return res;
        }


        public string ExtractDomainName(string Url)
        {
            Uri baseUri = new Uri(Url);
            var fullDomain = baseUri.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped);
            var domainParts = fullDomain
                .Split('.') 
                .Reverse() 
                .Take(2)  
                .Reverse(); 
            var domain = String.Join(".", domainParts);
            return domain;
        }

        public MODELEViewModel GetPrincipalModele(Guid? modeleId)
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var modele = db.MODELEs.Find(modeleId);

            var modeleVm = new MODELEViewModel();

            modeleVm.modeleId = modele.modeleId;
            modeleVm.logoUrl = modele.logoUrl;

            modeleVm.menu1_titre = modele.menu1_titre;
            modeleVm.menu2_titre = modele.menu2_titre;
            modeleVm.menu3_titre = modele.menu3_titre;
            modeleVm.menu4_titre = modele.menu4_titre;

            modeleVm.menu2_paragraphe2_titre = modele.menu2_paragraphe2_titre;
            modeleVm.menu2_contenu1 = modele.menu2_contenu1;
            modeleVm.menu2_contenu2 = modele.menu2_contenu2;
            modeleVm.menu2_paragraphe1_photoUrl = modele.menu2_paragraphe1_photoUrl;
            modeleVm.menu2_paragraphe2_photoUrl = modele.menu2_paragraphe2_photoUrl;

            modeleVm.photoALaUneUrl = modele.photoALaUneUrl;
            modeleVm.site_url = modele.site_url;


            return modeleVm;

        }

        public MODELEViewModel GetMenu2Modele(Guid? modeleId)
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var modele = db.MODELEs.Find(modeleId);

            var modeleVm = new MODELEViewModel();

            modeleVm.modeleId = modele.modeleId;
            modeleVm.logoUrl = modele.logoUrl;

            modeleVm.menu1_titre = modele.menu1_titre;
            modeleVm.menu2_titre = modele.menu2_titre;
            modeleVm.menu3_titre = modele.menu3_titre;
            modeleVm.menu4_titre = modele.menu4_titre;

            modeleVm.menu1_paragraphe1_titre = modele.menu1_paragraphe1_titre;
            modeleVm.menu1_paragraphe2_titre = modele.menu1_paragraphe2_titre;
            modeleVm.menu1_contenu1 = modele.menu1_contenu1;
            modeleVm.menu1_contenu2 = modele.menu1_contenu2;
            modeleVm.menu1_paragraphe1_photoUrl = modele.menu1_paragraphe1_photoUrl;


            modeleVm.photoALaUneUrl = modele.photoALaUneUrl;
            modeleVm.site_url = modele.site_url;


            return modeleVm;

        }
    }
}