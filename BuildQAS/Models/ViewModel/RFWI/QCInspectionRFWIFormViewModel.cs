using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionRFWIFormViewModel
    {
        public int QCInspectionRFWIFormID { get; set; }
        public string QCInspectionRFWINo { get; set; } = "Auto Number";
        public int ProjectID { get; set; }
        public int TradeID { get; set; }
        public Nullable<int> RequestBy { get; set; }
        public Nullable<System.DateTime> RequestOn { get; set; }
        public string RequestSignature { get; set; }
        public string NotiicationReceivedByName { get; set; }
        public string NotiicationReceivedSignature { get; set; }
        public Nullable<System.DateTime> NotiicationReceivedOn { get; set; }
        public byte InspectionNo { get; set; } = 1;
        public Nullable<System.DateTime> InspectionOn { get; set; }
        public Nullable<System.TimeSpan> InspectionStartOn { get; set; }
        public Nullable<System.TimeSpan> InspectionEndOn { get; set; }
        public int InspectorID { get; set; }
        public string RequestFor { get; set; }
        public string ItemOthers { get; set; }
        public string DetailedCheckListOthers { get; set; }
        public bool OtherTradeClearance_Structure { get; set; }
        public Nullable<int> OtherTradeClearance_StructureBy { get; set; }
        public string OtherTradeClearance_StructureSignature { get; set; }
        public Nullable<System.DateTime> OtherTradeClearance_StructureOn { get; set; }
        public bool OtherTradeClearance_MandE { get; set; }
        public Nullable<int> OtherTradeClearance_MandEBy { get; set; }
        public string OtherTradeClearance_MandESignature { get; set; }
        public Nullable<System.DateTime> OtherTradeClearance_MandEOn { get; set; }
        public bool OtherTradeClearance_Other { get; set; }
        public Nullable<int> OtherTradeClearance_OtherBy { get; set; }
        public string OtherTradeClearance_OtherSignature { get; set; }
        public Nullable<System.DateTime> OtherTradeClearance_OtherOn { get; set; }
        public Nullable<int> CompletedBy { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public string CompletedRemarks { get; set; }
        public string CompletedSignature { get; set; }
        public int ReInspectionFormID { get; set; } = 0;
        public string Status { get; set; } = "Pending";
        public int MobileQCInspectionRFWIFormID { get; set; } = 0;
        public string BatchID { get; set; } = "";
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; } = DateTime.Now;
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public QCInspectionProjectMasterViewModel qcinspection_project_master { get; set; }
        public QCInspectionRFWITradeMasterViewModel qcinspection_rfwi_trade_master { get; set; }
        public UserViewModel user { get; set; }
        public List<QCInspectionRFWIFormGeneralCheckListDetailViewModel> qcinspection_rfwi_form_general_checklist_detail { get; set; }
        public List<QCInspectionRFWIFormTradeDetailedCheckListDetailViewModel> qcinspection_rfwi_form_trade_detailed_checklist_detail { get; set; }
        public List<QCInspectionRFWIFormTradeItemDetailViewModel> qcinspection_rfwi_form_trade_item_detail { get; set; }
        public List<QCInspectionRFWIFormLocationDetailViewModel> qcinspection_rfwi_form_location_detail { get; set; }

        [NotMapped]
        public bool ProceedRequest { get; set; } = false;

        [NotMapped]
        public bool OtherSigned { get; set; } = false;

        [NotMapped]
        public string RequestByName { get; set; }

        [NotMapped]
        public string OtherTradeClearance_StructureByName { get; set; }

        [NotMapped]
        public string OtherTradeClearance_MandEByName { get; set; }

        [NotMapped]
        public string OtherTradeClearance_OtherByName { get; set; }

        [NotMapped]
        public List<SelectListItem> ProjectList { get; set; }

        [NotMapped]
        public List<SelectListItem> TradeList { get; set; }

        [NotMapped]
        public List<SelectListItem> InspectorList { get; set; }

        [NotMapped]
        public List<SelectListItem> GeneralCheckList { get; set; }

        [NotMapped]
        public List<SelectListItem> TradeDetailedCheckList { get; set; }

        [NotMapped]
        public List<SelectListItem> TradeItemList { get; set; }

        [NotMapped]
        public string InspectionStartTime { get; set; }

        [NotMapped]
        public string InspectionEndTime { get; set; }

        [NotMapped]
        public int TradeItemID { get; set; }

        //[NotMapped]
        //public QCInspectionRFWIFormLocationDetailViewModel RFWIFormLocationDetailViewModel { get; set; }

        [NotMapped]
        public string SelectedGeneralCheckListIds { get; set; }

        [NotMapped]
        public string SelectedTradeDetailedCheckListIds { get; set; }

        [NotMapped]
        public string SelectedTradeItemListIds { get; set; }

        [NotMapped]
        public List<QCInspectionRFWIFormLocationDetailViewModel> RFWIFormLocationDetailViewModels { get; set; }

    }

    public class AvailableSlotsModel
    {
        public string QCInspectionRFWINo { get; set; }
        public string ProjectName { get; set; }
        public Nullable<System.TimeSpan> InspectionStartOn { get; set; }
        public Nullable<System.TimeSpan> InspectionEndOn { get; set; }
    }

    public class InspectorModel
    {
        public int InspectorID { get; set; }
        public string InspectorName { get; set; }
    }

    public class QCInspectionRFWIFormMobileViewModel
    {
        public int QCInspectionRFWIFormID { get; set; }
        public string QCInspectionRFWINo { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int TradeID { get; set; }
        public string TradeName { get; set; }
        public Nullable<int> RequestBy { get; set; }
        public string RequestByName { get; set; }
        public string RequestOn { get; set; }
        public string RequestSignature { get; set; }
        public string NotiicationReceivedByName { get; set; }
        public string NotiicationReceivedSignature { get; set; }
        public string NotiicationReceivedOn { get; set; }
        public byte InspectionNo { get; set; } = 1;
        public string InspectionOn { get; set; }
        public string InspectionStartOn { get; set; }
        public string InspectionEndOn { get; set; }
        public int InspectorID { get; set; }
        public string InspectorName { get; set; }
        public string RequestFor { get; set; }
        public string ItemOthers { get; set; }
        public string DetailedCheckListOthers { get; set; }
        public bool OtherTradeClearance_Structure { get; set; }
        public Nullable<int> OtherTradeClearance_StructureBy { get; set; }
        public string OtherTradeClearance_StructureByName { get; set; }
        public string OtherTradeClearance_StructureSignature { get; set; }
        public string OtherTradeClearance_StructureOn { get; set; }
        public bool OtherTradeClearance_MandE { get; set; }
        public Nullable<int> OtherTradeClearance_MandEBy { get; set; }
        public string OtherTradeClearance_MandEByName { get; set; }
        public string OtherTradeClearance_MandESignature { get; set; }
        public string OtherTradeClearance_MandEOn { get; set; }
        public bool OtherTradeClearance_Other { get; set; }
        public Nullable<int> OtherTradeClearance_OtherBy { get; set; }
        public string OtherTradeClearance_OtherByName { get; set; }
        public string OtherTradeClearance_OtherSignature { get; set; }
        public string OtherTradeClearance_OtherOn { get; set; }
        public Nullable<int> CompletedBy { get; set; }
        public string CompletedByName { get; set; }
        public string CompletedDate { get; set; }
        public string CompletedRemarks { get; set; }
        public string CompletedSignature { get; set; }
        public int ReInspectionFormID { get; set; } = 0;
        public string Status { get; set; }
        public int MobileQCInspectionRFWIFormID { get; set; } = 0;
        public string BatchID { get; set; } = "";
        public int MobileStatus { get; set; } = 0;
        public int CreatedOrUpdatedByUserId { get; set; }
        public string CreatedOrUpdatedDate { get; set; }
        public bool ProceedRequest { get; set; } = false;
        public bool OtherSigned { get; set; } = false;

        public List<QCInspectionRFWIFormGeneralCheckListDetailMobileViewModel> QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels { get; set; }
        public List<QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModel> QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels { get; set; }
        public List<QCInspectionRFWIFormTradeItemDetailMobileViewModel> QCInspectionRFWIFormTradeItemDetailMobileViewModels { get; set; }
        public List<QCInspectionRFWIFormLocationDetailMobileViewModel> QCInspectionRFWIFormLocationDetailMobileViewModels { get; set; }
    }

    public class QCInspectionRFWIFormMobileDeleteViewModel
    {
        public int QCInspectionRFWIFormID { get; set; }
    }

}


