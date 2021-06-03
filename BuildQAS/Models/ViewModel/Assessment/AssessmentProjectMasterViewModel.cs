using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentProjectMasterViewModel
    {

        public int ProjectID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        [Required]
        public string Project_Name { get; set; }
        [Required]
        public string Project_ID { get; set; }
        public string Project_DocNo { get; set; }
        [Required]
        public string Developer_Name { get; set; }
        [Required]
        public string Contractor_Name { get; set; }
        [Required]
        public Nullable<System.DateTime> Assessment_StartOn { get; set; }
        [Required]
        public Nullable<System.DateTime> Assessment_EndOn { get; set; }
        public string Assessment_Dates { get; set; }
        public Nullable<int> DevelopmentTypeID { get; set; }
        public decimal? ArchitecturalWorksWeightage { get; set; }
        public decimal? MEWorksWeightage { get; set; }
        public decimal? BuildQASScore { get; set; }
        public decimal? MinimumCompliancePercentageThreshold { get; set; }
        public Nullable<int> Is_ExternalWallApplicable { get; set; }
        public Nullable<int> Is_ExternalWorksApplicable { get; set; }
        public Nullable<int> Is_RoofApplicable { get; set; }
        public Nullable<int> Is_FieldWindowWTTApplicable { get; set; }
        public Nullable<int> Is_WetAreaWTTApplicable { get; set; }
        public string FieldWindowWTT_Contractor_Name { get; set; }
        public string FieldWindowWTT_Witness_Name { get; set; }
        public string WetAreaWTT_Contractor_Name { get; set; }
        public string WetAreaWTT_Witness_Name { get; set; }
        public Nullable<int> Is_Completed { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        [NotMapped]
        public string StartDate { get; set; }

        [NotMapped]
        public string EndDate { get; set; }

        [NotMapped]
        public string Assessors { get; set; }

        public AssessmentDevelopmentTypeMasterViewModel assessment_development_type_master { get; set; }
        public CompanyMasterViewModel company_master { get; set; }
        public List<AssessmentProjectAssessorsDetailViewModel> assessment_project_assessors_detail { get; set; }
    }


    public class AssessmentProjectMasterMobileViewModel
    {
        public int ProjectID { get; set; }
        public string Project_Name { get; set; }
        public string Project_ID { get; set; }
        public string Project_DocNo { get; set; }
        public string Developer_Name { get; set; }
        public string Contractor_Name { get; set; }
        public string Assessment_Dates { get; set; }
        public decimal? ArchitecturalWorksWeightage { get; set; }
        public decimal? MEWorksWeightage { get; set; }
        public decimal? BuildQASScore { get; set; }
        public decimal? MinimumCompliancePercentageThreshold { get; set; }
        public string FieldWindowWTT_Contractor_Name { get; set; }
        public string FieldWindowWTT_Witness_Name { get; set; }
        public string WetAreaWTT_Contractor_Name { get; set; }
        public string WetAreaWTT_Witness_Name { get; set; }
        public AssessmentDevelopmentTypeMasterViewModel  AssessmentDevelopmentTypeMasterMobileView { get; set; }
        public List<AssessmentProjectAssessorsDetailMobileViewModel> AssessmentProjectAssessorsDetailMobileViewModels { get; set; }
    }
}