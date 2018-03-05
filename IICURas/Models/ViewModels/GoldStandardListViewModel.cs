using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class GoldStandardListViewModel
    {
        public int TrainingPublicationID { get; set; }
        
        [DisplayName("Training Publication Number")]
        public int TrainingPublicationNumber { get; set; }

        public DateTime LastUpdatedTime { get; set; }
    }
}