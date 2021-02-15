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
using ICSharpCode.SharpZipLib.Zip;
using System.Text;

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

                    var datetimeForDownloadFile = DateUtilities.ConvertToDateForDownloadFile(selectedFile.Result.FileDetail.CreationDate);

                    string downloadFileName = SubscriptionId + "." + selectedFile.Result.FileDetail.AttachmentType + "." + datetimeForDownloadFile + "." + selectedFile.Result.FileDetail.FileExtention;


                    return File(selectedFile.Result.Content, selectedFile.Result.FileDetail.MIMEType, downloadFileName);

                }

                return View();
            }

            return View();
        }


        //public ActionResult DownloadZipFile(long SubscriptionId)
        //{
        //    var archiveFile = new MasterISSFileManager();

        //    var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);
        //    List<FileDetailViewModel> archiveFileLists;


        //    var fileException = archiveFileList.InternalException;
        //    string downloadFileZipName = SubscriptionId + "." + "zip";

        //    if (fileException == null)
        //    {

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //            {
        //                var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
        ////            {
        ////                CreationDate = a.CreationDate,
        ////                FileExtention = a.FileExtention,
        //                MIMEType = a.MIMEType,
        //                ServerSideName = a.ServerSideName,
        //                AttachmentType = (int)a.AttachmentType

        //            });


        //            foreach (var item in viewResults)
        //            {
        //                Files = archiveFile.GetClientAttachment(SubscriptionId, item.ServerSideName);
        //            }
        //foreach (var doc in archiveFileList)
        //{
        //    var file = archive.CreateEntry(doc.ServerSideName);
        //    using (var stream = file.Open())
        //    {
        //        stream.Write(Files, 0, subscriptionFileList.Count);
        //    }
        //}
        //                    }

        //                    //return File(memoryStream.ToArray(), "application/zip", downloadFileZipName);
        //                };

        ////using (ZipFile zip = new ZipFile())
        ////{
        //    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
        //    zip.AddDirectoryByName("Files");
        //    foreach (FileModel file in files)
        //    {
        //        if (file.IsSelected)
        //        {
        //            zip.AddFile(file.FilePath, "Files");
        //        }
        //    }
        //    string zipName = String.Format("FilesZip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        zip.Save(memoryStream);
        //        return File(memoryStream.ToArray(), "application/zip", zipName);
        //    }
        //}
        //    }

        //    return View();
        //}

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

                    //foreach (var item in viewResults)
                    //{
                    //    var Filesk = archiveFile.GetClientAttachment(SubscriptionId, item.ServerSideName);
                    //}


                    //return File(Files,Files. , downloadFileZipName);
                    //for (int i = 0; i < viewResults.Count(); i++)
                    //{


                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    //    {
                    //        foreach (var doc in subscriptionFileList)
                    //        {
                    //            var file = archive.CreateEntry(doc.ServerSideName);
                    //            using (var stream = file.Open())
                    //            {
                    //                stream.Write(, 0, subscriptionFileList.Count());
                    //            }
                    //        }
                    //    }

                    //    return File(memoryStream.ToArray(), "application/zip", downloadFileZipName);
                    //};


                    //using (ZipFile zip = new ZipFile())
                    //{
                    //    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    //    zip.AddDirectoryByName("Files");
                    //    foreach (FileModel file in files)
                    //    {
                    //        if (file.IsSelected)
                    //        {
                    //            zip.AddFile(file.FilePath, "Files");
                    //        }
                    //    }
                    //    using (MemoryStream memoryStream = new MemoryStream())
                    //    {
                    //        zip.Save(memoryStream);
                    //        return File(memoryStream.ToArray(), "application/zip", zipName);
                    //    }
                    //}


                    string downloadFileZipName = SubscriptionId + "." + "zip";

                    var docs = viewResults.ToList();

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var doc in docs)
                            {                                
                                memoryStream.WriteTo(archiveFile.GetClientAttachment(SubscriptionId, doc.ServerSideName).Result.Content);
                            }
                        }

                        return File(memoryStream.ToArray(), "application/zip", downloadFileZipName);
                    };



                }
            }
            return View();
        }





        //public ActionResult Downloadall(long SubscriptionId)
        //{
        //    var archiveFile = new MasterISSFileManager();

        //    var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);

        //    var fileException = archiveFileList.InternalException;

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

        //            });



        //            var docs = subscriptionFileList;


        //            using (var memoryStream = new MemoryStream())
        //            {
        //                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //                {
        //                    foreach (var doc in docs)
        //                    {
        //                        var k = archiveFile.GetClientAttachment(SubscriptionId,doc.ServerSideName);

        //                        var file = archive.CreateEntry(doc.ServerSideName);
        //                        using (var stream = file.Open())
        //                        {
        //                            stream.Write(, 0, archiveFileList.Result.Count());
        //                        }
        //                    }
        //                }

        //                return File(memoryStream.ToArray(), "application/zip", "xxxx.zip");
        //            };
        //        }
        //    }
        //    return View();
        //}

    }
}


