using IICURas.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
//using System.Web;
using System.Web.Mvc;
//using System.Web.Security;
//using Microsoft.Ajax.Utilities;
//using WebMatrix.WebData;
using IICURas.Authorization;
using IICURas.Models.ViewModels;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;

namespace IICURas.Controllers
{ 
    [Authorize]
    [Restrict("Suspended")]

    public class SummaryController : Controller
    {
        private readonly IICURas.Models.IICURasContext _db = new IICURas.Models.IICURasContext();
        //     
        // GET: /Summary/
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var records = _db.Records.Where(r => r.DeleteRecord != true
            //&& r.Category.CategoryName == "Intervention" 
            );

            var summaryVM = new SummaryViewModel()
            {
                   TotalRecord = (int)IICURas.Enums.RecordLimit.Total,

                   numberRecords = records.Count(),

                   numberRecordsPassTC1 = records.Count(r => r.AcceptanceStatus_randomizer == "Pass TC1"),

                   numberRecordsFailTC1 = records.Count(r => r.AcceptanceStatus_randomizer != null && r.AcceptanceStatus_randomizer != "Pass TC1"),

                   numberAccept = records.Count(r => r.AcceptanceStatus != null && r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1"),

                   numberNonAccept = records.Count(r => r.AcceptanceStatus_randomizer == "Pass TC1" 
                   && ((r.AcceptanceStatus != null && r.AcceptanceStatus != "Accepted") 
                   || r.MNAcceptance.Contains("Reject") 
                   || r.MNAcceptance.Contains("Withdrawn") || 
                   r.MNAcceptance.Contains("N/A")
                   //|| r.MNAcceptance.Contains("Incomplete")
                   || r.MNAcceptance.Contains("Error")
                   )),

                numberUploaded = records.Count(r => r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1" && r.PaperDocuments.Count(pd => pd.DeletePaperDocument != true) > 0),

                numberReconciliationCompleted = records.Count(r => r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1" && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.Reconciliation) >=1 && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null && rc.Reconciliation) >= 1),

                numberReconciliationInProgress = records.Count(r => r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1" && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.Reconciliation) >= 1 && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null && rc.Reconciliation) == 0),

                numberDoubleReviewed = records.Count(r => r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1" && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.Reconciliation == false) >= 2 && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.Reconciliation == false && rc.ComplesionDate != null) >= 2 && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.Reconciliation) == 0),

                numberDoubleReviewInProgress = records.Count(r => r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1" && r.ReviewCompletions.Count(rc => rc.Status == "Current") == 2 && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 1 && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.Reconciliation) == 0),

                numberSingleReiewed = records.Count(r => r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1" && r.ReviewCompletions.Count(rc => rc.Status == "Current") == 1 && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 1),

                numberSingleReiewedInProgress = records.Count(r =>  r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1" && r.ReviewCompletions.Count(rc => rc.Status == "Current") == 1 && r.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 0 ),

                ProgressArray = GetProgressArray(records.SelectMany(r => r.ReviewCompletions.Where(rc => rc.Status == "Current" && rc.LastUpdateTime != null)).AsEnumerable())
            };

            return View(summaryVM);
        }

        private static double[,] GetProgressArray(IEnumerable<ReviewCompletion> reviewCompletions)
        {
            const long nCut = 10;

            var progressArray = new double[nCut, 2];

            var startDate = reviewCompletions.Select(rc => rc.ComplesionDate).Min()?? default(DateTime);

            var endDate = reviewCompletions.Select(rc => rc.ComplesionDate).Max() ?? default(DateTime);
              
            var timeGrid = (endDate - startDate).TotalSeconds/nCut;

            for (var ii = 0; ii < nCut; ii++)
            {
                progressArray[ii, 0] = startDate.AddSeconds(timeGrid * (ii + 1)).ToOADate();
                progressArray[ii, 1] = reviewCompletions.Count(rc => rc.ComplesionDate >= startDate
                    && rc.ComplesionDate <= startDate.AddSeconds(timeGrid * (ii + 1)));
            }

            return progressArray;
        }

        public ActionResult TopScore()
        {
            var scoreVM = new LeaderboardViewModel(
                _db.UserProfiles.Include(up => up.ReviewCompletions).Where(
                    u =>
                        u.Promotions.Any(p => p.status == "Current") ||
                        u.ReviewCompletions.Any(rc => rc.Status == "Current" && rc.ComplesionDate != null))
                );

            return View(scoreVM);
        }

        public ActionResult FinalResult()
        {
            //var FinalResultVM = new ReviewResultViewModel(_db.Records.Where(r =>
            //r.DeleteRecord == false
            //&& r.AcceptanceStatus == "Accepted"
            //&& r.AcceptanceStatus_randomizer == "Pass TC1"
            //&& r.ReviewCompletions.Count(rc => rc.ComplesionDate != null && rc.Status == Enums.Status.Current.ToString()
            //&& rc.Reconciliation
            //) == 1)
            //    .SelectMany(r => r.ReviewCompletions.FirstOrDefault(rc =>
            //    rc.ComplesionDate != null
            //    && rc.Status == Enums.Status.Current.ToString()
            //    && rc.Reconciliation
            //    ).PaperQualities.Where(pq => pq.Status == Enums.Status.Current.ToString())).ToList());

            var FinalResultVM = new ReviewResultViewModel(_db.Records.Where(r =>
                    r.DeleteRecord == false
                    && r.AcceptanceStatus == "Accepted"
                    && r.AcceptanceStatus_randomizer == "Pass TC1"
                    && r.ReviewCompletions.Count(rc => rc.ComplesionDate != null && rc.Status == Enums.Status.Current.ToString() && rc.Reconciliation ) == 1)
                    .SelectMany(r => r.ReviewCompletions.Where(rc =>
                        rc.ComplesionDate != null
                        && rc.Status == Enums.Status.Current.ToString()
                )).SelectMany(rc => rc.PaperQualities.Where(pq => pq.Status == Enums.Status.Current.ToString())).ToList());

            return File(new System.Text.UTF8Encoding().GetBytes(CollectionToCsv(FinalResultVM.ReviewResultVMs)), "text/csv", "ReviewFinalResult.csv");
        }

        public ActionResult FirstReviewResult()
        {
            var FirstReviewResultVM = new ReviewResultViewModel(_db.Records.Where(r => r.DeleteRecord == false
             && r.AcceptanceStatus == "Accepted" && r.AcceptanceStatus_randomizer == "Pass TC1"
             && r.ReviewCompletions.Count(rc => rc.ComplesionDate != null && rc.Status == Enums.Status.Current.ToString() && rc.Reconciliation) == 1)
                 .SelectMany(r => r.ReviewCompletions.FirstOrDefault(rc => rc.ComplesionDate != null && rc.Status == Enums.Status.Current.ToString() 
                 && !rc.Reconciliation).PaperQualities.Where(pq => pq.Status == Enums.Status.Current.ToString())).ToList());

            return File(new System.Text.UTF8Encoding().GetBytes(CollectionToCsv(FirstReviewResultVM.ReviewResultVMs)), "text/csv", "ReviewResult.csv");
        }

        public ActionResult SpeciesResult()
        {
            var SpeciesVM = new SpeciesResultViewModel(_db.Records.Where(r => 
            r.DeleteRecord == false
  && r.AcceptanceStatus == "Accepted"
            && r.AcceptanceStatus_randomizer == "Pass TC1"
            && r.ReviewCompletions.Count(rc => rc.ComplesionDate != null && rc.Status == Enums.Status.Current.ToString() && rc.Reconciliation) == 1)
              .SelectMany(r =>  r.ReviewCompletions.FirstOrDefault(rc => rc.ComplesionDate != null && rc.Status == Enums.Status.Current.ToString() && rc.Reconciliation).LinkRecordUserSpecies).ToList());

            string[] propertyNames = { "PublicatinID", "SpeciesID", "SpeciesName" };

            var foo = SpeciesVM.SpeciesVMs as List<SpeciesViewModel>;

            //return File(CsvFileWriter.WriteToFile<SpeciesViewModel>("SpeciesResult.csv", foo, propertyNames));

            return File(new System.Text.UTF8Encoding().GetBytes(CollectionToCsv(SpeciesVM.SpeciesVMs)), "text/csv", "SpeciesResult.csv");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ChecklistReview()
        {
            var Reviews = (from r in _db.Records.Where(p => p.DeleteRecord != true && p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) >= 2).SelectMany(p => p.PaperQualities)
                           group r by new { r.CheckListCheckListID, r.RecordRecordID } into pairedReviews
                           select new ChecklistPublicationReviewViewModel()
                           {
                               ChecklistID = pairedReviews.FirstOrDefault().CheckListCheckListID,
                               Section = pairedReviews.FirstOrDefault().CheckList.Section,
                               ItemNumber = pairedReviews.FirstOrDefault().CheckList.ItemNumber,
                               Item = pairedReviews.FirstOrDefault().CheckList.Item,
                               Agreement = pairedReviews.Count() > 1 && (pairedReviews.Select(pr => pr.OptionOptionID).Min() == pairedReviews.Select(pr => pr.OptionOptionID).Max()),
                               CheckListNumber = pairedReviews.FirstOrDefault().CheckList.CheckListNumber,
                               CheckListName = pairedReviews.FirstOrDefault().CheckList.CheckListName,
                               Criteria = pairedReviews.FirstOrDefault().CheckList.Criteria,
                               Critical = pairedReviews.FirstOrDefault().CheckList.Critical
                           }).ToList();

            var AgreementRateVM = from r in Reviews
                                  group r by r.ChecklistID into groupedReviews
                                  select new ChecklistReviewViewModel
                                  {
                                      ChecklistID = groupedReviews.FirstOrDefault().ChecklistID,
                                      Section = groupedReviews.FirstOrDefault().Section,
                                      ItemNumber = groupedReviews.FirstOrDefault().ItemNumber,
                                      Rate = (double)groupedReviews.Count(gr => gr.Agreement) / groupedReviews.Count(),
                                      Item = groupedReviews.FirstOrDefault().Item,
                                      CheckListNumber = groupedReviews.FirstOrDefault().CheckListNumber,
                                      CheckListName = groupedReviews.FirstOrDefault().CheckListName,
                                      Criteria = groupedReviews.FirstOrDefault().Criteria,
                                      Critical = groupedReviews.FirstOrDefault().Critical,
                                  };

            return View(AgreementRateVM.OrderBy(ar => ar.ChecklistID).ToList());
        }

        [Authorize(Roles = "Funder")]
        public ActionResult ResultsPreview()
        {
            var Reviews = _db.ReviewCompletions.Where(rc => rc.ComplesionDate != null && rc.Reconciliation &&  rc.Status == Enums.Status.Current.ToString() && rc.Record.DeleteRecord != true && rc.PaperQualities.Count(pq => pq.Status == Enums.Status.Current.ToString())>0)
            .SelectMany(p => p.PaperQualities.Where(pq => pq.Status == Enums.Status.Current.ToString()))
                .GroupBy(pq => pq.CheckListCheckListID).ToList().Select(qpg => new ChecklistResultViewModel(qpg)).ToList();

            return View(Reviews.OrderBy(ar => ar.ChecklistID).ToList());
        }


#region Method
        /// <summary>
        /// Return a CSV file in a string.
        /// </summary>
        /// <param name="collection">IList.</param>
        /// <returns>string.</returns>
        public static string CollectionToCsv<T>(IList<T> collection)
        {

            if (collection == null) throw new ArgumentNullException("collection", "Value can not be null or nothing!");

            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < collection.Count; index++)
            {
                object item = collection[index];

                if (index == 0)
                {
                    sb.Append(ObjectToCsvHeader(item));
                }
                sb.Append(ObjectToCsvData(item));
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a comma delimeted string of all the objects property values names.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <returns>string.</returns>
        public static string ObjectToCsvData(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", "Value can not be null or Nothing!");
            }

            StringBuilder sb = new StringBuilder();
            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties();

            for (int index = 0; index < pi.Length; index++)
            {
                sb.Append(pi[index].GetValue(obj, null));

                if (index < pi.Length - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a comma delimeted string of all the objects property names.
        /// </summary>
        /// <param name="obj">object.</param>
        /// <returns>string.</returns>
        public static string ObjectToCsvHeader(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", "Value can not be null or Nothing!");
            }

            StringBuilder sb = new StringBuilder();
            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties();

            for (int index = 0; index < pi.Length; index++)
            {
                sb.Append(pi[index].Name);

                if (index < pi.Length - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

#endregion

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
