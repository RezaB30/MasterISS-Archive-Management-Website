using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Archive_Management_Website.ViewModels
{
    public class FileAndAttachmentViewModel
    {
        public List<FileDetailViewModel> FileDetailList { get; set; }
        public List<AttachmentTypesViewModel> AttachmentTypeList{ get; set; }
    }
}