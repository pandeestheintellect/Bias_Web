using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentInternalFinishesEntryViewModel
    {
        public string Block_Unit { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public int ProjectID { get; set; }
        public int? LocationID { get; set; }
        public int PCount { get; set; }
        public int SCount { get; set; }
        public int CCount { get; set; }
        public string ModuleAndProcess { get; set; }
        public AssessmentProjectMasterViewModel projectMasterView { get; set; }
        public List<ModuleAndProcessModel> moduleAndProcessesList { get; set; }
        public List<AssessmentTypeLocationMasterViewModel> assessmentTypeLocationMasterViews { get; set; }
        public AssessmentInternalFinishesIndexViewModel assessmentInternalFinishesIndexViewModel { get; set; }
        public List<AssessmentTypeModuleMasterViewModel> assessmentTypeModuleMasterViewModels { get; set; }
        public List<AssessmentTypeModuleProcessMasterViewModel> assessmentTypeModuleProcessMasterViewModels { get; set; }
        public List<AssessmentInternalFinishesTransMasterViewModel> assessmentInternalFinishesTransMasterViewModels { get; set; }
        public List<AssessmentInternalFinishesTransDetailViewModel> assessmentInternalFinishesTransDetailViewModels { get; set; }
    }

    public class ModuleAndProcessModel
    {
        public string ModuleNames { get; set; }
        public List<string> ProcessIds { get; set; }
    }

    public class AssessmentInternalFinishesMobileHeaderViewModel
    {
        public AssessmentProjectMasterMobileViewModel ProjectMasterMobileView { get; set; }
        public List<AssessmentTypeLocationMasterMobileViewModel> AssessmentTypeLocationMasterMobileViews { get; set; }
        public List<AssessmentTypeModuleMasterMobileViewModel> AssessmentTypeModuleMasterMobileViewModels { get; set; }
    }
}