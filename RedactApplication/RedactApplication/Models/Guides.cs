using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;


namespace RedactApplication.Models
{
    class Guides
    {
        public List<GUIDEViewModel> GetListGuide()
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var req = db.GUIDEs.ToList();
            List<GUIDEViewModel> listeGuide = new List<GUIDEViewModel>();

            foreach (var guide in req)
            {

                listeGuide.Add(new GUIDEViewModel()
                {
                    guideId = guide.guideId,
                    demandeur = guide.demandeur,
                    redacteur = guide.redacteur,
                    date_creation = guide.date_creation,
                    type = guide.type,
                    mot_cle_pricipal = guide.mot_cle_pricipal,
                    mot_cle_secondaire = guide.mot_cle_secondaire,
                    consigne_autres = guide.consigne_autres,
                    lien_pdf = guide.lien_pdf,
                    grammes1 = guide.grammes1,
                    grammes2 = guide.grammes2,
                    grammes3 = guide.grammes3,
                    entities = guide.entities,
                    titre = guide.titre,
                    chapo = guide.chapo,
                    sous_titre_1 = guide.sous_titre_1,
                    paragraphe_1 = guide.paragraphe_1,
                    sous_titre_2 = guide.sous_titre_2,
                    paragraphe_2 = guide.paragraphe_2
                });

            }
            return listeGuide.OrderBy(x => x.date_creation).ToList();
        }

        public GUIDEViewModel GetDetailsGuide(Guid? guideId)
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var guide = db.GUIDEs.Find(guideId);

            var guideVm = new GUIDEViewModel();
            guideVm.guideId = guide.guideId;
            guideVm.demandeur = guide.demandeur;
            guideVm.redacteur = guide.redacteur;
            guideVm.date_creation = guide.date_creation;
            guideVm.type = guide.type;
            guideVm.mot_cle_pricipal = guide.mot_cle_pricipal;
            guideVm.mot_cle_secondaire = guide.mot_cle_secondaire;
            guideVm.consigne_autres = guide.consigne_autres;
            guideVm.lien_pdf = guide.lien_pdf;
            guideVm.grammes1 = guide.grammes1;
            guideVm.grammes2 = guide.grammes2;
            guideVm.grammes3 = guide.grammes3;
            guideVm.entities = guide.entities;
            guideVm.titre = guide.titre;
            guideVm.chapo = guide.chapo;
            guideVm.sous_titre_1 = guide.sous_titre_1;
            guideVm.paragraphe_1 = guide.paragraphe_1;
            guideVm.sous_titre_2 = guide.sous_titre_2;
            guideVm.paragraphe_2 = guide.paragraphe_2;

            return guideVm;

        }


    }
}