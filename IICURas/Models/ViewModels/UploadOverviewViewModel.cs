using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class UploadOverviewViewModel
    {
        [DisplayName("Manuscript Number")]
        public string PaperNumber { get; set; }
        
        public int RecordRecordID { get; set; }
        
        [DisplayName("Number of Files") ]
        public  int NumberOfDocuments { get; set; }

        public string TC1Status { get; set; }
        public string TC1StatusComments { get; set; }

        public string MSStatus { get; set; }
        public string MSStatusComments { get; set; }

        public string SpreadsheetStatus { get; set; }
        public string SpreadsheetStatusComments { get; set; }
    }
}