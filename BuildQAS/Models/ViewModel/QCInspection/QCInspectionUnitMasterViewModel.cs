using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionUnitMasterViewModel
    {
        public int UnitID { get; set; }
        public int LevelID { get; set; }
        [Required]
        public string UnitName { get; set; }
        public int OrderBy { get; set; } = 1;
        public int IsActive { get; set; } = 1;
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public QCInspectionLevelMasterViewModel qcinspection_level_master { get; set; }

        [NotMapped]
        public List<SelectListItem> LevelList { get; set; }
    }

    public class QCInspectionLocationMobileViewModel
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public int OrderBy { get; set; } = 1;
    }
}