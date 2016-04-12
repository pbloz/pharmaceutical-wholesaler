using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Hurtownia.Startup))]
namespace Hurtownia
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
