using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentJointMasterViewModel
    {
        public int AssessmentJointID { get; set; }
        public string AssessmentJointName { get; set; }
        public int? OrderBy { get; set; }
        public int IsActive { get; set; } = 1;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AssessmentJointMasterMobileViewModel
    {
        public int AssessmentJointID { get; set; }
        public string AssessmentJointName { get; set; }
        public int? OrderBy { get; set; }
    }
}