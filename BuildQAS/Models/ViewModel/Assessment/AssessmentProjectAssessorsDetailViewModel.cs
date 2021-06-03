using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentProjectAssessorsDetailViewModel
    {
        public int ProjectAssessorsDetailID { get; set; }
        public int? ProjectID { get; set; }
        public int? AssessorsID { get; set; }
        public int RowNo { get; set; } = 1;
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public AssessorsMasterViewModel assessors_master { get; set; }
    }

    public class AssessmentProjectAssessorsDetailMobileViewModel
    {
        public int? AssessorsID { get; set; }
        public string AssessorsName { get; set; }
        public int RowNo { get; set; } = 1;
    }
}