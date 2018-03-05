using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class GoldStandard
    {
        public GoldStandard()
        {

        }

        [Key]
        public int GoldStandardID { get; set; }

        [ForeignKey("TrainingPublication")]
        public int PublicationPublicationID { get; set; }

        [ForeignKey("CheckList")]
        public int CheckListCheckListID { get; set; }

        [ForeignKey("Option")]
        public int OptionOptionID { get; set; }

        public string Comments { get; set; }

        // virtual
        public virtual Option Option { get; set; }

        public virtual CheckList CheckList { get; set; }

        public virtual TrainingPublication TrainingPublication { get; set; }
    }
}