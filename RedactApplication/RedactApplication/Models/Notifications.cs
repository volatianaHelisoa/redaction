using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RedactApplication.Models
{
    public class Notifications
    {
        public List<NOTIFICATIONViewModel> GetListNotifications(Guid currentUser)
        {
            redactapplicationEntities db = new redactapplicationEntities();
            var req = db.NOTIFICATIONs.Where(x => x.toId == currentUser && x.statut == true).ToList();
            List<NOTIFICATIONViewModel> listeNotif = new List<NOTIFICATIONViewModel>();
            foreach (var notification in req)
            {
                var fromUtilisateur = this.GetUtilisateur(notification.fromId);
                var toUtilisateur = this.GetUtilisateur(notification.toId);
                var commande = this.GetCommande(notification.commandeId);
                listeNotif.Add(new NOTIFICATIONViewModel()
                {
                    notificationId = notification.notificationId,
                    FROMUSER = fromUtilisateur,
                    fromId = notification.fromId,
                    TOUSER = toUtilisateur,
                    toId = notification.toId,
                    COMMANDE = commande,
                    commandeId = notification.commandeId,
                    statut = notification.statut

                });

            }
            return listeNotif.OrderBy(x => x.statut).ToList();

        }

        public UTILISATEUR GetUtilisateur(Guid? id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            UTILISATEUR utilisateur = db.UTILISATEURs.SingleOrDefault(x => x.userId == id);
            return utilisateur;
        }

        public COMMANDE GetCommande(Guid? id)
        {
            redactapplicationEntities db = new Models.redactapplicationEntities();
            COMMANDE commande = db.COMMANDEs.SingleOrDefault(x => x.commandeId == id);
            return commande;
        }


        public IEnumerable<NOTIFICATIONViewModel> GetAllMessages(Guid? redactId)
        {
            var messages = new List<NOTIFICATIONViewModel>();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT * FROM [dbo].[NOTIFICATION] where toId ='"+redactId+"' and statut = 1", connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UTILISATEUR fromUser = GetUtilisateur((Guid) reader["fromId"]);
                        UTILISATEUR toUser = GetUtilisateur((Guid)reader["toId"]);
                        COMMANDE commande = (!string.IsNullOrEmpty(reader["commandeId"].ToString()))? GetCommande((Guid) reader["commandeId"]) : new COMMANDE();
                        if ((bool) reader["statut"])
                        {

                            messages.Add(item: new NOTIFICATIONViewModel()
                            {
                                notificationId = (Guid) reader["notificationId"],
                                commandeId = (Guid) reader["commandeId"],
                                commanderef = (int) commande.commandeREF,
                                statut = (bool) reader["statut"],
                                fromId = (Guid) reader["fromId"],
                                fromUserName = fromUser.userNom,
                                toId = (Guid) reader["toId"],
                                message = reader["message"].ToString(),

                                datenotif = Convert.ToDateTime(reader["datenotif"])
                            });
                        }
                    }
                }
            }

            return messages.OrderByDescending(x=>x.datenotif).Take(6);
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MessagesHub.SendMessages();
            }
        }

      

    }
}