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
    public class ArchiveController : BaseController
    {
        Logger archiveLogger = LogManager.GetLogger("archive");

        public ActionResult UploadNewFile(long Id, int AttachmentType)
        {
            return View();
        }

        [HttpPost]
        //public ActionResult UploadNewFile(long Id, /*IEnumerable<HttpPostedFileBase> newAttachments*/ HttpPostedFileBase newAttachment ,int AttachmentType)
        public ActionResult UploadNewFile(UploadFileViewModel uploadFile,int AttachmentType)
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
                      
                        //var attachmentType = (ClientAttachmentTypes)uploadFile.AttachmentType;
                        var attachmentType = (ClientAttachmentTypes)AttachmentType;


                        var fileType = file.FileName.Split('.').LastOrDefault();

                        var fileManager = new MasterISSFileManager();
                        var newFile = new FileManagerClientAttachmentWithContent(file.InputStream, new FileManagerClientAttachment(attachmentType, fileType));
                        var result = fileManager.SaveClientAttachment(uploadFile.Id, newFile);
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
        public ActionResult Index(long Id)
        {
            string HasArchiveFileMessage = string.Empty;


            using (var db = new RadiusREntities())
            {
                var result = db.Subscriptions.Find(Id);
                if (result == null)
                {
                    ViewBag.NoSubscriberFound = Localization.Model.NoSubscriberFound;
                    return View();
                }

            }

            var archiveFile = new MasterISSFileManager();
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
                    return View(viewResults);
                }
                ViewBag.Id = Id;
                ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
                return View();
            }
            ViewBag.Id = Id;
            ViewBag.Exception = Localization.Model.Exception;
            return View();
        }

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
                    return View(viewName: "Index", model: viewResults);
                }
                ViewBag.Id = Id;
                ViewBag.HasArchiveFileMessage = Localization.Model.HasArchiveFileMessage;
                //return View();
                return RedirectToAction("Index", "Archive");
            }
            ViewBag.Id = Id;
            ViewBag.Exception = Localization.Model.Exception;

            return RedirectToAction("Index", "Archive");
        }

        //[HttpPost]
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
                var attachmentTypeItems = attachmentTypesList.GetList().Select(t=>new AttachmentTypesViewModel()
                { 
                    AttachmentTypeEnumName=t.Value,
                    AttachmentTypeEnumNumber=t.Key
                });


                ////var k = Enum.GetValues(typeof(ClientAttachmentTypes));

                //foreach (var attachmentTypeName in Enum.GetNames(typeof(ClientAttachmentTypes)))
                //{
                //    attachmentTypeNameList.Add(new AttachmentTypesViewModel()
                //    {
                //        AttachmentTypeEnumName = attachmentTypeName
                //    });
                //}

                //foreach (int attachmentTypeNumber in Enum.GetValues(typeof(ClientAttachmentTypes)))
                //{
                //    attachmentTypeNumberList.Add(new AttachmentTypesViewModel()
                //    {
                //        AttachmentTypeEnumNumber = attachmentTypeNumber
                //    });
                //}

                //var typeList = new List<AttachmentTypesViewModel>();

                //typeList = Enum.GetValues(typeof(ClientAttachmentTypes)).Cast<>.Select(t => new AttachmentTypesViewModel
                //{
                //    AttachmentTypeEnumNumber =t.AttachmentTypeEnumNumber,
                //    AttachmentTypeEnumName = t.AttachmentTypeEnumName
                //});
                //for (int i = 0; i < Enum.GetValues(typeof(ClientAttachmentTypes)).Length; i++)
                //{
                //    attachmentTypeNumberList.Add(new AttachmentTypesViewModel()
                //    {
                //        AttachmentTypeEnumNumber = 
                //    });
                //}


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


                    //var viewModel = new FileAndAttachmentViewModel
                    var viewModel = new UploadFileViewModel

                    {
                        //AttachmentTypeNameList = attachmentTypeNameList,
                        //AttachmentTypeNumberList = attachmentTypeNumberList,
                        //AttachmentTypeList= attachmentTypesList.GetList().Cast<AttachmentTypesViewModel>().ToList(),

                        //AttachmentTypeList = attachmentTypesList.GetList().Select(t=>new AttachmentTypesViewModel
                        //{ AttachmentTypeEnumName=t.Value,
                        //AttachmentTypeEnumNumber=t.Key
                        //}),
                        AttachmentTypeList = attachmentTypeItems.ToList(),

                        //AttachmentTypeList = k,
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
            ViewBag.Exception = Localization.Model.Exception;
            return RedirectToAction("Manage", "Archive");//bak buraya
        }
    }
}


