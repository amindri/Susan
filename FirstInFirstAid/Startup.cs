using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FirstInFirstAid.Startup))]
namespace FirstInFirstAid
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
