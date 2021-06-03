using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentTypeModuleProcessMasterViewModel
    {
        public int AssessmentTypeModuleProcessID { get; set; }
        public int AssessmentTypeModuleID { get; set; }
        public int? AssessmentTypeLocationID { get; set; } = null;
        [Required]
        public string AssessmentTypeModuleProcessName { get; set; }
        public int OrderBy { get; set; }
        public int IsActive { get; set; } = 1;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public AssessmentTypeModuleMasterViewModel assessment_type_module_master { get; set; }
        public AssessmentTypeLocationMasterViewModel assessment_type_location_master { get; set; }
    }

    public class AssessmentTypeModuleProcessMasterMobileViewModel
    {
        public int AssessmentTypeModuleProcessID { get; set; }
        public int AssessmentTypeModuleID { get; set; }
        public int AssessmentTypeLocationID { get; set; } 
        public string AssessmentTypeModuleProcessName { get; set; }
        public int OrderBy { get; set; }
    }
}