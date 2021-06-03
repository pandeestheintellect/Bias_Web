using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionDefectFormViewModel
    {
        public int QCInspectionDefectID { get; set; }
        public string QCInspectionDefectNo { get; set; }
        public int ProjectID { get; set; }
        public int UnitID { get; set; }
        public int TradeID { get; set; }
        public int DefectTypeID { get; set; }
        public string DefectRemarks { get; set; }
        public int SubcontractorID { get; set; }
        public int ProjectManagerID { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string ApprovedRemarks { get; set; }
        public Nullable<int> ReDoBy { get; set; }
        public Nullable<System.DateTime> ReDoDate { get; set; }
        public string ReDoRemarks { get; set; }
        public Nullable<int> ReDoDoneBy { get; set; }
        public Nullable<System.DateTime> ReDoDoneDate { get; set; }
        public string ReDoDoneRemarks { get; set; }
        public Nullable<int> RectificationBy { get; set; }
        public Nullable<System.DateTime> RectificationDate { get; set; }
        public string RectificationRemarks { get; set; }
        public string RectificationSignature { get; set; }
        public Nullable<int> ReworkBy { get; set; }
        public Nullable<System.DateTime> ReworkDate { get; set; }
        public string ReworkRemarks { get; set; }
        public Nullable<int> CompletedBy { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public string CompletedRemarks { get; set; }
        public string CompletedSignature { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; } = "Pending";
        public int MobileQCInspectionDefectID { get; set; } = 0;
        public string BatchID { get; set; } = "";
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public QCInspectionProjectMasterViewModel qcinspection_project_master { get; set; }
        public QCInspectionUnitMasterViewModel qcinspection_unit_master { get; set; }
        public QCInspectionTradeMasterViewModel qcinspection_trade_master { get; set; }
        public QCInspectionDefectTypeMasterViewModel qcinspection_defect_type_master { get; set; }
        public QCInspectionSubcontractorMasterViewModel qcinspection_subcontractor_master { get; set; }
        public UserViewModel user { get; set; }
        public List<QCInspectionDefectFormFilesViewModel> qcinspection_defect_files { get; set; }
        
        public List<HttpPostedFileBase> DefectFiles { get; set; }
        public List<HttpPostedFileBase> RectifyFiles { get; set; }

        [NotMapped]
        public string FilePath { get; set; }

        [NotMapped]
        public string ProjectName { get; set; }

        [NotMapped]
        public string LocationName { get; set; }

        [NotMapped]
        public List<SelectListItem> ProjectList { get; set; }

        [NotMapped]
        public List<SelectListItem> UnitList { get; set; }

        [NotMapped]
        public List<SelectListItem> TradeList { get; set; }

        [NotMapped]
        public List<SelectListItem> DefectTypeList { get; set; }

        [NotMapped]
        public List<SelectListItem> SubcontractorList { get; set; }

        [NotMapped]
        public List<SelectListItem> ProjectManagerList { get; set; }

        [NotMapped]
        public List<QCInspectionDefectFormFilesMobileViewModel> DefectFormFiles { get; set; }
    }

    public class FileUploadViewModel
    {
        public string filename { get; set; }
        public string data { get; set; }
    }

    public class QCInspectionDefectFormMobileViewModel
    {
        public int QCInspectionDefectID { get; set; }
        public string QCInspectionDefectNo { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public int TradeID { get; set; }
        public string TradeName { get; set; }
        public int DefectTypeID { get; set; }
        public string DefectTypeName { get; set; }
        public string DefectRemarks { get; set; }
        public int SubcontractorID { get; set; }
        public string SubcontractorName { get; set; }
        public int ProjectManagerID { get; set; }
        public string ProjectManagerName { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public string ApprovedDate { get; set; }
        public string ApprovedRemarks { get; set; }
        public Nullable<int> ReDoBy { get; set; }
        public string ReDoByName { get; set; }
        public string ReDoDate { get; set; }
        public string ReDoRemarks { get; set; }
        public Nullable<int> ReDoDoneBy { get; set; }
        public string ReDoDoneByName { get; set; }
        public string ReDoDoneDate { get; set; }
        public string ReDoDoneRemarks { get; set; }
        public Nullable<int> RectificationBy { get; set; }
        public string RectificationByName { get; set; }
        public string RectificationDate { get; set; }
        public string RectificationRemarks { get; set; }
        public string RectificationSignature { get; set; }
        public Nullable<int> ReworkBy { get; set; }
        public string ReworkByName { get; set; }
        public string ReworkDate { get; set; }
        public string ReworkRemarks { get; set; }
        public Nullable<int> CompletedBy { get; set; }
        public string CompletedByName { get; set; }
        public string CompletedDate { get; set; }
        public string CompletedRemarks { get; set; }
        public string CompletedSignature { get; set; }
        public string Status { get; set; }
        public int MobileQCInspectionDefectID { get; set; } = 0;
        public string BatchID { get; set; } = "";
        public int MobileStatus { get; set; } = 0;
        public int CreatedOrUpdatedByUserId { get; set; }
        public string CreatedOrUpdatedDate { get; set; }
        public List<QCInspectionDefectFormFilesMobileViewModel> DefectFiles { get; set; }
    }

    public class QCInspectionDefectFormMobileDeleteViewModel
    {
        public int QCInspectionDefectID { get; set; }
    }

}