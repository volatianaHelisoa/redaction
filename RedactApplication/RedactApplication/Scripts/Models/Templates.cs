using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class Templates
    {
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
    }
}