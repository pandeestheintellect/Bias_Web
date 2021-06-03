using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionSubcontractorMasterViewModel
    {
        public int SubcontractorID { get; set; }
        public int CompanyID { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Mob { get; set; }
        public string Email { get; set; }
        public int IsActive { get; set; } = 1;
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        [NotMapped]
        [Display(Name = "User Name")]
        public int SubCon_ID { get; set; } = 0;

        [NotMapped]
        public string User_Name { get; set; }

        [NotMapped]
        public string Trades { get; set; }
        [JsonIgnore]
        public CompanyMasterViewModel company_master { get; set; }
        public List<QCInspectionSubcontractorTradeDetailViewModel> qcinspection_subcontractor_trade_detail { get; set; }
    }
}