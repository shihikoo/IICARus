using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class ChecklistOptionLink
    {
        public ChecklistOptionLink()
        {
            Status = "Current";
        }

        [Key]
        public int ChecklistOptionLinkID { get; set; }

        [ForeignKey("CheckLists")]
        public int CheckListID { get; set; }

        [ForeignKey("Option")]
        public int OptionID { get; set; }

        public string Status { get; set; }

        public virtual CheckList CheckLists { get; set; }

        public virtual Option Option { get; set; }

    }
}