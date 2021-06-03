using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel
    {
        public int AssessmentWAWTTDetailResultID { get; set; }
        public int? AssessmentWAWTTID { get; set; }
        public int? AssessmentTypeModuleProcessID { get; set; }
        public int? AssessmentWAWTTResultID { get; set; }
        public string Result { get; set; } = "";
        public int RowNo { get; set; } = 1;
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

    }

    public class AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModel
    {
        public int AssessmentWAWTTDetailResultID { get; set; }
        public int? AssessmentTypeModuleProcessID { get; set; }
        public int? AssessmentWAWTTResultID { get; set; }
        public int Result { get; set; } = 3;
    }
}