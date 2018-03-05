using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace IICURas.Models
{
    public class TranscriptViewModel
    {
        public int PubID { get; set; }

        [DisplayName("Training Publication Number")]
        public int PubNumber { get; set; }

        public ICollection<TranscriptItemViewModel> Items { get; set; }

        public int? Points { get; set; }

        public DateTime CompletionTime { get; set; }

        public DateTime CreateTime { get; set; }

        public bool? Pass { get; set; }

        public int TotalPoints { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public float Percentage => (float)Points / (float)TotalPoints;

        public int TraningReviewID { get; set; }

        public bool passCheatCriteria => !(Percentage*100.0 <= (float)IICURas.Enums.CheatingCriteria.percentage) || !((CompletionTime - CreateTime).Seconds < (float)Enums.CheatingCriteria.time);

    }
}
