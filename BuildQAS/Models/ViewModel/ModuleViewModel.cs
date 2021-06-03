using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
namespace BuildInspect.Models.ViewModel
{
  

    public class ModuleViewModel
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public int order_by { get; set; }
        public List<ScreenViewModel> rapid_screens { get; set; }

    }
}