using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class GoldStandardItemInputModel
    {

        public int GoldStandardID { get; set; }

        public int PublicationPublicationID { get; set; }

        public int CheckListCheckListID { get; set; }

        [DisplayName("Option")]
        public int OptionID { get; set; }

        [DisplayName("Option")]
        public string OptionValue { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }

        public List<Option> Options { get; set; }

        public IEnumerable<SelectListItem> OptionList
        {
            get
            {
                return Options.Select(o => new SelectListItem
                {
                    Value = o.OptionID.ToString(),
                    Text = o.OptionName,

                });
            }
        }
        
    }
}