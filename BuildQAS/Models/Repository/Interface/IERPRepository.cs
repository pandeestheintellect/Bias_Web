using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Repository.Imp
{
    public interface IERPRepository
    {
        List<ProjectMasterViewModel> GetAllProjects();
        ProjectMasterViewModel GetProject(int id);
        List<DashboardSummaryViewModel> GetDashboardSummary(int userid, int groupid, int companyid, DateTime startdt, DateTime enddt);
    }
}