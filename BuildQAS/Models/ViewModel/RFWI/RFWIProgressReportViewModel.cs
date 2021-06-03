using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class RFWIProgressReportViewModel
    {
        public int ProjectID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string Project_Name { get; set; }
        public string Project_ID { get; set; }
        public System.DateTime StartOn { get; set; }
        public System.DateTime EndOn { get; set; }
        public Nullable<int> Is_Completed { get; set; }
        public int PendingCount { get; set; } = 0;
        public int RequestedCount { get; set; } = 0;
        public int RejectedCount { get; set; } = 0;
        public int CompletedCount { get; set; } = 0;
        public double PendingPercentage { get; set; } = 0;
        public double RequestedPercentage { get; set; } = 0;
        public double RejectedPercentage { get; set; } = 0;
        public double CompletedPercentage { get; set; } = 0;

        public List<RFWIProgressReportDetailModel> RFWIProgressReportDetailModels { get; set; }
    }

    public class RFWIProgressReportDetailModel
    {
        public int QCInspectionRFWIFormID { get; set; }
        public string RFWINo { get; set; }
        public System.DateTime? RequestedOn { get; set; }
        public string RequestedBy { get; set; }
        public string Trade { get; set; }
        public string InspectorName { get; set; }
        public System.DateTime? InspectionOn { get; set; }
        public string Status { get; set; }
    }
}