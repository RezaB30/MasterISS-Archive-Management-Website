using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Archive_Management_Website.ViewModels
{
    public class DownloadFileViewModel
    {
        string FileName { get; set; }
        byte[] FileStream { get; set; }
        string FileType { get; set; }
    }
}