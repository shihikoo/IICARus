namespace IICURas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PaperQuality
    {



        public PaperQuality()
        {
            Status = "Current";
            CreatedOn = DateTime.Now;            
        }

        [Key]
        public int PaperQualityID { get; set; }

        [ForeignKey("Record")]
        public int RecordRecordID { get; set; }

        [ForeignKey("CheckList")]
        public int CheckListCheckListID { get; set; }

        [ForeignKey("Option")]
        public int? OptionOptionID { get; set; }

        [ForeignKey("UserProfile")]
        public int ReviewerId { get; set; }

        [ForeignKey("ReviewCompletion")]
        public int ReviewCompletionReviewCompletionID { get; set; }

        public string Comments { get; set; }

        public string Status { get; set; }

        public DateTime CreatedOn { get; set; }

        private DateTime? lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public virtual Option Option { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual CheckList CheckList { get; set; }

        public virtual Record Record { get; set; }

        public virtual ReviewCompletion ReviewCompletion { get; set; }

        

        public void Archive()
        {
            Status = Enums.Status.Archived.ToString();
            LastUpdateTime = DateTime.Now;

        }

        public void Unarchive()
        {
            Status = Enums.Status.Current.ToString();
            LastUpdateTime = DateTime.Now;

        }

    }
}
