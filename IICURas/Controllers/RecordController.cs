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
    [Authorize(Roles = "Administrator, Randomizer")]
    [Restrict("Suspended")]
    public class RecordController : Controller
    {
        private readonly IICURas.Models.IICURasContext _db = new IICURas.Models.IICURasContext();
        private readonly List<string> _countryList = new List<string>() { "Canada", "China", "France", "Germany", "Japan", "United States", "United Kingdom" };
        private const int RecordLimit = 1300;

        // GET: /Record/
        [Authorize(Roles = "Randomizer")]
        public ActionResult Index(string searchTerm = null, string sortOrder = "RandomizationTime")
        {
            if (!string.IsNullOrEmpty(searchTerm)) searchTerm = searchTerm.Trim();

            ViewBag.searchTerm = searchTerm;
            var totalRecord = _db.Records.Count(r => r.DeleteRecord != true && r.AcceptanceStatus_randomizer == "Pass TC1");
            ViewBag.totalRecord = totalRecord;
            ViewBag.RecordLimit = RecordLimit;


            if (!User.IsInRole("administrator"))
            {
                var records = from r in _db.Records
                              where r.EntryUser == User.Identity.Name && r.DeleteRecord != true
                              orderby r.RandomizationTimeUTC descending
                              select new RecordViewModel
                              {
                                  RecordID = r.RecordID,
                                  PaperNumber = r.PaperNumber,
                                  CountryCountryID = r.Country.CountryID,
                                  CountryCountryName = r.Country.CountryName,
                                  CategoryCategoryID = r.Category.CategoryID,
                                  CategoryCategoryName = r.Category.CategoryName,
                                  RandomizationTime = r.RandomizationTime,
                                  EntryUser = r.EntryUser,
                                  AnimalResearch = true,
                                  hadarrivechecklist = r.hadarrivechecklist,
                                  Comments = r.Comments,
                                  Action = (r.CategoryCategoryID == 1) ? "No" : (r.hadarrivechecklist == "Yes" ? "No" : "Yes")
                              };

                var result = records.Where(r => (r.PaperNumber.Contains(searchTerm) || searchTerm == null));
                switch (sortOrder)
                {
                    case "CountryCountryName":
                        result = result.OrderBy(s => s.CountryCountryName);
                        break;
                    case "CountryCountryName desc":
                        result = result.OrderByDescending(s => s.CountryCountryName);
                        break;
                    case "hadarrivechecklist":
                        result = result.OrderBy(s => s.hadarrivechecklist);
                        break;
                    case "hadarrivechecklist desc":
                        result = result.OrderByDescending(s => s.hadarrivechecklist);
                        break;
                    case "PaperNumber":
                        result = result.OrderBy(s => s.PaperNumber);
                        break;
                    case "PaperNumber desc":
                        result = result.OrderByDescending(s => s.PaperNumber);
                        break;
                    case "Action":
                        result = result.OrderBy(s => s.Action);
                        break;
                    case "Action desc":
                        result = result.OrderByDescending(s => s.Action);
                        break;
                    case "EntryUser":
                        result = result.OrderBy(s => s.EntryUser);
                        break;
                    case "EntryUser desc":
                        result = result.OrderByDescending(s => s.EntryUser);
                        break;
                    case "RandomizationTime":
                        result = result.OrderBy(s => s.RandomizationTime);
                        break;
                    case "RandomizationTime desc":
                        result = result.OrderByDescending(s => s.RandomizationTime);
                        break;
                    default:
                        result = result.OrderBy(s => s.PaperNumber);
                        break;
                }
                ViewBag.sortOrder = sortOrder;

                return View(result.ToList());
            }
            else
            {
                var records = from r in _db.Records
                              where r.DeleteRecord != true
                              orderby r.RandomizationTimeUTC descending
                              select new RecordViewModel
                              {
                                  RecordID = r.RecordID,
                                  PaperNumber = r.PaperNumber,
                                  CountryCountryID = r.Country.CountryID,
                                  CountryCountryName = r.Country.CountryName,
                                  CategoryCategoryID = r.Category.CategoryID,
                                  CategoryCategoryName = r.Category.CategoryName,
                                  AnimalResearch = true,
                                  RandomizationTime = r.RandomizationTime,
                                  EntryUser = r.EntryUser,
                                  hadarrivechecklist = r.hadarrivechecklist,
                                  Comments = r.Comments,
                                  Action = (r.CategoryCategoryID == 1) ? "No" : (r.hadarrivechecklist == "Yes" ? "No" : "Yes")
                              };


                var result = records.Where(r => (r.PaperNumber.Contains(searchTerm) || searchTerm == null || searchTerm == "*"));
                switch (sortOrder)
                {
                    case "CountryCountryName":
                        result = result.OrderBy(s => s.CountryCountryName);
                        break;
                    case "CountryCountryName desc":
                        result = result.OrderByDescending(s => s.CountryCountryName);
                        break;
                    case "hadarrivechecklist":
                        result = result.OrderBy(s => s.hadarrivechecklist);
                        break;
                    case "hadarrivechecklist desc":
                        result = result.OrderByDescending(s => s.hadarrivechecklist);
                        break;
                    case "PaperNumber":
                        result = result.OrderBy(s => s.PaperNumber);
                        break;
                    case "PaperNumber desc":
                        result = result.OrderByDescending(s => s.PaperNumber);
                        break;
                    case "Action":
                        result = result.OrderBy(s => s.Action);
                        break;
                    case "Action desc":
                        result = result.OrderByDescending(s => s.Action);
                        break;
                    case "EntryUser":
                        result = result.OrderBy(s => s.EntryUser);
                        break;
                    case "EntryUser desc":
                        result = result.OrderByDescending(s => s.EntryUser);
                        break;
                    case "RandomizationTime":
                        result = result.OrderBy(s => s.RandomizationTime);
                        break;
                    case "RandomizationTime desc":
                        result = result.OrderByDescending(s => s.RandomizationTime);
                        break;
                    default:
                        result = result.OrderBy(s => s.PaperNumber);
                        break;
                }
                ViewBag.sortOrder = sortOrder;

                return View(result.ToList());
            }
        }
        //
        // GET: /Record/Create

        [Authorize(Roles = "Randomizer")]
        public ActionResult Create()
        {
            if (_db.Records.Count(r => r.DeleteRecord != true && r.AcceptanceStatus_randomizer == "Pass TC1") < RecordLimit + 1)
            {

                ViewBag.CategoryCategoryID = new SelectList(_db.Categories, "CategoryID", "CategoryName");
                ViewBag.CountryCountryID = new SelectList(_db.Countries, "CountryID", "CountryName");

                var viewrecord = new RecordViewModel();

                return View(viewrecord);
            }
            else { 
            
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Record/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Randomizer")]
        public ActionResult Create(RecordViewModel viewrecord)
        {

            if (ModelState.IsValid)
            {
                var currentCountryName = from c in _db.Countries
                                         where c.CountryID == viewrecord.CountryCountryID
                                         select c.CountryName;

                viewrecord.CountryCountryName = currentCountryName.First();

                TempData["viewrecord"] = viewrecord;

                return RedirectToAction("Review");
            }

            ViewBag.CategoryCategoryID = new SelectList(_db.Categories, "CategoryID", "CategoryName", viewrecord.CategoryCategoryID);
            ViewBag.CountryCountryID = new SelectList(_db.Countries, "CountryID", "CountryName", viewrecord.CountryCountryID);
            return View(viewrecord);
        }

        //
        // GET: /Record/Review
        [Authorize(Roles = "Randomizer")]
        public ActionResult Review()
        {
            if (TempData["viewrecord"] != null)
            {
                var viewrecord = TempData["viewrecord"] as RecordViewModel;
                TempData["viewrecord"] = viewrecord;

                return View(viewrecord);
            }
            else {
                return RedirectToAction("Create");
            }   
        }
        
        //
        //Post /Record/Review
        [HttpPost]
          [ValidateAntiForgeryToken]
        [Authorize(Roles = "Randomizer")]
        public ActionResult Review(RecordViewModel viewrecord)
        {
            if (viewrecord != null)
            {
            Record record = new Record();

            record.PaperNumber = viewrecord.PaperNumber.Trim();
            record.CountryCountryID = viewrecord.CountryCountryID;
            record.Comments = viewrecord.Comments;
            record.hadarrivechecklist = viewrecord.hadarrivechecklist;
            if (record.hadarrivechecklist == "Yes" | record.CategoryCategoryID == 1) record.AuthorCompliance = "N/A";  

            string CountryName = _db.Countries.Where(c=>c.CountryID == record.CountryCountryID).Select(c => c.CountryName).First();

            if (ModelState.IsValid)
            {
                record.CategoryCategoryID = CategoryRandom(CountryName);

            record.EntryUser = User.Identity.Name;
            
            record.RandomizationTime = DateTime.Now;
            record.RandomizationTimeUTC = DateTime.UtcNow;

            _db.Records.Add(record);
            _db.SaveChanges();

            //if (record.CategoryCategoryID == 1)
            //{
            //   ViewBag.Action = "NO";
            //}
            //else
            //{
            //    if (record.hadarrivechecklist == "Yes") { ViewBag.Action = "NO"; }
            //    else { ViewBag.Action = "Please request ARRIVE checklist"; }
            //}

            ViewBag.Action = (record.CategoryCategoryID == 1) ? "Do NOT request ARRIVE checklist" : ((record.hadarrivechecklist == "Yes") ? "Do NOT request ARRIVE checklist" : "Request ARRIVE checklist");

            var message = "Thank you for randomizing paper for IICURas.<br> <br> The following details have been recorded: <br/> <br/>Paper Number: "
                            + viewrecord.PaperNumber.ToString()
                            + "<br /> Country of Corresponding Author: "
                            + viewrecord.CountryCountryName.ToString()
                            + "<br / > Manuscript has already provided ARRIVE checklist: "
                            + viewrecord.hadarrivechecklist.ToString()
                            + "<br />Action: "
                            + ViewBag.Action
                            + "<br/ > <br/> IICRUas Team<br/>"
                            + DateTime.Now.Date;

            sendemail(User.Identity.Name,message);

            return RedirectToAction("Details", new {id = record.RecordID});
            }

            return View(viewrecord);
            }

            else
            {
                return RedirectToAction("Create");
            }
        }

        //
        // GET: /Record/Details/5
        [Authorize(Roles = "Randomizer")]
        public ActionResult Details(int id = 0)
        {
            
            var record = _db.Records.Find(id);
            if (record == null)
            {
                return HttpNotFound();
            }

            ViewBag.Action = (record.CategoryCategoryID == 1) ? "Do NOT request ARRIVE checklist" : ((record.hadarrivechecklist == "Yes") ? "Do NOT request ARRIVE checklist" : "Request ARRIVE checklist");



            return View(record);
        }

        [Authorize(Roles = "Randomizer")]
        public ActionResult Edit()
        {
            var viewrecord = TempData["viewrecord"] as RecordViewModel;

            if (viewrecord == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryCategoryID = new SelectList(_db.Categories, "CategoryID", "CategoryName", viewrecord.CategoryCategoryID);
            ViewBag.CountryCountryID = new SelectList(_db.Countries, "CountryID", "CountryName", viewrecord.CountryCountryID);
            return View(viewrecord);
        }

        //
        // POST: /Record/Edit/5
        [Authorize(Roles = "Randomizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RecordViewModel viewrecord)
        {
            if (ModelState.IsValid)
            {
                viewrecord.CountryCountryName = _db.Countries.Where(c => c.CountryID == viewrecord.CountryCountryID).Select(c => c.CountryName).First();
                
                TempData["viewrecord"] = viewrecord;

                return RedirectToAction("Review");
            }
            ViewBag.CategoryCategoryID = new SelectList(_db.Categories, "CategoryID", "CategoryName", viewrecord.CategoryCategoryID);
            ViewBag.CountryCountryID = new SelectList(_db.Countries, "CountryID", "CountryName", viewrecord.CountryCountryID);
            return View(viewrecord);
        }
  
         //GET: /Record/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id = 0)
        {
            Record record = _db.Records.Find(id);

            if (record == null)
            {
                return HttpNotFound();
            }
            return View(record);
        }

        //
        // POST: /Record/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Record record = _db.Records.Find(id);
            record.DeleteRecord = true;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

         [Authorize(Roles = "Administrator")]
        public ActionResult Summary()
        {
            //var record = _db.Records.Include(r => r.Category).Include(r => r.Country).Where(r => r.DeleteRecord != true);

            var records = _db.Records.Include(r => r.Category).Include(r => r.Country).Where(r => r.DeleteRecord != true && r.ReviewCompletions.Count(rc => rc.Status == "Current") >=3);

            List<RSummaryViewModel> summaryView = new List<RSummaryViewModel>();
            foreach (var category in _db.Categories.Select(c => c.CategoryName))
            {
              foreach (var country in _countryList) {
                  summaryView.Add(new RSummaryViewModel()
                  {
                    CountryName = country,
                    CategoryName = category,
                    CountOfPaper = records.Count(r =>r.Category.CategoryName == category && r.Country.CountryName == country)
                });
                }
              summaryView.Add(new RSummaryViewModel()
            {
                CountryName = "Other Countries",
                CategoryName = category,
                CountOfPaper = records.Count(r => r.Category.CategoryName == category && !_countryList.Contains(r.Country.CountryName))
            });
            }
                         
            ViewBag.CountryList = _countryList;
            ViewBag.CountryList.Add("Other Countries");
            ViewBag.CategoryList = _db.Categories.Select(c => c.CategoryName);



            return View(summaryView);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
            public JsonResult ValidatePaperNumber(string PaperNumber)
            {
                    return Json(!_db.Records.Any(m => m.PaperNumber == PaperNumber), JsonRequestBehavior.AllowGet);
            }

        private int CategoryRandom(string CountryName)
        {
            //[, "Other Countries"]
            Random random = new Random();
            int n1 = 0;
            int n2 = 0;
            if (_countryList.Contains(CountryName))
            {
                var myrecord1 = from r in _db.Records
                                where r.DeleteRecord != true && r.Country.CountryName == CountryName && r.CategoryCategoryID == 1
                                select r;
                
                
                var myrecord2 = from r in _db.Records
                                where r.DeleteRecord != true &&  r.Country.CountryName == CountryName && r.CategoryCategoryID == 2
                                select r;

                n1 = myrecord1.Count();
                n2 = myrecord2.Count();
            }else {
                var myrecord1 = from r in _db.Records
                                where r.DeleteRecord != true &&  !_countryList.Contains(r.Country.CountryName) && r.CategoryCategoryID == 1
                                select r;

                var myrecord2 = from r in _db.Records
                                where r.DeleteRecord != true && !_countryList.Contains(r.Country.CountryName) && r.CategoryCategoryID == 2
                                select r;
                n1 = myrecord1.Count();
                n2 = myrecord2.Count();
            }

            if (n1 == n2)
            {
                return random.Next(1, 3);
            }
            else
            {
                int bigID = (n1 > n2) ? 1 : 2;
                return (random.Next(1, 101) < 26) ? bigID : (3 - bigID);
            }
        }

        private void sendemail(string user, string message)
        {

            MailMessage mail = new MailMessage();
            
            mail.To.Add(new MailAddress(user));
            mail.From = new MailAddress("IICARus.Project@ed.ac.uk");
            mail.Bcc.Add(new MailAddress("multipart.camarades@gmail.com"));
            mail.Subject = "Thank you for randomizing paper for IICURas";

            mail.Body = message;

            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();

            //smtp.Send(mail);

            // Attempt to send the email
            try
            {
                smtp.Send(mail);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Issue sending email: " + e.Message);
            }
        }


        #endregion
    }
}