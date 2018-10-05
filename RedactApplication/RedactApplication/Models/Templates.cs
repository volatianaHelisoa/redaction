using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class Templates
    {

        public List<TEMPLATEViewModel> GetListTemplate()
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var req = db.TEMPLATEs.ToList();
            List<TEMPLATEViewModel> listeTemplate = new List<TEMPLATEViewModel>();

            foreach (var template in req)
            {

                listeTemplate.Add(new TEMPLATEViewModel()
                {
                    templateId = template.templateId,
                    dateCreation = template.dateCreation,
                    projetId = template.projetId,
                    PROJET = template.PROJET,
                    THEME = template.THEME,
                    themeId = template.themeId,
                    url = template.url,
                    modeleId = template.modeleId,
                    MODELE = template.MODELE,
                    ftpUser = template.ftpUser,
                    ftpPassword = template.ftpPassword,
                    userId = template.userId,
                    UTILISATEUR = template.UTILISATEUR,
                    html = template.html,
                    ip = template.ip
                });

            }
            return listeTemplate.OrderByDescending(x => x.dateCreation).ToList();

        }

        public IEnumerable<SelectListItem> GetListProjetItem()
        {
            using (var context = new redactapplicationEntities())
            {
                List<SelectListItem> listprojet = context.PROJETS.AsNoTracking()
                    .OrderBy(n => n.projet_name)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.projetId.ToString(),
                            Text = n.projet_name
                        }).ToList();
                var projetItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- selectionner projet ---"
                };
                listprojet.Insert(0, projetItem);
                return new SelectList(listprojet, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListThemeItem()
        {
            using (var context = new redactapplicationEntities())
            {
                List<SelectListItem> listtheme = context.THEMES.AsNoTracking()
                    .OrderBy(n => n.theme_name)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.themeId.ToString(),
                            Text = n.theme_name
                        }).ToList();
                var themeItem = new SelectListItem()
                {
                    Value = null,
                    Text = "--- selectionner thématique ---"
                };
                listtheme.Insert(0, themeItem);
                return new SelectList(listtheme, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListThemeItem(string term)
        {
            using (var context = new redactapplicationEntities())
            {
                var theme = context.THEMES.Where(x => x.theme_name.StartsWith(term));
                List<SelectListItem> listtheme = theme
                    .OrderBy(n => n.theme_name)
                    .Select(n =>
                        new SelectListItem
                        {
                            Value = n.themeId.ToString(),
                            Text = n.theme_name
                        }).ToList();

                return new SelectList(listtheme, "Value", "Text");
            }
        }

    }
}