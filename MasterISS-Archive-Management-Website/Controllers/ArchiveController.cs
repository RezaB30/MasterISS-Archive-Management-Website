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

        public ActionResult UploadNewFile(long Id, int AttachmentType)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Manage(long id, UploadFileViewModel uploadFile, int AttachmentType)
        {
            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(id);

            var fileException = archiveFileList.InternalException;

            var subscriptionFileList = archiveFileList.Result;

            ViewBag.id = id;
            if (uploadFile.Files == null)
            {
                TempData["NullFilesErrorMessage"]= MasterISS_Archive_Management_Website.Localization.Model.NullFilesErrorMessage; 
                //ViewBag.NullFilesErrorMessage = MasterISS_Archive_Management_Website.Localization.Model.NullFilesErrorMessage;
                //return View(viewModel);
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
                    TempData["FileSizeError"]= string.Format(Localization.Model.FileSizeError, displayMaxFileSizemb);
                }
                if (filesCount > uploadMaxFileCount)
                {
                    //ViewBag.FileCountError = string.Format(Localization.Model.FileCountError, uploadMaxFileCount);
                    TempData["FileCountError"]= string.Format(Localization.Model.FileCountError, uploadMaxFileCount); ;
                    //return View(viewModel);
                    return RedirectToAction("Manage", "Archive", new { id = id });
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
                            //return Json(new { uploadedFiles = result.Result });
                            if (uploadFile.Files.Count() > 1)
                            {
                                //ViewBag.UploadFileStatus = Localization.Model.UploadFilesStatus;
                                TempData["UploadFileStatus"] = Localization.Model.UploadFilesStatus;
                            }
                            else
                            {
                                //ViewBag.UploadFileStatus = Localization.Model.UploadFileStatus;
                                TempData["UploadFileStatus"]= Localization.Model.UploadFileStatus;
                            }
                            //return View();
                        }
                        else
                        {
                            //ViewBag.UploadStatus = Localization.Model.UploadStatus;
                            TempData["UploadStatus"]= Localization.Model.UploadStatus;
                        }
                    }
                    else
                    {                        //return View(viewModel);
                        return RedirectToAction("Manage", "Archive", new { id = id });
                    }
                }
                //return View(viewModel);
                return RedirectToAction("Manage", "Archive", new { id = id });
            }
            //return View(viewModel);
            return RedirectToAction("Manage", "Archive", new { id=id});
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

                        //ViewBag.Title = string.Format(Localization.Model.SubscriberArchiveFileDetails, id);

                        return View(viewResults);
                    }
                    ViewBag.SubscriberNo = SubscriberNo;
                    ViewBag.id = id;
                    ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
                    //ViewBag.Title = MasterISS_Archive_Management_Website.Localization.Model.HomePage;


                    return View();
                }
                ViewBag.id = id;
              
                //ViewBag.Title = MasterISS_Archive_Management_Website.Localization.Model.HomePage;

                archiveLogger.Error($":SubscriberId :{id} - {fileException.Message}");

                return RedirectToAction("ErrorPage", "Archive");
            }
            else
            {
                //ViewBag.IdValidationMessage = MasterISS_Archive_Management_Website.Localization.Model.ArchiveNoIsRequired;
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

                //if (subscriptionFileList.Count() != 0)
                //{
                //    var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
                //    {
                //        CreationDate = a.CreationDate,
                //        FileExtention = a.FileExtention,
                //        MIMEType = a.MIMEType,
                //        ServerSideName = a.ServerSideName,
                //        AttachmentType = (int)a.AttachmentType,

                //    }).OrderByDescending(d => d.CreationDate);

                    //ViewBag.id = id;

                    return RedirectToAction("Index", "Archive", new { id = id });
                //}
                //ViewBag.id = id;
                //ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
                //return RedirectToAction("Index", "Archive");
            }
            //ViewBag.id = id;
            //ViewBag.Exception = Localization.Model.Exception;
            archiveLogger.Error($":SubscriberId :{id} - {fileException.Message}");

            return RedirectToAction("ErrorPage", "Archive");
        }

        [HttpGet]
        public ActionResult Manage(long id)
        {
            string hasArchiveFileMessage = string.Empty;

            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(id);

            var fileException = archiveFileList.InternalException;

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
                    return View(viewModel);
                }
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

                            using (var currentResult = archiveFile.GetClientAttachment(id, doc.ServerSideName))
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


