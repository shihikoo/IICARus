using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class ReviewInputModel
    {
        public int ReviewCompleteID { get; set; }

        [DisplayName("Publication ID")]
        public int RecordID { get; set; }

        public ICollection<ReadViewModel> Files { get; set; }

        public int NFiles => Files?.Count() ?? 0;

        public ICollection<ReviewItemInputModel> ReviewItems { get; set; }
        
        public int NReviewCompleted
        {
            get
            {
                return ReviewItems.Count(tr => tr.OptionID != 0);

            }
        }

        public int[] SpeciesIDs { get; set; }

        public List<Specie> Species { get; set; }

        [DisplayName("Conflict of Interest")]
        public string COI { get; set; }

        [DisplayName("Funding Resource")]
        public string Funding { get; set; }

        public IEnumerable<SelectListItem> SpecieList
        {
            get
            {
                return Species.Select(o => new SelectListItem
                {
                    Value = o.SpecieID.ToString(),
                    Text = o.SpecieName,

                });
            }
        }
    }
}