using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentWetAreaWaterTightnessTestTransMasterViewModel
    {
        public int AssessmentWAWTTID { get; set; }
        public int? ProjectID { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public string Drawing_Image { get; set; }
        public string Other_Result { get; set; }
        public int MobileAssessmentWAWTTID { get; set; } = 0;
        public string BatchID { get; set; } = "";
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public AssessmentProjectMasterViewModel assessment_project_master { get; set; }
        public List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> assessment_wet_area_water_tightness_test_tran_detail { get; set; }
        public List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> assessment_wet_area_water_tightness_test_tran_detail_result { get; set; }
    }

    public class AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel
    {
        public int AssessmentWAWTTID { get; set; }
        public int? ProjectID { get; set; }
        public string AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public string Drawing_Image { get; set; }
        public string Other_Result { get; set; }
        public int MobileAssessmentWAWTTID { get; set; }
        public string BatchID { get; set; }
        public int Status { get; set; }
        public int CreatedOrUpdatedByUserId { get; set; }

        public List<AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModel> AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModels { get; set; }
        public List<AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModel> AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModels { get; set; }
    }

    public class AssessmentWetAreaWaterTightnessTestTransMasterMobileDeleteViewModel
    {
        public int AssessmentWAWTTID { get; set; }
    }
}