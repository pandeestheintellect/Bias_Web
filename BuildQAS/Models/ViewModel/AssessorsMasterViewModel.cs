using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessorsMasterViewModel
    {
        public int AssessorsID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        [Required]
        public string AssessorsName { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int IsActive { get; set; } = 1;

        public CompanyMasterViewModel company_master { get; set; }

        [NotMapped]
        [Display(Name = "User Name")]
        public int Assessor_ID { get; set; } = 0;

        [NotMapped]
        public string User_Name { get; set; }
    }


    public class AssessorsMasterMobileViewModel
    {
        public int AssessorsID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string AssessorsName { get; set; }
    }
}