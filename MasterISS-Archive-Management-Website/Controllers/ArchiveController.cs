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
using RezaB.Web.CustomAttributes;
using System.IO;
using System.IO.Compression;
using System.Drawing.Drawing2D;
using System.Text;
using RadiusR.FileManagement.SpecialFiles;

namespace MasterISS_Archive_Management_Website.Controllers
{
    public class ArchiveController : BaseController
    {
        Logger archiveLogger = LogManager.GetLogger("archive");

        public ActionResult UploadNewFile(long SubscriptionId, int AttachmentType)
        {
            return View();
        }

        [HttpPost]
        //public ActionResult UploadNewFile(long SubscriptionId, /*IEnumerable<HttpPostedFileBase> newAttachments*/ HttpPostedFileBase newAttachment ,int AttachmentType)
        public ActionResult UploadNewFile(UploadFileViewModel uploadFile)

        {
            //foreach (var file in newAttachments)
            //{
            if (Request.Files.Count > 0)
            {
                var attachmentType = (ClientAttachmentTypes)uploadFile.AttachmentType;

                //var fileType = file.FileName.Split('.').LastOrDefault();
                var fileType = uploadFile.File.FileName.Split('.').LastOrDefault();


                var fileManager = new MasterISSFileManager();
                var newFile = new FileManagerClientAttachmentWithContent(uploadFile.File.InputStream, new FileManagerClientAttachment(attachmentType, fileType));
                var result = fileManager.SaveClientAttachment(uploadFile.SubscriptionId, newFile);
                if (result.InternalException != null)
                {
                    return Content("Hata");
                }

                //return RedirectToAction("Manage", "Archive");
                return Json("file uploaded successfully");
            }
            else
            {
                return View();
            }
  
        }


        //[HttpPost]
        //public ActionResult UploadFiles(IEnumerable<HttpPostedFileBase> files)
        //{
        //    foreach (var file in files)
        //    {
        //        string filePath = Guid.NewGuid() + Path.GetExtension(file.FileName);
        //        file.SaveAs(Path.Combine(Server.MapPath("~/UploadedFiles"), filePath));
        //        //Here you can write code for save this information in your database if you want
        //    }
        //    return Json("file uploaded successfully");
        //}



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
            //archiveFile.SaveClientAttachment(SubscriptionId,);

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



        //[HttpPost]
        public ActionResult Delete(long SubscriptionId, string FileName)
        {
            //long SubscriptionId = 59012;

            var archiveFile = new MasterISSFileManager();

            var removeArchiveFile = archiveFile.RemoveClientAttachment(SubscriptionId, FileName);

            Session["subId"] = SubscriptionId;

            return RedirectToAction("Manage", "Archive");
        }

        //[HttpPost]
        public ActionResult Manage(long SubscriptionId)
        {
            //long SubscriptionId = 59012;

            //SubscriptionId = (long)Session["SubscriptionId"];

            string hasArchiveFileMessage = string.Empty;

            var archiveFile = new MasterISSFileManager();

            //if( Session["subId"] !=null )
            //{
            //    SubscriptionId =(long) Session["subId"];
            //    var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);

            //    var fileException = archiveFileList.InternalException;

            //    //var k = new RezaB.Web.CustomAttributes.EnumTypeAttribute();

            //    if (fileException == null)
            //    {
            //        var subscriptionFileList = archiveFileList.Result;

            //        if (subscriptionFileList != null)
            //        {
            //            var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
            //            {
            //                CreationDate = a.CreationDate,
            //                FileExtention = a.FileExtention,
            //                MIMEType = a.MIMEType,
            //                ServerSideName = a.ServerSideName,
            //                AttachmentType = (int)a.AttachmentType

            //                //}).OrderByDescending(d => d.CreationDate).ThenBy(f=>f.AttachmentType);
            //            });
            //            //return View(viewResults);

            //            var viewResultLists = viewResults.OrderBy(e => e.AttachmentType).ThenByDescending(d => d.CreationDate);
            //            //var file = viewResults.Select(f => new { fileName = f.ServerSideName }).First().ToString();
            //            //var t = archiveFile.GetClientAttachment(SubscriptionId, file);
            //            //var k = t.Result;
            //            //return View(k);
            //            ViewBag.SubscriptionId = SubscriptionId;
            //            return View(viewResultLists);
            //        }
            //        //return View();
            //    }
            //}

            //else
            //{
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
                //return View();
            }
            //}


            //var k = new RezaB.Web.CustomAttributes.EnumTypeAttribute();


            return View();
        }

        //public ActionResult GetOrdeByArchiveFiles()
        //{
        //    return View();
        //}



        //[HttpPost]
        public ActionResult Download(long SubscriptionId, string FileName)
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
                    //var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
                    //{
                    //    CreationDate = a.CreationDate,
                    //    FileExtention = a.FileExtention,
                    //    MIMEType = a.MIMEType,
                    //    ServerSideName = a.ServerSideName,
                    //    AttachmentType = (int)a.AttachmentType//sıralama için kullan

                    //});

                    //var fileName = viewResults.Select(f => new { fileName = f.ServerSideName }).First().ToString();

                    var selectedFile = archiveFile.GetClientAttachment(SubscriptionId, FileName);
                    if (selectedFile.InternalException == null)
                    {
                        var datetimeForDownloadFile = DateUtilities.ConvertToDateForDownloadFile(selectedFile.Result.FileDetail.CreationDate);

                        string downloadFileName = SubscriptionId + "." + selectedFile.Result.FileDetail.AttachmentType + "." + datetimeForDownloadFile + "." + selectedFile.Result.FileDetail.FileExtention;

                        return File(selectedFile.Result.Content, selectedFile.Result.FileDetail.MIMEType, downloadFileName);
                    }

                }

                return View();
            }

            return View();
        }



        public ActionResult DownloadZipFile(long SubscriptionId)
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

                    });

                    string downloadFileZipName = SubscriptionId + "." + "zip";

                    var docs = viewResults.ToList();

                    var resultStream = new MemoryStream();

                    using (var zipArchive = new ZipArchive(resultStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var doc in docs)
                        {

                            using (var currentResult = archiveFile.GetClientAttachment(SubscriptionId, doc.ServerSideName))
                            {
                                if (currentResult.InternalException == null)
                                {
                                    var newZipEntry = zipArchive.CreateEntry(currentResult.Result.FileDetail.ServerSideName, CompressionLevel.Optimal);
                                    //var newZipEntry = zipArchive.CreateEntry(CreateArchiveAttachmentName(currentResult.Result.FileDetail), CompressionLevel.Optimal);

                                    using (var temp = newZipEntry.Open())
                                    {
                                        currentResult.Result.Content.CopyTo(temp);
                                    }
                                }
                                else
                                {
                                    return RedirectToAction("Manage", "Archive");
                                }
                            }
                        }

                    }
                    resultStream.Seek(0, SeekOrigin.Begin);

                    return File(resultStream, "application/zip", downloadFileZipName);
                }
            }
            return RedirectToAction("Manage", "Archive");
        }
    }
}


