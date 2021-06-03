using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentExternalWallTransMasterViewModel
    {
        public int AssessmentEWID { get; set; }
        public int? ProjectID { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public int? LocationID { get; set; }
        public string Drawing_Image { get; set; }
        public int MobileAssessmentEWID { get; set; } = 0;
        public string BatchID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public AssessmentProjectMasterViewModel assessment_project_master { get; set; }
        public AssessmentTypeLocationMasterViewModel assessment_type_location_master { get; set; }
        public List<AssessmentExternalWallTransDetailViewModel> assessment_external_wall_trn_detail { get; set; }
    }

    public class AssessmentExternalWallTransMasterMobileViewModel
    {
        public int AssessmentEWID { get; set; }
        public int? ProjectID { get; set; }
        public string AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public int? LocationID { get; set; }
        public string LocationName { get; set; }
        public string Drawing_Image { get; set; }
        public int MobileAssessmentEWID { get; set; }
        public string BatchID { get; set; }
        public int Status { get; set; }
        public int CreatedOrUpdatedByUserId { get; set; }

        public List<AssessmentExternalWallTransDetailMobileViewModel>  AssessmentExternalWallTransDetailMobileViewModels { get; set; }
    }

    public class AssessmentExternalWallTransMasterMobileDeleteViewModel
    {
        public int AssessmentEWID { get; set; }
    }
}