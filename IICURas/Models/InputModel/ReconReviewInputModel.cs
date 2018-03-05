using IICURas.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class ReconReviewInputModel
    {
        public ReconReviewInputModel(ReviewCompletion review, ReviewCompletion r1, ReviewCompletion r2, List<Specie> species)
        {
            ReviewCompleteID = review.ReviewCompletionID;
            RecordID = review.RecordRecordID;
            Files = review.Record.PaperDocuments.Where(p => p.DeletePaperDocument == false).Select(pd => new ReadViewModel(pd));

            ReviewItems = (from newreview in review.PaperQualities
                          from newreview1 in r1.PaperQualities.Where(r => r.CheckListCheckListID == newreview.CheckListCheckListID)
                          from newreview2 in r2.PaperQualities.Where(r => r.CheckListCheckListID == newreview.CheckListCheckListID)
                          select new ReReviewItemInputModel(newreview, newreview1, newreview2)).ToList()
                          .OrderBy(c => float.Parse(c.ItemNumber, CultureInfo.InvariantCulture.NumberFormat)).ThenBy(c => c.CheckListNumber).ToList();

            SpeciesIDs = (r1.LinkRecordUserSpecies.Where(l => l.Status == Enums.Status.Current.ToString()).Select(l => l.SpecieID).ToArray()
                .Intersect(r1.LinkRecordUserSpecies.Where(l => l.Status == Enums.Status.Current.ToString()).Select(l => l.SpecieID).ToArray())).ToArray();


            Species = species;
            r1Species = string.Join(", ", r1.LinkRecordUserSpecies.Select(s => s.Species.SpecieName));
            r2Species = string.Join(", ", r2.LinkRecordUserSpecies.Select(s => s.Species.SpecieName));
            SpeciesIDs1 = r1.LinkRecordUserSpecies.Select(l => l.SpecieID).ToArray();
            SpeciesIDs2 = r2.LinkRecordUserSpecies.Select(l => l.SpecieID).ToArray();

            COI = review.Record.ConflicOfIntest;
            Funding = review.Record.Funding;
        }

        public ReconReviewInputModel(ReviewCompletion review, ReviewCompletion r1, ReviewCompletion r2, List<Specie> species, ReviewCompletion r3)
        {
            ReviewCompleteID = review.ReviewCompletionID;
            RecordID = review.RecordRecordID;
            Files = review.Record.PaperDocuments.Where(p => p.DeletePaperDocument == false).Select(pd => new ReadViewModel(pd));

            ReviewItems = ( from newreview in review.PaperQualities
                              from newreview1 in r1.PaperQualities.Where(r => r.CheckListCheckListID == newreview.CheckListCheckListID)
                              from newreview2 in r2.PaperQualities.Where(r => r.CheckListCheckListID == newreview.CheckListCheckListID)
                          from newreview3 in r3.PaperQualities.Where(r => r.CheckListCheckListID == newreview.CheckListCheckListID)
                          select new ReReviewItemInputModel(newreview, newreview1, newreview2, newreview3)).ToList()
                          .OrderBy(c => float.Parse(c.ItemNumber, CultureInfo.InvariantCulture.NumberFormat)).ThenBy(c => c.CheckListNumber).ToList();
            
            SpeciesIDs = r3.LinkRecordUserSpecies.Where(l => l.Status == Enums.Status.Current.ToString()).Select(l => l.SpecieID).ToArray();

            Species = species;

            r1Species = string.Join(", ", r1.LinkRecordUserSpecies.Select(s => s.Species.SpecieName));
            r2Species = string.Join(", ", r2.LinkRecordUserSpecies.Select(s => s.Species.SpecieName));
            SpeciesIDs1 = r1.LinkRecordUserSpecies.Select(l => l.SpecieID).ToArray();
            SpeciesIDs2 = r2.LinkRecordUserSpecies.Select(l => l.SpecieID).ToArray();

            COI = review.Record.ConflicOfIntest;
            Funding = review.Record.Funding;
        }

        public ReconReviewInputModel() { }
        public int ReviewCompleteID { get; set; }

        [DisplayName("Publication ID")]
        public int RecordID { get; set; }

        public IEnumerable<ReadViewModel> Files { get; set; }

        public int NFiles => Files?.Count() ?? 0;

        public IList<ReReviewItemInputModel> ReviewItems { get; set; }
        
        public int NReviewCompleted
        {
            get
           {
                return ReviewItems.Count(tr => tr.OptionID != 0);
            }
        }
        public string r1Species{ get; set; }
        public string r2Species  { get; set; }
        public int[] SpeciesIDs { get; set; }
        public int[] SpeciesIDs1 { get; set; }
        public int[] SpeciesIDs2 { get; set; }
        public List<Specie> Species { get; set; }

        [DisplayName("Conflict of Interest")]
        public string COI { get; set; }

        [DisplayName("Funding Resource")]
        public string Funding { get; set; }

        public IEnumerable<SelectListItem> SpecieList
        {
            get
            {
                return Species.Select(o => new SelectListItem
                {
                    Value = o.SpecieID.ToString(),
                    Text = o.SpecieName,
                });
            }
        }
    }
}