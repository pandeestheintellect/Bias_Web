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
    
    public partial class assessment_development_type_master
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public assessment_development_type_master()
        {
            this.assessment_project_master = new HashSet<assessment_project_master>();
            this.assessment_type_module_weightage = new HashSet<assessment_type_module_weightage>();
        }
    
        public int DevelopmentTypeID { get; set; }
        public string DevelopmentTypeName { get; set; }
        public Nullable<decimal> ArchitecturalWorksWeightage { get; set; }
        public Nullable<decimal> MEWorksWeightage { get; set; }
        public Nullable<decimal> BuildQASScore { get; set; }
        public Nullable<decimal> MinimumCompliancePercentageThreshold { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<assessment_project_master> assessment_project_master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<assessment_type_module_weightage> assessment_type_module_weightage { get; set; }
    }
}
