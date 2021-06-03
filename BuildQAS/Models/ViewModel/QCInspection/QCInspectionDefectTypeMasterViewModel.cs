using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionDefectTypeMasterViewModel
    {
        public int DefectTypeID { get; set; }
        [Required]
        public string DefectName { get; set; }
        public int OrderBy { get; set; } = 1;
        public int IsActive { get; set; } = 1;
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}