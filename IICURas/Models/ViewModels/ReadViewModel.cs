using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IICURas.Models
{
    public class ReadViewModel
    {
        public ReadViewModel(PaperDocument paperDocument) {
            TrainingDocumentID = paperDocument.PaperDocumentsID;
            FileName = paperDocument.FileName;
            FileType = paperDocument.FileType;
            FileUrl = paperDocument.FileUrl;

        }
        public ReadViewModel(){}
        public int TrainingDocumentID { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        [DisplayName("File Type")]
        public string FileType { get; set; }

        [DataType(DataType.Url)]
        public string FileUrl { get; set; }

        
    }
}