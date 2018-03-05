using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using IICURas.Models;
using IICURas.Authorization;
namespace IICURas.Controllers
{

    [Authorize(Roles = "Administrator, Randomizer")]
    [Restrict("Suspended")]
    public class MSStatusController : Controller
    {
        private readonly IICURas.Models.IICURasContext _db = new IICURas.Models.IICURasContext();

        //
        // GET: /PaperStatus/

        public ActionResult Index(string searchTerm = null, string sortOrder = "PaperNumber")
        {
            if (!string.IsNullOrEmpty(searchTerm)) searchTerm = searchTerm.Trim();

            var isRandomizer = User.IsInRole("Randomizer");

            var MSStatus = from r in _db.Records
                           where ((searchTerm == null || searchTerm == "" || r.PaperNumber.Contains(searchTerm)) && r.DeleteRecord != true)
                           orderby r.StatusUpdateTime descending
                           select new MSStatusViewModel
                           {
                               PaperNumber = r.PaperNumber,
                               AcceptanceStatus_randomizer = r.AcceptanceStatus_randomizer,
                               StatusEntryUser_randomizer = r.StatusEntryUser_randomizer,
                               StatusUpdateTime_randomizer = r.StatusUpdateTime_randomizer,
                               RandomisationTime = r.RandomizationTime,
                               RecordID = r.RecordID,
                               StatusComments_randomizer = isRandomizer ? r.StatusComments_randomizer : "",
                               AuthorCompliance = isRandomizer ? r.AuthorCompliance : "",
                           };

            ViewBag.ActionNeededRecord = _db.Records.Count(r => r.DeleteRecord != true && r.AcceptanceStatus_randomizer == null);
            ViewBag.TotalRecord = _db.Records.Count(r => r.DeleteRecord != true);
            ViewBag.searchTerm = searchTerm;

            var result = MSStatus;
            switch (sortOrder)
            {
                case "AcceptanceStatus_randomizer":
                    result = result.OrderBy(s => s.AcceptanceStatus_randomizer);
                    break;
                case "AcceptanceStatus_randomizer desc":
                    result = result.OrderByDescending(s => s.AcceptanceStatus_randomizer);
                    break;
                case "RandomisationTime":
                    result = result.OrderBy(s => s.AcceptanceStatus_randomizer);
                    break;
                case "RandomisationTime desc":
                    result = result.OrderByDescending(s => s.AcceptanceStatus_randomizer);
                    break;
                case "StatusEntryUser_randomizer":
                    result = result.OrderBy(s => s.StatusEntryUser_randomizer);
                    break;
                case "StatusEntryUser_randomizer desc":
                    result = result.OrderByDescending(s => s.StatusEntryUser_randomizer);
                    break;
                case "StatusUpdateTime_randomizer":
                    result = result.OrderBy(s => s.StatusUpdateTime_randomizer);
                    break;
                case "StatusUpdateTime_randomizer desc":
                    result = result.OrderByDescending(s => s.StatusUpdateTime_randomizer);
                    break;
                case "StatusComments_randomizer":
                    result = result.OrderBy(s => s.StatusComments_randomizer);
                    break;
                case "StatusComments_randomizer desc":
                    result = result.OrderByDescending(s => s.StatusComments_randomizer);
                    break;
                case "PaperNumber":
                    result = result.OrderBy(s => s.PaperNumber);
                    break;
                case "PaperNumber desc":
                    result = result.OrderByDescending(s => s.PaperNumber);
                    break;
                case "AuthorCompliance":
                    result = result.OrderBy(s => s.AuthorCompliance);
                    break;
                case "AuthorCompliance desc":
                    result = result.OrderByDescending(s => s.AuthorCompliance);
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
            var record = _db.Records.Find(id);

            var MSStatus = new MSStatusViewModel
            {
                PaperNumber = record.PaperNumber,
                AcceptanceStatus_randomizer = record.AcceptanceStatus_randomizer,
                StatusEntryUser_randomizer = record.StatusEntryUser_randomizer,
                StatusUpdateTime_randomizer = record.StatusUpdateTime_randomizer,
                RecordID = record.RecordID,
                StatusComments_randomizer = record.StatusComments_randomizer,
                AuthorCompliance = record.AuthorCompliance
            };

            ViewBag.action = (record.CategoryCategoryID == 1) ? "No" : (record.hadarrivechecklist == "Yes" ? "No" : "Yes");

            return View(MSStatus);
        }

        //
        // POST: /PaperStatus/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MSStatusViewModel MSStatusRecord)
        {
            if (!ModelState.IsValid) return View(MSStatusRecord);

            var userid = (int)Membership.GetUser().ProviderUserKey;

            var id = MSStatusRecord.RecordID;

            var record = _db.Records.Find(id);

            record.AcceptanceStatus_randomizer = MSStatusRecord.AcceptanceStatus_randomizer;
            record.StatusEntryUser_randomizer = User.Identity.Name;
            record.StatusComments_randomizer = MSStatusRecord.StatusComments_randomizer;
            record.StatusUpdateTime_randomizer = DateTime.Now;
            record.StatusUpdateTimeUTC_randomizer = DateTime.UtcNow;
            record.AuthorCompliance = MSStatusRecord.AuthorCompliance;

            if (record.AcceptanceStatus_randomizer == "Error")
            {

                var reviewCompletsions = _db.ReviewCompletions.Where(rc => rc.RecordRecordID == id && rc.Status == "Current");

                if (reviewCompletsions.Any())
                {
                    foreach (var rc in reviewCompletsions)
                    {
                        rc.Status = "TC1Error";
                        rc.UserID = userid;

                        if (!rc.PaperQualities.Any()) continue;

                        foreach (var r in rc.PaperQualities)
                        {
                            r.Status = "TC1Error";
                        }
                    }

                }
            }

            if (record.AcceptanceStatus_randomizer == "Pass TC1")
            {

                var reviewCompletsions = _db.ReviewCompletions.Where(rc => rc.RecordRecordID == id && rc.Status == "TC1Error");

                if (reviewCompletsions.Any())
                {
                    foreach (var rc in reviewCompletsions)
                    {
                        rc.Status = "Current";
                        rc.UserID = userid;

                        if (rc.PaperQualities.All(p => p.Status != "TC1Error")) continue;

                        foreach (var r in rc.PaperQualities.Where(p => p.Status == "TC1Error"))
                        {
                            r.Status = "Current";

                        }
                    }

                }
            }

            _db.SaveChanges();
            return RedirectToAction("Index");

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
