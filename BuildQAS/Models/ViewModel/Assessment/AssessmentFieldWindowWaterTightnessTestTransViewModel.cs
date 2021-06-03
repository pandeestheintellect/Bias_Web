using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentFieldWindowWaterTightnessTestTransViewModel
    {
        public int AssessmentFWWTTID { get; set; }
        public int ProjectID { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public int AssessmentWallID { get; set; }
        public int AssessmentWindowID { get; set; }
        public int AssessmentJointID { get; set; }
        public int AssessmentDirectionID { get; set; }
        public int AssessmentLeakID { get; set; }
        public string Result { get; set; } = "1";
        public string Drawing_Image { get; set; }
        public int MobileAssessmentFWWTTID { get; set; } = 0;
        public string BatchID { get; set; } = "";
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public AssessmentProjectMasterViewModel assessment_project_master { get; set; }
        public AssessmentWallMasterViewModel assessment_wall_master { get; set; }
        public AssessmentWindowMasterViewModel assessment_window_master { get; set; }
        public AssessmentJointMasterViewModel assessment_joint_master { get; set; }
        public AssessmentDirectionMasterViewModel assessment_direction_master { get; set; }
        public AssessmentLeakMasterViewModel assessment_leak_master { get; set; }
    }

    public class AssessmentFieldWindowWaterTightnessTestTransMobileViewModel
    {
        public int AssessmentFWWTTID { get; set; }
        public int ProjectID { get; set; }
        public string AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public int AssessmentWallID { get; set; }
        public int AssessmentWindowID { get; set; }
        public int AssessmentJointID { get; set; }
        public int AssessmentDirectionID { get; set; }
        public int AssessmentLeakID { get; set; }
        public int Result { get; set; } = 1;
        public string Drawing_Image { get; set; }
        public int MobileAssessmentFWWTTID { get; set; } = 0;
        public string BatchID { get; set; } = "";
        public int Status { get; set; } = 0;
        public int CreatedOrUpdatedByUserId { get; set; } = 1;
    }

    public class AssessmentFieldWindowWaterTightnessTestTransMobileDeleteViewModel
    {
        public int AssessmentFWWTTID { get; set; }
    }

}