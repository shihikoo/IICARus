using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IICURas.Models.ViewModel
{
    public class ReconReviewViewModel
    {
        public ReconReviewViewModel(ReviewCompletion reviewCompletion) {
            ReviewCompleteID = reviewCompletion.ReviewCompletionID;
            RecordID = reviewCompletion.RecordRecordID;
            //Files = reviewCompletion.Record.PaperDocuments.Where(p => p.DeletePaperDocument == false).Select(pd => new ReadViewModel(pd)); ;
            ReviewItems = reviewCompletion.PaperQualities.Where(pq => pq.Status == Enums.Status.Current.ToString()).Select(pq => new ReviewItemViewModel(pq));
            Species = reviewCompletion.LinkRecordUserSpecies.Select(l => l.Species.SpecieName).ToArray();
        }

        public int ReviewCompleteID { get; set; }

        [DisplayName("Publication ID")]
        public int RecordID { get; set; }

        //public IEnumerable<ReadViewModel> Files { get; set; }

        //public int NFiles => Files?.Count() ?? 0;

        public IEnumerable<ReviewItemViewModel> ReviewItems { get; set; }
        
        //public int NReviewCompleted
        //{
        //    get
        //    {
        //        return ReviewItems.Count(tr => tr.OptionID != 0);

        //    }
        //}

        public string[] Species { get; set; }

        //public List<Specie> Species { get; set; }

        //[DisplayName("Conflict of Interest")]
        //public string COI { get; set; }

        //[DisplayName("Funding Resource")]
        //public string Funding { get; set; }

        //public IEnumerable<SelectListItem> SpecieList
        //{
        //    get
        //    {
        //        return Species.Select(o => new SelectListItem
        //        {
        //            Value = o.SpecieID.ToString(),
        //            Text = o.SpecieName,

        //        });
        //    }
        //}
    }
}