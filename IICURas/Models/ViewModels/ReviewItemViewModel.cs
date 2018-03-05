using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class ReviewItemViewModel
    {
        public ReviewItemViewModel(PaperQuality Review)
        {
            ReviewID = Review.PaperQualityID;
            CheckListID = Review.CheckListCheckListID;
            Section = Review.CheckList.Section;
            ItemNumber = Review.CheckList.ItemNumber;
            CheckListNumber = Review.CheckList.CheckListNumber;
            CheckListName = Review.CheckList.CheckListName;
            Criteria = Review.CheckList.Criteria;
            Critical = Review.CheckList.Critical;
            Comments = Review.Comments;
            Option = Review.Option.OptionName;
        }

        public int ReviewID { get; set; }

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

        public string Option { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
    }
}
