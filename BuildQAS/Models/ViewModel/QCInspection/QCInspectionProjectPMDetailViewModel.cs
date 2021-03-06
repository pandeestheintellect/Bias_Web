using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionProjectPMDetailViewModel
    {
        public int ProjectPMDetailID { get; set; }
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }

        public UserViewModel user { get; set; }
    }

    public class QCInspectionProjectPMDetailMobileViewModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int RowNo { get; set; } = 1;
    }
}