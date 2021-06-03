using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionProgressReportViewModel
    {
        public int ProjectID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string Project_Name { get; set; }
        public string Project_ID { get; set; }
        public System.DateTime StartOn { get; set; }
        public System.DateTime EndOn { get; set; }
        public Nullable<int> Is_Completed { get; set; }
        public int TotalCount { get; set; } = 0;
        public int RequestedCount { get; set; } = 0;
        public int RectifiedCount { get; set; } = 0;
        public int CompletedCount { get; set; } = 0;
        public double TotalPercentage { get; set; } = 0;
        public double RequestedPercentage { get; set; } = 0;
        public double RectifiedPercentage { get; set; } = 0;
        public double CompletedPercentage { get; set; } = 0;

        public List<QCInspectionProgressReportSummaryModel> QCInspectionProgressReportSummaryModels { get; set; }
        public List<QCInspectionProgressReportDetailModel> QCInspectionProgressReportDetailModels { get; set; }
    }

    public class QCInspectionProgressReportSummaryModel
    {
        public string Trade { get; set; }
        public string SubcontractorName { get; set; }
        public int RequestedCount { get; set; } = 0;
        public int RectifiedCount { get; set; } = 0;
        public int CompletedCount { get; set; } = 0;
        public double CompletedPercentage { get; set; } = 0;
    }

    public class QCInspectionProgressReportDetailModel
    {
        public int QCInspectionDefectID { get; set; }
        public string QCInspectionDefectNo { get; set; }
        public System.DateTime? RequestedOn { get; set; }
        public string Trade { get; set; }
        public string SubcontractorName { get; set; }
        public System.DateTime? InspectionOn { get; set; }
        public string Status { get; set; }
    }
}