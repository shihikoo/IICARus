using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IICURas.Models
{
    public class UploadViewModel
    {
        public int RecordRecordID { get; set; }

        public int PaperDocumentsID { get; set; }

        [DisplayName("Paper Number")]
        public string PaperNumber { get; set; }

        [DisplayName("File Name")]
        [RegularExpression(@"([a-zA-Z 0-9.&'-]+)", ErrorMessage = "File Name should be alphanumeric.")]
        public string FileName { get; set; }

        [DisplayName("File Type")]
        public string FileType { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [DisplayName("File")]
        public HttpPostedFileBase FileUpload { get; set; }

        [DataType(DataType.Url)]
        public string FileUrl { get; set; }

        [DisplayName("Uploader")]
        public string UploadUser { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [DisplayName("Update Time")]
        public DateTime LastUpdateTime { get; set; }

        public Boolean DeletePaperDocument { get; set; }
    }
}