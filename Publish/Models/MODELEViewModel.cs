using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RedactApplication.Models
{
    public class MODELEViewModel
    {
        public System.Guid modeleId { get; set; }
        public string logoUrl { get; set; }
        public string menu1_titre { get; set; }
        public string menu2_titre { get; set; }
        public string menu3_titre { get; set; }
        public string menu4_titre { get; set; }
        public string menu1_paragraphe1_titre { get; set; }
        public string menu1_paragraphe2_titre { get; set; }
        public string menu1_contenu1 { get; set; }
        public string menu1_contenu2 { get; set; }
        public string menu1_paragraphe1_photoUrl { get; set; }
        public string menu1_paragraphe1_alt { get; set; }
        public string menu1_paragraphe2_photoUrl { get; set; }
        public string menu1_paragraphe2_alt { get; set; }

        public string menu2_paragraphe1_titre { get; set; }
        public string menu2_paragraphe2_titre { get; set; }   
        public string menu2_contenu1 { get; set; }
        public string menu2_contenu2 { get; set; }
        public string menu2_paragraphe1_photoUrl { get; set; }
        public string menu2_paragraphe1_alt { get; set; }
        public string menu2_paragraphe2_photoUrl { get; set; }
        public string menu2_paragraphe2_alt { get; set; }

        public string menu3_paragraphe1_titre { get; set; }
        public string menu3_paragraphe2_titre { get; set; }
        public string menu3_contenu1 { get; set; }
        public string menu3_contenu2 { get; set; }
        public string menu3_paragraphe1_photoUrl { get; set; }
        public string menu3_paragraphe1_alt { get; set; }
        public string menu3_paragraphe2_photoUrl { get; set; }
        public string menu3_paragraphe2_alt { get; set; }

        public string menu4_paragraphe1_titre { get; set; }
        public string menu4_paragraphe2_titre { get; set; }
        public string menu4_contenu1 { get; set; }
        public string menu4_contenu2 { get; set; }
        public string menu4_paragraphe1_photoUrl { get; set; }
        public string menu4_paragraphe1_alt { get; set; }
        public string menu4_paragraphe2_photoUrl { get; set; }
        public string menu4_paragraphe2_alt { get; set; }

        public string photoALaUneUrl { get; set; }
        public string site_url { get; set; }
        public string domaine { get; set; }
        public string meta_title { get; set; }
        public string meta_description { get; set; }

        public string menu1_meta_description { get; set; }
        public string menu2_meta_description { get; set; }
        public string menu3_meta_description { get; set; }
        public string menu4_meta_description { get; set; }

        public string menu1_link { get; set; }
        public string menu2_link { get; set; }
        public string menu3_link { get; set; }
        public string menu4_link { get; set; }

        public string menu1_link_class { get; set; }
        public string menu2_link_class { get; set; }
        public string menu3_link_class { get; set; }
        public string menu4_link_class { get; set; }

        public string favicone { get; set; }
    }
}