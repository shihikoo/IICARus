using IICURas.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using IICURas.Authorization;

namespace IICURas.Controllers
{
    [Authorize(Roles = "Administrator, Reconciler, Reviewer, SeniorReviewer")]
    [Restrict("Suspended")]
    public class ReviewController : Controller
    {
        private readonly IICURas.Models.IICURasContext _db = new IICURas.Models.IICURasContext();
        //       
        //GET: /Review/
        public ActionResult Index()
        {
            var userid = (int)Membership.GetUser().ProviderUserKey; 

            var user = _db.UserProfiles.Find(userid);

            var reviewCompletions = user.ReviewCompletions.Where(rc => rc.Status == "Current");

            var todoNUmber = Roles.IsUserInRole(user.UserName, "SeniorReviewer") 
                ? _db.Records.Count(r => r.DeleteRecord == false 
                && r.PaperDocuments.Count(pd => pd.DeletePaperDocument == false) > 0 
                && r.AcceptanceStatus == "Accepted"
                && r.AcceptanceStatus_randomizer == "Pass TC1"
                && r.ReviewCompletions.Count(rc => rc.Status == "Current" && !rc.Reconciliation) == 0
                )
                : _db.Records.Count(r => r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate !=null) == 1 
                && r.ReviewCompletions.Count(rc =>rc.Status == "Current" && !rc.Reconciliation) < (int)IICURas.Enums.ReviewComplesionThreshold.MinimumReviewedNum
                && r.ReviewCompletions.Count(rc =>rc.Status == "Current" && !rc.Reconciliation && rc.UserID == userid) == 0
                );

            var reviewOverviewVm = new ReviewOverviewViewModel
                                   {
                                       isSenior = Roles.IsUserInRole("SeniorReviewer "),
                                       CompleteNumber = reviewCompletions.Count(rc => rc.Status == "Current" && !rc.Reconciliation && rc.ComplesionDate != null),
                                       OngoingNumber = reviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate == null && !rc.Reconciliation),
                                       TodoNumber = todoNUmber,
                                       OngoingReview = reviewCompletions.Where(rc => rc.Status == "Current" && rc.ComplesionDate == null && !rc.Reconciliation).Select(rc => new OngoingReviewViewModel()
                                       {
                                           PublicationID = rc.RecordRecordID,
                                           ReviewCompletionID = rc.ReviewCompletionID,
                                           LastUpdatedTime = rc.LastUpdateTime,
                                           CompletedNumber = rc.PaperQualities.Count(pq => pq.Status == "Current" && pq.OptionOptionID != null),
                                           TotalNumber = rc.PaperQualities.Count(pq => pq.Status == "Current"),
                                           SpeciesNumber = rc.LinkRecordUserSpecies.Count(lrus => lrus.Status == "Current"),     
                                           isSenior                                     = Roles.IsUserInRole("SeniorReviewer "),
                                       })                                       
                                   };

