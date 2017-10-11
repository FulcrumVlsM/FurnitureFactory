using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FurnitureFactory.Startup))]
namespace FurnitureFactory
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
