using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserOrder.Startup))]
namespace UserOrder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
