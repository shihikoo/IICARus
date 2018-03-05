using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class MSStatusViewModel
    {
        public int RecordID { get; set; }

        [DisplayName("MS Number")]
        public string PaperNumber { get; set; }

        [DisplayName("TC1 Status")]
        public string AcceptanceStatus_randomizer { get; set; }

        [Display(Name = "ARRIVE checklist compliance")]
        [Required()]
        public string AuthorCompliance { get; set; }

        [DisplayName("Updated by")]
        public string StatusEntryUser_randomizer { get; set; }

        [DisplayName("Update Time")]
        public DateTime StatusUpdateTime_randomizer { get; set; }

        [DisplayName("Randomisation Time")]
        public DateTime RandomisationTime { get; set; }

        [DisplayName("Comments")]
        [DataType(DataType.MultilineText)]
        public string StatusComments_randomizer { get; set; }
    }
}