using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using RedactApplication.Models;
using Microsoft.AspNet.SignalR;

namespace RedactApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           
            SqlDependency.Start(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
      

        protected void Application_Error(object sender, EventArgs e)
        {
            var serverError = Server.GetLastError() as HttpException;
            if (null != serverError)
            {
                int errorCode = serverError.GetHttpCode();
                if (400 == errorCode || 401 == errorCode || 403 == errorCode || 404 == errorCode || 500 == errorCode)
                {
                    Server.ClearError();
                    Response.RedirectToRoute("404-PageNotFound", new { });
                }
            }
        }

        void Application_End()
        {
            SqlDependency.Stop(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }

        [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
        public class CheckSessionOutAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext context = HttpContext.Current;
                if (context.Session != null)
                {
                    if (context.Session.IsNewSession)
                    {
                        string sessionCookie = context.Request.Headers["Cookie"];

                        if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId", StringComparison.Ordinal) >= 0))
                        {
                            FormsAuthentication.SignOut();
                            if (!string.IsNullOrEmpty(context.Request.RawUrl))
                            {
                                var redirectTo =
                                    $"~/Login/Accueil?ReturnUrl={HttpUtility.UrlEncode(context.Request.RawUrl)}";
                                filterContext.Result = new RedirectResult(redirectTo);
                                return;
                            }

                        }
                    }
                }

                base.OnActionExecuting(filterContext);
            }
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
        public class AuthorizeRoleAttribute : System.Web.Mvc.AuthorizeAttribute
        {
            private List<string> LoadAccess(int dataRole)
            {
                List<string> temp = new List<string>();
                switch (dataRole)
                {
                    case 0:
                        temp.Add("Home");
                       
                        break;
                    case 1:
                        temp.Add("Referenceur");
                        break;
                    case 2:
                        temp.Add("Redacteur");
                        break;

                    case 4:
                        temp.Add("Template");
                        break;
                    default:
                        return null;
                }
                return temp;
            }
            public override void OnAuthorization(AuthorizationContext filterContext)
            {
                HttpContext context = HttpContext.Current;
                try
                {
                    Guid? GuidUser = null;
                    if (filterContext.RequestContext.HttpContext.User.Identity.Name != null)
                    {
                        Guid tempId;
                        string userId = filterContext.RequestContext.HttpContext.User.Identity.Name;
                        bool val = Guid.TryParse(userId, out tempId);
                        if (val == true)
                        {
                            GuidUser = tempId;
                        }
                    }
                    if (GuidUser != null)
                    {
                        redactapplicationEntities db = new redactapplicationEntities();
                        UTILISATEUR user = db.UTILISATEURs.Find(GuidUser);
                        UserRole userRoleLink = db.UserRoles.FirstOrDefault(x => x.idUser == user.userId);
                        if (userRoleLink != null)
                        {
                            ROLE userRole = db.ROLEs.Find(userRoleLink.idRole);
                            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                            bool authorized = false;
                            List<string> access = null;
                            if (userRole != null)
                            {
                                int id = userRole.roleId;
                                if (id == 4 || id == 3 || id == 2 || id == 1 )
                                {
                                    access = LoadAccess(0);
                                }
                                if ( id == 5 || id == 6)
                                {
                                     controllerName = "Template";
                                    access = LoadAccess(4);
                                }


                                }

                            if (access != null)
                            {
                                foreach (var data in access)
                                {
                                    if (data.Contains(controllerName))
                                    {
                                        authorized = true;
                                        break;
                                    }
                                }
                            }
                            if (!authorized)
                            {
                                ExitUser(ref context, ref filterContext);
                                return;
                            }
                        }
                    }
                    else
                    {
                        ExitUser(ref context, ref filterContext);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    ExitUser(ref context, ref filterContext);
                    return;
                }
            }

            private void ExitUser(ref HttpContext context, ref AuthorizationContext filterContext)
            {
                FormsAuthentication.SignOut();
                if (!string.IsNullOrEmpty(context.Request.RawUrl))
                {
                    filterContext.Result = new RedirectResult("~/Login/Accueil");
                }
            }
        }


    }
}
