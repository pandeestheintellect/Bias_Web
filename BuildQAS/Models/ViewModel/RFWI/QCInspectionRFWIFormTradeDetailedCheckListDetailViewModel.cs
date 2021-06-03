using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionRFWIFormTradeDetailedCheckListDetailViewModel
    {
        public int RFWIFormTradeDetailedCheckListDetailID { get; set; }
        public int QCInspectionRFWIFormID { get; set; }
        public int TradeDetailedCheckListID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public QCInspectionRFWITradeDetailedCheckListDetailViewModel qcinspection_rfwi_trade_detailed_checklist_detail { get; set; }
    }

    public class QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModel
    {
        public int TradeDetailedCheckListID { get; set; }
        public string DetailedCheckListName { get; set; }
        public int OrderBy { get; set; } = 1;
    }
}