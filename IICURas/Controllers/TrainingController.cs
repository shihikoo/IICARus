using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IICURas.Models;
using IICURas.Authorization;

namespace IICURas.Controllers
{
    [Authorize(Roles = "Trainee, Reviewer")]
    [Restrict("Suspended")]
    public class TrainingController : Controller
    {
        private readonly IICURasContext _db = new IICURasContext();

        public ActionResult Index(string searchTerm, string sortOrder)
        {
            var user = _db.UserProfiles.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (user == null) return View("Error");

            var HasNewTraining = user.TrainingReviews.Count(tr => tr.Status == "Current") < _db.TrainingPublications.Count(td => td.Status == "Current");

            var NumOngoingTraining = user.TrainingReviews.Count(tr => tr.Status == "Current" && tr.Points == null);

            var HasCompletedTraining = user.TrainingReviews.Any(tr => tr.Status == "Current" && tr.Points != null);

            var CoverVM = new TrainingCoverViewModel
            {
                NewTraningLinkStyle = (HasNewTraining && NumOngoingTraining < (int) Enums.Training.OpeningTrainingLimit) ? "" : "non-active disabled",
                OnoingTraningLinkStyle = NumOngoingTraining > 0 ? "" : "non-active disabled",
                CompletedTraningLinkStyle = HasCompletedTraining ? "" : "non-active disabled",
                ArchiveLinkStyle = HasNewTraining ? "non-active disabled" : "",           
            };

            return View(CoverVM);
        }

