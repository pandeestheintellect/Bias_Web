using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionRFWIFormLocationDetailViewModel
    {
        public int RFWIFormLocationDetailID { get; set; }
        public int QCInspectionRFWIFormID { get; set; }
        public int UnitID { get; set; }
        public int QCInspectionDrawingReferenceFileID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public QCInspectionUnitMasterViewModel qcinspection_unit_master { get; set; }
        public QCInspectionProjectRFWIDrawingReferenceFilesViewModel qcinspection_project_rfwi_drawing_reference_files { get; set; }

        [NotMapped]
        public List<SelectListItem> UnitList { get; set; }

        [NotMapped]
        public List<SelectListItem> DrawingReferenceFilesList { get; set; }
    }

    public class QCInspectionRFWIFormLocationDetailMobileViewModel
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public int QCInspectionDrawingReferenceFileID { get; set; }
        public string FileCaption { get; set; }
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
    }
}