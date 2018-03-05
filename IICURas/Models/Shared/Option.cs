using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class Option
    {
         public Option()
        {
            Reviews = new HashSet<PaperQuality>();
            ChecklistOptionLinks = new HashSet<ChecklistOptionLink>();
            TrainingReviewItems = new HashSet<TrainingReviewItem>();
            GoldStandards = new HashSet<GoldStandard>();
        }

        public int OptionID { get; set; }
        public string OptionName { get; set; }

        public virtual ICollection<PaperQuality> Reviews { get; set; }
        public virtual ICollection<TrainingReviewItem> TrainingReviewItems { get; set; }
        public virtual ICollection<ChecklistOptionLink> ChecklistOptionLinks { get; set; }
        public virtual ICollection<GoldStandard> GoldStandards { get; set; }
    }
}