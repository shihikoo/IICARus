using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace IICURas.Models
{
    public class TrainingReview
    {
        public TrainingReview()
        {
            Status = "Current";
            CreatedOn = DateTime.Now;

            TrainingReviewItems = new HashSet<TrainingReviewItem>();

        }

        public int TrainingReviewID { get; set; }

        [ForeignKey("UserProfile")]
        public int UserID { get; set; }

        [ForeignKey("TrainingPublication")]
        public int TrainingPublicationID { get; set; }

        public int? Points { get; set; }

        public int TotalPoints { get; set; }

        public bool? Pass { get; set; }

        public string Status { get; set; }

        public DateTime CreatedOn { get; set; }

        private DateTime? lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        // virtual
        public virtual UserProfile UserProfile { get; set; }

        public virtual TrainingPublication TrainingPublication { get; set; }

        public virtual ICollection<TrainingReviewItem> TrainingReviewItems { get; set; }


    }
}
