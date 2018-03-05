using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IICURas.Models.ViewModels
{
    public class ChecklistPublicationReviewViewModel
    {
        public int ChecklistID { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public bool Agreement { get; set; }

        public string CheckListNumber { get; set; }

        public string CheckListName { get; set; }

        public bool Critical { get; set; }

        public string Criteria { get; set; }

    }
}
