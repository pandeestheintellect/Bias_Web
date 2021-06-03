using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionRFWIFormTradeItemDetailViewModel
    {
        public int RFWIFormTradeDetailedCheckListDetailID { get; set; }
        public int QCInspectionRFWIFormID { get; set; }
        public int TradeItemID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public QCInspectionRFWITradeItemDetailViewModel qcinspection_rfwi_trade_item_detail { get; set; }
    }

    public class QCInspectionRFWIFormTradeItemDetailMobileViewModel
    {
        public int TradeItemID { get; set; }
        public string ItemName { get; set; }
        public int OrderBy { get; set; } = 1;
    }
}