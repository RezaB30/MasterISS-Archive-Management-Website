using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.DB;
using RadiusR.FileManagement;
using MasterISS_Archive_Management_Website.ViewModels;

namespace MasterISS_Archive_Management_Website.Controllers
{
    public class ArchiveController : BaseController
    {
        Logger archiveLogger = LogManager.GetLogger("archive");


        public ActionResult Index()
        {

            // archiveLogger.Error($": {}- : {}");

            return View();
        }

        [HttpPost]
        public ActionResult Index(long SubscriptionId)
        {
            string hasArchiveFileMessage = string.Empty;

            // archiveLogger.Error($": {}- : {}");

            //if (ModelState.IsValid)
            //{

            //using (var db = new RadiusR.DB.RadiusREntities())
            //{
            //    var result = db.Subscriptions.Find(SubscriptionId);
            //}


            var archiveFile = new RadiusR.FileManagement.MasterISSFileManager();
            var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);


            var fileException = archiveFileList.InternalException;



            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;



                //var per = from s in db.Personels
                ////         select s;
                //if (!String.IsNullOrEmpty(SubscriptionNo))
                //{
                //    per = per.Where(s => s.Name.Contains(SubscriptionNo));
                //}

                //return View(per.ToList());

                if (subscriptionFileList != null)
                {
                    var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
                    {
                        CreationDate = a.CreationDate,
                        FileExtention = a.FileExtention,
                        MIMEType = a.MIMEType,
                        ServerSideName = a.ServerSideName,
                        AttachmentType = (int)a.AttachmentType

                    });
                    return View(viewResults);

                }

                ViewBag.hasArchiveFileMessage = "has";

                return View();

                //}
                //var k = archiveFile.GetClientAttachment(SubscriptionNo, "k").Result.FileDetail.;
                //var k = archiveFile.GetClientAttachment.Content.;


            }

            return View();
        }
    }
}