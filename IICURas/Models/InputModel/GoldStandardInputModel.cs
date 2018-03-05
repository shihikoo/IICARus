using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class GoldStandardInputModel
    {

        public int PublicationID { get; set; }

        [DisplayName("Training Publication Numbe")]
        public int PubNumber { get; set; }

        public GoldStandardItemInputModel GoldStandardItemIM { get; set; }

        public ChecklistItemInputModel ChecklistItemIM { get; set; }

    }
}