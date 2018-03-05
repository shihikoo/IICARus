namespace IICURas.Models
{

    using System.Collections.Generic;

    public  class CheckList
    {
        public CheckList()
        {
            PaperQualities = new HashSet<PaperQuality>();
            ChecklistOptionLinks = new HashSet<ChecklistOptionLink>();
            TrainingReviewItems = new HashSet<TrainingReviewItem>();
        }

        public int CheckListID { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }    

        public string Item { get; set; }

        public string CheckListNumber { get; set; }

        public string CheckListName { get; set; }

        public bool Critical { get; set; }

        public string Criteria { get; set; }

        public virtual ICollection<PaperQuality> PaperQualities { get; set; }

        public virtual ICollection<TrainingReviewItem> TrainingReviewItems { get; set; }

        public virtual ICollection<ChecklistOptionLink> ChecklistOptionLinks { get; set; }

        public virtual ICollection<GoldStandard> GoldStandards { get; set; }
    }
}
