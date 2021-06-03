using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentFieldWindowWaterTightnessTestEntryViewModel
    {
        public int AssessmentFWWTTID { get; set; }
        public int ProjectID { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public string Drawing_Image { get; set; }
        public int? WallID { get; set; }
        public int? WindowID { get; set; }
        public int? JointID { get; set; }
        public int? DirectionID { get; set; }
        public int? LeakID { get; set; }
        public AssessmentProjectMasterViewModel projectMasterView { get; set; }
        public List<AssessmentWallMasterViewModel> assessmentWallMasterViewModels { get; set; }
        public List<AssessmentWindowMasterViewModel> assessmentWindowMasterViewModels { get; set; }
        public List<AssessmentJointMasterViewModel> assessmentJointMasterViewModels { get; set; }
        public List<AssessmentDirectionMasterViewModel> assessmentDirectionMasterViewModels { get; set; }
        public List<AssessmentLeakMasterViewModel> assessmentLeakMasterViewModels { get; set; }
        public AssessmentFieldWindowWaterTightnessTestIndexViewModel assessmentFieldWindowWaterTightnessTestIndexViewModel { get; set; }
        public List<AssessmentFieldWindowWaterTightnessTestTransViewModel> assessmentFieldWindowWaterTightnessTestTransViewModels { get; set; }
    }

    public class AssessmentFieldWindowWaterTightnessTestMobileHeaderViewModel
    {
        public AssessmentProjectMasterMobileViewModel ProjectMasterMobileView { get; set; }
        public List<AssessmentWallMasterMobileViewModel> AssessmentWallMasterMobileViewModels  { get; set; }
        public List<AssessmentWindowMasterMobileViewModel> AssessmentWindowMasterMobileViewModels { get; set; }
        public List<AssessmentJointMasterMobileViewModel> AssessmentJointMasterMobileViewModels { get; set; }
        public List<AssessmentDirectionMasterMobileViewModel> AssessmentDirectionMasterMobileViewModels { get; set; }
        public List<AssessmentLeakMasterMobileViewModel> AssessmentLeakMasterMobileViewModels { get; set; }
    }
}