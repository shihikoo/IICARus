namespace IICURas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PaperDocument
    {
        [Key]
        public int PaperDocumentsID { get; set; }

        [ForeignKey("Record")]
        public int? RecordRecordID { get; set; }

        public string UploadUser { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string Comments { get; set; }

        public Boolean DeletePaperDocument { get; set; }

        public string FileUrl { get; set; }

        private DateTime? lastUpdateTimeUTC;
        public DateTime LastUpdateTimeUTC
        {
            get { return lastUpdateTimeUTC ?? DateTime.UtcNow; }
            set { lastUpdateTimeUTC = value; }
        }

        private DateTime? lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public virtual Record Record { get; set; }
        
    }
}
