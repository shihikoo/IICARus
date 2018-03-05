using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class OngoingReviewViewModel
    {
                [ForeignKey("Manuscript No.")]
        public int ReviewCompletionID { get; set; }

                [ForeignKey("Manuscript No.")]
        public int PublicationID { get; set; }


                [ForeignKey("Last Updated Time")]
                public DateTime LastUpdatedTime { get; set; }

                public int CompletedNumber { get; set; }

                public int TotalNumber { get; set; }

                public int SpeciesNumber { get; set; }

        public bool isSenior { get; set; }

        public int OngingPublicatinLimit
           =>
               isSenior
                   ? (int)IICURas.Enums.OngoingPublicationLimit.InternalReviewer
                   : (int)(int)IICURas.Enums.OngoingPublicationLimit.ExternalReviewer;
    }
}