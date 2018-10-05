using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedactApplication.Models
{
    public class NOTIFICATIONViewModel
    {
        public System.Guid notificationId { get; set; }
        public Nullable<System.Guid> commandeId { get; set; }
        public Nullable<bool> statut { get; set; }
        public Nullable<System.Guid> fromId { get; set; }
        public Nullable<System.Guid> toId { get; set; }
        public Nullable<System.DateTime> datenotif { get; set; }
        public string message { get; set; }

        public int commanderef { get; set; }
        public string fromUserName { get; set; }
        public  string permalink { get; set; }

        public virtual COMMANDE COMMANDE { get; set; }
        public virtual UTILISATEUR FROMUSER { get; set; }
        public virtual UTILISATEUR TOUSER { get; set; }
    }
}