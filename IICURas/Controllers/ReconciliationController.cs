using IICURas.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IICURas.Authorization;
using IICURas.Models.ViewModels;
using System.Globalization;

namespace IICURas.Controllers
{
    [Authorize(Roles = "Administrator, Reconciler")]
    [Restrict("Suspended")]
    public class ReconciliationController : Controller
    {
        private readonly IICURasContext _db = new IICURasContext();
        private int userid = (int)Membership.GetUser().ProviderUserKey;

        public ActionResult Index()
        {
            var user = _db.UserProfiles.Find(userid);

            var reviewCompletions = user.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString() && rc.Reconciliation == true);

            var todoNUmber = _db.Records.Count(r => r.DeleteRecord == false && r.PaperDocuments.Count(pd => pd.DeletePaperDocument == false) > 0 
                && r.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null && !rc.Reconciliation) >=2  
                && r.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.Reconciliation) == 0
                && r.ReviewCompletions.Count(rc => rc.UserID == userid && rc.Status == Enums.Status.Current.ToString() && !rc.Reconciliation) == 0);

            var reviewOverviewVm = new ReconOverviewViewModel(reviewCompletions, todoNUmber);

            return View(reviewOverviewVm);
        }

        public ActionResult NewReview()
        {
            var avaliableRecord = _db.Records.Where(
                    r =>r.PaperDocuments.Count(pd => pd.DeletePaperDocument == false) > 0 && r.DeleteRecord == false && r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1"
                    && r.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null && !rc.Reconciliation) >= 2
                    && r.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.Reconciliation) == 0
                    && r.ReviewCompletions.Count(rc => rc.UserID == userid && rc.Status == Enums.Status.Current.ToString() && !rc.Reconciliation) == 0);

            var  randomRecord = avaliableRecord.OrderBy(r => Guid.NewGuid()).FirstOrDefault();

            if (randomRecord == null)
            {
                throw new Exception("There are no more publication for you to review. Please contact admin for further assistance. Thank you for reviewing on IICARus Project");
            }

            var CheckList = _db.CheckLists.Include("ChecklistOptionLinks");

            var newReviewCompletion = new ReviewCompletion()
            {
                UserID = userid,
                RecordRecordID = randomRecord.RecordID,
                Role = Enums.userRoles.Reconciler,
                LastUpdateTime = DateTime.Now,
                Reconciliation = true
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

            var r1 = newReviewCompletion.Record.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()).OrderBy(s => s.LastUpdateTime).FirstOrDefault();
            var r2 = newReviewCompletion.Record.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()).OrderBy(s => s.LastUpdateTime).Skip(1).FirstOrDefault();
            var reviewIM = new ReconReviewInputModel(newReviewCompletion, r1, r2, _db.Species.ToList());
            
            return View(reviewIM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewReview(ReconReviewInputModel ReviewIM)
        {
            if (!ModelState.IsValid) throw new Exception("Model State error");

            var ExistingReviewCompletion = _db.ReviewCompletions.Find(ReviewIM.ReviewCompleteID);

            if (ExistingReviewCompletion.UserID != userid || ExistingReviewCompletion.Status != "Current") throw new Exception("Review not valid.");

            ExistingReviewCompletion.Role = IICURas.Enums.userRoles.Reconciler;

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
                if (ExistingReviewItem.Comments != item.Comments) ExistingReviewItem.Comments = item.Comments;
                if (ExistingReviewItem.OptionOptionID != item.OptionID || ExistingReviewItem.Comments != item.Comments) ExistingReviewItem.LastUpdateTime = DateTime.Now;
            }

            ExistingReviewCompletion.LastUpdateTime = DateTime.Now;

            if (ExistingReviewCompletion.LinkRecordUserSpecies.Any())
            {
                foreach (var ExistingSpecie in ExistingReviewCompletion.LinkRecordUserSpecies)
                {
                    if (ReviewIM.SpeciesIDs != null && ReviewIM.SpeciesIDs.Any() && ReviewIM.SpeciesIDs.Contains(ExistingSpecie.SpecieID))
                    {
                        if (ExistingSpecie.Status != "Current")
                        {
                            ExistingSpecie.Status = "Current";
                            ExistingSpecie.LastUpdateTime = DateTime.Now;
                        }
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
                foreach (var linkRUP in from NewSpecieID in ReviewIM.SpeciesIDs
                                        where !ExistingReviewCompletion.LinkRecordUserSpecies.Select(l => l.SpecieID).Contains(NewSpecieID)
                                        select new LinkRecordUserSpecie()
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
            var reviewComplesion = _db.ReviewCompletions.Find(rcid);

            if (reviewComplesion == null || reviewComplesion.Status != Enums.Status.Current.ToString()
                || reviewComplesion.PaperQualities.Count(pq => pq.Status == Enums.Status.Current.ToString()) == 0 || reviewComplesion.UserID != userid)
                throw new Exception("review not valid.");
            var r1 = reviewComplesion.Record.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()).OrderBy(s => s.LastUpdateTime).FirstOrDefault();
            var r2 = reviewComplesion.Record.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()).OrderBy(s => s.LastUpdateTime).Skip(1).FirstOrDefault();
            var reviewIM = new ReconReviewInputModel(reviewComplesion, r1, r2, _db.Species.ToList(), reviewComplesion);

            return View(reviewIM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContinueReview(ReviewInputModel ReviewIM)
        {
            if (!ModelState.IsValid) throw new Exception("Model State error");

            var ExistingReviewCompletion = _db.ReviewCompletions.Find(ReviewIM.ReviewCompleteID);

            if (ExistingReviewCompletion.UserID != userid || ExistingReviewCompletion.Status != "Current") throw new Exception("Review not valid.");

            ExistingReviewCompletion.Role = IICURas.Enums.userRoles.Reconciler;

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
                if (ExistingReviewItem.Comments != item.Comments) ExistingReviewItem.Comments = item.Comments;
                if (ExistingReviewItem.OptionOptionID != item.OptionID || ExistingReviewItem.Comments != item.Comments) ExistingReviewItem.LastUpdateTime = DateTime.Now;
            }

            ExistingReviewCompletion.LastUpdateTime = DateTime.Now;

            if (ExistingReviewCompletion.LinkRecordUserSpecies.Any())
            {
                foreach (var ExistingSpecie in ExistingReviewCompletion.LinkRecordUserSpecies)
                {
                    if (ReviewIM.SpeciesIDs != null && ReviewIM.SpeciesIDs.Any() && ReviewIM.SpeciesIDs.Contains(ExistingSpecie.SpecieID))
                    {
                        if(ExistingSpecie.Status != "Current")
                        { 
                            ExistingSpecie.Status = "Current";
                            ExistingSpecie.LastUpdateTime = DateTime.Now;
                        }
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
                foreach (var linkRUP in from NewSpecieID in ReviewIM.SpeciesIDs
                                        where !ExistingReviewCompletion.LinkRecordUserSpecies.Select(l => l.SpecieID).Contains(NewSpecieID)
                                        select new LinkRecordUserSpecie()
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

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


    }
}
