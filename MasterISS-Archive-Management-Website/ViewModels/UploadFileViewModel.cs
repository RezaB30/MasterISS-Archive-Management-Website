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
        public int AttachmentType { get; set; }
        //public HttpPostedFileBase File { get; set; }
        [Required(ErrorMessage="You Must Upload File")]
        public IEnumerable <HttpPostedFileBase> Files { get; set; }
        public List<FileDetailViewModel> FileDetailList { get; set; }
        public List<AttachmentTypesViewModel> AttachmentTypeList { get; set; }
    }
}