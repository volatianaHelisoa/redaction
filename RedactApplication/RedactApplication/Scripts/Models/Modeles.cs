using System;
using System.Collections.Generic;
using System.Linq;
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
           

            return modeleVm;

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