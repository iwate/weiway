using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeiWay.Startup))]
namespace WeiWay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
