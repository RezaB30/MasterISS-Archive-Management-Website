using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MasterISS_Archive_Management_Website
{
    static class DateUtilities
    {
        public static string ConvertToDateForDownloadFile(DateTime date)
        {
            var fileDate = date.ToString("yyyy-MM-dd_HH_mm_ss", CultureInfo.InvariantCulture);
            return fileDate;
        }
    }
}