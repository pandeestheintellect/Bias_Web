using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentReportViewModel
    {
        public bool DAR1IF { get; set; } = true;
        public bool DAR2FLOOR { get; set; } = true;
        public bool DAR2WALL { get; set; } = true;
        public bool DAR2CEILING { get; set; } = true;
        public bool DAR2DOOR { get; set; } = true;
        public bool DAR2WINDOW { get; set; } = true;
        public bool DAR2COMPONENT { get; set; } = true;
        public bool DAR2ME { get; set; } = true;
        public string DAR1IFChartType { get; set; } = "BAR";
        public string DAR2FLOORFChartType { get; set; } = "BAR";
        public string DAR2WALLFChartType { get; set; } = "BAR";
        public string DAR2CEILINGFChartType { get; set; } = "BAR";
        public string DAR2DOORFChartType { get; set; } = "BAR";
        public string DAR2WINDOWFChartType { get; set; } = "BAR";
        public string DAR2COMPONENTFChartType { get; set; } = "BAR";
        public string DAR2MEFChartType { get; set; } = "BAR";
        public int PageCount { get; set; } = 16;
        public string PDFFilename { get; set; } = "AssessmentReports.pdf";
        public AssessmentProjectMasterViewModel projectMasterViewModel { get; set; }
        public List<AssessmentSummaryDetailModel> assessmentSummaryDetailModels { get; set; }
        public List<ModuleAndProcessModel> moduleAndProcessesList { get; set; }
        public List<AssessmentTypeModuleMasterViewModel> assessmentTypeModuleMasterViewModels { get; set; }
        public AssessmentSummaryViewModel assessmentSummaryViewModel { get; set; }
        public List<AssessmentReportDetailModel> DAR1IFList { get; set; }
        public List<AssessmentReportDetailModel> DAR2FLOORList { get; set; }
        public List<AssessmentReportDetailModel> DAR2WALLList { get; set; }
        public List<AssessmentReportDetailModel> DAR2CEILINGList { get; set; }
        public List<AssessmentReportDetailModel> DAR2DOORList { get; set; }
        public List<AssessmentReportDetailModel> DAR2WINDOWList { get; set; }
        public List<AssessmentReportDetailModel> DAR2COMPONENTList { get; set; }
        public List<AssessmentReportDetailModel> DAR2MEList { get; set; }
    }

    public class AssessmentReportDetailModel
    {
        public int ProjectID { get; set; }
        public int AssessmentTypeID { get; set; }
        public int? AssessmentTypeModuleID { get; set; }
        public string AssessmentTypeModuleName { get; set; }
        public int? AssessmentTypeModuleProcessID { get; set; }
        public string AssessmentTypeModuleProcessName { get; set; }
        public int NoOfTicks { get; set; }
        public int NoofChecks { get; set; }
        public decimal Compliances { get; set; }
        public decimal NonCompliances { get; set; }
        public int? OrderBy { get; set; }
    }
}