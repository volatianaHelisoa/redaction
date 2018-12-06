using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedactApplication.Models
{
    public class Guide
    {
        public string type { get; set; }
        public string query { get; set; }
        public string locale { get; set; }
        public string localeName { get; set; }
        public string grammes1 { get; set; }
        public string grammes2 { get; set; }
        public string grammes3 { get; set; }
        public string entities { get; set; }

    }
}