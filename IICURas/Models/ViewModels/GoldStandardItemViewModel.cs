using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class GoldStandardItemViewModel
    {
        public int CheckListCheckListID { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string CheckListNumber { get; set; }

        public string CheckListName { get; set; }

        public string Criteria { get; set; }

        public bool Critical { get; set; }

        public int GoldStandardID { get; set; }
        
        [DisplayName("Gold Standard")]
        public string GSOptionValue { get; set; }
        [DisplayName("Explanation")]
        public string GSComments { get; set; }

        public IEnumerable<GoldStandardItemByReviewerViewModel> GoldStandardItemsByReviewer { get; set; }

        public int NReviewCompleted => GoldStandardItemsByReviewer?.Count() ?? 0;
    }
}