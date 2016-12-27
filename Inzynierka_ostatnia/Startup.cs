using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Inzynierka_ostatnia.Startup))]
namespace Inzynierka_ostatnia
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
