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
    
    public partial class employee
    {
        public int EmpID { get; set; }
        public string Employee_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoPath { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public Nullable<int> ScheduleID { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DoJ { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> DoR { get; set; }
        public Nullable<int> IsActive { get; set; }
    }
}
