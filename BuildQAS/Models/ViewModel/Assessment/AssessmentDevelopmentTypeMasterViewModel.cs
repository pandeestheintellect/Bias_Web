using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentDevelopmentTypeMasterViewModel
    {
        public int DevelopmentTypeID { get; set; }
        public string DevelopmentTypeName { get; set; }
        public decimal? ArchitecturalWorksWeightage { get; set; }
        public decimal? MEWorksWeightage { get; set; }
        public decimal? BuildQASScore { get; set; }
        public decimal? MinimumCompliancePercentageThreshold { get; set; }
    }
    
}