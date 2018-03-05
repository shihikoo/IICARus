using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class ChecklistItemInputModel
    {
        public int GoldStandardID { get; set; }

        public int CheckListID { get; set; }

         [DisplayName("Section")]
        public string Section { get; set; }

         [DisplayName("Item Number")]
        public string ItemNumber { get; set; }

         [DisplayName("Item")]
        public string Item { get; set; }

         [DisplayName("CheckList Number")]
        public string CheckListNumber { get; set; }

         [DisplayName("CheckList Name")]
        public string CheckListName { get; set; }

         [DisplayName("Criteria")]
        public string Criteria { get; set; }

         [DisplayName("Critical")]
        public bool Critical { get; set; }

         //[DisplayName("Current Options")]
         //public IEnumerable<string> CurrentOptions { get; set; }

         // [DisplayName("Current Options")]
         //public IEnumerable<int> CurrentOptionLinks { get; set; }

          [DisplayName("Current Options")]
          public IEnumerable<Option> CurrentOptions { get; set; }

         [DisplayName("Options")]
         public IEnumerable<Option> Options { get; set; }

         public int AddOptionID { get; set; }

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