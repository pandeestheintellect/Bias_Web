using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class FilterViewModel
    {

       
        public string Month { get; set; }
        public string Year { get; set; }
        public int userid { get; set; }
        public string Zone { get; set; }

        public int SchoolID { get; set; }

        public List<ChecklistSummaryMatrixViewModel> cls { get; set; }
        public List<ChecklistFileViewModel> clfdownload { get; set; }
    }
}