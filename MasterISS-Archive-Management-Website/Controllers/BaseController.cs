using NLog;
using RezaB.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MasterISS_Archive_Management_Website.Controllers
{
    public class BaseController: Controller
    {
        protected static Logger logger = LogManager.GetLogger("main");

        protected override void OnException(ExceptionContext filterContext)
        {
            var message = string.Empty;

            filterContext.Result = new ViewResult
            {
                //ViewName = "~/Views/Home/Error.cshtml",
            };
            filterContext.ExceptionHandled = true;
            logger.Error(filterContext.Exception);

        }


        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            //Localization in Base controller:

            string lang = CookieTools.getCulture(Request.Cookies);

            var routeData = RouteData.Values;
            var routeCulture = routeData.Where(r => r.Key == "lang").FirstOrDefault();
            if (string.IsNullOrEmpty((string)routeCulture.Value))
            {
                routeData.Remove("lang");
                routeData.Add("lang", lang);

                Thread.CurrentThread.CurrentUICulture =
                Thread.CurrentThread.CurrentCulture =
                CultureInfo.GetCultureInfo(lang);

                Response.RedirectToRoute(routeData);
            }
            else
            {
                lang = (string)RouteData.Values["lang"];

                Thread.CurrentThread.CurrentUICulture =
                    Thread.CurrentThread.CurrentCulture =
                    CultureInfo.GetCultureInfo(lang);
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            ViewBag.Version = version.ToString(3);
            return base.BeginExecuteCore(callback, state);
        }



        [AllowAnonymous]
        [HttpGet, ActionName("Language")]
        public virtual ActionResult Language(string culture, string sender)
        {
            CookieTools.SetCultureInfo(Response.Cookies, culture);

            Dictionary<string, object> responseParams = new Dictionary<string, object>();
            Request.QueryString.CopyTo(responseParams);
            responseParams.Add("lang", culture);

            return RedirectToAction(sender, new RouteValueDictionary(responseParams));
        }
    }
}