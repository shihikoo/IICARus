using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IICURas.Models.ViewModels
{
    public class ReconOverviewViewModel
    {
        public ReconOverviewViewModel( IEnumerable<ReviewCompletion> userReviewCompletions, int todoNumber)
        {
            CompleteNumber = userReviewCompletions.Count(rc => rc.ComplesionDate != null);
            OngoingNumber = userReviewCompletions.Count(rc => rc.ComplesionDate == null);
            OngoingReview = userReviewCompletions.Where(rc => rc.ComplesionDate == null).Select(rc => new ReconOngoingReviewViewModel(rc));
            TodoNumber = todoNumber;
        }

        public int CompleteNumber { get; set; }

        public int OngoingNumber { get; set; }

        public int TotalNumber => OngoingNumber + CompleteNumber + TodoNumber;

        public IEnumerable<ReconOngoingReviewViewModel> OngoingReview { get; set; }

        public int TodoNumber { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public float CompletePercentage => (float)CompleteNumber / TotalNumber;

        public string CompletePercentageStyle => "width: " + CompletePercentage.ToString() + "%;";

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public float OngoingPercentage => (float)TodoNumber / TotalNumber;

        public string OngoingPercentageStyle => "width: " + OngoingPercentage.ToString() + "%;";

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public float TodoPercentage => (float)TodoNumber / TotalNumber;

        public string TodoPercentageStyle => "width: " + TodoPercentage.ToString() + "%;";

        public int OngingPublicatinLimitRecon => (int) Enums.OngoingPublicationLimit.Reconciler;

        public string NewpublictionAvailability => (TodoNumber > 0 && OngoingNumber < OngingPublicatinLimitRecon) ? "" : "disabled";
    }

    public class ReconOngoingReviewViewModel
    {
        public ReconOngoingReviewViewModel(ReviewCompletion ReviewCompletion) {
            ReviewCompletionID = ReviewCompletion.ReviewCompletionID;
            PublicationID = ReviewCompletion.RecordRecordID;
            LastUpdatedTime = ReviewCompletion.LastUpdateTime;
            CompletedNumber = ReviewCompletion.PaperQualities.Count(pq => pq.Status == Enums.Status.Current.ToString() && pq.OptionOptionID != null);
            TotalNumber = ReviewCompletion.PaperQualities.Count(pq => pq.Status == Enums.Status.Current.ToString());
            SpeciesNumber = ReviewCompletion.LinkRecordUserSpecies.Count(lrus => lrus.Status == "Current");

        }

        [ForeignKey("Manuscript No.")]
        public int ReviewCompletionID { get; set; }

        [ForeignKey("Manuscript No.")]
        public int PublicationID { get; set; }

        [ForeignKey("Last Updated Time")]
        public DateTime LastUpdatedTime { get; set; }

        public int CompletedNumber { get; set; }

        public int TotalNumber { get; set; }

        public int SpeciesNumber { get; set; }
 
        //private int OngingPublicatinLimitRecon => (int)Enums.OngoingPublicationLimit.Reconciler;
    }
}