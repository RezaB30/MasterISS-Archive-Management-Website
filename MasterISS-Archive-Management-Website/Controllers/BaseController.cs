using NLog;
using RadiusR.DB;
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

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    var message = string.Empty;

        //    filterContext.Result = new ViewResult
        //    {
        //        //ViewName = "~/Views/Home/Error.cshtml",
        //    };
        //    filterContext.ExceptionHandled = true;
        //    logger.Error(filterContext.Exception);

        //}


        //protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        //{
        //    //Localization in Base controller:

        //    string lang = CookieTools.getCulture(Request.Cookies);

        //    var routeData = RouteData.Values;
        //    var routeCulture = routeData.Where(r => r.Key == "lang").FirstOrDefault();
        //    if (string.IsNullOrEmpty((string)routeCulture.Value))
        //    {
        //        routeData.Remove("lang");
        //        routeData.Add("lang", lang);

        //        Thread.CurrentThread.CurrentUICulture =
        //        Thread.CurrentThread.CurrentCulture =
        //        CultureInfo.GetCultureInfo(lang);

        //        Response.RedirectToRoute(routeData);
        //    }
        //    else
        //    {
        //        lang = (string)RouteData.Values["lang"];

        //        Thread.CurrentThread.CurrentUICulture =
        //            Thread.CurrentThread.CurrentCulture =
        //            CultureInfo.GetCultureInfo(lang);
        //    }

        //    var version = Assembly.GetExecutingAssembly().GetName().Version;
        //    ViewBag.Version = version.ToString(3);
        //    return base.BeginExecuteCore(callback, state);
        //}



        //[AllowAnonymous]
        //[HttpGet, ActionName("Language")]
        //public virtual ActionResult Language(string culture, string sender)
        //{
        //    CookieTools.SetCultureInfo(Response.Cookies, culture);

        //    Dictionary<string, object> responseParams = new Dictionary<string, object>();
        //    Request.QueryString.CopyTo(responseParams);
        //    responseParams.Add("lang", culture);

        //    return RedirectToAction(sender, new RouteValueDictionary(responseParams));
        //}


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

        protected override void OnException(ExceptionContext filterContext)
        {
            if (!(filterContext.Exception is System.Web.Mvc.HttpAntiForgeryException))
            {
                if (filterContext.Exception is System.Data.Entity.Validation.DbEntityValidationException)
                {
                    var entityException = filterContext.Exception as System.Data.Entity.Validation.DbEntityValidationException;
                    logger.Error(filterContext.Exception, string.Join(Environment.NewLine, entityException.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(se => se.PropertyName + "->" + se.ErrorMessage))));
                }
                else
                {
                    logger.Error(filterContext.Exception);
                }
            }
            //var error = ErrorHandler.GetMessage(filterContext.Exception, Request.IsLocal);
            //filterContext.ExceptionHandled = true;
            ////filterContext.HttpContext.Response.Clear();
            //filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ////filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            //filterContext.Result = Error(error.Message, error.Details);
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

        public virtual ActionResult Error(string message, string details)
        {
            if (Request.IsAjaxRequest())
            {
                return Json(new { Code = 1, Message = message, Details = details }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.Message = message;
            ViewBag.Details = details;
            return View("ErrorDialogBox");
        }

        protected void SetupPages<T>(int? page, ref IQueryable<T> viewResults)
        {
            var totalCount = viewResults.Count();
            var pagesCount = (int)Math.Ceiling((float)totalCount / (float)AppSettings.TableRows);
            ViewBag.PageCount = pagesCount;
            ViewBag.PageTotalCount = totalCount;

            if (!page.HasValue)
            {
                page = 0;
            }

            viewResults = viewResults.Skip(page.Value * AppSettings.TableRows).Take(AppSettings.TableRows);
        }

        protected string ViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

    }
}