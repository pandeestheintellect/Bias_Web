using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentRoofConstructionEntryViewModel
    {
        public int AssessmentRFCID { get; set; }
        public string Block_Unit { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public int ProjectID { get; set; }
        public int? LocationID { get; set; }
        public string Drawing_Image { get; set; }
        public string ModuleAndProcess { get; set; }
        public AssessmentProjectMasterViewModel projectMasterView { get; set; }
        public List<ModuleAndProcessModel> moduleAndProcessesList { get; set; }
        public List<AssessmentTypeLocationMasterViewModel> assessmentTypeLocationMasterViews { get; set; }
        public AssessmentRoofConstructionIndexViewModel assessmentRoofConstructionIndexViewModel { get; set; }
        public List<AssessmentTypeModuleMasterViewModel> assessmentTypeModuleMasterViewModels { get; set; }
        public List<AssessmentTypeModuleProcessMasterViewModel> assessmentTypeModuleProcessMasterViewModels { get; set; }
        public List<AssessmentRoofConstructionTransMasterViewModel> assessmentRoofConstructionTransMasterViewModels { get; set; }
        public List<AssessmentRoofConstructionTransDetailViewModel> assessmentRoofConstructionTransDetailViewModels { get; set; }
    }

    public class AssessmentRoofConstructionMobileHeaderViewModel
    {
        public AssessmentProjectMasterMobileViewModel ProjectMasterMobileView { get; set; }
        public List<AssessmentTypeLocationMasterMobileViewModel> AssessmentTypeLocationMasterMobileViews { get; set; }
        public List<AssessmentTypeModuleMasterMobileViewModel> AssessmentTypeModuleMasterMobileViewModels { get; set; }
    }
}