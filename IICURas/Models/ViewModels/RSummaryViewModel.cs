using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class RSummaryViewModel
    {
        public string CategoryName { get; set; }

        [DisplayName("Country ")]
        public string CountryName { get; set; }

        [DisplayName("Number of Papers")]
        public int CountOfPaper { get; set; }
    }
}