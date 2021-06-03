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
    
    public partial class assessment_type_module_Process_master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public assessment_type_module_Process_master()
        {
            this.assessment_external_wall_trn_detail = new HashSet<assessment_external_wall_trn_detail>();
            this.assessment_external_works_trn_detail = new HashSet<assessment_external_works_trn_detail>();
            this.assessment_internal_finishes_trn_detail = new HashSet<assessment_internal_finishes_trn_detail>();
            this.assessment_roof_construction_trn_detail = new HashSet<assessment_roof_construction_trn_detail>();
            this.assessment_wet_area_water_tightness_test_tran_detail = new HashSet<assessment_wet_area_water_tightness_test_tran_detail>();
            this.assessment_wet_area_water_tightness_test_tran_detail_result = new HashSet<assessment_wet_area_water_tightness_test_tran_detail_result>();
        }
    
        public int AssessmentTypeModuleProcessID { get; set; }
        public Nullable<int> AssessmentTypeModuleID { get; set; }
        public Nullable<int> AssessmentTypeLocationID { get; set; }
        public string AssessmentTypeModuleProcessName { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<assessment_external_wall_trn_detail> assessment_external_wall_trn_detail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<assessment_external_works_trn_detail> assessment_external_works_trn_detail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<assessment_internal_finishes_trn_detail> assessment_internal_finishes_trn_detail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<assessment_roof_construction_trn_detail> assessment_roof_construction_trn_detail { get; set; }
        public virtual assessment_type_location_master assessment_type_location_master { get; set; }
        public virtual assessment_type_module_master assessment_type_module_master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<assessment_wet_area_water_tightness_test_tran_detail> assessment_wet_area_water_tightness_test_tran_detail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<assessment_wet_area_water_tightness_test_tran_detail_result> assessment_wet_area_water_tightness_test_tran_detail_result { get; set; }
    }
}
