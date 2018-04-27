using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FirstInFirstAid.Startup))]

namespace FirstInFirstAid
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }

        public void ConfigureServices(IServiceCollection services)
        {

        }
    }
}
