using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentTypeModuleMasterViewModel
    {
        public int AssessmentTypeModuleID { get; set; }
        public int? AssessmentTypeID { get; set; }
        [Required]
        public string AssessmentTypeModuleName { get; set; }
        [Required]
        public string AssessmentTypeModuleShortName { get; set; }
        public int? OrderBy { get; set; }
        public int NoOfRow { get; set; }
        public int? ProcessCount { get; set; }
        public int IsActive { get; set; } = 1;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public AssessmentTypeMasterViewModel assessment_type_master { get; set; }

        [NotMapped]
        public List<AssessmentTypeModuleProcessMasterViewModel> AssessmentTypeModuleProcessMasterViewModels { get; set; }
    }

    public class AssessmentTypeModuleMasterMobileViewModel
    {
        public int AssessmentTypeModuleID { get; set; }
        public int? AssessmentTypeID { get; set; }
        public string AssessmentTypeModuleName { get; set; }
        public string AssessmentTypeModuleShortName { get; set; }
        public int? OrderBy { get; set; }
        public int NoOfRow { get; set; }

        public List<AssessmentTypeModuleProcessMasterMobileViewModel> AssessmentTypeModuleProcessMasterViewModels { get; set; }
    }

}