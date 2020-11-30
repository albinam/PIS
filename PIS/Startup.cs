using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PIS.Startup))]
namespace PIS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
