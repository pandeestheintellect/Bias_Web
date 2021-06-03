using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class UserViewModel
    {

        public int UserID { get; set; }
        [Display(Name = "User ID"), Required, StringLength(80)]
        public string UserName { get; set; }
        [Display(Name = "Password"), Required, DataType(DataType.Password),
        RegularExpression(@"^(?=.*\d)(?=.*[a-zA-Z]).{8,120}$", ErrorMessage = "Password error")]
        public string Password { get; set; }
        [Display(Name = "User Name"), Required, StringLength(80)]
        public string DisplayName { get; set; }
        
        public string Curr_Password { get; set; }

        [Display(Name = "Employee Name")]
        public Nullable<int> EmpID { get; set; }

        [Display(Name = "User Group"), Required]
        public Nullable<int> GroupID { get; set; }
        public string LastLogin { get; set; }
        [Display(Name = "User ID")]
        public string UID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<int> CompanyID { get; set; }
        
        public string Email { get; set; }
        public string Mobile { get; set; }

        public string ServiceType { get; set; }
        public string Zone { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }

        public Nullable<int> TeamLead_ID { get; set; }
        public Nullable<int> Admin_ID { get; set; }
        public Nullable<int> ProjMngr_ID { get; set; }
        public Nullable<int> SubCon_ID { get; set; }
        public Nullable<int> Supvsr_ID { get; set; }
        public Nullable<int> Assessor_ID { get; set; }
        public string DeviceID { get; set; }


        public CompanyMasterViewModel company_master { get; set; }
        public GroupViewModel usergroup { get; set; }

        public HttpPostedFileBase photo_path { get; set; }


    }

    public class MobileUserViewModel
    {

        public int UserID { get; set; }
        public int GroupID { get; set; }
        public int CompanyID { get; set; }
        public string ServiceType { get; set; }
        public string DisplayName { get; set; }
        public string Zone { get; set; }
    }

    public class SmartFMUserViewModel
    {

        public int SmartFMUserID { get; set; }
        public int AppGroupID { get; set; }
        public int AppClientID { get; set; }
        public string UserName { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public Nullable<int> IsActive { get; set; }
        public string Password { get; set; }
    }


}
