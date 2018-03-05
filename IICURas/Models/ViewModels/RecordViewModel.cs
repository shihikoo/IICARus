using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class RecordViewModel
    {
        public int RecordID { get; set; }

        [Required(ErrorMessage = "Manuscript Number is required.")]
        [StringLength(50)]
        [DisplayName("Manuscript Number")]
        //[RegularExpression(@"([a-zA-Z0-9.&'-]+)", ErrorMessage = "Manuscript should be alphanumeric with no space.")]
        [RegularExpression(@"[\s-]*PONE-D-15-([0-9]{5,5}[\s-]*$)", ErrorMessage = "Please use the format: PONE-D-15-XXXXX where X = number.")]

        [MinLength(1,ErrorMessage="Manuscript Number must be longer than 1 digit")]
        [Remote("ValidatePaperNumber", "Record", ErrorMessage =
            "This manuscript number is already in the database. Please check your manuscript number and try again. If you have any questions about this particular manuscript number, please contact us.")]       
        public string PaperNumber { get; set; }

        [DisplayName("Country of Corresponding Author")]
        [Required(ErrorMessage = "Country of Corresponding Author is required")]
        public int CountryCountryID { get; set; }

        [DisplayName("Country")]
        public string CountryCountryName { get; set; }

        public int CategoryCategoryID { get; set; }

        [DisplayName("Category")]
        public string CategoryCategoryName { get; set; }

        [DisplayName("Action")]
        public string Action { get; set; }

        [MustBeTrue(ErrorMessage = "You need to add ARRIVE trial flag to the manuscript")]
        [Required(ErrorMessage = "You need to add ARRIVE trial flag to the manuscript")]
        [DisplayName("Have you added the ARRIVE trial flag to the manuscript?")]
        public Boolean trialflag { get; set; }

        [MustBeTrue(ErrorMessage = "Please only enter manuscripts that report laboratory-based in vivo animal research.")]
        [Required(ErrorMessage = "You must only randomize animal research related manuscript")]
        [DisplayName("Does this manuscript report laboratory-based in vivo animal research?")]
        public Boolean AnimalResearch { get; set; }

        [Required]
        [DisplayName("Does the submitted manuscript already contain the ARRIVE checklist?")]
        public string hadarrivechecklist { get; set; }

        [DisplayName("Entry Time")]
        public DateTime RandomizationTime { get; set; }

        [DisplayName("User")]
        public string EntryUser { get; set; }

        [DisplayName("Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class MustBeTrueAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                return value != null && value is bool && (bool)value;
            }
        }



    }
}
