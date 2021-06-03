using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Imp
{
    public interface IERPServices
    {

        List<ProjectMasterViewModel> GetAllProjects();
        ProjectMasterViewModel GetProject(int id);
    }
}