using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class ReviewViewModel
    {
        public int PaperQualityID { get; set; }

        public int RecordRecordID { get; set; }

        [DisplayName("Manuscript Number")]
        public string PaperNumber { get; set; }

        public int CheckListCheckListID { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string CheckListNumber { get; set; }

        public string CheckListName { get; set; }

        public string Status { get; set; }

        [DataType(DataType.MultilineText)]
        public string Criteria { get; set; }

        public int? OptionID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public DateTime LastUpdateTime { get; set; }


    }
}