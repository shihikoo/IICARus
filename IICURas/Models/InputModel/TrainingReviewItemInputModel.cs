using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class TrainingReviewItemInputModel
    {
        public int TrainingReviewID { get; set; }

        public int TrainingReviewItemID {get;set;}

        public int TrainingPublicationID { get; set; }

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

        public List<Option> Options { private get; set; }

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
