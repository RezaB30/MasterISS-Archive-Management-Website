using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}