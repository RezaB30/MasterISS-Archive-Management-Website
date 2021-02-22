using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace MasterISS_Archive_Management_Website.ViewModels
{
    public class AttachmentTypesViewModel
    {
        public int AttachmentTypeEnumNumber { get; set; }
        public string AttachmentTypeEnumName { get; set; }

    }
}