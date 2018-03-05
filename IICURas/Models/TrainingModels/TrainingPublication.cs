using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class TrainingPublication
    {

        public TrainingPublication()
        {
            Status = "Current";
            CreatedOn = DateTime.Now;

            TrainingDocuments = new HashSet<TrainingDocument>();
            TrainingReviews = new HashSet<TrainingReview>();
            TrainingReviewItems = new HashSet<TrainingReviewItem>();
        }

        public int TrainingPublicationID { get; set; }

        public int TrainingPublicationNumber { get; set; }

        public int PMID { get; set; }

        public string Doi { get; set; }

        public string Status { get; set; }
        public DateTime CreatedOn { get; private set; }

        private DateTime? lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        // virtual
        public virtual ICollection<TrainingDocument> TrainingDocuments { get; set; }

        public virtual ICollection<TrainingReviewItem> TrainingReviewItems { get; set; }

        public virtual ICollection<TrainingReview> TrainingReviews { get; set; }

        public virtual ICollection<GoldStandard> GoldStandards { get; set; } 
    }
}