            return View(reviewOverviewVm);
        }

        public ActionResult NewReview()
        {
            var userid = (int)Membership.GetUser().ProviderUserKey;

            var randomRecord = new Record();
             
            if (User.IsInRole("SeniorReviewer"))
            {
                var avaliableRecord = _db.Records.Where(
                    r => r.AcceptanceStatus == "Accepted"
                    && r.AcceptanceStatus_randomizer == "Pass TC1"
                    && r.ReviewCompletions.Count(rc => rc.Status == "Current") == 0
                    && r.PaperDocuments.Count(pd => pd.DeletePaperDocument == false) > 0
                    && r.DeleteRecord==false);

                randomRecord = avaliableRecord.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
            }
            else if (User.IsInRole("Reviewer"))
            {
                var avaliableRecord = _db.Records.Where(
                    r => r.DeleteRecord == false
                      && r.PaperDocuments.Count(pd => pd.DeletePaperDocument == false) > 0
                       && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 1
                        && r.ReviewCompletions.Count(rc => rc.Status == "Current") == 1
                        && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.UserID == userid) == 0
                      );

                randomRecord = avaliableRecord.OrderBy(r => Guid.NewGuid()).FirstOrDefault();    
            }
            else
            {
                ViewBag.ErrorMessage = "You do not have qualification for reviewing for IICARus project. Please complete the training programe first. Thank you for your interest in IICARus project.";
                return View("Error");
            }

            if (randomRecord == null)
            { ViewBag.ErrorMessage = "There are no more publication for you to review. Please contact admin for further assistance. Thank you for reviewing on IICARus Project";
                return View("Error");}

            /////////////// save review completion & review items
            var newReviewCompletion = new ReviewCompletion()
            {
                UserID = userid,
                RecordRecordID = randomRecord.RecordID,
                Role = User.IsInRole("SeniorReviewer") ? IICURas.Enums.userRoles.SeniorReviewer : IICURas.Enums.userRoles.Reviewer,
                LastUpdateTime = DateTime.Now
            };

            _db.ReviewCompletions.Add(newReviewCompletion);

            var newReviewItems = (from c in _db.CheckLists
                                  where !string.IsNullOrEmpty(c.CheckListNumber)
                                  select c).AsEnumerable().Select(checklist => new PaperQuality()
                {
                    CheckListCheckListID = checklist.CheckListID,
                    ReviewerId = userid,
                    RecordRecordID = randomRecord.RecordID,
                    ReviewCompletion = newReviewCompletion,
                    LastUpdateTime = DateTime.Now
                }).ToList();

            _db.PaperQualities.AddRange(newReviewItems);

            _db.SaveChanges();

            var reviewIM = new ReviewInputModel
            {
                ReviewCompleteID = newReviewCompletion.ReviewCompletionID,

                RecordID = randomRecord.RecordID,

                Species = _db.Species.ToList(),

                COI = randomRecord.ConflicOfIntest,

                Funding = randomRecord.Funding,

                Files = randomRecord.PaperDocuments.Where(pd => pd.DeletePaperDocument == false).Select(pd => new ReadViewModel
                {
                    TrainingDocumentID = pd.PaperDocumentsID,
                    FileName = pd.FileName,
                    FileType = pd.FileType,
                    FileUrl = pd.FileUrl,
                }).ToList(),

                ReviewItems = newReviewItems.Select(c => new ReviewItemInputModel
                {
                    ReviewItemID = c.PaperQualityID,
                    PublicationID = c.RecordRecordID,
                    CheckListID = c.CheckListCheckListID,
                    Section = c.CheckList.Section,
                    Item = c.CheckList.Item,
                    ItemNumber = c.CheckList.ItemNumber,
                    CheckListName = c.CheckList.CheckListName,
                    CheckListNumber = c.CheckList.CheckListNumber,
                    Criteria = c.CheckList.Criteria,
                    Critical = c.CheckList.Critical,
                    Options = c.CheckList.ChecklistOptionLinks.Where(o => o.Status == "Current").Select(o => o.Option).ToList(),
                }).ToList().OrderBy(c => float.Parse(c.ItemNumber, CultureInfo.InvariantCulture.NumberFormat)).ThenBy(c => c.CheckListNumber).ToList(),
            };

            return View(reviewIM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewReview(ReviewInputModel ReviewIM)
        {
            if (!ModelState.IsValid) return View("Error");

            var userid = (int)Membership.GetUser().ProviderUserKey;

            var ExistingReviewCompletion = _db.ReviewCompletions.Find(ReviewIM.ReviewCompleteID);

            if(ExistingReviewCompletion.UserID != userid || ExistingReviewCompletion.Status != "Current") View("Error");

            var ExistingReviewItems = ExistingReviewCompletion.PaperQualities.Where(pq => pq.Status == "Current");

            foreach (var item in ReviewIM.ReviewItems)
            {
                var ExistingReviewItem = ExistingReviewItems.FirstOrDefault(ri =>
                         ri.CheckListCheckListID == item.CheckListID
                      && ri.PaperQualityID == item.ReviewItemID
                      && ri.RecordRecordID == item.PublicationID
                      && ri.ReviewerId == userid);

                if (ExistingReviewItem == null) continue;

                if (ExistingReviewItem.OptionOptionID != item.OptionID) ExistingReviewItem.OptionOptionID = item.OptionID;
                if (ExistingReviewItem.Comments != item.Comments) ExistingReviewItem.Comments = item.Comments;
                if (ExistingReviewItem.OptionOptionID != item.OptionID || ExistingReviewItem.Comments != item.Comments) ExistingReviewItem.LastUpdateTime = DateTime.Now;
            }

            ExistingReviewCompletion.LastUpdateTime = DateTime.Now;

            if (ReviewIM.SpeciesIDs != null)
            {
                foreach (var linkRUP in ReviewIM.SpeciesIDs.Select(eachSpecie => new LinkRecordUserSpecie()
                {
                    UserID = userid,
                    RecordID = ReviewIM.RecordID,
                    SpecieID = eachSpecie,
                    ReviewCompletion = ExistingReviewCompletion,
                    LastUpdateTime = DateTime.Now
                }))
                {
                    _db.LinkRecordUserSpecies.Add(linkRUP);
                }
            }

            if (Request.Form["Submit"] != null) ExistingReviewCompletion.ComplesionDate = DateTime.Now;

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public FileResult DownloadDocument(int tdid)
        {
            var paperdocument = _db.PaperDocuments.Find(tdid);
            if (ModelState.IsValid)
            {
                return File(paperdocument.FileUrl, paperdocument.FileType);
            }
            else
            { throw new HttpException(404, "File not Found."); }
        }
        public ActionResult ContinueReview(int rcid)
        {
            var userid = (int)Membership.GetUser().ProviderUserKey;

            var reviewComplesion = _db.ReviewCompletions.Find(rcid);

            if (reviewComplesion == null || reviewComplesion.Status != "Current" || reviewComplesion.PaperQualities.Count(pq => pq.Status == "Current") == 0 || reviewComplesion.UserID != userid)
                return View("Error");

            var reviewIM = new ReviewInputModel
            {
                RecordID = reviewComplesion.RecordRecordID,

                Species = _db.Species.ToList(),

                SpeciesIDs = reviewComplesion.Record.LinkRecordUserSpecies.Where(l => l.Status == "Current" && l.ReviewCompletionID == reviewComplesion.ReviewCompletionID).Select(l => l.SpecieID).ToArray(),

                ReviewCompleteID = reviewComplesion.ReviewCompletionID,

                Files = reviewComplesion.Record.PaperDocuments.Where(pd => pd.DeletePaperDocument == false).Select(pd => new ReadViewModel
                {
                    TrainingDocumentID = pd.PaperDocumentsID,
                    FileName = pd.FileName,
                    FileType = pd.FileType,
                    FileUrl = pd.FileUrl,
                }).ToList(),

                COI = reviewComplesion.Record.ConflicOfIntest,

                Funding = reviewComplesion.Record.Funding,

                ReviewItems = reviewComplesion.PaperQualities.Select(pq => new ReviewItemInputModel()
                {
                    ReviewItemID = pq.PaperQualityID,
                    PublicationID = pq.RecordRecordID,
                    CheckListID = pq.CheckListCheckListID,
                    OptionID = pq.OptionOptionID,
                    Comments = pq.Comments,
                    Section = pq.CheckList.Section,
                    Item = pq.CheckList.Item,
                    ItemNumber = pq.CheckList.ItemNumber,
                    CheckListName = pq.CheckList.CheckListName,
                    CheckListNumber = pq.CheckList.CheckListNumber,
                    Criteria = pq.CheckList.Criteria,
                    Critical = pq.CheckList.Critical,
                    Options = pq.CheckList.ChecklistOptionLinks.Where(o => o.Status == "Current").Select(o => o.Option).ToList(),
                }).ToList().OrderBy(c => float.Parse(c.ItemNumber, CultureInfo.InvariantCulture.NumberFormat)).ThenBy(c => c.CheckListNumber).ToList(),
            };

            return View(reviewIM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContinueReview(ReviewInputModel ReviewIM)
        {
            if (!ModelState.IsValid) return View("Error");

            var userid = (int)Membership.GetUser().ProviderUserKey;

            var ExistingReviewCompletion = _db.ReviewCompletions.FirstOrDefault(rc => rc.ReviewCompletionID == ReviewIM.ReviewCompleteID && rc.Status == "Current" && rc.UserID == userid);

            if (ExistingReviewCompletion == null) return View("Error");

        //    ExistingReviewCompletion.Role = User.IsInRole("SeniorReviewer") ? IICURas.Enums.userRoles.SeniorReviewer : IICURas.Enums.userRoles.Reviewer;

            var ExistingReviewItems = ExistingReviewCompletion.PaperQualities.Where(pq => pq.Status == "Current");

            foreach (var item in ReviewIM.ReviewItems)
            {
                var ExistingReviewItem = ExistingReviewItems.FirstOrDefault(ri => 
                         ri.CheckListCheckListID == item.CheckListID
                      && ri.PaperQualityID == ri.PaperQualityID
                      && ri.RecordRecordID == ri.RecordRecordID
                      && ri.ReviewerId == userid);

                if (ExistingReviewItem == null) continue;

                if (ExistingReviewItem.OptionOptionID != item.OptionID) ExistingReviewItem.OptionOptionID = item.OptionID;
                if (ExistingReviewItem.Comments != item.Comments)  ExistingReviewItem.Comments = item.Comments;
                if (ExistingReviewItem.OptionOptionID != item.OptionID || ExistingReviewItem.Comments != item.Comments) ExistingReviewItem.LastUpdateTime = DateTime.Now;
            }

            ExistingReviewCompletion.LastUpdateTime = DateTime.Now;

            if (ExistingReviewCompletion.LinkRecordUserSpecies.Any())
            {
                foreach (var ExistingSpecie in ExistingReviewCompletion.LinkRecordUserSpecies)
                {
                    if (ReviewIM.SpeciesIDs != null && ReviewIM.SpeciesIDs.Any() && ReviewIM.SpeciesIDs.Contains(ExistingSpecie.SpecieID) && ExistingSpecie.Status != "Current")
                    {
                        ExistingSpecie.Status = "Current";
                        ExistingSpecie.LastUpdateTime = DateTime.Now;
                    }
                    else
                    {
                        ExistingSpecie.Status = "Archived";
                        ExistingSpecie.LastUpdateTime = DateTime.Now;
                    }
                }
            }

            if (ReviewIM.SpeciesIDs != null && ReviewIM.SpeciesIDs.Any())
            {
                foreach (var linkRUP in from NewSpecieID in ReviewIM.SpeciesIDs where !ExistingReviewCompletion.LinkRecordUserSpecies.Select(l => l.SpecieID).Contains(NewSpecieID) select new LinkRecordUserSpecie()
                {
                    UserID = userid,
                    RecordID = ReviewIM.RecordID,
                    SpecieID = NewSpecieID,
                    ReviewCompletion = ExistingReviewCompletion,
                    LastUpdateTime = DateTime.Now
                })
                {
                    _db.LinkRecordUserSpecies.Add(linkRUP);
                }
            }

            if (Request.Form["Submit"] != null) ExistingReviewCompletion.ComplesionDate = DateTime.Now;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SubmitReview(int rcid)
        {
           var reviewComplesion = _db.ReviewCompletions.Find(rcid);

           if (reviewComplesion != null && reviewComplesion.Status == "Current")
           {
               reviewComplesion.ComplesionDate = DateTime.Now;
           }
           _db.SaveChanges();
           return RedirectToAction("Index");
        }

        public ActionResult UserReviewList(string username)
        {
            var user = _db.UserProfiles.FirstOrDefault(u => u.UserName == username);

            if (user == null) View("Error");

            var reviewComplesions = (from rcom in _db.ReviewCompletions
                where rcom.UserID == user.UserId
                && rcom.Record.DeleteRecord == false && rcom.Record.AcceptanceStatus == "Accepted" && rcom.Record.AcceptanceStatus_randomizer == "Pass TC1" 
                                     select rcom).AsEnumerable().Select(rc => 
                                     new ReviewListViewModel
                                    {
                                         Status = rc.Status,
                                        RecordID = rc.RecordRecordID,
                                        ReviewComplesionID = rc.ReviewCompletionID,
                                        CompleteTime = rc.ComplesionDate,
                                        CreateTime = rc.CreatedOn,
                                        LastupDateTime = rc.LastUpdateTime,
                                        nCompletedItems = rc.PaperQualities.Count(pq => pq.Status == "Current" && pq.OptionOptionID > 0 ),
                                        nSpecies = rc.LinkRecordUserSpecies.Count(l => l.Status == "Current"),
                                        nReviewCompleted = rc.Record.ReviewCompletions.Count(arc => arc.Status == "Current"),
                                        AgreementRate = getReviewAgreement(rc.Record.ReviewCompletions.Where(arc => arc.Status == "Current" && arc.ComplesionDate.HasValue).ToList())                               
                                    });

            return View(reviewComplesions.OrderByDescending(rc => rc.CompleteTime).ThenByDescending(rc => rc.LastupDateTime).ToList());
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ArchiveReview(int reviewCompletionID)
        {
            var reviewComplesion = _db.ReviewCompletions.Find(reviewCompletionID);

            if (reviewComplesion == null || reviewComplesion.Status != Enums.Status.Current.ToString()) { ViewBag.ErrorMessage = "Review not found."; return View("Error"); }

            reviewComplesion.Archive();

            _db.SaveChanges();

           return RedirectToAction("UserReviewList", new { username = reviewComplesion.UserProfile.UserName });
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult UnarchiveReview(int reviewCompletionID)
        {
            var reviewComplesion = _db.ReviewCompletions.Find(reviewCompletionID);

            if (reviewComplesion == null || reviewComplesion.Status == Enums.Status.Current.ToString()) { ViewBag.ErrorMessage = "Review not found."; return View("Error"); }

            reviewComplesion.Unarchive();

            _db.SaveChanges();

            return RedirectToAction("UserReviewList", new { username = reviewComplesion.UserProfile.UserName });
        }

        private string getReviewAgreement(IList<ReviewCompletion> reviewCompletions)
        {
            if (reviewCompletions.Count() < 2) { return "N/A";}
            if (reviewCompletions.Count() == 2)
            {
                var seniorReviews = reviewCompletions.FirstOrDefault(rc => rc.Role == Enums.userRoles.SeniorReviewer);
                var juniorReviews = reviewCompletions.FirstOrDefault(rc => rc.Role == Enums.userRoles.Reviewer);

                return CompareTwoReview(seniorReviews, juniorReviews).ToString("P", CultureInfo.InvariantCulture);
            }

                //var seniorReviews = reviewCompletions.(rc => rc.Role == Enums.userRoles.SeniorReviewer);
                //var juniorReviews = reviewCompletions.(rc => rc.Role == Enums.userRoles.Reviewer);

                //return CompareTwoReview(seniorReviews, juniorReviews).ToString("P", CultureInfo.InvariantCulture);


            return "";
        }

        private double CompareTwoReview(ReviewCompletion rc1, ReviewCompletion rc2)
        {
            var num = (from r1 in rc1.PaperQualities
                from r2 in
                    rc2.PaperQualities.Where(
                        rc2pq =>
                            rc2pq.CheckListCheckListID == r1.CheckListCheckListID &&
                            rc2pq.OptionOptionID == r1.OptionOptionID)
                select r1).Count();

            return num * 1.0/rc1.PaperQualities.Count();
        }

        // [Authorize(Roles = "Administrator")]
        //public ActionResult Review(int id)
        //{
        //    var userid = (int)Membership.GetUser().ProviderUserKey;
        //    var record = _db.Records.Find(id);
        //    var currentrecord = _db.PaperQualities.Count(pq => pq.RecordRecordID == id && pq.UserProfile.UserName == User.Identity.Name);
        //    if (currentrecord == 0)
        //    {
        //        var reviewViewmodel = _db.CheckLists.ToList().Select(checklist => new ReviewViewModel()
        //        {
        //            RecordRecordID = id, PaperNumber = record.PaperNumber, CheckListCheckListID = checklist.CheckListID, CheckListNumber = checklist.CheckListNumber, CheckListName = checklist.CheckListName, Criteria = checklist.Criteria, Item = checklist.Item, ItemNumber = checklist.ItemNumber, Section = checklist.Section
        //        }).ToList();
        //        var SpecieSpecieID = _db.LinkRecordUserSpecies.Where(l => l.RecordID == id && l.UserID == userid && l.Status != "Delete").Select(l => l.SpecieID);
        //        ViewBag.SpecieSpecieID = new MultiSelectList(_db.Species, "SpecieID", "SpecieName", SpecieSpecieID);
        //        return View(reviewViewmodel.ToList());
        //    }
        //    else
        //    {
        //        var reviewViewmodel = from rv in _db.PaperQualities
        //                              where rv.RecordRecordID == id && rv.Record.DeleteRecord != true && rv.UserProfile.UserName == User.Identity.Name
        //                              select new ReviewViewModel
        //                              {
        //                                  RecordRecordID = id,
        //                                  CheckListCheckListID = rv.CheckListCheckListID,
        //                                  PaperQualityID = rv.PaperQualityID,
        //                                  CheckListName = rv.CheckList.CheckListName,
        //                                  OptionID = rv.OptionOptionID,
        //                                  Criteria = rv.CheckList.Criteria,
        //                                  Comments = rv.Comments,
        //                                  LastUpdateTime = rv.LastUpdateTime,
        //                                  PaperNumber = rv.Record.PaperNumber,
        //                                  Item = rv.CheckList.Item,
        //                                  CheckListNumber = rv.CheckList.CheckListNumber,
        //                                  ItemNumber = rv.CheckList.ItemNumber,
        //                                  Section = rv.CheckList.Section,
        //                              };

        //        var SpecieSpecieID = _db.LinkRecordUserSpecies.Where(l => l.RecordID == id && l.UserID == userid && l.Status != "Delete").Select(l => l.SpecieID);
        //        ViewBag.SpecieSpecieID = new MultiSelectList(_db.Species, "SpecieID", "SpecieName", SpecieSpecieID);

        //        return View(reviewViewmodel.ToList());
        //    }
        //    //ViewBag.CheckListCheckListID = new SelectList(db.CheckLists, "CheckListID", "CheckListName");
        //    //return View();
        //}

        ////
        //// POST: /Review/Review/5

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles="Administrator")]
        //public ActionResult Review(int id, IList<ReviewViewModel> reviewRecord, List<int> Species)
        //{
        //    //var record = _db.Records.Find(id);

        //    var userid = (int)Membership.GetUser().ProviderUserKey;

        //    if (!ModelState.IsValid) return View("Error");

        //    if (_db.LinkRecordUserSpecies.Any(l => l.RecordID == id && l.UserID == userid && l.Status == "Current"))
        //    {
        //        var linkRup1 = from l in _db.LinkRecordUserSpecies
        //                       where l.RecordID == id && l.UserID == userid && l.Status != "Archived"
        //                       select l;

        //        foreach (var eachlink in linkRup1) eachlink.Status = "Archived";

        //        foreach (var linkRUP in Species.Select(eachSpecie => new LinkRecordUserSpecie
        //        {
        //            UserID = userid,
        //            RecordID = id,
        //            SpecieID = eachSpecie
        //        }))
        //        {
        //            _db.LinkRecordUserSpecies.Add(linkRUP);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var linkRUP in Species.Select(eachSpecie => new LinkRecordUserSpecie
        //        {
        //            UserID = userid,
        //            RecordID = id,
        //            SpecieID = eachSpecie
        //        }))
        //        {
        //            _db.LinkRecordUserSpecies.Add(linkRUP);
        //        }
        //    }

        //    foreach (var eachReview in reviewRecord)
        //    {
        //        if (eachReview.PaperQualityID > 0)
        //        {
        //            var pqrecord = _db.PaperQualities.Find(eachReview.PaperQualityID);
        //            pqrecord.OptionOptionID = eachReview.OptionID;
        //            pqrecord.Comments = eachReview.Comments;
        //        }
        //        else
        //        {
        //            var pq = new PaperQuality
        //            {
        //                CheckListCheckListID = eachReview.CheckListCheckListID,
        //                OptionOptionID = eachReview.OptionID,
        //                Comments = eachReview.Comments,
        //                UserProfile = {UserName = User.Identity.Name},
        //                RecordRecordID = eachReview.RecordRecordID,
        //                LastUpdateTime = DateTime.Now
        //            };
        //            _db.PaperQualities.Add(pq);
        //        }
        //    }

        //    _db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        ////
        //// GET: /Review/Read/5

        public ActionResult Read(int id)
        {
            ViewBag.N = _db.PaperDocuments.Count(m => m.DeletePaperDocument != true && m.RecordRecordID == id);

            Record record = _db.Records.Find(id);

            TempData["RecordID"] = record.RecordID;
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
                throw new HttpException(404, "File not Found.");
            }
        }

        public ActionResult Help()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
