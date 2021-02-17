using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MasterISS_Archive_Management_Website.ViewModels
{
    public class UploadFileViewModel
    {       
        public long SubscriptionId { get; set; }
        public int AttachmentType { get; set; }
        public HttpPostedFileBase File { get; set; }
        public IEnumerable <HttpPostedFileBase> Files { get; set; }
    }
}