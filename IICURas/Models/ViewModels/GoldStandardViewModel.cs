using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class GoldStandardViewModel
    {
        public int PublicationPublicationID { get; set; }

        [DisplayName("Training Publication Number")]
        public int PubNumber { get; set; }

        public ICollection<ReadViewModel> Files { get; set; }

        public int NFiles
        {
            get
            {
                return Files.Count();

            }
        }

        public IEnumerable<GoldStandardItemViewModel> GoldStandardItems { get; set; }
    }
}