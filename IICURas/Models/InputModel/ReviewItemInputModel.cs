using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class ReviewItemInputModel
    {
        public ReviewItemInputModel(PaperQuality Review)
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
        }

        public ReviewItemInputModel() { }
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
                    Text = o.OptionName                
                });
            }
        }
    }
}
