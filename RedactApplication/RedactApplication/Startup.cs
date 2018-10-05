using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RedactApplication.Startup))]
namespace RedactApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
