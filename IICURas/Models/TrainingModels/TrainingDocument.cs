namespace IICURas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class TrainingDocument
    {
        public TrainingDocument() {
            Status = "Current";
        }

        [Key]
        public int TrainingDocumentID { get; set; }

        [ForeignKey("TrainingPublication")]
        public int TrainingPublicationID { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileUrl { get; set; }

        public string Status { get; set; }

        private DateTime? lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public virtual TrainingPublication TrainingPublication { get; set; }       
    }
}
