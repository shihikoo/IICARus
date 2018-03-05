using System.Linq;
using IICURas.Models;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace IICURas.Models
{
    public class ResultViewModel
    {
        public ReviewResultViewModel ReviewResultVM { get; set; }

        public SpeciesResultViewModel SpeciesResultVM { get; set; }
    }

    public class ReviewResultViewModel
    {
        public ReviewResultViewModel(IList<PaperQuality> PaperQualities)
        {
            ReviewResultVMs = PaperQualities.Select(pq => new ReviewForResultViewModel(pq)).ToList();
        }

        public ReviewResultViewModel()
        {
        }

        public IList<ReviewForResultViewModel>  ReviewResultVMs { get; set; }
    }

    public class ReviewForResultViewModel
    {
        public ReviewForResultViewModel(PaperQuality PaperQualities)
        {
            PaperQualityID = PaperQualities.PaperQualityID;
            RecordID = PaperQualities.RecordRecordID;
            PaperNumber = PaperQualities.Record.PaperNumber;
            CheckListCheckListID = PaperQualities.CheckListCheckListID;
            ReviewCompletionID = PaperQualities.ReviewCompletionReviewCompletionID;
            OptionName = PaperQualities.Option.OptionName;
            Category = PaperQualities.Record.Category.CategoryName;
            Country = PaperQualities.Record.Country.CountryName;
            AuthorCompliance = PaperQualities.Record.AuthorCompliance;
            HadArriveChecklist = PaperQualities.Record.hadarrivechecklist;
            Comments = Regex.Replace(PaperQualities.Comments == null ? "" : Regex.Replace(PaperQualities.Comments, @"\t|\n|\r", ""), @"[,]", " ;");
            Checklistname =  Regex.Replace(PaperQualities.CheckList.CheckListName == null ? "" : Regex.Replace(PaperQualities.CheckList.CheckListName, @"\t|\n|\r", ""), @"[,]", "; ");

            //Random rand = new Random();
            //Category = rand.Next(1, 3) == 1 ? "Intervention" : "Control";

        }

        public int PaperQualityID { get; set; }

        public int RecordID { get; set; }

        public string PaperNumber { get; set; }

        public int CheckListCheckListID { get; set; }

        public int ReviewCompletionID { get; set; }

        //public string Section { get; set; }

        //public string ItemNumber { get; set; }

        //public string Item { get; set; }

        //public string CheckListNumber { get; set; }

        public string Checklistname { get; set; }

        //public string Status { get; set; }

        //public string Criteria { get; set; }

        //public int OptionID { get; set; }

        public string OptionName { get; set; }
        public string Country { get; set; }
        public string Category { get; set; }

        public string AuthorCompliance { get; set; }

        public string HadArriveChecklist { get; set; }

        public string Comments { get; set; }

    }

    public class SpeciesResultViewModel
    {
        public SpeciesResultViewModel(IList<LinkRecordUserSpecie> LinkRecordUserSpecies)
        {
            SpeciesVMs = LinkRecordUserSpecies.Select(li => new SpeciesViewModel(li)).ToList();
        }

        public SpeciesResultViewModel()
        {
        }

        public IList<SpeciesViewModel> SpeciesVMs { get; set; }
    }
    public class SpeciesViewModel
    {
        public SpeciesViewModel(LinkRecordUserSpecie LinkRecordUserSpecie) : this() {
            PublicatinID = LinkRecordUserSpecie.RecordID;
            SpeciesID = LinkRecordUserSpecie.SpecieID;
            SpeciesName = LinkRecordUserSpecie.Species.SpecieName;
        }

        public SpeciesViewModel()
        {
        }

        public int PublicatinID { get; set; }

        public int SpeciesID { get; set; }

        public string SpeciesName { get; set; }
    }
}