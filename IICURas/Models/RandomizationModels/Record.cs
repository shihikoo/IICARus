using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace IICURas.Models
{
    public class Record
    {
        public Record()
        {
            PaperDocuments = new HashSet<PaperDocument>();
            PaperQualities = new HashSet<PaperQuality>();
            LinkRecordUserSpecies = new HashSet<LinkRecordUserSpecie>();
            ReviewCompletions = new HashSet<ReviewCompletion>();
        }

        public int RecordID { get; set; }

        [DisplayName("Manuscript Number")]
        public string PaperNumber { get; set; }

        public int CountryCountryID { get; set; }

        public int CategoryCategoryID { get; set; }

        public string EntryUser { get; set; }

        [DisplayName("ARRIVE checklist already included")]
        public string hadarrivechecklist { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public Boolean DeleteRecord { get; set; }

        public void SetDefautvalue()
        {
            DeleteRecord = false;
        }

        [DataType(DataType.DateTime)]
        public DateTime RandomizationTimeUTC { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime RandomizationTime { get; set; }

        // MS status by randomizer
        public string AcceptanceStatus_randomizer { get; set; }

        public string StatusEntryUser_randomizer { get; set; }

        [DataType(DataType.MultilineText)]
        public string StatusComments_randomizer { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StatusUpdateTimeUTC_randomizer { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StatusUpdateTime_randomizer { get; set; }

        public string AuthorCompliance { get; set; }

        // Paper status by uploader
        public string AcceptanceStatus { get; set; }

        public string StatusEntryUser { get; set; }

        public string Doi { get; set; }

        [DataType(DataType.MultilineText)]
        public string StatusComments { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StatusUpdateTimeUTC { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StatusUpdateTime{ get; set; }

        public string MNAcceptance { get; set; }

        public string ConflicOfIntest { get; set; }

        public string Funding  { get; set; }

        // virtual
        public virtual Category Category { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<PaperDocument> PaperDocuments { get; set; }

        public virtual ICollection<PaperQuality> PaperQualities { get; set; }

        public virtual ICollection<LinkRecordUserSpecie> LinkRecordUserSpecies { get; set; }

        public virtual ICollection<ReviewCompletion> ReviewCompletions { get; set; }
    }
}
