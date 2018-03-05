using System;
//using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using IICURas.Models;


namespace IICURas.Models
{
    public class ReviewListViewModel
    {
        [DisplayName("MS ID")]
        public int RecordID { get; set; }

        public int ReviewComplesionID { get; set; }

        [DisplayName("Agreement Rate")]
        public string AgreementRate { get; set; }

        [DisplayName("Complete Time")]
        public DateTime? CompleteTime { get; set; }

        [DisplayName("Creation Time")]
        public DateTime CreateTime { get; set; }

        [DisplayName("Last Updated Time")]
        public DateTime LastupDateTime { get; set; }

        [DisplayName("# of Completed Items")]
        public int nCompletedItems { get; set; }

        [DisplayName("# of Species")]
        public int nSpecies { get; set; }

        [DisplayName("Reviews for this MS")]
        public int nReviewCompleted { get; set; }


        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Time Used")]
        [DisplayFormat(DataFormatString = "{0:dd\\.hh\\:mm\\:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan ReviewTimespan => (LastupDateTime - CreateTime);

    }
}
