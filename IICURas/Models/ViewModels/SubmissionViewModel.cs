using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class SubmissionViewModel
    {
        public bool Pass { get; set; }

        public int Points { get; set; }

        public int TotalPoints { get; set; }

        public int TraningReviewID { get; set; }

        public int PromotionID { get; set; }

        public bool Promoted { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public float Percentage
        {
            get
            {

                return (float)Points / (float)TotalPoints;

            }
        }
    }
}