using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionBlockMasterViewModel
    {
        public int BlockID { get; set; }
        public int ProjectID { get; set; }
        [Required]
        public string BlockName { get; set; }
        public int OrderBy { get; set; } = 1;
        public int IsActive { get; set; } = 1;
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        //public QCInspectionProjectMasterViewModel qcinspection_project_master { get; set; }
        [NotMapped]
        public List<QCInspectionLevelMasterViewModel> qcinspection_level_master { get; set; }

    }
}