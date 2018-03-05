using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class ReReviewItemInputModel
    {
        public ReReviewItemInputModel(PaperQuality Review, PaperQuality r1, PaperQuality r2, PaperQuality r3)
        {
            ReviewItemID = Review.PaperQualityID;
            CheckListID = Review.CheckListCheckListID;
            Section = Review.CheckList.Section;
            ItemNumber = Review.CheckList.ItemNumber;
            CheckListNumber = Review.CheckList.CheckListNumber;
            CheckListName = Review.CheckList.CheckListName;
            Criteria = Review.CheckList.Criteria;
            Critical = Review.CheckList.Critical;

            Options = Review.CheckList.ChecklistOptionLinks.Where(li => li.Status == Enums.Status.Current.ToString()).Select(l => l.Option);

            OptionID1 = r1.OptionOptionID;
            Result1 = r1.OptionOptionID== null? "" : r1.Option.OptionName;
            Comment1 = r1.Comments;

            OptionID2 = r2.OptionOptionID;
            Result2 = r2.OptionOptionID == null ? "" : r2.Option.OptionName;
            Comment2 = r2.Comments;

            OptionID = r3.OptionOptionID;
            Comments = r3.Comments;
            
        }

        public ReReviewItemInputModel(PaperQuality Review, PaperQuality r1, PaperQuality r2)
        {
            ReviewItemID = Review.PaperQualityID;
            CheckListID = Review.CheckListCheckListID;
            Section = Review.CheckList.Section;
            ItemNumber = Review.CheckList.ItemNumber;
            CheckListNumber = Review.CheckList.CheckListNumber;
            CheckListName = Review.CheckList.CheckListName;
            Criteria = Review.CheckList.Criteria;
            Critical = Review.CheckList.Critical;
            Comments = Review.Comments;
            Options = Review.CheckList.ChecklistOptionLinks.Select(l => l.Option);

            OptionID1 =  r1.OptionOptionID;
            Result1 = r1.OptionOptionID== null? "" : r1.Option.OptionName;
            Comment1 = r1.Comments;

            OptionID2 = r2.OptionOptionID;
            Result2 = r2.OptionOptionID == null ? "" : r2.Option.OptionName;
            Comment2 = r2.Comments;

            OptionID = OptionID1 == OptionID2 ? OptionID1 : null;
        }

        public ReReviewItemInputModel() { }
        //public int ReviewID { get; set; }

        public int ReviewItemID {get;set;}

        public int PublicationID { get; set; }

        public int CheckListID { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string CheckListNumber { get; set; }

        public string CheckListName { get; set; }

        [DataType(DataType.MultilineText)]
        public string Criteria { get; set; }

        public bool Critical { get; set; }

        public int? OptionID { get; set; }
        public int? OptionID1 { get; set; }
        public int? OptionID2 { get; set; }
        public string Result1 { get; set; }

        public string Result2 { get; set; }

        public string Comment1 { get; set; }
        public string Comment2 { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public IEnumerable<Option> Options { get; set; }

        public IEnumerable<SelectListItem> OptionList
        {
            get
            {
                return Options.Select(o => new SelectListItem
                {
                    Value = o.OptionID.ToString(),
                    Text = o.OptionName,
                
                }) ; }
        }
    }
}
