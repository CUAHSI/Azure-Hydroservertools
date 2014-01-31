using HydroServerToolsRepository;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HydroServerTools.Startup))]
namespace HydroServerTools
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            AutoMapperWebConfiguration.Configure();
        }
    }
}
