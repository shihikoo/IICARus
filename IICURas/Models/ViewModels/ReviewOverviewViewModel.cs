using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class ReviewOverviewViewModel
    {
        public bool isSenior { get; set; }

        public int CompleteNumber { get; set; }

        public int OngoingNumber { get; set; }

        public int TotalNumber => OngoingNumber + CompleteNumber + TodoNumber;

        public IEnumerable<OngoingReviewViewModel> OngoingReview { get; set; }

        public int TodoNumber { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public float CompletePercentage => (float)CompleteNumber / TotalNumber;

        public string CompletePercentageStyle => "width: " + CompletePercentage.ToString() + "%;";

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public float OngoingPercentage => (float)OngoingNumber / TotalNumber;

        public string OngoingPercentageStyle => "width: " + OngoingPercentage.ToString() + "%;";

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public float TodoPercentage => (float)TodoNumber / TotalNumber;

        public string TodoPercentageStyle => "width: " + TodoPercentage.ToString() + "%;";

        public int OngingPublicatinLimit
            =>
                isSenior
                    ? (int) IICURas.Enums.OngoingPublicationLimit.InternalReviewer
                    : (int) (int) IICURas.Enums.OngoingPublicationLimit.ExternalReviewer;

        public string NewpublictionAvailability => (TodoNumber > 0 && OngoingNumber < OngingPublicatinLimit) ? "" : "disabled";
        //((TodoNumber > 0 && OngoingNumber < (int)IICURas.Enums.OngoingPublicationLimit.Number) || isSenior) ? "" : "disabled";
    }
}