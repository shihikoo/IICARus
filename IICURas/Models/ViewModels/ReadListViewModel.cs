using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class ReadListViewModel
    {
        [DisplayName("Manuscript ID")]
        public int RecordID { get; set; }

        [DisplayName("Manuscript Number")]
        public string PaperNumber { get; set; }

        public ICollection<ReadViewModel> Files { get; set; }

        public int NFiles => Files.Count();
    }
}