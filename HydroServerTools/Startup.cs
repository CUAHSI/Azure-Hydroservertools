extern alias AliasHSTR; //Source: https://stackoverflow.com/questions/9194495/type-exists-in-2-assemblies

using Microsoft.Owin;
using Owin;

using HydroServerToolsRepository;

[assembly: OwinStartupAttribute(typeof(HydroServerTools.Startup))]
namespace HydroServerTools
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            AliasHSTR.HydroServerToolsRepository.AutoMapperWebConfiguration.Configure();
        }
    }
}
