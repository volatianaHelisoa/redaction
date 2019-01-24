//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RedactApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class COMMANDE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COMMANDE()
        {
            this.NOTIFICATIONs = new HashSet<NOTIFICATION>();
        }
    
        public System.Guid commandeId { get; set; }
        public Nullable<System.Guid> commandeReferenceurId { get; set; }
        public Nullable<System.Guid> commandeRedacteurId { get; set; }
        public Nullable<System.DateTime> date_cmde { get; set; }
        public Nullable<System.DateTime> date_livraison { get; set; }
        public string ordrePriorite { get; set; }
        public Nullable<System.Guid> commandeTypeId { get; set; }
        public Nullable<int> nombre_mots { get; set; }
        public string mot_cle_pricipal { get; set; }
        public string mot_cle_secondaire { get; set; }
        public string texte_ancrage { get; set; }
        public string consigne_references { get; set; }
        public string consigne_autres { get; set; }
        public string balise_titre { get; set; }
        public string contenu_livre { get; set; }
        public Nullable<bool> etat_paiement { get; set; }
        public Nullable<System.Guid> commandeProjetId { get; set; }
        public Nullable<System.Guid> commandeStatutId { get; set; }
        public Nullable<System.Guid> commandeThemeId { get; set; }
        public Nullable<System.Guid> commandeToken { get; set; }
        public Nullable<System.DateTime> dateToken { get; set; }
        public Nullable<int> commandeREF { get; set; }
        public Nullable<System.DateTime> dateLivraisonReel { get; set; }
        public Nullable<System.Guid> factureId { get; set; }
        public string remarques { get; set; }
        public Nullable<System.Guid> tagId { get; set; }
        public Nullable<System.Guid> siteId { get; set; }
        public Nullable<bool> etat_sms { get; set; }
        public string guide_id { get; set; }
    
        public virtual COMMANDE_TYPE COMMANDE_TYPE { get; set; }
        public virtual FACTURE FACTURE { get; set; }
        public virtual PROJET PROJET { get; set; }
        public virtual UTILISATEUR REDACTEUR { get; set; }
        public virtual STATUT_COMMANDE STATUT_COMMANDE { get; set; }
        public virtual THEME THEME { get; set; }
        public virtual UTILISATEUR REFERENCEUR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NOTIFICATION> NOTIFICATIONs { get; set; }
        public virtual TAG TAG { get; set; }
        public virtual SITE SITE { get; set; }
    }
}
