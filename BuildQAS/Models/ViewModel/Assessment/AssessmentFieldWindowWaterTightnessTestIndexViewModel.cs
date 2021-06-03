using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentFieldWindowWaterTightnessTestIndexViewModel
    {
        public int ProjectID { get; set; }
        public int CompanyID { get; set; }
        public string Project_Name { get; set; }
        public string Project_ID { get; set; }
        public string Developer_Name { get; set; }
        public string Contractor_Name { get; set; }
        public string Assessment_Dates { get; set; }
        public string Assessors { get; set; }
        public int NoofCompliances { get; set; }
        public int NoofChecks { get; set; }
        public decimal Percentage { get; set; }
        public decimal Weightage { get; set; }
        public decimal WeightedScore { get; set; }
    }
}