//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BuildInspect.Models.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class qcinspection_rfwi_form_trade_detailed_checklist_detail
    {
        public int RFWIFormTradeDetailedCheckListDetailID { get; set; }
        public int QCInspectionRFWIFormID { get; set; }
        public int TradeDetailedCheckListID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        public virtual qcinspection_rfwi_trade_detailed_checklist_detail qcinspection_rfwi_trade_detailed_checklist_detail { get; set; }
        public virtual qcinspection_rfwi_form qcinspection_rfwi_form { get; set; }
    }
}