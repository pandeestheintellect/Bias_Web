using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Interface
{
    public class ERPServices : IERPServices
    {
        private readonly IERPRepository erpRepository;
        public ERPServices(IERPRepository _erpRepository)
        {
            erpRepository = _erpRepository;
        }

        public List<ProjectMasterViewModel> GetAllProjects()
        {
            return erpRepository.GetAllProjects();
        }
        public ProjectMasterViewModel GetProject(int id)
        {
            return erpRepository.GetProject(id);
        }
        
    }
}