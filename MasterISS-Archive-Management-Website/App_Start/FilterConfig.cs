﻿using System.Web;
using System.Web.Mvc;

namespace MasterISS_Archive_Management_Website
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
