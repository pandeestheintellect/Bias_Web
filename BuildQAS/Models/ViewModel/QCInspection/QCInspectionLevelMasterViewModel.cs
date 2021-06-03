using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionLevelMasterViewModel
    {

        public int LevelID { get; set; }
        public int BlockID { get; set; }
        [Required]
        public string LevelName { get; set; }
        public int OrderBy { get; set; } = 1;
        public int IsActive { get; set; } = 1;
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        // public QCInspectionBlockMasterViewModel qcinspection_block_master { get; set; }

        [NotMapped]
        public string BlockName { get; set; }

        [NotMapped]
        public List<SelectListItem> BlockList { get; set; }
    }
}