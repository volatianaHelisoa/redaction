using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace RedactApplication
{
    public class MessagesHub : Hub
    {
        private static string conString =
            ConfigurationManager.ConnectionStrings["redactapplicationEntities"].ToString();
        public void Hello()
        {
            Clients.All.hello();
        }

        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MessagesHub>();
            context.Clients.All.updateMessages();
        }

        public void HelloServer()
        {
            Clients.All.hello("Hello message to all clients");
        }
    }
}