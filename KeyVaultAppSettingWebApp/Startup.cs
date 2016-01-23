using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KeyVaultAppSettingWebApp.Startup))]
namespace KeyVaultAppSettingWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
