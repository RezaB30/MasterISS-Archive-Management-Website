using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RadiusR.DB;
using RadiusR.FileManagement;
using MasterISS_Archive_Management_Website.ViewModels;
using System.Net;

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

            //var archiveFileS = new RadiusR.FileManagement.FileManagerBasicFile();


            var fileException = archiveFileList.InternalException;

            //var message = archiveFileList.InternalException.Message;


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


                if (subscriptionFileList.Count() != 0)
                {
                    var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
                    {
                        CreationDate = a.CreationDate,
                        FileExtention = a.FileExtention,
                        MIMEType = a.MIMEType,
                        ServerSideName = a.ServerSideName,
                        AttachmentType = (int)a.AttachmentType,//sıralama için kullan

                    }).OrderByDescending(d => d.CreationDate);

                    //var file = viewResults.Select(f => new { fileName = f.ServerSideName }).First().ToString();

                    //var t = archiveFile.GetClientAttachment(SubscriptionId, file);


                    //var dn = t.InternalException;

                    //if (dn != null)
                    //{

                    //}

                    //var k = t.Result;
                    ViewBag.SubscriptionId = SubscriptionId;

                    return View(viewResults);
                }

                ViewBag.hasArchiveFileMessage = "Subscriber Has Not File";

                return View();

                //}
                //var k = archiveFile.GetClientAttachment(SubscriptionNo, "k").Result.FileDetail.;
                //var k = archiveFile.GetClientAttachment.Content.;

            }

            return View();
        }

        public ActionResult DownloadAll(long SubscriptionId)
        {
            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);

            var fileException = archiveFileList.InternalException;


            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                if (subscriptionFileList != null)
                {
                    var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
                    {
                        CreationDate = a.CreationDate,
                        FileExtention = a.FileExtention,
                        MIMEType = a.MIMEType,
                        ServerSideName = a.ServerSideName,
                        AttachmentType = (int)a.AttachmentType

                        //}).OrderByDescending(d => d.CreationDate).ThenBy(f=>f.AttachmentType);

                    });



                    var stream = new WebClient().OpenRead("http://files.cnblogs.com/ldp615/SubId.rar");
               

                    return File(stream, "application/x-zip-compressed", "SubId.rar");

                    //return View(viewResults);
                    //var selectFile = viewResults.Select(a => a.ServerSideName).FirstOrDefault();

                    //var removeArchiveFile = archiveFile.RemoveClientAttachment(SubscriptionId, selectFile);

                    //return RedirectToAction("Manage", "Archive");
                }
            }
            return View();
        }

        //[HttpPost]
        public ActionResult Delete(long SubscriptionId, string FileName)
        {
            //long SubscriptionId = 59012;

            var archiveFile = new MasterISSFileManager();

            //var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);

            //var fileException = archiveFileList.InternalException;

            var removeArchiveFile = archiveFile.RemoveClientAttachment(SubscriptionId, FileName);

            return RedirectToAction("Manage", "Archive");
            //if (fileException == null)
            //{
            //    var subscriptionFileList = archiveFileList.Result;

            //    if (subscriptionFileList != null)
            //    {
            //        var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
            //        {
            //            CreationDate = a.CreationDate,
            //            FileExtention = a.FileExtention,
            //            MIMEType = a.MIMEType,
            //            ServerSideName = a.ServerSideName,
            //            AttachmentType = (int)a.AttachmentType

            //            //}).OrderByDescending(d => d.CreationDate).ThenBy(f=>f.AttachmentType);

            //        });
            //        //return View(viewResults);
            //        //var selectFile = viewResults.Select(a => a.ServerSideName).FirstOrDefault();

            //        var removeArchiveFile = archiveFile.RemoveClientAttachment(SubscriptionId, FileName);

            //        return RedirectToAction("Manage", "Archive");
            //    }
            //}
            return View();
        }

        //[HttpPost]
        public ActionResult Manage(long SubscriptionId)
        {
            //long SubscriptionId = 59012;

            string hasArchiveFileMessage = string.Empty;

            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);

            var fileException = archiveFileList.InternalException;


            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                if (subscriptionFileList != null)
                {
                    var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
                    {
                        CreationDate = a.CreationDate,
                        FileExtention = a.FileExtention,
                        MIMEType = a.MIMEType,
                        ServerSideName = a.ServerSideName,
                        AttachmentType = (int)a.AttachmentType

                        //}).OrderByDescending(d => d.CreationDate).ThenBy(f=>f.AttachmentType);

                    });
                    //return View(viewResults);

                    var viewResultLists = viewResults.OrderBy(e => e.AttachmentType).ThenByDescending(d => d.CreationDate);
                    //var file = viewResults.Select(f => new { fileName = f.ServerSideName }).First().ToString();

                    //var t = archiveFile.GetClientAttachment(SubscriptionId, file);
                    //var k = t.Result;
                    //return View(k);
                    ViewBag.SubscriptionId = SubscriptionId;
                    return View(viewResultLists);

                }
                return View();
            }
            return View();
        }

        //public ActionResult GetOrdeByArchiveFiles()
        //{
        //    return View();
        //}



        [HttpPost]
        public ActionResult Download(long SubscriptionId)
        {
            string hasArchiveFileMessage = string.Empty;


            var archiveFile = new RadiusR.FileManagement.MasterISSFileManager();
            var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);


            var fileException = archiveFileList.InternalException;


            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                if (subscriptionFileList.Count() != 0)
                {
                    var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
                    {
                        CreationDate = a.CreationDate,
                        FileExtention = a.FileExtention,
                        MIMEType = a.MIMEType,
                        ServerSideName = a.ServerSideName,
                        AttachmentType = (int)a.AttachmentType//sıralama için kullan

                    });

                    var file = viewResults.Select(f => new { fileName = f.ServerSideName }).First().ToString();

                    var t = archiveFile.GetClientAttachment(SubscriptionId, file);


                    return View(viewResults);

                }

                ViewBag.hasArchiveFileMessage = "Subscriber Has Not File";

                return View();
            }

            return View();
        }
    }
}