using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentWetAreaWaterTightnessTestEntryViewModel
    {
        public int AssessmentWAWTTID { get; set; }
        public string Block_Unit { get; set; }
        public string Other_Result { get; set; }
        public string Drawing_Image { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public int ProjectID { get; set; }
        public string ModuleAndProcess { get; set; }
        public int? ResultCount { get; set; }
        public AssessmentProjectMasterViewModel projectMasterView { get; set; }
        public List<ModuleAndProcessModel> moduleAndProcessesList { get; set; }
        public AssessmentWetAreaWaterTightnessTestIndexViewModel assessmentWetAreaWaterTightnessTestIndexViewModel { get; set; }
        public List<AssessmentTypeModuleMasterViewModel> assessmentTypeModuleMasterViewModels { get; set; }
        public List<AssessmentTypeModuleProcessMasterViewModel> assessmentTypeModuleProcessMasterViewModels { get; set; }
        public List<AssessmentWetAreaWaterTightnessTestResultMasterViewModel>  assessmentWetAreaWaterTightnessTestResultMasterViewModels { get; set; }
        public List<AssessmentWetAreaWaterTightnessTestTransMasterViewModel> assessmentWetAreaWaterTightnessTestTransMasterViewModels { get; set; }
        public List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> assessmentWetAreaWaterTightnessTestTransDetailViewModels { get; set; }
        public List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> assessmentWetAreaWaterTightnessTestTransDetailResultViewModels { get; set; }
    }

    public class AssessmentWetAreaWaterTightnessTestMobileHeaderViewModel
    {
        public AssessmentProjectMasterMobileViewModel ProjectMasterMobileView { get; set; }
        public List<AssessmentTypeModuleMasterMobileViewModel> AssessmentTypeModuleMasterMobileViewModels { get; set; }
    }
}