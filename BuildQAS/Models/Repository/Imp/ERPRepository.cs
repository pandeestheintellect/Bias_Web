using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuildInspect.Models.Domain;
using AutoMapper;
using System.Data.Entity;
using BuildInspect.Models.Utility;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Globalization;

namespace BuildInspect.Models.Repository.Interface
{
  
    public class ERPRepository : IERPRepository
    {
        BuildInspectEntities BInDB = new BuildInspectEntities();
        Logger logger = LogManager.GetCurrentClassLogger();

        public ProjectMasterViewModel GetProject(int id)
        {
            var res = BInDB.project_master.Find(id);
            var prj = Mapper.Map<ProjectMasterViewModel>(res);
            return prj;
        }

        public List<ProjectMasterViewModel> GetAllProjects()
        {
            var res = BInDB.project_master.ToList();
            var lists = Mapper.Map<List<ProjectMasterViewModel>>(res);
            return lists;
        }

        public List<DashboardSummaryViewModel> GetDashboardSummary(int userid, int groupid, int companyid, DateTime startdt, DateTime enddt)
        {
            var dCurrentDayofThisMonth = DateTime.Today.ToString("yyyy-MM-dd");
            var dFirstDayOfCurrMonth = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
            var dFirstDayOfThisMonth = startdt.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = enddt.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            var sql = "exec GetDashboardSummary " + userid + ", "+groupid+ ", " + companyid+ ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "'";

            var obj = BInDB.Database.SqlQuery<DashboardSummaryViewModel>(sql).ToList();
            return obj;
        }

        string getFileExtention(string filename)
        {
            var result = filename.Split(new string[] { }, StringSplitOptions.None);
            return result[result.Length - 1];
        }

    }
}

