using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace RedactApplication.Models
{
    public static class MailClient
    {
        private static readonly SmtpClient Client;
        static MailClient()
        {
            Client = new SmtpClient
            {
                Host =
                    ConfigurationManager.AppSettings["SmtpServer"],
                Port =
                    Convert.ToInt32(
                        ConfigurationManager.AppSettings["SmtpPort"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl =  Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"])
        };
            Client.UseDefaultCredentials = false;
            Client.Credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["SmtpUser"],
                ConfigurationManager.AppSettings["SmtpPass"]);
        }

       


        private static bool SendMessage(string from, string to,
            string subject, string body)
        {
            MailMessage mm = null;
            bool isSent = false;
            try
            {
                // Create our message
                if (to.Contains(";"))
                {
                    List<string> mails = to.Split(';').ToList();
                    mm = new MailMessage();
                    mm.From = new MailAddress(from);
                    foreach (var mail in mails)
                    {
                        mm.To.Add(mail);
                    }
                    mm.Subject = subject;
                    mm.Body = body;
                }
                else
                {
                    mm = new MailMessage(from, to, subject, body);
                }
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions =
                    DeliveryNotificationOptions.OnFailure;
              

                // Send it
                Client.Send(mm);
                isSent = true;
            }
            // Catch any errors, these should be logged and
            // dealt with later
            catch (Exception ex)
            {
                // If you wish to log email errors,
                // add it here...
                var exMsg = ex.Message;
            }
            finally
            {
                mm.Dispose();
            }
            return isSent;
        }


        public static bool SendMail(string email,string content,string subject)
        {
            string body = content;
            return SendMessage(
                ConfigurationManager.AppSettings["adminEmail"],
                email, subject, body);
        }
    }
}