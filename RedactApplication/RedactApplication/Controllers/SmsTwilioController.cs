using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Twilio;
using Twilio.AspNet.Mvc;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;

namespace RedactApplication.Controllers
{
    public class SmsTwilioController : Controller
    {
        public class SmsController : TwilioController
        {
            [HttpPost]
            public TwiMLResult Index()
            {
                var messagingResponse = new MessagingResponse();
                messagingResponse.Message("The Robots are coming! Head for the hills!");

                return TwiML(messagingResponse);
            }
        }

        public void SendSms(string msgBody="test sms")
        {
           
            var accountSid = System.Configuration.ConfigurationManager.AppSettings["SMSAccountIdentification"];
            var authToken = System.Configuration.ConfigurationManager.AppSettings["SMSAccountPassword"];
            var phonenumber = System.Configuration.ConfigurationManager.AppSettings["SMSAccountFrom"];

            TwilioClient.Init(accountSid, authToken);

            var to = new PhoneNumber("+261343816216");
            var message = MessageResource.Create(
                to,
                from: new PhoneNumber(phonenumber),
                body: msgBody);
            if (!string.IsNullOrEmpty(message.Sid) )
            {
                Console.WriteLine(message.Sid);
                RedirectToRoute("Home", new RouteValueDictionary {
                    { "controller", "Commandes" },
                    { "action", "ListCommandes" }
                });
            }
        }
    }
}