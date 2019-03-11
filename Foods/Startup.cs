using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Foods.Startup))]
namespace Foods
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
