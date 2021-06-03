using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionRFWIFormGeneralCheckListDetailViewModel
    {
        public int RFWIFormGeneralCheckListDetailID { get; set; }
        public int QCInspectionRFWIFormID { get; set; }
        public int GeneralCheckListID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public QCInspectionRFWIGeneralCheckListMasterViewModel qcinspection_rfwi_general_checklist_master { get; set; }
    }

    public class QCInspectionRFWIFormGeneralCheckListDetailMobileViewModel
    {
        public int GeneralCheckListID { get; set; }
        public string GeneralCheckListName { get; set; }
        public int OrderBy { get; set; } = 1;
    }
}