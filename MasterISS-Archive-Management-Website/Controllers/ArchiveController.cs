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
            long uploadMaxFileSize = CustomerWebsiteSettings.MaxSupportAttachmentSize;//byte

            var uploadMaxFileCount = Properties.Settings.Default.uploadMaxFileCount;

            var filesSize = 0;
            var filesCount = 0;

            foreach (var file in uploadFile.Files)
            {
                filesSize = filesSize + file.ContentLength;
                filesCount = filesCount + 1;
            }

            if (filesSize > uploadMaxFileSize || filesCount > uploadMaxFileCount)
            {
                if (filesSize > uploadMaxFileSize)
                {
                    var displayMaxFileSize = RezaB.Data.Formating.RateLimitFormatter.ToTrafficMixedResults(((decimal)uploadMaxFileSize) * 1024, true);

                    var displayMaxFileSizemb = $"{displayMaxFileSize.FieldValue} {displayMaxFileSize.RateSuffix}";

                    ViewBag.FileSizeError = string.Format(Localization.Model.FileSizeError, displayMaxFileSizemb);
                }
                if (filesCount > uploadMaxFileCount)
                {
                    ViewBag.FileCountError = string.Format(Localization.Model.FileCountError, uploadMaxFileCount);
                    return View(uploadFile);
                }
            }

            else
            {
                //if (ModelState.IsValid)
                //{
                foreach (var file in uploadFile.Files)
                {
                    if (Request.Files.Count > 0)
                    {
                        var attachmentType = (ClientAttachmentTypes)uploadFile.AttachmentType;

                        var fileType = file.FileName.Split('.').LastOrDefault();

                        var fileManager = new MasterISSFileManager();
                        var newFile = new FileManagerClientAttachmentWithContent(file.InputStream, new FileManagerClientAttachment(attachmentType, fileType));
                        var result = fileManager.SaveClientAttachment(uploadFile.SubscriptionId, newFile);
                        if (result.InternalException != null)
                        {
                            return Content("Hata");
                        }
                        //return RedirectToAction("Manage", "Archive");
                        //return Json("file uploaded successfully");
                        if (result.Result == true)
                        {
                            //return Json(new { uploadedFiles = result.Result });
                            if (uploadFile.Files.Count() > 1)
                            {
                                ViewBag.UploadFileStatus = Localization.Model.UploadFilesStatus;
                            }
                            else
                            {
                                ViewBag.UploadFileStatus = Localization.Model.UploadFileStatus;
                            }
                            //return View();
                        }
                        else
                        {
                            ViewBag.UploadStatus = Localization.Model.UploadStatus;
                        }
                    }
                    else
                    {
                        return View(uploadFile);
                    }
                }
                //}
            }

            return View(uploadFile);
        }


        public ActionResult Index()
        {

            // archiveLogger.Error($": {}- : {}");

            return View();
        }

        [HttpPost]
        public ActionResult Index(long SubscriptionId)
        {
            string HasArchiveFileMessage = string.Empty;


            using (var db = new RadiusREntities())
            {
                var result = db.Subscriptions.Find(SubscriptionId);
                if (result == null)
                {
                    ViewBag.NoSubscriberFound = Localization.Model.NoSubscriberFound;
                    return View();
                }

            }

            var archiveFile = new MasterISSFileManager();
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
                        AttachmentType = (int)a.AttachmentType,//sıralama için kullan

                    }).OrderByDescending(d => d.CreationDate);

                    ViewBag.SubscriptionId = SubscriptionId;
                    return View(viewResults);
                }
                ViewBag.SubscriptionId = SubscriptionId;
                ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
                return View();
            }
            ViewBag.SubscriptionId = SubscriptionId;
            ViewBag.Exception = Localization.Model.Exception;
            return View();
        }

        //[HttpPost]
        //public ActionResult Delete(string ser)
        //{ return View(); }


        [HttpPost]
        public ActionResult Delete(long SubscriptionId, string serverSideName /*FileDetailViewModel file*/)
        {
            var archiveFile = new MasterISSFileManager();

            var removeArchiveFile = archiveFile.RemoveClientAttachment(SubscriptionId,/* file.*/serverSideName);

            var exception = removeArchiveFile.InternalException;

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
                        AttachmentType = (int)a.AttachmentType,//sıralama için kullan

                    }).OrderByDescending(d => d.CreationDate);

                    ViewBag.SubscriptionId = SubscriptionId;
                    return View(viewName: "Index", model: viewResults);
                }
                ViewBag.SubscriptionId = SubscriptionId;
                ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
                //return View();
                return RedirectToAction("Index", "Archive");
            }
            ViewBag.SubscriptionId = SubscriptionId;
            ViewBag.Exception = Localization.Model.Exception;

            return RedirectToAction("Index", "Archive");
        }

        //[HttpPost]
        public ActionResult Manage(long SubscriptionId)
        {



            string hasArchiveFileMessage = string.Empty;

            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(SubscriptionId);

            var fileException = archiveFileList.InternalException;

            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                var attachmentTypeList = new List<AttachmentTypesViewModel>();

                foreach (int attachmentType in Enum.GetValues(typeof(ClientAttachmentTypes)))
                {
                    attachmentTypeList.Add(new AttachmentTypesViewModel()
                    {
                        AttachmentTypeEnumNumber = attachmentType
                    });
                }

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
                    ViewBag.SubscriptionId = SubscriptionId;


                    var viewModel = new FileAndAttachmentViewModel
                    {
                        AttachmentTypeList = attachmentTypeList,
                        FileDetailList = viewResultLists.ToList()
                    };
                    return View(viewModel);

                    //return View(viewResultLists);
                }
                return View();
            }
            ViewBag.Exception = Localization.Model.Exception;
            //}

            //var k = new RezaB.Web.CustomAttributes.EnumTypeAttribute();

            return View();
        }


        public ActionResult Download(long SubscriptionId, string FileName)
        {
            string hasArchiveFileMessage = string.Empty;
            var archiveFile = new MasterISSFileManager();
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
            ViewBag.Exception = Localization.Model.Exception;
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
            ViewBag.Exception = Localization.Model.Exception;
            return RedirectToAction("Manage", "Archive");//bak buraya
        }
    }
}