        public ActionResult NewTraining()
        {
            var userid = (int)Membership.GetUser().ProviderUserKey;

            var allTraining = _db.TrainingPublications.Where(tp => !tp.TrainingReviews.Where(tr => tr.Status == "Current").Select(tr => tr.UserID).Contains(userid));

            if (!allTraining.Any()) return RedirectToAction("Index");

            var RandomTraining = allTraining.OrderBy(t => Guid.NewGuid()).FirstOrDefault();

            /////////////// save training review & training review items

            var newTrainingReview = new TrainingReview
            {
                UserID = userid,
                TrainingPublicationID = RandomTraining.TrainingPublicationID,
                TotalPoints = _db.CheckLists.Count(tr => !string.IsNullOrEmpty(tr.CheckListNumber)),
                LastUpdateTime = DateTime.Now
            };

            _db.TrainingReviews.Add(newTrainingReview);

            var newTrainReviewItem = (from item in _db.CheckLists
                where !string.IsNullOrEmpty(item.CheckListNumber)
                select item).AsEnumerable().Select(checklist => new TrainingReviewItem
                {
                    CheckListCheckListID = checklist.CheckListID,
                    ReviewerId = userid,
                    TrainingPublicationID = RandomTraining.TrainingPublicationID,
                    TrainingReview = newTrainingReview,
                    LastUpdateTime = DateTime.Now
                }).ToList();

            _db.TrainingReviewItems.AddRange(newTrainReviewItem);
            
            _db.SaveChanges();

            /////////////// create training input model

            var trainingInputModel = new TrainingInputModel
            {
                TrainingReviewID = newTrainingReview.TrainingReviewID,

                PubID = newTrainingReview.TrainingPublicationID,

                PubNumber = RandomTraining.PMID,

                Files = RandomTraining.TrainingDocuments.Where(pd => pd.Status == "Current").Select(pd => new ReadViewModel
                {
                    TrainingDocumentID = pd.TrainingDocumentID,
                    FileName = pd.FileName,
                    FileType = pd.FileType,
                    FileUrl = pd.FileUrl
                }).ToList(),

                TrainingReviewItems = newTrainReviewItem.Select(c => new TrainingReviewItemInputModel
                {
                    TrainingReviewItemID = c.TrainingReviewItemID,
                    TrainingPublicationID = RandomTraining.TrainingPublicationID,
                    CheckListID = c.CheckList.CheckListID,
                    Section = c.CheckList.Section,
                    Item = c.CheckList.Item,
                    ItemNumber = c.CheckList.ItemNumber,
                    CheckListName = c.CheckList.CheckListName,
                    CheckListNumber = c.CheckList.CheckListNumber,
                    Criteria = c.CheckList.Criteria,
                    Critical = c.CheckList.Critical,
                    Options = c.CheckList.ChecklistOptionLinks.Where(o => o.Status == "Current").Select(o => o.Option).ToList(),
                    TrainingReviewID = newTrainingReview.TrainingReviewID

                }).ToList().OrderBy(c => float.Parse(c.ItemNumber, CultureInfo.InvariantCulture.NumberFormat)).ThenBy(c => c.CheckListNumber).ToList()
            };

            return View(trainingInputModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewTraining(IEnumerable<TrainingReviewItemInputModel> trainingReviewItemIM)
        {

            if (!ModelState.IsValid) return View("Error");
            if (trainingReviewItemIM == null) return View("Error");

            var userid = (int)Membership.GetUser().ProviderUserKey;

            var user = _db.UserProfiles.Find(userid);

            var existingTraingReview = _db.TrainingReviews.Find(trainingReviewItemIM.FirstOrDefault().TrainingReviewID);

            foreach (var item in trainingReviewItemIM)
            {
                var existingTraingReviewItem = _db.TrainingReviewItems.Find(item.TrainingReviewItemID);

                if (existingTraingReviewItem == null || existingTraingReviewItem.Status != "Current" || existingTraingReviewItem.ReviewerId != userid) return View("Error");

                if (existingTraingReviewItem.OptionOptionID != item.OptionID) existingTraingReviewItem.OptionOptionID = item.OptionID;

                if (existingTraingReviewItem.Comments != item.Comments) existingTraingReviewItem.Comments = item.Comments;

                if (existingTraingReviewItem.OptionOptionID != item.OptionID || existingTraingReviewItem.Comments != item.Comments) existingTraingReviewItem.LastUpdateTime = DateTime.Now;
            }

            existingTraingReview.LastUpdateTime = DateTime.Now;

            _db.SaveChanges();

            //grade the review if used save and submit button
            if (Request.Form["Submit"] == null) return RedirectToAction("OnGoingTrainingList");

            var score = GetTheScore(trainingReviewItemIM.FirstOrDefault().TrainingPublicationID, existingTraingReview.TrainingReviewItems);

            existingTraingReview.Points = Math.Abs(score);
            existingTraingReview.Pass = score >= ((float)Enums.PassingCriteria.PassingPercentage);
            existingTraingReview.LastUpdateTime = DateTime.Now;

            _db.SaveChanges();

            //check whether the reviewer is qualified for a promotion or not.
            var promotionResult = PromotionCheck(user, _db.TrainingReviews);

            if (promotionResult > 0) Roles.AddUserToRole(user.UserName, "Reviewer");

            var submissionViewModel = new SubmissionViewModel
            {
                Pass = existingTraingReview.Pass == true ? true : false,
                Points = (int)existingTraingReview.Points,
                TotalPoints = existingTraingReview.TotalPoints,
                TraningReviewID = existingTraingReview.TrainingReviewID,
                Promoted = promotionResult > 0,
                PromotionID = promotionResult
            };

            return View("Submission", submissionViewModel);
        }


        public ActionResult SubmitTraining(int trainingReviewid)
        {
            //var totalPoints = _db.CheckLists.Count(c => c.CheckListName != null && c.CheckListName != "");

            var userid = (int)Membership.GetUser().ProviderUserKey;

            var user = _db.UserProfiles.Find(userid);

            var trainingReview = _db.TrainingReviews.Find(trainingReviewid);

            if (trainingReview == null || trainingReview.UserID != userid || trainingReview.Status != "Current" || trainingReview.TrainingReviewItems.Count(tri => tri.Status == "Current") == 0) return View("Error");

            var score = GetTheScore(trainingReview.TrainingPublicationID, trainingReview.TrainingReviewItems.Where(tri => tri.Status == "Current"));

            trainingReview.Points = Math.Abs(score);
            trainingReview.Pass = score >= (_db.CheckLists.Count(c => c.CheckListName != null && c.CheckListName != "") * (float)Enums.PassingCriteria.PassingPercentage)/100;
            trainingReview.LastUpdateTime = DateTime.Now;

                _db.SaveChanges();

                //check whether the reviewer is qualified for a promotion or not.
                var promotionResult = PromotionCheck(user, _db.TrainingReviews);

                if (promotionResult > 0) Roles.AddUserToRole(user.UserName, "Reviewer");

                var submissionViewModel = new SubmissionViewModel
                {
                    Pass = trainingReview.Pass == true ? true : false,
                    Points = (int)trainingReview.Points,
                    TotalPoints = trainingReview.TotalPoints,
                    TraningReviewID = trainingReview.TrainingReviewID,
                    Promoted = promotionResult > 0,
                    PromotionID = promotionResult
                };

                return View("Submission", submissionViewModel);
        }

        private int PromotionCheck(UserProfile user, IEnumerable<TrainingReview> trainingReviews)
        {
            const int passingTimes = (int)Enums.PassingCriteria.PassingTimes;

            if ( _db.Promotions.Count(p => p.UserID == user.UserId && p.status == "Current") != 0 || 
                trainingReviews
                    .Where(tr => tr.UserID == user.UserId && tr.Status == "Current" && tr.Pass != null)
                    .OrderByDescending(tr => tr.LastUpdateTime)
                    .Take(passingTimes)
                    .Count(tr => tr.Pass == true) != passingTimes) return 0;

            var promotion = new Promotion
            {
                UserID = user.UserId,
                oldRole = Enums.userRoles.Trainee,
                newRole = Enums.userRoles.Reviewer,
                Name = user.ForeName + user.SurName,
                LastUpdateTime = DateTime.Now,
            };

            _db.Promotions.Add(promotion);
            _db.SaveChanges();
            return promotion.PromotionID;
        }
           
        //   just a small test for looking at the submission page. No real use
        [Authorize(Roles = "Administrator")]
        public ActionResult TestSubmission()
        {
            var submissionViewModel = new SubmissionViewModel
            {
                Pass = true,
                Points = 10,
                TotalPoints = 20,
                TraningReviewID = 1,
                Promoted = true,
                PromotionID = 2
            };

            return View("Submission", submissionViewModel);
        }

        private int GetTheScore(int pubid, IEnumerable<TrainingReviewItem> trainingReviewItems)
        {
            var goldstandard = _db.GoldStandards.Where(gs => gs.PublicationPublicationID == pubid);

            if (!goldstandard.Any()) return 0;

            var wrongAnswers = ( from tr in trainingReviewItems.ToList()
                from gs in
                    goldstandard.Where(
                        gs =>
                            tr.CheckListCheckListID == gs.CheckListCheckListID && tr.OptionOptionID != gs.OptionOptionID)
                select gs).ToList();

            var score = (goldstandard.Count() - wrongAnswers.Count()) * (wrongAnswers.Any(a => a.CheckList.Critical) ? -1 : 1);

            return score;
        }

        public ActionResult CompletedTrainingList(string username)
        {
            int userid;

            if (username == null) userid = (int)Membership.GetUser().ProviderUserKey;
            else if (User.IsInRole("Administrator"))
            {
                userid = (int)Membership.GetUser(username).ProviderUserKey;
                if (userid == 0) return View("Error");
            }
            else
            {
                return View("Error");
            }

            //int userid = (int)Membership.GetUser().ProviderUserKey;

            var query = _db.TrainingReviews
                .Where(tr => tr.UserID == userid && tr.Points != null)
                .Select(tr => new CompletedTrainingListViewModel
            {
                UserID = tr.UserID,
                PubID = tr.TrainingPublicationID,
                PubNumber = tr.TrainingPublication.TrainingPublicationNumber,
                Points = tr.Points,
                CompletionTime = tr.LastUpdateTime,
                Pass = tr.Pass,
                TotalPoints = tr.TotalPoints,
                TraningReviewID = tr.TrainingReviewID,
                Status = tr.Status,
                StartTime = tr.CreatedOn,
                
                });

            return View(query.ToList());
        }

        public ActionResult OnGoingTrainingList()
        {
            int userid = (int)Membership.GetUser().ProviderUserKey;

            var query = _db.TrainingReviews.Where(tr => tr.UserID == userid && tr.Points == null && tr.Status == "Current")
                .Select(tr => new OngoingTrainingListViewModel
            {
                UserID = tr.UserID,
                PubID = tr.TrainingPublicationID,
                PubNumber = tr.TrainingPublication.TrainingPublicationNumber,
                LastUpdatedTime = tr.LastUpdateTime,
                TraningReviewID = tr.TrainingReviewID,
                NumberOfTotalItems = tr.TrainingReviewItems.Count(tri => tri.Status == "Current"),
                NumberOfCompletedItems = tr.TrainingReviewItems.Count(tri => tri.Status == "Current" && tri.OptionOptionID > 0)
            });

            return View(query);
        }

        public ActionResult Transcript(int trainingReviewid)
        {
            var traningReview = _db.TrainingReviews.Find(trainingReviewid);

            if (traningReview == null || traningReview.Status != "Current" )
            {
                ViewBag.ErrorMessage = "Transcript not avliable.";
                View("Error");
            }

            int userid = (int)Membership.GetUser().ProviderUserKey;

            if (traningReview.UserID != userid && Roles.IsUserInRole("Administrator"))
            {
                ViewBag.ErrorMessage = "You do not have the rights to view this transcript.";
                View("Error");
            }

       //     var GoldStandards = _db.GoldStandards.Where(gs => gs.PublicationPublicationID == traningReview.TrainingPublicationID);

            var transcriptVM = new TranscriptViewModel
            {
                PubID = traningReview.TrainingPublicationID,
                PubNumber = traningReview.TrainingPublication.TrainingPublicationNumber,
                Points = traningReview.Points,
                CompletionTime = traningReview.LastUpdateTime,
                CreateTime = traningReview.CreatedOn,
                Pass = traningReview.Pass,
                TotalPoints = traningReview.TotalPoints,
                Items = GetTranscriptItem(traningReview.TrainingReviewItems, _db.GoldStandards.Where(gs => gs.PublicationPublicationID == traningReview.TrainingPublicationID)).ToList()
            };

            if (transcriptVM.passCheatCriteria) return View(transcriptVM);

            foreach (var item in transcriptVM.Items) item.CorrectAnswer = "";

            return View(transcriptVM);
        }

        private static IEnumerable<TranscriptItemViewModel> GetTranscriptItem(IEnumerable<TrainingReviewItem> traningReviewItems, IEnumerable<GoldStandard> GoldStandards)
        {
            return (from gs in GoldStandards
                join item in traningReviewItems on gs.CheckListCheckListID equals item.CheckListCheckListID into combine
                from newitem in combine.DefaultIfEmpty()
                select new TranscriptItemViewModel
                {
                    TrainingPublicationID = gs.PublicationPublicationID,
                    CheckListID = gs.CheckListCheckListID,
                    Comments = gs.Comments,
                    CorrectOptionID = gs.OptionOptionID,
                    CorrectAnswer = gs.Option.OptionName,
                    YourOptionID = newitem == null ? 0 : newitem.OptionOptionID,
                    YourComments = newitem == null ? "" : newitem.Comments,
                    YourAnswer = (newitem?.Option == null) ? "-" : newitem.Option.OptionName,
                    Section = gs.CheckList.Section,
                    ItemNumber = gs.CheckList.ItemNumber,
                    Item = gs.CheckList.Item,
                    CheckListNumber = gs.CheckList.CheckListNumber,
                    CheckListName = gs.CheckList.CheckListName,
                    Criteria = gs.CheckList.Criteria,
                    Critical = gs.CheckList.Critical
                }).OrderBy(c => float.Parse(c.ItemNumber, CultureInfo.InvariantCulture.NumberFormat))
                .ThenBy(c => c.CheckListNumber);
        }

        public ActionResult ArchiveTraining()
        {
            int userid = (int)Membership.GetUser().ProviderUserKey;

            var reviews = _db.TrainingReviews.Where(r => r.UserID == userid && r.Status == "Current");

            var reviewItems = _db.TrainingReviewItems.Where(r => r.ReviewerId == userid && r.Status == "Current");

            foreach (var rev in reviews) rev.Status = "Archived";

            foreach (var revItem in reviewItems) revItem.Status = "Archived";

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ContinueTraining(int trainingReviewid)
        {
            int userid = (int)Membership.GetUser().ProviderUserKey;

            var existingTraining = _db.TrainingReviews.Find(trainingReviewid);

            if (existingTraining == null || existingTraining.UserID != userid || existingTraining.Status != "Current") return View("Error");

            var trainingInputModel = new TrainingInputModel
            {
                PubID = existingTraining.TrainingPublicationID,

                PubNumber = existingTraining.TrainingPublication.TrainingPublicationNumber,

                Files = existingTraining.TrainingPublication.TrainingDocuments.Where(pd => pd.Status == "Current").Select(pd => new ReadViewModel
                {
                    TrainingDocumentID = pd.TrainingDocumentID,
                    FileName = pd.FileName,
                    FileType = pd.FileType,
                    FileUrl = pd.FileUrl
                }).ToList(),

                TrainingReviewItems = existingTraining.TrainingReviewItems.Select(item => new TrainingReviewItemInputModel
                {
                    TrainingPublicationID = item.TrainingPublicationID,
                    CheckListID = item.CheckListCheckListID,
                    Section = item.CheckList.Section,
                    Item = item.CheckList.Item,
                    ItemNumber = item.CheckList.ItemNumber,
                    CheckListName = item.CheckList.CheckListName,
                    CheckListNumber = item.CheckList.CheckListNumber,
                    Criteria = item.CheckList.Criteria,
                    Critical = item.CheckList.Critical,
                    Options = item.CheckList.ChecklistOptionLinks.Where(o => o.Status == "Current").Select(o => o.Option).ToList(),
                    TrainingReviewItemID = item.TrainingReviewItemID,
                    Comments = item.Comments,
                    OptionID = item.OptionOptionID,
                    TrainingReviewID = item.TrainingReviewID
                }).ToList().OrderBy(c => float.Parse(c.ItemNumber, CultureInfo.InvariantCulture.NumberFormat)).ThenBy(c => c.CheckListNumber).ToList()
            };

            return View(trainingInputModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ContinueTraining(IEnumerable<TrainingReviewItemInputModel> trainingReviewItemIM)
        {
            if (!ModelState.IsValid) return View("Error");
            if (trainingReviewItemIM == null) return View("Error");

            var userid = (int)Membership.GetUser().ProviderUserKey;

            var user = _db.UserProfiles.Find(userid);

            var existingTraingReview = _db.TrainingReviews.Find(trainingReviewItemIM.FirstOrDefault().TrainingReviewID);

            foreach (var item in trainingReviewItemIM)
            {
                var existingTraingReviewItem = _db.TrainingReviewItems.Find(item.TrainingReviewItemID);

                if (existingTraingReviewItem == null || existingTraingReviewItem.Status != "Current" || existingTraingReviewItem.ReviewerId != userid) return View("Error");

                if (existingTraingReviewItem.OptionOptionID != item.OptionID) existingTraingReviewItem.OptionOptionID = item.OptionID;

                if(existingTraingReviewItem.Comments != item.Comments) existingTraingReviewItem.Comments = item.Comments;

                if (existingTraingReviewItem.OptionOptionID != item.OptionID || existingTraingReviewItem.Comments != item.Comments) existingTraingReviewItem.LastUpdateTime = DateTime.Now;
            }

            existingTraingReview.LastUpdateTime = DateTime.Now;

            _db.SaveChanges();

            //grade the review if used save and submit button
            if (Request.Form["Submit"] == null) return RedirectToAction("OnGoingTrainingList");

            var score = GetTheScore(trainingReviewItemIM.FirstOrDefault().TrainingPublicationID, existingTraingReview.TrainingReviewItems);

            existingTraingReview.Points = Math.Abs(score);
            existingTraingReview.Pass = score >= ((float)Enums.PassingCriteria.PassingPercentage);
            existingTraingReview.LastUpdateTime = DateTime.Now;
            
            _db.SaveChanges();

            //check whether the reviewer is qualified for a promotion or not.
            var promotionResult = PromotionCheck(user, _db.TrainingReviews);

            if (promotionResult > 0) Roles.AddUserToRole(user.UserName, "Reviewer");

            var submissionViewModel = new SubmissionViewModel
            {
                Pass = existingTraingReview.Pass == true ? true : false,
                Points = (int)existingTraingReview.Points,
                TotalPoints = existingTraingReview.TotalPoints,
                TraningReviewID = existingTraingReview.TrainingReviewID,
                Promoted = promotionResult > 0,
                PromotionID = promotionResult
            };

            return View("Submission", submissionViewModel);
        }

        [Authorize(Roles=("Administrator"))]
        public ActionResult GoldStandardList()
        {
            var goldstandardlist = _db.TrainingPublications.Select(tp => new GoldStandardListViewModel
                {
                    TrainingPublicationID = tp.TrainingPublicationID,

                    TrainingPublicationNumber = tp.TrainingPublicationNumber
                });
            return View(goldstandardlist);
        }

        [Authorize(Roles = ("Administrator"))]
        public ActionResult GoldStandardReview(int pubid)
        {
            var trainingpub = _db.TrainingPublications.FirstOrDefault(tp => tp.Status == "Current" && tp.TrainingPublicationID == pubid);

            if (trainingpub == null)
            {
                ViewBag.ErrorMessage = "Training publication not found.";
                return View("Error");
            }

            var goldstandardsVM = new GoldStandardViewModel
            {
                PublicationPublicationID = trainingpub.TrainingPublicationID,
                PubNumber = trainingpub.TrainingPublicationNumber,
                Files = trainingpub.TrainingDocuments.Where(pd => pd.Status == "Current").Select(pd => new ReadViewModel
                {
                    TrainingDocumentID = pd.TrainingDocumentID,
                    FileName = pd.FileName,
                    FileType = pd.FileType,
                    FileUrl = pd.FileUrl
                }).ToList(),

                GoldStandardItems = trainingpub.GoldStandards.Select(gs => new GoldStandardItemViewModel
                {
                    CheckListCheckListID = gs.CheckListCheckListID,
                    CheckListName = gs.CheckList.CheckListName,
                    CheckListNumber = gs.CheckList.CheckListNumber,
                    Criteria = gs.CheckList.Criteria,
                    Critical = gs.CheckList.Critical,
                    GoldStandardID = gs.GoldStandardID,
                    GSComments = gs.Comments,
                    Item = gs.CheckList.Item,
                    ItemNumber = gs.CheckList.ItemNumber,
                    Section = gs.CheckList.Section,
                    GSOptionValue = gs.Option.OptionName,
                    GoldStandardItemsByReviewer = GetGoldStandardItemsByReviewer(gs)
                }).ToList().OrderBy(c => float.Parse(c.ItemNumber, CultureInfo.InvariantCulture.NumberFormat)).ThenBy(c => c.CheckListNumber).ToList()
                
            };
            return View(goldstandardsVM);
        }


        [Authorize(Roles = ("Administrator"))]
        private IEnumerable<GoldStandardItemByReviewerViewModel> GetGoldStandardItemsByReviewer(GoldStandard gs)
        {
            var trainingReviewItems = gs.TrainingPublication.TrainingReviewItems.Where(tr => tr.Status == "Current" && tr.CheckListCheckListID == gs.CheckListCheckListID && tr.TrainingReview.Status == "Current" && tr.TrainingReview.Points != null);

            if (!trainingReviewItems.Any()) return null;

            var goldStandardItemByReviewerVM = trainingReviewItems.Select(tri => new GoldStandardItemByReviewerViewModel
            {
                        ReviewerUsername = tri.UserProfile.UserName,
                        ReviewerOptionValue = tri.OptionOptionID == null ? "" : tri.Option.OptionName,
                        ReviewerComments = tri.Comments,
                        ColorStyle = tri.OptionOptionID == null ? "red" : tri.OptionOptionID == gs.OptionOptionID ? "green" : "red"
                    });

            return goldStandardItemByReviewerVM.ToList();
        }

        [Authorize(Roles = ("Administrator"))]
        public ActionResult EditGoldStandard(int gsid)
        {
            var gs = _db.GoldStandards.Find(gsid);
            var gsVM = new GoldStandardInputModel
            {
                PublicationID = gs.PublicationPublicationID,

                PubNumber = gs.TrainingPublication.TrainingPublicationNumber,

                GoldStandardItemIM = new GoldStandardItemInputModel
                {
                    Comments = gs.Comments,
                    GoldStandardID = gs.GoldStandardID,
                    OptionID = gs.OptionOptionID,
                     OptionValue = gs.Option.OptionName,
                    Options = gs.CheckList.ChecklistOptionLinks.Select(o => o.Option).ToList()
                },
                ChecklistItemIM = new ChecklistItemInputModel
                {
                    GoldStandardID = gs.GoldStandardID,
                    CheckListID = gs.CheckListCheckListID,
                    CheckListName = gs.CheckList.CheckListName,
                    CheckListNumber = gs.CheckList.CheckListNumber,
                    Section = gs.CheckList.Section,
                    Item = gs.CheckList.Item,
                    Criteria = gs.CheckList.Criteria,
                    ItemNumber = gs.CheckList.ItemNumber,
                    Critical = gs.CheckList.Critical,
                    CurrentOptions = gs.CheckList.ChecklistOptionLinks.Where(o => o.Status == "Current").Select(co => co.Option),
                    Options = _db.Options.Distinct()
                }

            };

            return View(gsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Administrator"))]
        public ActionResult EditGoldStandardItem(GoldStandardItemInputModel GoldStandardItemIM)
        {
            if (!ModelState.IsValid) return View("Error");

            var gs = _db.GoldStandards.Find(GoldStandardItemIM.GoldStandardID);

            gs.Comments = GoldStandardItemIM.Comments;
            gs.OptionOptionID = GoldStandardItemIM.OptionID;           

            _db.SaveChanges();

            return RedirectToAction("EditGoldStandard", new { gsid = gs.GoldStandardID});
        }
          
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Administrator"))]
        public ActionResult EditChecklistItem(ChecklistItemInputModel ChecklistItemIM)
        {
            if (!ModelState.IsValid) return View("Error");

            var checklist = _db.CheckLists.Find(ChecklistItemIM.CheckListID);

            checklist.CheckListName = ChecklistItemIM.CheckListName;
            checklist.CheckListNumber = ChecklistItemIM.CheckListNumber;
            checklist.Criteria = ChecklistItemIM.Criteria;
            checklist.Critical = ChecklistItemIM.Critical;
            checklist.Item = ChecklistItemIM.Item;
            checklist.ItemNumber = ChecklistItemIM.ItemNumber;
            checklist.Section = ChecklistItemIM.Section;

            _db.SaveChanges();

            return RedirectToAction("EditGoldStandard", new { gsid = ChecklistItemIM.GoldStandardID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Administrator"))]
        public ActionResult AddOption(ChecklistItemInputModel ChecklistItemIM)
        {
            if (!ModelState.IsValid) return View("Error");

            var checklistOptionLink = _db.ChecklistOptionLinks.FirstOrDefault(col => col.CheckListID == ChecklistItemIM.CheckListID && col.OptionID == ChecklistItemIM.AddOptionID);

            if (checklistOptionLink == null)
            {
                checklistOptionLink = new ChecklistOptionLink
                {
                    CheckListID = ChecklistItemIM.CheckListID,
                    OptionID = ChecklistItemIM.AddOptionID
                };
                _db.ChecklistOptionLinks.Add(checklistOptionLink);
            }
            else
            {
                if (checklistOptionLink.Status != "Current") checklistOptionLink.Status = "Current";
            }

            _db.SaveChanges();

            return RedirectToAction("EditGoldStandard", new { gsid = ChecklistItemIM.GoldStandardID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOption(ChecklistItemInputModel ChecklistItemIM, int OptionID)
        {
            if (!ModelState.IsValid) return View("Error");

            var checklistOptionLink = _db.ChecklistOptionLinks.FirstOrDefault(col => col.CheckListID == ChecklistItemIM.CheckListID && col.OptionID == OptionID);

            if(checklistOptionLink == null) 
            {
                return View("Error");
            }
            if (checklistOptionLink.Status == "Current") checklistOptionLink.Status = "Archived";

            _db.SaveChanges();

            return RedirectToAction("EditGoldStandard", new { gsid = ChecklistItemIM.GoldStandardID });
        }

        public ActionResult Read(int id)
        {
            var TrainingPublication = _db.TrainingPublications.Find(id);

            if (TrainingPublication == null || TrainingPublication.Status != "Current")
            {
                ViewBag.ErrorMessage = "Manuscript not found.";
                return View("Error");
            }

            var ReadListVM = new ReadListViewModel
                                         {
                                             RecordID = id,
                PaperNumber = TrainingPublication.TrainingPublicationNumber.ToString(),
                                             Files = TrainingPublication.TrainingDocuments.Where(pd => pd.Status == "Current").Select(pd => new ReadViewModel
                                             {
                                                 TrainingDocumentID = pd.TrainingDocumentID,
                                                 FileName = pd.FileName,
                                                 FileType = pd.FileType,
                                                 FileUrl = pd.FileUrl
                                             }).ToList()
                                         };
            return View(ReadListVM);

        }

        public FileResult DownloadDocument(int tdid)
        {
            TrainingDocument traingDocument = _db.TrainingDocuments.Find(tdid);
            if (ModelState.IsValid)
            {
                return File(traingDocument.FileUrl, traingDocument.FileType);
            }
            throw new HttpException(404, "File not Found.");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}