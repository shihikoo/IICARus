using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IICURas.Models.ViewModels
{
    public class ChecklistResultViewModel
    {
        public ChecklistResultViewModel(IEnumerable<PaperQuality> paperQualities) : this(
            paperQualities.FirstOrDefault().CheckList.CheckListID,
            paperQualities.FirstOrDefault().CheckList.Section,
            paperQualities.FirstOrDefault().CheckList.ItemNumber,
            paperQualities.FirstOrDefault().CheckList.Item,
            paperQualities.FirstOrDefault().CheckList.CheckListNumber,
            paperQualities.FirstOrDefault().CheckList.CheckListName,
            paperQualities.FirstOrDefault().CheckList.Critical,
            paperQualities.FirstOrDefault().CheckList.Criteria,
            paperQualities.GroupBy(pq => pq.OptionOptionID, pq => pq, (key, g) => new OptionResultViewModel(g, paperQualities.Count())).OrderByDescending(gg => gg.OptionName)
            ){ }

        public ChecklistResultViewModel(int checklistID, string section, string itemNumber, string item, string checklistNumber, string checklistName, 
            bool critical, string criteria, IEnumerable<OptionResultViewModel> optionResultVMs) : this()
        {
            ChecklistID = ChecklistID;
            Section = section;
            ItemNumber = itemNumber;
            Item = item;
            CheckListNumber = checklistNumber;
            CheckListName = checklistName;
            Critical = critical;
            Criteria = criteria;
            OptionResultVMs = optionResultVMs.ToList();
        }

        public ChecklistResultViewModel() {
        }

        public int ChecklistID { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string CheckListNumber { get; set; }

        public string CheckListName { get; set; }

        public bool Critical { get; set; }

        public string Criteria { get; set; }

        public int TotalSelectionsNum => OptionResultVMs.Sum(o => o.NumOfSelectionsOnOption);

        public List<OptionResultViewModel> OptionResultVMs { get; set; }
    }

    public class OptionResultViewModel {

        public OptionResultViewModel(IEnumerable<PaperQuality> reviews, int TotalSelectionsNum) : this(reviews.FirstOrDefault().Option.OptionName, TotalSelectionsNum, reviews.Count())
        {
           
        }

        public OptionResultViewModel(string optionName, int totalSelectionsNum, int numOfSelectionsOnOption) : this()
        {
            OptionName = optionName;
            TotalSelectionsNum = totalSelectionsNum;
            NumOfSelectionsOnOption = numOfSelectionsOnOption;
        }

        public OptionResultViewModel()
        {
        }

        public int TotalSelectionsNum { get; set; }

        public string OptionName { get; set; }

        public int NumOfSelectionsOnOption { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public double Rate => (float) NumOfSelectionsOnOption / (float)TotalSelectionsNum;
    }


}
