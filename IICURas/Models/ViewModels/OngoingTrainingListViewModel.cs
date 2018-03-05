using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class OngoingTrainingListViewModel
    {
        public int UserID { get; set; }

        public int PubID { get;set;}

        public int ProjectId { get; set; }

        [DisplayName("Training Publication Number")]
        public int PubNumber { get; set; }

        public int NumberOfTotalItems { get; set; }

        public int NumberOfCompletedItems { get; set; }

        public DateTime LastUpdatedTime { get; set; }

        public int TraningReviewID { get; set; }


    }

}