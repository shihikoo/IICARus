using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class Specie
    {
        public Specie()
        {
            LinkRecordUserSpecies = new HashSet<LinkRecordUserSpecie>();
        }

        [Key]
        public int SpecieID { get; set; }

        public string SpecieName { get; set; }

        //virtual

        public virtual ICollection<LinkRecordUserSpecie> LinkRecordUserSpecies { get; set; }
    }
}