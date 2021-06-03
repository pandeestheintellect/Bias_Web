using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionRFWITradeMasterViewModel
    {
        public int TradeID { get; set; }
        [Required]
        public string TradeName { get; set; }
        public int IsActive { get; set; } = 1;
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public List<QCInspectionRFWITradeDetailedCheckListDetailViewModel> qcinspection_rfwi_trade_detailed_checklist_detail { get; set; }
        public List<QCInspectionRFWITradeItemDetailViewModel> qcinspection_rfwi_trade_item_detail { get; set; }

        [NotMapped]
        public QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeDetailedCheckListDetail { get; set; }

        [NotMapped]
        public QCInspectionRFWITradeItemDetailViewModel RFWITradeItemDetail { get; set; }
    }
}