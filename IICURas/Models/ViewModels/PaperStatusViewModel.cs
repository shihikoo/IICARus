using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class PaperStatusViewModel
    {
        public int RecordID { get; set; }

        [DisplayName("Manuscript Number")]
        public string PaperNumber { get; set; }

        [DisplayName("TC1 Status")]
        public string TC1Status { get; set; }

         [DisplayName("DOI")]
        public string Doi { get; set; }

        [DisplayName("Acceptance Status")]
        public string AcceptanceStatus { get; set; }

        [DisplayName("Updated by")]
        public string StatusEntryUser { get; set; }

        [DisplayName("Update Time")]
        public DateTime StatusUpdateTime { get; set; }

        [DisplayName("Comments")]
        [DataType(DataType.MultilineText)]
        public string StatusComments { get; set; }
    }
}