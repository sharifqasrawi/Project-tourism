using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TravelAndTourismWebsite.Startup))]
namespace TravelAndTourismWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
