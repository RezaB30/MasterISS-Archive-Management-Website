using Microsoft.Owin;
using Owin;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

[assembly: OwinStartup(typeof(MasterISS_Archive_Management_Website.App_Start.Startup))]

namespace MasterISS_Archive_Management_Website.App_Start
{
    public class Startup
    {
        //    public void Configuration(IAppBuilder app)
        //    {
        //        var lang = CultureInfo.CurrentCulture.ToString();
        //        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        //        app.UseCookieAuthentication(new CookieAuthenticationOptions
        //        {
        //            CookieName = "MasterISS-Archive-Management-Website",
        //            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        //            LoginPath = new PathString(value: "/" + lang + "/Archive/Index"),//yetkisi olmayan
        //            CookieHttpOnly = true,
        //            CookieSecure = CookieSecureOption.SameAsRequest
        //        });
        //    }
        //}


        public void Configuration(IAppBuilder app)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            CookieAuthenticationProvider provider = new CookieAuthenticationProvider();

            var originalHandler = provider.OnApplyRedirect;

            //Our logic to dynamically modify the path
            provider.OnApplyRedirect = context =>
            {
                var mvcContext = new HttpContextWrapper(HttpContext.Current);
                var routeData = RouteTable.Routes.GetRouteData(mvcContext);

                //Get the current language  
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("lang", routeData.Values["lang"]);

                //Reuse the RetrunUrl
                Uri uri = new Uri(context.RedirectUri);
                string returnUrl = HttpUtility.ParseQueryString(uri.Query)[context.Options.ReturnUrlParameter];
                routeValues.Add(context.Options.ReturnUrlParameter, returnUrl);

                //Overwrite the redirection uri
                context.RedirectUri = url.Action("Login", "Auth", routeValues);
                originalHandler.Invoke(context);
            };

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString(url.Action("Login", "Auth")),
                //Set the Provider
                Provider = provider,
                CookieName = "RadiusR",
                ExpireTimeSpan = Properties.Settings.Default.CookieExpiration,
                CookieSecure = CookieSecureOption.SameAsRequest,
                CookieSameSite = Microsoft.Owin.SameSiteMode.None,
                CookieHttpOnly = true
            });
        }
        //public partial class Startup
        //{
        //    public void Configuration(IAppBuilder app)
        //    {
        //        UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        //        CookieAuthenticationProvider provider = new CookieAuthenticationProvider();

        //        var originalHandler = provider.OnApplyRedirect;

        //        //Our logic to dynamically modify the path
        //        provider.OnApplyRedirect = context =>
        //        {
        //            var mvcContext = new HttpContextWrapper(HttpContext.Current);
        //            var routeData = RouteTable.Routes.GetRouteData(mvcContext);

        //            //Get the current language  
        //            RouteValueDictionary routeValues = new RouteValueDictionary();
        //            routeValues.Add("lang", routeData.Values["lang"]);

        //            //Reuse the RetrunUrl
        //            //Uri uri = new Uri(context.RedirectUri);
        //            //string returnUrl = HttpUtility.ParseQueryString(uri.Query)[context.Options.ReturnUrlParameter];
        //            //routeValues.Add(context.Options.ReturnUrlParameter, returnUrl);

        //            ////Overwrite the redirection uri
        //            //context.RedirectUri = url.Action("Login", "Auth", routeValues);
        //            //originalHandler.Invoke(context);
        //        };

        //        //app.UseCookieAuthentication(new CookieAuthenticationOptions
        //        //{
        //        //    AuthenticationType = "ApplicationCookie",
        //        //    LoginPath = new PathString(url.Action("Login", "Auth")),
        //        //    //Set the Provider
        //        //    Provider = provider,
        //        //    CookieName = "RadiusR",
        //        //    ExpireTimeSpan = Properties.Settings.Default.CookieExpiration,
        //        //    CookieSecure = CookieSecureOption.SameAsRequest,
        //        //    CookieSameSite = Microsoft.Owin.SameSiteMode.None,
        //        //    CookieHttpOnly = true
        //        //});
        //    }
        //}
    }
}
