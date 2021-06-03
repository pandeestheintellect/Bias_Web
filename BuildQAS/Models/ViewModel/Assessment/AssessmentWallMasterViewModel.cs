using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentWallMasterViewModel
    {
        public int AssessmentWallID { get; set; }
        [Required]
        public string AssessmentWallName { get; set; }
        public int? OrderBy { get; set; }
        public int IsActive { get; set; } = 1;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AssessmentWallMasterMobileViewModel
    {
        public int AssessmentWallID { get; set; }
        public string AssessmentWallName { get; set; }
        public int? OrderBy { get; set; }
    }
}