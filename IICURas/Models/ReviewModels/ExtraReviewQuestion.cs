using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class ExtraReviewQuestion
    {
        public ExtraReviewQuestion()
        {
            Status = "Current";
            CreatedOn = DateTime.Now;
        }

        [Key]
        public int ExtraReviewQuestionID { get; set; }

        [ForeignKey("Records")]
        public int RecordID { get; set; }

        [ForeignKey("UserProfile")]
        public int UserID { get; set; }

        [ForeignKey("Option")]
        public int OptionID { get; set; }

        [ForeignKey("ReviewCompletion")]
        public int ReviewCompletionID { get; set; }

        public string Status { get; set; }

        private DateTime CreatedOn { get; set; }

        private DateTime? lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public virtual Record Records { get; set; }

        public virtual Option Option { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual ReviewCompletion ReviewCompletion { get; set; }
    }
}