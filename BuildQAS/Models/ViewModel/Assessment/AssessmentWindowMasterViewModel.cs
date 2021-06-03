using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentWindowMasterViewModel
    {
        public int AssessmentWindowID { get; set; }
        public string AssessmentWindowName { get; set; }
        public int? OrderBy { get; set; }
        public int IsActive { get; set; } = 1;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AssessmentWindowMasterMobileViewModel
    {
        public int AssessmentWindowID { get; set; }
        public string AssessmentWindowName { get; set; }
        public int? OrderBy { get; set; }
    }
}