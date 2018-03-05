using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IICURas.Models;
using System.Net.Mail;
using System.Web.UI;
using System.Web.Security;
using System.IO;
using System;
using FluentValidation;
using IICURas.Authorization;

namespace IICURas.Controllers
{
    [Authorize(Roles = "Administrator, Uploader,Reviewer")]
    [Restrict("Suspended")]

    //public class CustomerValidator : AbstractValidator<UploadViewModel>
    //{
    //    public CustomerValidator()
    //    {
    //        RuleFor(upload => upload.).NotNull();
    //    }
    //}

    public class UploadController : Controller
    {

        private readonly IICURas.Models.IICURasContext _db = new IICURas.Models.IICURasContext();

        //
        // GET: /Index/
       [Authorize(Roles = "Administrator, Uploader")]
        public ActionResult Index(string searchTerm, string sortOrder)
        {
            if (!string.IsNullOrEmpty(searchTerm)) searchTerm = searchTerm.Trim();

            var validrecord = from r in _db.Records
                              where r.DeleteRecord != true 
                              //&& r.AcceptanceStatus == "Accepted"
                              select r;

            var PdRecord = from r in validrecord
                           orderby r.PaperDocuments.Count(),  r.StatusUpdateTime ascending          
                           select new UploadOverviewViewModel
                           {
                               PaperNumber = r.PaperNumber,
                               RecordRecordID = r.RecordID,
                               NumberOfDocuments = r.PaperDocuments.Count(pd => pd.DeletePaperDocument != true),
                               TC1Status = r.AcceptanceStatus_randomizer,
                               MSStatus = r.AcceptanceStatus,
                               SpreadsheetStatus = r.MNAcceptance
                           };

            ViewBag.ActionNeededRecord = PdRecord.Count(pd => pd.NumberOfDocuments == 0 && pd.MSStatus == "Accepted" && pd.TC1Status == "Pass TC1");
            ViewBag.TotalRecord =PdRecord.Count();
            ViewBag.searchTerm = searchTerm;

            var result = PdRecord.Where(pd => (pd.PaperNumber.Contains(searchTerm) || searchTerm == null));
            if (sortOrder == null) sortOrder = "PaperNumber";
            switch (sortOrder)
            {
                case "NumberOfDocuments":
                    result = result.OrderBy(s => s.NumberOfDocuments);
                    break;
                case "NumberOfDocuments desc":
                    result = result.OrderByDescending(s => s.NumberOfDocuments);
                    break;
                case "PaperNumber":
                    result = result.OrderBy(s => s.PaperNumber);
                    break;
                case "PaperNumber desc":
                    result = result.OrderByDescending(s => s.PaperNumber);
                    break;
                default:
                    result = result.OrderBy(s => s.PaperNumber);
                    break;
            }
            ViewBag.sortOrder = sortOrder;

            return View(result.ToList());
        }

        //   GET: /Upload/Upload
        [Authorize(Roles = "Administrator, Uploader")]
        public ActionResult Upload(int id = 0, string ErrMessage="")
        {
            ViewBag.N = _db.PaperDocuments.Count(m => m.DeletePaperDocument != true && m.RecordRecordID == id);
            ViewBag.ErrorMessage = ErrMessage;

            var record = _db.Records.Find(id);
            TempData["RecordID"] = id;

            if (record == null)  return View("Error");

                ViewBag.papernumber = record.PaperNumber;

                if (ViewBag.N > 0)
                {
                    var uploadviewmodel = from pd in _db.PaperDocuments
                                          where (pd.RecordRecordID == id && pd.DeletePaperDocument != true)
                                          select new UploadViewModel
                                          {
                                              RecordRecordID = id,
                                              PaperNumber = pd.Record.PaperNumber,
                                              PaperDocumentsID = pd.PaperDocumentsID,
                                              FileName = pd.FileName,
                                              FileType = pd.FileType,
                                              UploadUser = pd.UploadUser,
                                              Comments = pd.Comments,
                                              FileUrl = pd.FileUrl,
                                              LastUpdateTime = pd.LastUpdateTime,

                                          };


                    return View(uploadviewmodel.ToList());
                }
                else
                {
                    var uploadviewmodel = from r in _db.Records
                                          where (r.RecordID == id && r.DeleteRecord != true)
                                          select new UploadViewModel
                                          {
                                              RecordRecordID = id,
                                              PaperNumber = r.PaperNumber,

                                          };

                    return View(uploadviewmodel.ToList());
                }
            
        }

