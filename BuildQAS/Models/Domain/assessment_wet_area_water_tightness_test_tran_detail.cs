//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BuildInspect.Models.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class assessment_wet_area_water_tightness_test_tran_detail
    {
        public int AssessmentWAWTTDetailID { get; set; }
        public Nullable<int> AssessmentWAWTTID { get; set; }
        public Nullable<int> AssessmentTypeModuleProcessID { get; set; }
        public string Result { get; set; }
        public int RowNo { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual assessment_type_module_Process_master assessment_type_module_Process_master { get; set; }
        public virtual assessment_wet_area_water_tightness_test_tran assessment_wet_area_water_tightness_test_tran { get; set; }
    }
}
