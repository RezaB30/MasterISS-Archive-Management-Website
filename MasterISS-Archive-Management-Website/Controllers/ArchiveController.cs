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
using RezaB.Data.Localization;

namespace MasterISS_Archive_Management_Website.Controllers
{
    [Authorize]
    public class ArchiveController : BaseController
    {
        Logger archiveLogger = LogManager.GetLogger("archive");

        public ActionResult Test()
        {
            return View();
        }
        public ActionResult UploadNewFile(long Id, int AttachmentType)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Manage(long id, UploadFileViewModel uploadFile, int AttachmentType)
        {
            string SubscriberNo = string.Empty;
            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(id);

            var fileException = archiveFileList.InternalException;

            var subscriptionFileList = archiveFileList.Result;

            ViewBag.id = id;

            using (var db = new RadiusREntities())
            {
                var result = db.Subscriptions.Find(id);
                if (result == null)
                {
                    ViewBag.NoSubscriberFound = Localization.Model.NoSubscriberFound;
                    return View();
                }

                SubscriberNo = result.SubscriberNo;
                TempData["SubscriberNo"] = SubscriberNo;
            }

            if (uploadFile.Files == null)
            {
                TempData["NullFilesErrorMessage"] = MasterISS_Archive_Management_Website.Localization.Model.NullFilesErrorMessage;
                TempData["SubscriberNo"] = SubscriberNo;

                return RedirectToAction("Manage", "Archive", new { id = id });
            }

            long uploadMaxFileSize = CustomerWebsiteSettings.MaxSupportAttachmentSize;//byte
            //long uploadMaxFileSize = 200;//byte

            var uploadMaxFileCount = Properties.Settings.Default.uploadMaxFileCount;
            //var uploadMaxFileCount = 3;

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

                    //ViewBag.FileSizeError = string.Format(Localization.Model.FileSizeError, displayMaxFileSizemb);
                    TempData["FileSizeError"] = string.Format(Localization.Model.FileSizeError, displayMaxFileSizemb);
                }
                if (filesCount > uploadMaxFileCount)
                {
                    //ViewBag.FileCountError = string.Format(Localization.Model.FileCountError, uploadMaxFileCount);
                    TempData["FileCountError"] = string.Format(Localization.Model.FileCountError, uploadMaxFileCount); ;
                    //return View(viewModel);
                    TempData["SubscriberNo"] = SubscriberNo;

                    return RedirectToAction("Manage", "Archive", new { id = id });
                }
            }

            else
            {

                foreach (var file in uploadFile.Files)
                {
                    if (Request.Files.Count > 0)
                    {
                        var attachmentType = (ClientAttachmentTypes)AttachmentType;

                        var fileType = file.FileName.Split('.').LastOrDefault();

                        var fileManager = new MasterISSFileManager();
                        var newFile = new FileManagerClientAttachmentWithContent(file.InputStream, new FileManagerClientAttachment(attachmentType, fileType));
                        var result = fileManager.SaveClientAttachment(id, newFile);
                        if (result.InternalException != null)
                        {
                            return RedirectToAction("ErrorPage", "Archive");
                        }

                        if (result.Result == true)
                        {
                            if (uploadFile.Files.Count() > 1)
                            {
                                TempData["UploadFileStatus"] = Localization.Model.UploadFilesStatus;
                            }
                            else
                            {
                                TempData["UploadFileStatus"] = Localization.Model.UploadFileStatus;
                            }
                        }
                        else
                        {
                            TempData["UploadStatus"] = Localization.Model.UploadStatus;
                        }
                    }
                    else
                    {
                        TempData["SubscriberNo"] = SubscriberNo;
                        return RedirectToAction("Manage", "Archive", new { id = id });
                    }
                }
                TempData["SubscriberNo"] = SubscriberNo;

                return RedirectToAction("Manage", "Archive", new { id = id });
            }
            TempData["SubscriberNo"] = SubscriberNo;