        //
        // POST: /Upload/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Uploader")]
        public ActionResult Upload(FormCollection formCollection, int recordid, string papernumber)
        {
           var ErrorMessage = "";
           var uploadfile = Request.Files["uploadfile"];

           var validFileTypes = new string[]
                {   "application/pdf",
                    "application/msword",
                    "application/postscript",
                    "application/zip",
                    "application/x-compressed",
                    "application/x-zip-compressed",
                    "application/octet-stream",
                    "multipart/x-zip",
                    "multipart/x-gzip",
                    "application/x-gzip",
                    "application/vnd.ms-excel",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "image/bmp",
                    "image/x-windows-bmp",
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png",
                    "image/tiff",
                    "image/x-tiff"
                };

            if (uploadfile == null || uploadfile.ContentLength == 0)
            {
                ModelState.AddModelError("CustomError", "Please choose a file.");
                ErrorMessage = "This field is required";
            }
            else if (!validFileTypes.Contains(uploadfile.ContentType))
            {
                ModelState.AddModelError("CustomError", "File Format is not supported.");
                ErrorMessage = "File Format is not supported.";
            }
            else if (uploadfile.ContentLength > 104857600)
            {
                ModelState.AddModelError("CustomError", "The maximum size is 100MB.");
                ErrorMessage = "The maximum size is 100MB.";
            }
            else if (!uploadfile.FileName.Contains(papernumber))
            {
                ModelState.AddModelError("CustomError", "File name does not match paper number. Please check again.");
                ErrorMessage = "File name does not match paper number. Please check again.";
            }

            if (!ModelState.IsValid) return RedirectToAction("Upload", new {id = recordid, ErrMessage = ErrorMessage});

            var file = new PaperDocument
            {
                //   RecordRecordID = Convert.ToInt32(formCollection["RecordID"]),
                RecordRecordID = recordid,
                FileName = uploadfile.FileName,
                FileType = uploadfile.ContentType,
                DeletePaperDocument = false
            };

            const string path = "~/App_Data/upload";
            if (!Directory.Exists(Server.MapPath(path)))
            {
                Directory.CreateDirectory(Server.MapPath(path));
            }
            //DirectoryInfo di = new DirectoryInfo(path);

            //var folder = Server.MapPath(file.RecordRecordID.ToString());
            //DirectoryInfo d2 = di.CreateSubdirectory(folder);

            var uploadDir = $"{path}/{file.RecordRecordID}";

            if (!Directory.Exists(Server.MapPath(uploadDir)))
            {
                Directory.CreateDirectory(Server.MapPath(uploadDir));
            }

            file.Comments = formCollection["inputcomments"];

            if (formCollection["inputfilename"] != null && formCollection["inputfilename"].Length > 0)
            { file.FileName = formCollection["inputfilename"]; }
            else { file.FileName = Path.GetFileName(uploadfile.FileName); }

            var palist = from pd in _db.PaperDocuments
                where pd.RecordRecordID == file.RecordRecordID
                select pd.FileName;
            
            if(palist.Contains(file.FileName)) file.FileName = "new_"+file.FileName; 

            if (formCollection["Comments"] != null && formCollection["inputfilename"].Length > 0)
            {
                file.Comments = formCollection["Comments"];
            }
            
            file.FileUrl = Path.Combine(Server.MapPath(uploadDir), file.FileName);
            
            uploadfile.SaveAs(file.FileUrl);
            file.UploadUser = User.Identity.Name;
           
            _db.PaperDocuments.Add(file);
            _db.SaveChanges();

            return RedirectToAction("Upload", recordid);
        }


        public FileResult DownloadDocument(int pqid)
        {
            PaperDocument paperdocument = _db.PaperDocuments.Find(pqid);
            if (ModelState.IsValid)
            {
                return File(paperdocument.FileUrl, paperdocument.FileType);
            }
            else
            { throw new HttpException(404,"File not Found."); }
        }

        //
        // GET: /Upload/Delete/5
             [Authorize(Roles = "Administrator, Uploader")]
        public ActionResult Delete(int id)
        {
            try
            {
                 PaperDocument paperdocument = _db.PaperDocuments.Find(id);
                 TempData["RecordID"] = paperdocument.RecordRecordID;
                 paperdocument.DeletePaperDocument = true;
                 _db.SaveChanges();
                 return RedirectToAction("Upload", new { id = paperdocument.RecordRecordID });
            }
            catch
            {
                return View("Error");
            }

        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
