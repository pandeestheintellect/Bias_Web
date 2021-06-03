using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentWetAreaWaterTightnessTestResultMasterViewModel
    {
        public int AssessmentWAWTTResultID { get; set; }
        public string AssessmentWAWTTResult { get; set; }
        public int? OrderBy { get; set; }
        public int IsActive { get; set; } = 1;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AssessmentWetAreaWaterTightnessTestResultMasterMobileViewModel
    {
        public int AssessmentWAWTTResultID { get; set; }
        public string AssessmentWAWTTResult { get; set; }
        public int? OrderBy { get; set; }
    }
}