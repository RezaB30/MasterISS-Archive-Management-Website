using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MasterISS_Archive_Management_Website.ViewModels
{
    public class FileDetailViewModel
    {
        [EnumType(typeof(RadiusR.FileManagement.SpecialFiles.ClientAttachmentTypes), typeof(RadiusR.Localization.Lists.ClientAttachmentTypes))]
        [UIHint("LocalizedList")]
        public int AttachmentType { get; set; }
        public string MIMEType { get; set; }
        public DateTime CreationDate { get; set; }
        public string MD5 { get; set; }
        public string FileExtention { get; set; }
        public string ServerSideName { get; set; }



        //public long SubscriptionId { get; set; }

    }
}