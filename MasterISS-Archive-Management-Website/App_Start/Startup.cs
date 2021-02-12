using Microsoft.Owin;
using Owin;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;


[assembly: OwinStartup(typeof(MasterISS_Archive_Management_Website.App_Start.Startup))]

namespace MasterISS_Archive_Management_Website.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var lang = CultureInfo.CurrentCulture.ToString();
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = "MasterISS-Archive-Management-Website",
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(value: "/" + lang + "/Archive/Index"),//yetkisi olmayan
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest
            });
        }
    }
}
