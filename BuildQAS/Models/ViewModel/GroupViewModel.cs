using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class GroupViewModel
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        List<PermissionViewModel> rapid_permission { get; set; }
    }
}