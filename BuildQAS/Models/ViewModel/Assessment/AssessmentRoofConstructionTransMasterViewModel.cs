using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentRoofConstructionTransMasterViewModel
    {
        public int AssessmentRFCID { get; set; }
        public int? ProjectID { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public int? LocationID { get; set; }
        public int MobileAssessmentRFCID { get; set; } = 0;
        public string BatchID { get; set; } = "";
        public string Drawing_Image { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public AssessmentProjectMasterViewModel assessment_project_master { get; set; }
        public AssessmentTypeLocationMasterViewModel assessment_type_location_master { get; set; }
        public List<AssessmentRoofConstructionTransDetailViewModel> assessment_roof_construction_trn_detail { get; set; }
    }

    public class AssessmentRoofConstructionTransMasterMobileViewModel
    {
        public int AssessmentRFCID { get; set; }
        public int? ProjectID { get; set; }
        public string AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public int? LocationID { get; set; }
        public string LocationName { get; set; }
        public string Drawing_Image { get; set; }
        public int MobileAssessmentRFCID { get; set; }
        public string BatchID { get; set; }
        public int Status { get; set; }
        public int CreatedOrUpdatedByUserId { get; set; }

        public List<AssessmentRoofConstructionTransDetailMobileViewModel> AssessmentRoofConstructionTransDetailMobileViewModels { get; set; }
    }

    public class AssessmentRoofConstructionTransMasterMobileDeleteViewModel
    {
        public int AssessmentRFCID { get; set; }
    }
}