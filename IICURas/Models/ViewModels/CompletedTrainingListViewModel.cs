using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class CompletedTrainingListViewModel
    {
        public int UserID { get; set; }

        public int PubID { get; set; }

        [DisplayName("Publication Number")]
        public int PubNumber { get; set; }

        [DisplayName("Percentage")]
        public int? Points { get; set; }

        [DisplayName("Completion Time")]
        public DateTime CompletionTime { get; set; }

        public bool? Pass { get; set; }

        public int TotalPoints { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public float Percentage => (float)Points /TotalPoints;

        public int TraningReviewID { get; set; }

        public string Status { get; set; }

        public DateTime StartTime { get; set; }

        [DisplayName("Time Used")]
        [DisplayFormat(DataFormatString = "{0:dd\\.hh\\:mm\\:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan TimeUsed => (CompletionTime - StartTime);
    }
}