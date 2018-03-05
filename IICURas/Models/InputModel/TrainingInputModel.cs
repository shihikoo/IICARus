using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class TrainingInputModel
    {
        [DisplayName("Training Publication ID")]
        public int PubID { get; set; }

        public int TrainingReviewID { get; set; }

        [DisplayName("Training Publication Number")]
        public int PubNumber { get; set; }

        //public string COI { get; set; }

        //public string Funding { get; set; }

        public ICollection<ReadViewModel> Files { get; set; }

        public int NFiles => Files.Count();

        public ICollection<TrainingReviewItemInputModel> TrainingReviewItems{get;set;}

        public int NReviewCompleted => TrainingReviewItems.Count(tr => tr.OptionID != 0);

    }
}