using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentSummaryViewModel
    {
        public AssessmentProjectMasterViewModel projectMasterViewModel { get; set; }
        public List<AssessmentSummaryDetailModel> assessmentSummaryDetailModels { get; set; }
        public decimal? InternalFinishesWorksWeightage { get; set; } = 0;
        public decimal? InternalFinishesWorksWeightedScore { get; set; } = 0;
        public decimal? ArchitecturalWorksWeightage { get; set; } = 0;
        public decimal? ArchitecturalWorksWeightedScore { get; set; } = 0;
        public decimal? ArchitecturalWorksTotalScore { get; set; } = 0;
        public decimal? MEWorksWeightage { get; set; } = 0;
        public decimal? MEWorksWeightedScore { get; set; } = 0;
        public decimal? MEWorksTotalScore { get; set; } = 0;
        public decimal? FinalScore { get; set; } = 0;
    }

    public class AssessmentSummaryDetailModel
    {
        public int ProjectID { get; set; }
        public int AssessmentTypeID { get; set; }
        public int? AssessmentTypeModuleID { get; set; }
        public string AssessmentTypeModuleName { get; set; }
        public int NoofCompliances { get; set; }
        public int NoofChecks { get; set; }
        public decimal Percentage { get; set; }
        public decimal Weightage { get; set; }
        public decimal WeightedScore { get; set; }
        public string MainNonCompliances { get; set; }
        public int? OrderBy { get; set; }
        public int? Is_Applicable { get; set; }
    }
}