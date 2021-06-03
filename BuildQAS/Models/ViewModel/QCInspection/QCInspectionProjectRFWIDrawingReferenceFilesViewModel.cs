using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionProjectRFWIDrawingReferenceFilesViewModel
    {
        public int QCInspectionDrawingReferenceFileID { get; set; }
        public int ProjectID { get; set; }
        public string FileCaption { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        [NotMapped]
        public HttpPostedFileBase DrawingReferenceFile { get; set; }
    }

    public class QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel
    {
        public int QCInspectionDrawingReferenceFileID { get; set; }
        public string FileCaption { get; set; }
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
    }
}