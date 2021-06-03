using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentExternalWorksEntryViewModel
    {
        public int AssessmentEWKID { get; set; }
        public string Remarks { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public int ProjectID { get; set; }
        public int? LocationID { get; set; }
        public string Drawing_Image { get; set; }
        public string ModuleAndProcess { get; set; }
        public int? MaxProcessCount { get; set; }
        public AssessmentProjectMasterViewModel projectMasterView { get; set; }
        public List<AssessmentTypeLocationMasterViewModel> assessmentTypeLocationMasterViews { get; set; }
        public AssessmentExternalWorksIndexViewModel assessmentExternalWorksIndexViewModel { get; set; }
        public List<AssessmentTypeModuleMasterViewModel> assessmentTypeModuleMasterViewModels { get; set; }
        public List<AssessmentTypeModuleProcessMasterViewModel> assessmentTypeModuleProcessMasterViewModels { get; set; }
        public List<AssessmentExternalWorksTransMasterViewModel> assessmentExternalWorksTransMasterViewModels { get; set; }
        public List<AssessmentExternalWorksTransDetailViewModel> assessmentExternalWorksTransDetailViewModels { get; set; }
    }

    public class AssessmentExternalWorksMobileHeaderViewModel
    {
        public AssessmentProjectMasterMobileViewModel ProjectMasterMobileView { get; set; }
        public List<AssessmentTypeLocationMasterMobileViewModel> AssessmentTypeLocationMasterMobileViews { get; set; }
        public List<AssessmentTypeModuleMasterMobileViewModel> AssessmentTypeModuleMasterMobileViewModels { get; set; }
    }
}