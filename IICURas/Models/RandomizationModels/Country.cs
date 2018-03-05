namespace IICURas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Country
    {
        public Country()
        {
            Records = new HashSet<Record>();
        }

        public int CountryID { get; set; }

        [DisplayName("Country of the Corresponding Author")]
        public string CountryName { get; set; }

        public virtual ICollection<Record> Records { get; set; }
    }
}
