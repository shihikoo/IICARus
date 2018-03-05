using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace IICURas.Models
{
    public class TranscriptItemViewModel
    {
        public int TrainingReviewItemID { get; set; }

        public int TrainingPublicationID { get; set; }

        public int CheckListID { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string CheckListNumber { get; set; }

        public string CheckListName { get; set; }

        public bool Critical { get; set; }
        public string Criteria { get; set; }

        public int? YourOptionID { private get; set; }

         [DisplayName("Your Answer")]
        public string YourAnswer { get; set; }

        [DisplayName("Your Comment")]
        public string YourComments { get; set; }

        public int CorrectOptionID { private get; set; }
        [DisplayName("Correct Answer")]
        public string CorrectAnswer { get; set; }

          [DisplayName("Explanation")]
        public string Comments { get; set; }

        public bool Correct => CorrectOptionID == YourOptionID;
    }
}
