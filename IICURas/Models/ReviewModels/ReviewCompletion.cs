using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace IICURas.Models
{
    public class ReviewCompletion
    {

        public ReviewCompletion()
        {
            Status = "Current";
            CreatedOn = DateTime.Now;

            PaperQualities = new HashSet<PaperQuality>();
            LinkRecordUserSpecies = new HashSet<LinkRecordUserSpecie>();


        }

        public int ReviewCompletionID { get; set; }

        public DateTime? ComplesionDate { get; set; }

        public IICURas.Enums.userRoles Role { get; set; }

        [ForeignKey("UserProfile")]
        public int UserID { get; set; }

        [ForeignKey("Record")]  
        public int RecordRecordID { get; set; }   // this is actually recordid

        public string Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool Reconciliation { get; set; }

        private DateTime? lastUpdateTime;
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        //virtual
        public virtual UserProfile UserProfile { get; set; }

        public virtual Record Record { get; set; }

        public virtual ICollection<LinkRecordUserSpecie> LinkRecordUserSpecies { get; set; }

        public virtual ICollection<PaperQuality> PaperQualities { get; set; }


        public void Archive()
        {
            Status = Enums.Status.Archived.ToString();
            LastUpdateTime = DateTime.Now;

            foreach (var pq in PaperQualities) pq.Archive();
            
        }

        public void Unarchive()
        {
            Status = Enums.Status.Current.ToString();
            LastUpdateTime = DateTime.Now;

            foreach (var pq in PaperQualities) pq.Unarchive();
        }
    }
}
