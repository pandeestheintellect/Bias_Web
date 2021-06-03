using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentExternalWallTransDetailViewModel
    {
        public int AssessmentEWDetailID { get; set; }
        public int? AssessmentEWID { get; set; }
        public int? AssessmentTypeModuleProcessID { get; set; }
        public string Result { get; set; } = "1";
        public int RowNo { get; set; } = 1;
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public AssessmentTypeModuleProcessMasterViewModel assessment_type_module_Process_master { get; set; }
    }

    public class AssessmentExternalWallTransDetailMobileViewModel
    {
        public int AssessmentEWDetailID { get; set; }
        public int? AssessmentTypeModuleProcessID { get; set; }
        public int Result { get; set; } = 1;
        public int RowNo { get; set; } = 1;
    }
}