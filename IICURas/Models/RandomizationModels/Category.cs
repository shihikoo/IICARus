namespace IICURas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Category
    {
        public Category()
        {
            Records = new HashSet<Record>();
        }

        public int CategoryID { get; set; }

        [Required]
        [DisplayName("Category")]
        public string CategoryName { get; set; }

        public virtual ICollection<Record> Records { get; set; }
    }
}
