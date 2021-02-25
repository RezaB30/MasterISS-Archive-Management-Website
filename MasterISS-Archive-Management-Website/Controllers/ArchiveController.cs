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
        //public ActionResult UploadNewFile(long Id, /*IEnumerable<HttpPostedFileBase> newAttachments*/ HttpPostedFileBase newAttachment ,int AttachmentType)
        //public ActionResult UploadNewFile(UploadFileViewModel uploadFile, int AttachmentType)
        public ActionResult Manage(long id, UploadFileViewModel uploadFile, int AttachmentType)
        {
            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(id);

            var fileException = archiveFileList.InternalException;

            //if (fileException == null)
            //{
            var subscriptionFileList = archiveFileList.Result;

            //var attachmentTypeNameList = new List<AttachmentTypesViewModel>();
            //var attachmentTypeNumberList = new List<AttachmentTypesViewModel>();

            var attachmentTypesList = new LocalizedList<RadiusR.FileManagement.SpecialFiles.ClientAttachmentTypes, RadiusR.Localization.Lists.ClientAttachmentTypes>();
            //var at=attachmentTypesList.GetList(culture:null);
            var attachmentTypeItems = attachmentTypesList.GetList().Select(t => new AttachmentTypesViewModel()
            {
                AttachmentTypeEnumName = t.Value,
                AttachmentTypeEnumNumber = t.Key
            });

            //if (subscriptionFileList != null)
            //{
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
            ViewBag.Id = id;

            var viewModel = new UploadFileViewModel
            {
                AttachmentTypeList = attachmentTypeItems.ToList(),
                FileDetailList = viewResultLists.ToList()
            };

            ViewBag.Title = string.Format(Localization.Model.SubscriberArchiveFileDetails, id);

            //return View(viewModel);
            ////}
            //}

            //return View(uploadFile);
            //}





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
                    return View(viewModel);
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

                        //var attachmentType = (ClientAttachmentTypes)uploadFile.AttachmentType;
                        var attachmentType = (ClientAttachmentTypes)AttachmentType;


                        var fileType = file.FileName.Split('.').LastOrDefault();

                        var fileManager = new MasterISSFileManager();
                        var newFile = new FileManagerClientAttachmentWithContent(file.InputStream, new FileManagerClientAttachment(attachmentType, fileType));
                        var result = fileManager.SaveClientAttachment(id, newFile);
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
                        return View(viewModel);
                    }
                }
                return View(viewModel);
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Index(long? id)
        {

            // archiveLogger.Error($": {}- : {}");
            ViewBag.Title = MasterISS_Archive_Management_Website.Localization.Model.HomePage;

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

                        ViewBag.Id = id;

                        ViewBag.Title = string.Format(Localization.Model.SubscriberArchiveFileDetails, id);

                        return View(viewResults);
                    }
                    ViewBag.Id = id;
                    ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
                    ViewBag.Title = MasterISS_Archive_Management_Website.Localization.Model.HomePage;


                    return View();
                }
                ViewBag.Id = id;
                ViewBag.Exception = Localization.Model.Exception;
                ViewBag.Title = MasterISS_Archive_Management_Website.Localization.Model.HomePage;

                archiveLogger.Error($":SubscriberId :{id} - {fileException.Message}");

                return View();
            }

            return View();
        }

        //[HttpPost]
        //public ActionResult Index(long Id)
        //{
        //    string HasArchiveFileMessage = string.Empty;


        //    using (var db = new RadiusREntities())
        //    {
        //        var result = db.Subscriptions.Find(Id);
        //        if (result == null)
        //        {
        //            ViewBag.NoSubscriberFound = Localization.Model.NoSubscriberFound;
        //            return View();
        //        }

        //    }

        //    var archiveFile = new MasterISSFileManager();
        //    var archiveFileList = archiveFile.GetClientAttachmentsList(Id);

        //    var fileException = archiveFileList.InternalException;

        //    if (fileException == null)
        //    {
        //        var subscriptionFileList = archiveFileList.Result;

        //        if (subscriptionFileList.Count() != 0)
        //        {
        //            var viewResults = subscriptionFileList.Select(a => new FileDetailViewModel()
        //            {
        //                CreationDate = a.CreationDate,
        //                FileExtention = a.FileExtention,
        //                MIMEType = a.MIMEType,
        //                ServerSideName = a.ServerSideName,
        //                AttachmentType = (int)a.AttachmentType,//sıralama için kullan

        //            }).OrderByDescending(d => d.CreationDate);

        //            ViewBag.Id = Id;

        //            ViewBag.Title = string.Format(Localization.Model.SubscriberArchiveFileDetails, Id);

        //            return View(viewResults);
        //        }
        //        ViewBag.Id = Id;
        //        ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
        //        ViewBag.Title = MasterISS_Archive_Management_Website.Localization.Model.HomePage;


        //        return View();
        //    }
        //    ViewBag.Id = Id;
        //    ViewBag.Exception = Localization.Model.Exception;
        //    ViewBag.Title = MasterISS_Archive_Management_Website.Localization.Model.HomePage;

        //    archiveLogger.Error($":SubscriberId :{Id} - {fileException.Message}");

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Delete(string ser)
        //{ return View(); }


        [HttpPost]
        public ActionResult Delete(long Id, string serverSideName /*FileDetailViewModel file*/)
        {
            var archiveFile = new MasterISSFileManager();

            var removeArchiveFile = archiveFile.RemoveClientAttachment(Id,/* file.*/serverSideName);

            var exception = removeArchiveFile.InternalException;

            var archiveFileList = archiveFile.GetClientAttachmentsList(Id);

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

                    ViewBag.Id = Id;
                    ViewBag.Title = string.Format(Localization.Model.SubscriberArchiveFileDetails, Id);
                    return View(viewName: "Index", model: viewResults);
                }
                ViewBag.Id = Id;
                ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
                //return View();
                return RedirectToAction("Index", "Archive");
            }
            ViewBag.Id = Id;
            ViewBag.Exception = Localization.Model.Exception;
            archiveLogger.Error($":SubscriberId :{Id} - {fileException.Message}");


            return RedirectToAction("Index", "Archive");
        }

        [HttpGet]
        public ActionResult Manage(long Id)
        {
            string hasArchiveFileMessage = string.Empty;

            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(Id);

            var fileException = archiveFileList.InternalException;

            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                //var attachmentTypeNameList = new List<AttachmentTypesViewModel>();
                //var attachmentTypeNumberList = new List<AttachmentTypesViewModel>();

                var attachmentTypesList = new LocalizedList<RadiusR.FileManagement.SpecialFiles.ClientAttachmentTypes, RadiusR.Localization.Lists.ClientAttachmentTypes>();
                //var at=attachmentTypesList.GetList(culture:null);
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

                        //}).OrderByDescending(d => d.CreationDate).ThenBy(f=>f.AttachmentType);
                    });
                    //return View(viewResults);

                    var viewResultLists = viewResults.OrderBy(e => e.AttachmentType).ThenByDescending(d => d.CreationDate);
                    ViewBag.Id = Id;

                    var viewModel = new UploadFileViewModel
                    {
                        AttachmentTypeList = attachmentTypeItems.ToList(),
                        FileDetailList = viewResultLists.ToList()
                    };

                    ViewBag.Title = string.Format(Localization.Model.SubscriberArchiveFileDetails, Id);

                    return View(viewModel);

                    //return View(viewResultLists);
                }
                return View();
            }

            archiveLogger.Error($":SubscriberId :{Id} - {fileException.Message}");

            ViewBag.Exception = Localization.Model.Exception;
            //}

            //var k = new RezaB.Web.CustomAttributes.EnumTypeAttribute();

            return View();
        }

        [HttpGet]
        public ActionResult ManageTest(long Id)

        {
            string hasArchiveFileMessage = string.Empty;

            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(Id);

            var fileException = archiveFileList.InternalException;

            if (fileException == null)
            {
                var subscriptionFileList = archiveFileList.Result;

                //var attachmentTypeNameList = new List<AttachmentTypesViewModel>();
                //var attachmentTypeNumberList = new List<AttachmentTypesViewModel>();

                var attachmentTypesList = new LocalizedList<RadiusR.FileManagement.SpecialFiles.ClientAttachmentTypes, RadiusR.Localization.Lists.ClientAttachmentTypes>();
                //var at=attachmentTypesList.GetList(culture:null);
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

                        //}).OrderByDescending(d => d.CreationDate).ThenBy(f=>f.AttachmentType);
                    });
                    //return View(viewResults);

                    var viewResultLists = viewResults.OrderBy(e => e.AttachmentType).ThenByDescending(d => d.CreationDate);
                    ViewBag.Id = Id;

                    var viewModel = new UploadFileViewModel
                    {
                        AttachmentTypeList = attachmentTypeItems.ToList(),
                        FileDetailList = viewResultLists.ToList()
                    };

                    ViewBag.Title = string.Format(Localization.Model.SubscriberArchiveFileDetails, Id);

                    return View(viewModel);

                    //return View(viewResultLists);
                }
                return View();
            }

            archiveLogger.Error($":SubscriberId :{Id} - {fileException.Message}");

            ViewBag.Exception = Localization.Model.Exception;
            //}

            //var k = new RezaB.Web.CustomAttributes.EnumTypeAttribute();

            return View();
        }


        public ActionResult Download(long Id, string FileName)
        {
            string hasArchiveFileMessage = string.Empty;
            var archiveFile = new MasterISSFileManager();
            var archiveFileList = archiveFile.GetClientAttachmentsList(Id);

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

                    var selectedFile = archiveFile.GetClientAttachment(Id, FileName);
                    if (selectedFile.InternalException == null)
                    {
                        var datetimeForDownloadFile = DateUtilities.ConvertToDateForDownloadFile(selectedFile.Result.FileDetail.CreationDate);

                        string downloadFileName = Id + "." + selectedFile.Result.FileDetail.AttachmentType + "." + datetimeForDownloadFile + "." + selectedFile.Result.FileDetail.FileExtention;

                        return File(selectedFile.Result.Content, selectedFile.Result.FileDetail.MIMEType, downloadFileName);
                    }
                }
                return View();
            }
            archiveLogger.Error($":SubscriberId :{Id} - {fileException.Message}");

            ViewBag.Exception = Localization.Model.Exception;
            return View();
        }



        public ActionResult DownloadZipFile(long Id)
        {
            var archiveFile = new MasterISSFileManager();

            var archiveFileList = archiveFile.GetClientAttachmentsList(Id);

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

                    string downloadFileZipName = Id + "." + "zip";

                    var docs = viewResults.ToList();

                    var resultStream = new MemoryStream();

                    using (var zipArchive = new ZipArchive(resultStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var doc in docs)
                        {

                            using (var currentResult = archiveFile.GetClientAttachment(Id, doc.ServerSideName))
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
            archiveLogger.Error($":SubscriberId :{Id} - {fileException.Message}");

            ViewBag.Exception = Localization.Model.Exception;
            return RedirectToAction("Manage", "Archive");//bak buraya
        }
    }
}