            return RedirectToAction("Manage", "Archive", new { id = id });
        }

        [HttpGet]
        public ActionResult Index(long? id)
        {
            string SubscriberNo = string.Empty;
            if (id.HasValue)
            {
                string HasArchiveFileMessage = string.Empty;

                using (var db = new RadiusREntities())
                {
                    var result = db.Subscriptions.Find(id);
                    if (result == null)
                    {
                        ViewBag.NoSubscriberFound = Localization.Model.NoSubscriberFound;
                        return View();
                    }

                    SubscriberNo = result.SubscriberNo;
                }

                var archiveFile = new MasterISSFileManager();
                var archiveFileList = archiveFile.GetClientAttachmentsList(id.Value);

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

                        ViewBag.SubscriberNo = SubscriberNo;
                        ViewBag.id = id;
                        ViewBag.HasFileList = "Has File List";

                        return View(viewResults);
                    }
                    ViewBag.SubscriberNo = SubscriberNo;
                    ViewBag.id = id;
                    ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;


                    return View();
                }
                ViewBag.id = id;

                archiveLogger.Error($":SubscriberId :{id} - {fileException.Message}");

                return RedirectToAction("ErrorPage", "Archive");
            }
            else
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult Delete(long id, string serverSideName)
        {
            var archiveFile = new MasterISSFileManager();

            var removeArchiveFile = archiveFile.RemoveClientAttachment(id, serverSideName);

            var exception = removeArchiveFile.InternalException;

            var archiveFileList = archiveFile.GetClientAttachmentsList(id);

            var fileException = archiveFileList.InternalException;

            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                TempData["FileDeleted"] = MasterISS_Archive_Management_Website.Localization.Model.FileDeleted;
                return RedirectToAction("Index", "Archive", new { id = id });
            }

            archiveLogger.Error($":SubscriberId :{id} - {fileException.Message}");

            return RedirectToAction("ErrorPage", "Archive");
        }

        [HttpGet]
        public ActionResult Manage(long id)
        {
            var SubscriberNo = string.Empty;
            string hasArchiveFileMessage = string.Empty;

            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(id);

            var fileException = archiveFileList.InternalException;

            using (var db = new RadiusREntities())
            {
                var result = db.Subscriptions.Find(id);
                if (result == null)
                {
                    ViewBag.NoSubscriberFound = Localization.Model.NoSubscriberFound;
                    return View();
                }

                SubscriberNo = result.SubscriberNo;
                TempData["SubscriberNo"] = SubscriberNo;
            }


            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                var attachmentTypesList = new LocalizedList<RadiusR.FileManagement.SpecialFiles.ClientAttachmentTypes, RadiusR.Localization.Lists.ClientAttachmentTypes>();
                var attachmentTypeItems = attachmentTypesList.GetList().Select(t => new AttachmentTypesViewModel()
                {
                    AttachmentTypeEnumName = t.Value,
                    AttachmentTypeEnumNumber = t.Key
                });

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

                    var viewResultLists = viewResults.OrderBy(e => e.AttachmentType).ThenByDescending(d => d.CreationDate);
                    ViewBag.id = id;

                    var viewModel = new UploadFileViewModel
                    {
                        AttachmentTypeList = attachmentTypeItems.ToList(),
                        FileDetailList = viewResultLists.ToList()
                    };

                    TempData["SubscriberNo"] = SubscriberNo;


                return View(viewModel);
                }
                TempData["SubscriberNo"] = SubscriberNo;
                return View();
            }

            archiveLogger.Error($":SubscriberId :{id} - {fileException.Message}");

            ViewBag.Exception = Localization.Model.Exception;

            return RedirectToAction("ErrorPage", "Archive");
        }


        public ActionResult Download(long id, string FileName)
        {
            string hasArchiveFileMessage = string.Empty;
            var archiveFile = new MasterISSFileManager();
            var archiveFileList = archiveFile.GetClientAttachmentsList(id);

            var fileException = archiveFileList.InternalException;

            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                if (subscriptionFileList.Count() != 0)
                {
                    var selectedFile = archiveFile.GetClientAttachment(id, FileName);
                    if (selectedFile.InternalException == null)
                    {
                        var datetimeForDownloadFile = DateUtilities.ConvertToDateForDownloadFile(selectedFile.Result.FileDetail.CreationDate);

                        string downloadFileName = id + "." + selectedFile.Result.FileDetail.AttachmentType + "." + datetimeForDownloadFile + "." + selectedFile.Result.FileDetail.FileExtention;

                        return File(selectedFile.Result.Content, selectedFile.Result.FileDetail.MIMEType, downloadFileName);
                    }
                }
                return View();
            }
            archiveLogger.Error($":SubscriberId :{id} - {fileException.Message}");

            ViewBag.Exception = Localization.Model.Exception;
            //return View();
            return RedirectToAction("ErrorPage", "Archive");
        }


        public ActionResult DownloadZipFile(long id)
        {
            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(id);

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

                    string downloadFileZipName = id + "." + "zip";


                    var docs = viewResults.ToList();

                    var resultStream = new MemoryStream();

                    using (var zipArchive = new ZipArchive(resultStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var doc in docs)
                        {
                            //var datetimeForDownloadFile = DateUtilities.ConvertToDateForDownloadFile(doc.CreationDate);

                            //string downloadFileName = id + "." +doc.AttachmentType + "." + datetimeForDownloadFile + "." + doc.FileExtention;

                            using (var currentResult = archiveFile.GetClientAttachment(id, doc.ServerSideName))
                            {
                                if (currentResult.InternalException == null)
                                {
                                    var datetimeForDownloadFile = DateUtilities.ConvertToDateForDownloadFile(doc.CreationDate);

                                    //var attachmentTypesList = new LocalizedList<RadiusR.FileManagement.SpecialFiles.ClientAttachmentTypes, RadiusR.Localization.Lists.ClientAttachmentTypes>();
                                    //var attachmentTypeItems = attachmentTypesList.GetList().Select(t => new AttachmentTypesViewModel()
                                    //{
                                    //    AttachmentTypeEnumName = t.Value,
                                    //    AttachmentTypeEnumNumber = t.Key
                                    //});

                                    //string docAttachmentName;
                                    string downloadFileName;
                                    //foreach (var item in attachmentTypeItems)
                                    //{
                                    //    if (doc.AttachmentType == item.AttachmentTypeEnumNumber)
                                    //    {
                                            //docAttachmentName = item.AttachmentTypeEnumName;
                                            downloadFileName = id + "." + currentResult.Result.FileDetail.AttachmentType + "." + datetimeForDownloadFile + "." + doc.FileExtention;

                                            //var newZipEntry = zipArchive.CreateEntry(currentResult.Result.FileDetail.ServerSideName, CompressionLevel.Optimal);

                                            var newZipEntry = zipArchive.CreateEntry(downloadFileName, CompressionLevel.Optimal);


                                            using (var temp = newZipEntry.Open())
                                            {
                                                currentResult.Result.Content.CopyTo(temp);

                                            }
                                    //    }
                                    //}
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
            archiveLogger.Error($":SubscriberId :{id} - {fileException.Message}");

            ViewBag.Exception = Localization.Model.Exception;
            //return RedirectToAction("Manage", "Archive");//bak buraya
            return RedirectToAction("ErrorPage", "Archive");
        }

        public ActionResult ErrorPage()
        {
            return View();
        }
    }
}


