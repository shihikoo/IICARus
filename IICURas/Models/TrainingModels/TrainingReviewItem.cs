using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class TrainingReviewItem
    {
        public TrainingReviewItem()
        {
            Status = "Current";
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int TrainingReviewItemID { get; set; }

        [ForeignKey("TrainingPublication")]
        public int TrainingPublicationID { get; set; }

        [ForeignKey("CheckList")]
        public int CheckListCheckListID { get; set; }

        [ForeignKey("Option")]
        public int? OptionOptionID { get; set; }

        public string Comments { get; set; }

        [ForeignKey("UserProfile")]
        public int ReviewerId { get; set; }

        [ForeignKey("TrainingReview")]
        public int TrainingReviewID { get; set; }

        public string Status { get; set; }

        public DateTime CreatedOn { get; private set; }

        private DateTime? lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public virtual Option Option { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual CheckList CheckList { get; set; }

        public virtual TrainingPublication TrainingPublication { get; set; }

        public virtual TrainingReview TrainingReview { get; set; }


    }
}