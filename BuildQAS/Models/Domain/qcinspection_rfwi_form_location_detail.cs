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
    
    public partial class qcinspection_rfwi_form_location_detail
    {
        public int RFWIFormLocationDetailID { get; set; }
        public int QCInspectionRFWIFormID { get; set; }
        public int UnitID { get; set; }
        public Nullable<int> QCInspectionDrawingReferenceFileID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        public virtual qcinspection_project_rfwi_drawing_reference_files qcinspection_project_rfwi_drawing_reference_files { get; set; }
        public virtual qcinspection_unit_master qcinspection_unit_master { get; set; }
        public virtual qcinspection_rfwi_form qcinspection_rfwi_form { get; set; }
    }
}
