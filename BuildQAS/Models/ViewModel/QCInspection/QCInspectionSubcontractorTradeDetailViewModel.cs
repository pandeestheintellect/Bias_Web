using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionSubcontractorTradeDetailViewModel
    {
        public int SubcontractorTradeID { get; set; }
        public int SubcontractorID { get; set; }
        public int TradeID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        //public QCInspectionSubcontractorMasterViewModel qcinspection_subcontractor_master { get; set; }
        public QCInspectionTradeMasterViewModel qcinspection_trade_master { get; set; }
    }
}