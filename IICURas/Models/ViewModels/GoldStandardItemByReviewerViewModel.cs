using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class GoldStandardItemByReviewerViewModel
    {
        public string ReviewerUsername { get; set; }

         [DisplayName("Reviewer Answer")]
        public string ReviewerOptionValue { get; set; }
               [DisplayName("Reviewer Comments")]
        public string ReviewerComments { get; set; }

 
        public string ColorStyle { get; set; }
    }
}