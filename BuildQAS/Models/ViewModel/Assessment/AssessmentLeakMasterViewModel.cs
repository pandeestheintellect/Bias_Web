using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentLeakMasterViewModel
    {
        public int AssessmentLeakID { get; set; }
        public string AssessmentLeakName { get; set; }
        public int? OrderBy { get; set; }
        public int IsActive { get; set; } = 1;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AssessmentLeakMasterMobileViewModel
    {
        public int AssessmentLeakID { get; set; }
        public string AssessmentLeakName { get; set; }
        public int? OrderBy { get; set; }
    }
}