using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentTypeLocationMasterViewModel
    {
        public int AssessmentTypeLocationID { get; set; }
        public int AssessmentTypeID { get; set; }
        [Required]
        public string AssessmentTypeLocationName { get; set; }
        [Required]
        public string AssessmentTypeLocationShortName { get; set; }
        [Required]
        public string AssessmentTypeLocationType { get; set; }
        public int IsActive { get; set; } = 1;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public AssessmentTypeMasterViewModel assessment_type_master { get; set; }
    }

    public class AssessmentTypeLocationMasterMobileViewModel
    {
        public int AssessmentTypeLocationID { get; set; }
        public int AssessmentTypeID { get; set; }
        public string AssessmentTypeLocationName { get; set; }
        public string AssessmentTypeLocationShortName { get; set; }
        public string AssessmentTypeLocationType { get; set; }

        public List<AssessmentTypeModuleMasterMobileViewModel> AssessmentTypeModuleMasterMobileViewModels { get; set; }
    }
}