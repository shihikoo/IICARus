using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IICURas.Models;
using System.Net.Mail;
using System.Web.UI;
using System.Web.Security;
using IICURas.Authorization;
namespace IICURas.Controllers
{
    [Authorize(Roles = "Administrator, Uploader")]
    [Restrict("Suspended")]
    public class PaperStatusController : Controller
    {
        private IICURas.Models.IICURasContext db = new IICURas.Models.IICURasContext();

        //
        // GET: /PaperStatus/

        public ActionResult Index(string searchTerm = null, string sortOrder = "PaperNumber")
        {
            if (!string.IsNullOrEmpty(searchTerm)) searchTerm = searchTerm.Trim();

            var PaperStatusModel = from r in db.Records
                                   where ( (searchTerm == null || r.PaperNumber.Contains(searchTerm)) && r.DeleteRecord != true && r.AcceptanceStatus_randomizer != null)
                                   orderby r.StatusUpdateTime descending
                                   select new PaperStatusViewModel
                                   {
                                       PaperNumber = r.PaperNumber,
                                       TC1Status = r.AcceptanceStatus_randomizer,
                                       AcceptanceStatus = r.AcceptanceStatus,
                                       StatusEntryUser = r.StatusEntryUser,
                                       StatusUpdateTime = r.StatusUpdateTime,
                                       RecordID = r.RecordID,
                                       StatusComments = r.StatusComments,
                                       Doi = r.Doi,
                                   };

            ViewBag.ActionNeededRecord = db.Records.Where(r => r.DeleteRecord != true && r.AcceptanceStatus == null && r.AcceptanceStatus_randomizer != null && r.AcceptanceStatus_randomizer == "Pass TC1").Count();
            ViewBag.TotalRecord = db.Records.Where(r => r.DeleteRecord != true && r.AcceptanceStatus_randomizer != null && r.AcceptanceStatus_randomizer == "Pass TC1").Count();
            ViewBag.searchTerm = searchTerm;

            var result = PaperStatusModel;
            switch (sortOrder)
            {
                case "TC1Status":
                    result = result.OrderBy(s => s.TC1Status);
                    break;
                case "TC1Status desc":
                    result = result.OrderByDescending(s => s.TC1Status);
                    break;
                case "AcceptanceStatus":
                    result = result.OrderBy(s => s.AcceptanceStatus);
                    break;
                case "AcceptanceStatus desc":
                    result = result.OrderByDescending(s => s.AcceptanceStatus);
                    break;
                case "StatusEntryUser":
                    result = result.OrderBy(s => s.StatusEntryUser);
                    break;
                case "StatusEntryUser desc":
                    result = result.OrderByDescending(s => s.StatusEntryUser);
                    break;
                case "StatusUpdateTime":
                    result = result.OrderBy(s => s.StatusUpdateTime);
                    break;
                case "StatusUpdateTime desc":
                    result = result.OrderByDescending(s => s.StatusUpdateTime);
                    break;
                case "StatusComments":
                    result = result.OrderBy(s => s.StatusComments);
                    break;
                case "StatusComments desc":
                    result = result.OrderByDescending(s => s.StatusComments);
                    break;
                case "PaperNumber":
                    result = result.OrderBy(s => s.PaperNumber);
                    break;
                case "PaperNumber desc":
                    result = result.OrderByDescending(s => s.PaperNumber);
                    break;
                case "Doi":
                    result = result.OrderBy(s => s.PaperNumber);
                    break;
                case "Doi desc":
                    result = result.OrderByDescending(s => s.PaperNumber);
                    break;
                default:
                    result = result.OrderBy(s => s.PaperNumber);
                    break;
            }
            ViewBag.sortOrder = sortOrder;

            return View(result.ToList());

        }

      
        //
        // GET: /PaperStatus/Edit/5

        public ActionResult Edit(int id)
        {
            Record record = db.Records.Find(id);

            PaperStatusViewModel PaperStatusRecord = new PaperStatusViewModel
                                   {
                                       PaperNumber = record.PaperNumber,
                                       AcceptanceStatus = record.AcceptanceStatus,
                                       StatusEntryUser = record.StatusEntryUser,
                                       StatusUpdateTime = record.StatusUpdateTime,
                                       RecordID = record.RecordID,
                                       StatusComments = record.StatusComments,
                                       Doi = record.Doi,
                                   };
            return View(PaperStatusRecord);
        }

        //
        // POST: /PaperStatus/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PaperStatusViewModel PaperStatusRecord)
        {
            try{           
                Record record = db.Records.Find(id);
            
                 record.AcceptanceStatus = PaperStatusRecord.AcceptanceStatus;
                 record.StatusEntryUser = User.Identity.Name;
                 record.StatusComments = PaperStatusRecord.StatusComments;
                 record.StatusUpdateTime = DateTime.Now;
                 record.StatusUpdateTimeUTC = DateTime.UtcNow;
                 record.Doi = PaperStatusRecord.Doi;

                 if (ModelState.IsValid)
                 {
                     db.SaveChanges();
                     return RedirectToAction("Index");
                 }

                 else return View(id);
            } 
            catch
            {
                return View();
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
