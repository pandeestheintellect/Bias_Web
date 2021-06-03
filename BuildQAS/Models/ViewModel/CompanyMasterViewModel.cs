using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class CompanyMasterViewModel
    {

        public int CompanyID { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Required]
        [Display(Name = "SHORT NAME")]
        public string ShortName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        [Display(Name = "PIN CODE")]
        public string Pincode { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Mob { get; set; }
        public string Email { get; set; }
        [Display(Name = "REGISTRATION NO")]
        public string RegNo { get; set; }
        [Display(Name = "GST REGISTRATION NO")]
        public string GstRegNo { get; set; }
        public string LogoPath { get; set; }
        public Nullable<decimal> GST { get; set; }
        [Display(Name = "Company Logo")]
        public HttpPostedFileBase profile_Path { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> IsActive { get; set; }
    }
}