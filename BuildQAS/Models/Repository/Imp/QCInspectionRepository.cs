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
    public class QCInspectionRepository : IQCInspectionRepository
    {
        BuildInspectEntities BInDB = new BuildInspectEntities();
        Logger logger = LogManager.GetCurrentClassLogger();

        // Project
        //public List<QCInspectionProjectMasterViewModel> GetAllProjects(bool IncludeChilds = true)
        //{
        //    if(IncludeChilds)
        //    {
        //        var projects = BInDB.qcinspection_project_master.AsNoTracking().ToList();
        //        return Mapper.Map<List<QCInspectionProjectMasterViewModel>>(projects);
        //    }
        //    else
        //    {
        //        var projects = BInDB.qcinspection_project_master.AsNoTracking().ToList();
        //        return Mapper.Map<List<QCInspectionProjectMasterViewModel>>(projects);
        //    }
        //}

        public List<QCInspectionProjectMasterViewModel> GetAllProjects()
        {
            var projects = BInDB.qcinspection_project_master.AsNoTracking().ToList();
            return Mapper.Map<List<QCInspectionProjectMasterViewModel>>(projects);
        }

        public List<QCInspectionProjectMasterViewModel> GetAllProjectsBasedCompanyID(int? id)
        {
            var res = BInDB.qcinspection_project_master.AsNoTracking().Where(a => a.CompanyID == id).ToList();
            var lists = Mapper.Map<List<QCInspectionProjectMasterViewModel>>(res);
            return lists;
        }

        public List<QCInspectionProjectPMDetailViewModel> GetAllPMDetailByProjectID(int Id)
        {
            var res = BInDB.qcinspection_project_PM_detail.AsNoTracking().Where(a => a.ProjectID == Id).ToList();
            var lists = Mapper.Map<List<QCInspectionProjectPMDetailViewModel>>(res);
            return lists;
        }

        public List<QCInspectionProjectSupervisorDetailViewModel> GetAllSupervisorDetailByProjectID(int Id)
        {
            var res = BInDB.qcinspection_project_Supervisor_detail.AsNoTracking().Where(a => a.ProjectID == Id).ToList();
            var lists = Mapper.Map<List<QCInspectionProjectSupervisorDetailViewModel>>(res);
            return lists;
        }

        public List<QCInspectionProjectMEInspectorDetailViewModel> GetAllMEInspectorDetailByProjectID(int Id)
        {
            var res = BInDB.qcinspection_project_MEInspector_detail.AsNoTracking().Where(a => a.ProjectID == Id).ToList();
            var lists = Mapper.Map<List<QCInspectionProjectMEInspectorDetailViewModel>>(res);
            return lists;
        }

        public List<QCInspectionProjectStructureInspectorDetailViewModel> GetAllStructureInspectorDetailByProjectID(int Id)
        {
            var res = BInDB.qcinspection_project_StructureInspector_detail.AsNoTracking().Where(a => a.ProjectID == Id).ToList();
            var lists = Mapper.Map<List<QCInspectionProjectStructureInspectorDetailViewModel>>(res);
            return lists;
        }

        public List<QCInspectionProjectOtherInspectorDetailViewModel> GetAllOtherInspectorDetailByProjectID(int Id)
        {
            var res = BInDB.qcinspection_project_OtherInspector_detail.AsNoTracking().Where(a => a.ProjectID == Id).ToList();
            var lists = Mapper.Map<List<QCInspectionProjectOtherInspectorDetailViewModel>>(res);
            return lists;
        }

        public QCInspectionProjectMasterViewModel GetProject(int PId)
        {
            var project = BInDB.qcinspection_project_master.Find(PId);
            return Mapper.Map<QCInspectionProjectMasterViewModel>(project);
        }

        public int CreateProject(QCInspectionProjectMasterViewModel project, List<QCInspectionProjectPMDetailViewModel> PMViewModels, List<QCInspectionProjectSupervisorDetailViewModel> SupervisorViewModels, List<QCInspectionProjectMEInspectorDetailViewModel> MEInspectorViewModels, List<QCInspectionProjectStructureInspectorDetailViewModel> StructureInspectorViewModels, List<QCInspectionProjectOtherInspectorDetailViewModel> OtherInspectorViewModels)
        {
            try
            {
                var _db_project = Mapper.Map<qcinspection_project_master>(project);
                BInDB.qcinspection_project_master.Add(_db_project);
                foreach (var PMModel in PMViewModels)
                {
                    qcinspection_project_PM_detail PMDetail = new qcinspection_project_PM_detail();
                    PMDetail.ProjectID = _db_project.ProjectID;
                    PMDetail.UserID = PMModel.UserID;
                    PMDetail.CreatedBy = AppSession.GetCurrentUserId();
                    PMDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_PM_detail.Add(PMDetail);
                }
                foreach (var SupModel in SupervisorViewModels)
                {
                    qcinspection_project_Supervisor_detail SupvsorDetail = new qcinspection_project_Supervisor_detail();
                    SupvsorDetail.ProjectID = _db_project.ProjectID;
                    SupvsorDetail.UserID = SupModel.UserID;
                    SupvsorDetail.CreatedBy = AppSession.GetCurrentUserId();
                    SupvsorDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_Supervisor_detail.Add(SupvsorDetail);
                }

                foreach (var MEInspectorModel in MEInspectorViewModels)
                {
                    qcinspection_project_MEInspector_detail MEDetail = new qcinspection_project_MEInspector_detail();
                    MEDetail.ProjectID = _db_project.ProjectID;
                    MEDetail.UserID = MEInspectorModel.UserID;
                    MEDetail.CreatedBy = AppSession.GetCurrentUserId();
                    MEDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_MEInspector_detail.Add(MEDetail);
                }

                foreach (var StructureInspectorModel in StructureInspectorViewModels)
                {
                    qcinspection_project_StructureInspector_detail StructureDetail = new qcinspection_project_StructureInspector_detail();
                    StructureDetail.ProjectID = _db_project.ProjectID;
                    StructureDetail.UserID = StructureInspectorModel.UserID;
                    StructureDetail.CreatedBy = AppSession.GetCurrentUserId();
                    StructureDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_StructureInspector_detail.Add(StructureDetail);
                }

                foreach (var OtherInspectorModel in OtherInspectorViewModels)
                {
                    qcinspection_project_OtherInspector_detail OtherDetail = new qcinspection_project_OtherInspector_detail();
                    OtherDetail.ProjectID = _db_project.ProjectID;
                    OtherDetail.UserID = OtherInspectorModel.UserID;
                    OtherDetail.CreatedBy = AppSession.GetCurrentUserId();
                    OtherDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_OtherInspector_detail.Add(OtherDetail);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Project:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveProject(QCInspectionProjectMasterViewModel project, List<QCInspectionProjectPMDetailViewModel> PMViewModels, List<QCInspectionProjectSupervisorDetailViewModel> SupervisorViewModels, List<QCInspectionProjectMEInspectorDetailViewModel> MEInspectorViewModels, List<QCInspectionProjectStructureInspectorDetailViewModel> StructureInspectorViewModels, List<QCInspectionProjectOtherInspectorDetailViewModel> OtherInspectorViewModels)
        {
            try
            {
                var _db_project = BInDB.qcinspection_project_master.Find(project.ProjectID);
                _db_project.CompanyID = project.CompanyID;
                _db_project.Project_Name = project.Project_Name;
                _db_project.Project_ID = project.Project_ID;
                _db_project.StartOn = project.StartOn;
                _db_project.EndOn = project.EndOn;
                _db_project.Is_Completed = project.Is_Completed;
                _db_project.ProjectManagerID = project.ProjectManagerID;
                _db_project.UpdatedBy = project.UpdatedBy;
                _db_project.UpdatedDate = project.UpdatedDate;

                var _db_res_PM = BInDB.qcinspection_project_PM_detail.Where(a => a.ProjectID == project.ProjectID).ToList();
                foreach (var _dbDet in _db_res_PM)
                {
                    BInDB.qcinspection_project_PM_detail.Remove(_dbDet);
                }

                var _db_res_Supvsor = BInDB.qcinspection_project_Supervisor_detail.Where(a => a.ProjectID == project.ProjectID).ToList();
                foreach (var _dbDet in _db_res_Supvsor)
                {
                    BInDB.qcinspection_project_Supervisor_detail.Remove(_dbDet);
                }

                var _db_res_MEInspectors = BInDB.qcinspection_project_MEInspector_detail.Where(a => a.ProjectID == project.ProjectID).ToList();
                foreach (var _dbDet in _db_res_MEInspectors)
                {
                    BInDB.qcinspection_project_MEInspector_detail.Remove(_dbDet);
                }

                var _db_res_StructureInspectors = BInDB.qcinspection_project_StructureInspector_detail.Where(a => a.ProjectID == project.ProjectID).ToList();
                foreach (var _dbDet in _db_res_StructureInspectors)
                {
                    BInDB.qcinspection_project_StructureInspector_detail.Remove(_dbDet);
                }

                var _db_res_OtherInspectors = BInDB.qcinspection_project_OtherInspector_detail.Where(a => a.ProjectID == project.ProjectID).ToList();
                foreach (var _dbDet in _db_res_OtherInspectors)
                {
                    BInDB.qcinspection_project_OtherInspector_detail.Remove(_dbDet);
                }

                foreach (var PMModel in PMViewModels)
                {
                    qcinspection_project_PM_detail PMDetail = new qcinspection_project_PM_detail();
                    PMDetail.ProjectID = _db_project.ProjectID;
                    PMDetail.UserID = PMModel.UserID;
                    PMDetail.CreatedBy = AppSession.GetCurrentUserId();
                    PMDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_PM_detail.Add(PMDetail);
                }
                foreach (var SupModel in SupervisorViewModels)
                {
                    qcinspection_project_Supervisor_detail SupvsorDetail = new qcinspection_project_Supervisor_detail();
                    SupvsorDetail.ProjectID = _db_project.ProjectID;
                    SupvsorDetail.UserID = SupModel.UserID;
                    SupvsorDetail.CreatedBy = AppSession.GetCurrentUserId();
                    SupvsorDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_Supervisor_detail.Add(SupvsorDetail);
                }

                foreach (var MEInspectorModel in MEInspectorViewModels)
                {
                    qcinspection_project_MEInspector_detail MEDetail = new qcinspection_project_MEInspector_detail();
                    MEDetail.ProjectID = _db_project.ProjectID;
                    MEDetail.UserID = MEInspectorModel.UserID;
                    MEDetail.CreatedBy = AppSession.GetCurrentUserId();
                    MEDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_MEInspector_detail.Add(MEDetail);
                }

                foreach (var StructureInspectorModel in StructureInspectorViewModels)
                {
                    qcinspection_project_StructureInspector_detail StructureDetail = new qcinspection_project_StructureInspector_detail();
                    StructureDetail.ProjectID = _db_project.ProjectID;
                    StructureDetail.UserID = StructureInspectorModel.UserID;
                    StructureDetail.CreatedBy = AppSession.GetCurrentUserId();
                    StructureDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_StructureInspector_detail.Add(StructureDetail);
                }

                foreach (var OtherInspectorModel in OtherInspectorViewModels)
                {
                    qcinspection_project_OtherInspector_detail OtherDetail = new qcinspection_project_OtherInspector_detail();
                    OtherDetail.ProjectID = _db_project.ProjectID;
                    OtherDetail.UserID = OtherInspectorModel.UserID;
                    OtherDetail.CreatedBy = AppSession.GetCurrentUserId();
                    OtherDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_project_OtherInspector_detail.Add(OtherDetail);
                }

                BInDB.Entry(_db_project).State = EntityState.Modified;
                return BInDB.SaveChanges(); 
            }
            catch (Exception ex)
            {
                logger.Debug("Save Project:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteProject(int pID)
        {
            try
            {
                var _db_res_PM = BInDB.qcinspection_project_PM_detail.Where(a => a.ProjectID == pID).ToList();
                foreach (var _dbDet in _db_res_PM)
                {
                    BInDB.qcinspection_project_PM_detail.Remove(_dbDet);
                }

                var _db_res_Supvsor = BInDB.qcinspection_project_Supervisor_detail.Where(a => a.ProjectID == pID).ToList();
                foreach (var _dbDet in _db_res_Supvsor)
                {
                    BInDB.qcinspection_project_Supervisor_detail.Remove(_dbDet);
                }

                var _db_res_MEInspectors = BInDB.qcinspection_project_MEInspector_detail.Where(a => a.ProjectID == pID).ToList();
                foreach (var _dbDet in _db_res_MEInspectors)
                {
                    BInDB.qcinspection_project_MEInspector_detail.Remove(_dbDet);
                }

                var _db_res_StructureInspectors = BInDB.qcinspection_project_StructureInspector_detail.Where(a => a.ProjectID == pID).ToList();
                foreach (var _dbDet in _db_res_StructureInspectors)
                {
                    BInDB.qcinspection_project_StructureInspector_detail.Remove(_dbDet);
                }

                var _db_res_OtherInspectors = BInDB.qcinspection_project_OtherInspector_detail.Where(a => a.ProjectID == pID).ToList();
                foreach (var _dbDet in _db_res_OtherInspectors)
                {
                    BInDB.qcinspection_project_OtherInspector_detail.Remove(_dbDet);
                }

                var _db_res = BInDB.qcinspection_project_master.First(a => a.ProjectID == pID);
                BInDB.qcinspection_project_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Project:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public bool CheckProject(int ProjectID, string projectname)
        {
            try
            {
                var user = BInDB.qcinspection_project_master.Where(a => a.ProjectID != ProjectID && a.Project_Name == projectname).SingleOrDefault();
                if (user == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public int CompletedProject(int pid)
        {
            try
            {
                var _db_res = BInDB.qcinspection_project_master.First(a => a.ProjectID == pid);
                _db_res.Is_Completed = 1;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Completed Project:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }
        // Project

        // Masters

        //Subcontractor
        public List<QCInspectionSubcontractorMasterViewModel> GetAllSubcontractors()
        {
            var users = BInDB.qcinspection_subcontractor_master.ToList();
            var lstUserView = Mapper.Map<List<QCInspectionSubcontractorMasterViewModel>>(users);
            return lstUserView;
        }

        public List<QCInspectionSubcontractorMasterViewModel> GetAllSubcontractorsBasedCompanyID(int Id)
        {
            var res = BInDB.qcinspection_subcontractor_master.AsNoTracking().Where(a => a.CompanyID == Id).ToList();
            var lists = Mapper.Map<List<QCInspectionSubcontractorMasterViewModel>>(res);
            return lists;
        }

        public QCInspectionSubcontractorMasterViewModel GetSubcontractor(int Id)
        {
            var project = BInDB.qcinspection_subcontractor_master.Find(Id);
            return Mapper.Map<QCInspectionSubcontractorMasterViewModel>(project);
        }

        public int CreateSubcontractor(QCInspectionSubcontractorMasterViewModel subcontractor, List<QCInspectionSubcontractorTradeDetailViewModel> detailViewModels)
        {
            try
            {
                var _db_project = new qcinspection_subcontractor_master();
                _db_project.CompanyID = subcontractor.CompanyID;
                _db_project.Name = subcontractor.Name;
                _db_project.ShortName = subcontractor.ShortName;
                _db_project.Address = subcontractor.Address;
                _db_project.Country = subcontractor.Country;
                _db_project.Pincode = subcontractor.Pincode;
                _db_project.Tel = subcontractor.Tel;
                _db_project.Fax = subcontractor.Fax;
                _db_project.Mob = subcontractor.Mob;
                _db_project.Email = subcontractor.Email;
                _db_project.IsActive = subcontractor.IsActive;
                _db_project.CreatedBy = int.Parse(subcontractor.CreatedBy.ToString());
                _db_project.CreatedDate = DateTime.Now;
                foreach (var desc in detailViewModels)
                {
                    qcinspection_subcontractor_trade_detail detail = Mapper.Map<qcinspection_subcontractor_trade_detail>(desc);
                    detail.SubcontractorID = _db_project.SubcontractorID;
                    _db_project.qcinspection_subcontractor_trade_detail.Add(detail);
                }
                BInDB.qcinspection_subcontractor_master.Add(_db_project);
                BInDB.SaveChanges();
                return _db_project.SubcontractorID;
            }
            catch (Exception ex)
            {
                logger.Debug("Create Subcontractor:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveSubcontractor(QCInspectionSubcontractorMasterViewModel subcontractor, List<QCInspectionSubcontractorTradeDetailViewModel> detailViewModels)
        {
            try
            {
                var _db_project = BInDB.qcinspection_subcontractor_master.Find(subcontractor.SubcontractorID);
                _db_project.CompanyID = subcontractor.CompanyID;
                _db_project.Name = subcontractor.Name;
                _db_project.ShortName = subcontractor.ShortName;
                _db_project.Address = subcontractor.Address;
                _db_project.Country = subcontractor.Country;
                _db_project.Pincode = subcontractor.Pincode;
                _db_project.Tel = subcontractor.Tel;
                _db_project.Fax = subcontractor.Fax;
                _db_project.Mob = subcontractor.Mob;
                _db_project.Email = subcontractor.Email;
                _db_project.IsActive = subcontractor.IsActive;
                _db_project.UpdatedBy = subcontractor.UpdatedBy;
                _db_project.UpdatedDate = subcontractor.UpdatedDate;
                BInDB.Entry(_db_project).State = EntityState.Modified;

                var _db_res = BInDB.qcinspection_subcontractor_trade_detail.Where(a => a.SubcontractorID == subcontractor.SubcontractorID).ToList();
                foreach (var _dbDet in _db_res)
                {
                    BInDB.qcinspection_subcontractor_trade_detail.Remove(_dbDet);
                }
                foreach (var desc in detailViewModels)
                {
                    qcinspection_subcontractor_trade_detail detail = Mapper.Map<qcinspection_subcontractor_trade_detail>(desc);
                    detail.SubcontractorID = subcontractor.SubcontractorID;
                    BInDB.qcinspection_subcontractor_trade_detail.Add(detail);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Subcontractor:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteSubcontractor(int Id)
        {
            try
            {
                var _db_res1 = BInDB.qcinspection_subcontractor_trade_detail.Where(a => a.SubcontractorID == Id).ToList();
                foreach (var _dbDet in _db_res1)
                {
                    BInDB.qcinspection_subcontractor_trade_detail.Remove(_dbDet);
                }
                var _db_res = BInDB.qcinspection_subcontractor_master.First(a => a.SubcontractorID == Id);
                BInDB.qcinspection_subcontractor_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Subcontractor:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public bool CheckSubcontractor(int CompanyID, int SubcontractorID, string SubcontractorName)
        {
            try
            {
                var user = BInDB.qcinspection_subcontractor_master.Where(a => a.CompanyID == CompanyID && a.SubcontractorID != SubcontractorID && a.Name == SubcontractorName).SingleOrDefault();
                if (user == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Subcontractor

        //Block
        public QCInspectionBlockMasterViewModel GetBlock(int Id)
        {
            var Module = BInDB.qcinspection_block_master.Find(Id);
            return Mapper.Map<QCInspectionBlockMasterViewModel>(Module);
        }

        public int CreateBlock(QCInspectionBlockMasterViewModel Block)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_block_master>(Block);
                _db_res.IsActive = 1;
                BInDB.qcinspection_block_master.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Block:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveBlock(QCInspectionBlockMasterViewModel Block)
        {
            try
            {
                var _db_res = BInDB.qcinspection_block_master.First(a => a.BlockID == Block.BlockID);
                _db_res.BlockName = Block.BlockName;
                _db_res.OrderBy = Block.OrderBy;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Debug("Save Block:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteBlock(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_block_master.First(a => a.BlockID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_block_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Block:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionBlockMasterViewModel> GetAllBlocks()
        {
            var res = BInDB.qcinspection_block_master.Where(x => x.IsActive == 1).OrderBy(a => a.ProjectID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionBlockMasterViewModel>>(res);
            return lists;
        }

        public bool CheckBlock(int ProjectID, int BlockID, string BlockName)
        {
            try
            {
                var Module = BInDB.qcinspection_block_master.Where(a => a.ProjectID == ProjectID && a.BlockID != BlockID && a.BlockName == BlockName).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Block

        //Level
        public QCInspectionLevelMasterViewModel GetLevel(int Id)
        {
            var Module = BInDB.qcinspection_level_master.Find(Id);
            return Mapper.Map<QCInspectionLevelMasterViewModel>(Module);
        }

        public int CreateLevel(QCInspectionLevelMasterViewModel Level)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_level_master>(Level);
                _db_res.IsActive = 1;
                BInDB.qcinspection_level_master.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Level:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveLevel(QCInspectionLevelMasterViewModel Level)
        {
            try
            {
                var _db_res = BInDB.qcinspection_level_master.First(a => a.LevelID == Level.LevelID);
                _db_res.BlockID = Level.BlockID;
                _db_res.LevelName = Level.LevelName;
                _db_res.OrderBy = Level.OrderBy;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Debug("Save Level:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteLevel(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_level_master.First(a => a.LevelID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_level_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Level:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionLevelMasterViewModel> GetAllLevels()
        {
            var res = BInDB.qcinspection_level_master.Where(x => x.IsActive == 1).OrderBy(a => a.BlockID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionLevelMasterViewModel>>(res);
            return lists;
        }

        public bool CheckLevel(int BlockID, int LevelID, string LevelName)
        {
            try
            {
                var Module = BInDB.qcinspection_level_master.Where(a => a.BlockID == BlockID && a.LevelID != LevelID && a.LevelName == LevelName).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Level

        //Unit
        public QCInspectionUnitMasterViewModel GetUnit(int Id)
        {
            var Module = BInDB.qcinspection_unit_master.Find(Id);
            return Mapper.Map<QCInspectionUnitMasterViewModel>(Module);
        }

        public int CreateUnit(QCInspectionUnitMasterViewModel Unit)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_unit_master>(Unit);
                _db_res.IsActive = 1;
                BInDB.qcinspection_unit_master.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Unit:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveUnit(QCInspectionUnitMasterViewModel Unit)
        {
            try
            {
                var _db_res = BInDB.qcinspection_unit_master.First(a => a.UnitID == Unit.UnitID);
                _db_res.LevelID = Unit.LevelID;
                _db_res.UnitName = Unit.UnitName;
                _db_res.OrderBy = Unit.OrderBy;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Unit:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteUnit(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_unit_master.First(a => a.UnitID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_unit_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Unit:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionUnitMasterViewModel> GetAllUnits()
        {
            var res = BInDB.qcinspection_unit_master.Where(x => x.IsActive == 1).OrderBy(a => a.LevelID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionUnitMasterViewModel>>(res);
            return lists;
        }

        public bool CheckUnit(int LevelID, int UnitID, string UnitName)
        {
            try
            {
                var Module = BInDB.qcinspection_unit_master.Where(a => a.LevelID == LevelID && a.UnitID != UnitID && a.UnitName == UnitName).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Unit

        //RFWI Drawings Reference File
        public QCInspectionProjectRFWIDrawingReferenceFilesViewModel GetRFWIDrawingsReferenceFile(int Id)
        {
            var Module = BInDB.qcinspection_project_rfwi_drawing_reference_files.Find(Id);
            return Mapper.Map<QCInspectionProjectRFWIDrawingReferenceFilesViewModel>(Module);
        }

        public int CreateRFWIDrawingsReferenceFile(QCInspectionProjectRFWIDrawingReferenceFilesViewModel RFWIDrawingsReferenceFile)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_project_rfwi_drawing_reference_files>(RFWIDrawingsReferenceFile);
                BInDB.qcinspection_project_rfwi_drawing_reference_files.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create RFWI Drawings Reference File:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteRFWIDrawingsReferenceFile(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_project_rfwi_drawing_reference_files.First(a => a.QCInspectionDrawingReferenceFileID == Id);
                BInDB.qcinspection_project_rfwi_drawing_reference_files.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete RFWI Drawings Reference File:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionProjectRFWIDrawingReferenceFilesViewModel> GetAllRFWIDrawingsReferenceFiles(int Id)
        {
            var res = BInDB.qcinspection_project_rfwi_drawing_reference_files.Where(x => x.ProjectID == Id).OrderBy(a => a.QCInspectionDrawingReferenceFileID).ToList();
            var lists = Mapper.Map<List<QCInspectionProjectRFWIDrawingReferenceFilesViewModel>>(res);
            return lists;
        }

        public bool CheckRFWIDrawingsReferenceFile(int ProjectID, string FileCaption)
        {
            try
            {
                var Module = BInDB.qcinspection_project_rfwi_drawing_reference_files.Where(a => a.ProjectID == ProjectID && a.FileCaption == FileCaption).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //RFWI Drawings Reference File

        //Project File
        public QCInspectionProjectFilesViewModel GetProjectFile(int Id)
        {
            var Module = BInDB.qcinspection_project_files.Find(Id);
            return Mapper.Map<QCInspectionProjectFilesViewModel>(Module);
        }

        public int CreateProjectFile(QCInspectionProjectFilesViewModel ProjectFile)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_project_files>(ProjectFile);
                BInDB.qcinspection_project_files.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Project File:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteProjectFile(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_project_files.First(a => a.QCInspectionProjectFileID == Id);
                BInDB.qcinspection_project_files.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Project File:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionProjectFilesViewModel> GetAllProjectFiles(int Id)
        {
            var res = BInDB.qcinspection_project_files.Where(x => x.ProjectID == Id).OrderBy(a => a.QCInspectionProjectFileID).ToList();
            var lists = Mapper.Map<List<QCInspectionProjectFilesViewModel>>(res);
            return lists;
        }

        public bool CheckProjectFile(int ProjectID, string FileCaption)
        {
            try
            {
                var Module = BInDB.qcinspection_project_files.Where(a => a.ProjectID == ProjectID && a.FileCaption == FileCaption).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Project File


        //Defect Type
        public QCInspectionDefectTypeMasterViewModel GetDefectType(int Id)
        {
            var Module = BInDB.qcinspection_defect_type_master.Find(Id);
            return Mapper.Map<QCInspectionDefectTypeMasterViewModel>(Module);
        }

        public int CreateDefectType(QCInspectionDefectTypeMasterViewModel DefectType)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_defect_type_master>(DefectType);
                _db_res.IsActive = 1;
                BInDB.qcinspection_defect_type_master.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Defect Type:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveDefectType(QCInspectionDefectTypeMasterViewModel DefectType)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_type_master.First(a => a.DefectTypeID == DefectType.DefectTypeID);
                _db_res.DefectName = DefectType.DefectName;
                _db_res.OrderBy = DefectType.OrderBy;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Defect Type:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteDefectType(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_type_master.First(a => a.DefectTypeID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_defect_type_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Defect Type:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionDefectTypeMasterViewModel> GetAllDefectTypes()
        {
            var res = BInDB.qcinspection_defect_type_master.Where(x => x.IsActive == 1).OrderBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionDefectTypeMasterViewModel>>(res);
            return lists;
        }

        public bool CheckDefectType(int DefectTypeID, string DefectTypeName)
        {
            try
            {
                var Module = BInDB.qcinspection_defect_type_master.Where(a => a.DefectTypeID != DefectTypeID && a.DefectName == DefectTypeName).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Defect Type

        //Trade
        public QCInspectionTradeMasterViewModel GetTrade(int Id)
        {
            var Module = BInDB.qcinspection_trade_master.Find(Id);
            return Mapper.Map<QCInspectionTradeMasterViewModel>(Module);
        }

        public int CreateTrade(QCInspectionTradeMasterViewModel Trade)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_trade_master>(Trade);
                _db_res.IsActive = 1;
                BInDB.qcinspection_trade_master.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Trade:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveTrade(QCInspectionTradeMasterViewModel Trade)
        {
            try
            {
                var _db_res = BInDB.qcinspection_trade_master.First(a => a.TradeID == Trade.TradeID);
                _db_res.TradeName = Trade.TradeName;
                _db_res.OrderBy = Trade.OrderBy;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Trade:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteTrade(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_trade_master.First(a => a.TradeID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_trade_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Trade:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionTradeMasterViewModel> GetAllTrades()
        {
            var res = BInDB.qcinspection_trade_master.Where(x => x.IsActive == 1).OrderBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionTradeMasterViewModel>>(res);
            return lists;
        }

        public bool CheckTrade(int TradeID, string TradeName)
        {
            try
            {
                if(TradeID == 0)
                {
                    var Module = BInDB.qcinspection_trade_master.Where(a => a.TradeName == TradeName).SingleOrDefault();
                    if (Module == null)
                        return false;
                    else
                        return true;
                }
                else
                {
                    var Module = BInDB.qcinspection_trade_master.Where(a => a.TradeID != TradeID && a.TradeName == TradeName).SingleOrDefault();
                    if (Module == null)
                        return false;
                    else
                        return true;
                }
            }
            catch
            {
                return false;
            }
        }
        //Trade

        //RFWI General CheckList
        public QCInspectionRFWIGeneralCheckListMasterViewModel GetRFWIGeneralCheckList(int Id)
        {
            var Module = BInDB.qcinspection_rfwi_general_checklist_master.Find(Id);
            return Mapper.Map<QCInspectionRFWIGeneralCheckListMasterViewModel>(Module);
        }

        public int CreateRFWIGeneralCheckList(QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckList)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_rfwi_general_checklist_master>(RFWIGeneralCheckList);
                _db_res.IsActive = 1;
                BInDB.qcinspection_rfwi_general_checklist_master.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create RFWI General Check List:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveRFWIGeneralCheckList(QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckList)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_general_checklist_master.First(a => a.GeneralCheckListID == RFWIGeneralCheckList.GeneralCheckListID);
                _db_res.GeneralCheckListName = RFWIGeneralCheckList.GeneralCheckListName;
                _db_res.OrderBy = RFWIGeneralCheckList.OrderBy;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save RFWI General Check List:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteRFWIGeneralCheckList(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_general_checklist_master.First(a => a.GeneralCheckListID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_rfwi_general_checklist_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete RFWI General Check List:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionRFWIGeneralCheckListMasterViewModel> GetAllRFWIGeneralCheckLists()
        {
            var res = BInDB.qcinspection_rfwi_general_checklist_master.Where(x => x.IsActive == 1).OrderBy(a => a.GeneralCheckListID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionRFWIGeneralCheckListMasterViewModel>>(res);
            return lists;
        }

        public bool CheckRFWIGeneralCheckList(int GeneralCheckListID, string GeneralCheckListName)
        {
            try
            {
                var Module = BInDB.qcinspection_rfwi_general_checklist_master.Where(a => a.GeneralCheckListID != GeneralCheckListID && a.GeneralCheckListName == GeneralCheckListName).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //RFWI General CheckList

        //RFWI Trade
        public QCInspectionRFWITradeMasterViewModel GetRFWITrade(int Id)
        {
            var Module = BInDB.qcinspection_rfwi_trade_master.Find(Id);
            return Mapper.Map<QCInspectionRFWITradeMasterViewModel>(Module);
        }

        public int CreateRFWITrade(QCInspectionRFWITradeMasterViewModel RFWITrade)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_rfwi_trade_master>(RFWITrade);
                _db_res.IsActive = 1;
                BInDB.qcinspection_rfwi_trade_master.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create RFWI Trade:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveRFWITrade(QCInspectionRFWITradeMasterViewModel RFWITrade)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_trade_master.First(a => a.TradeID == RFWITrade.TradeID);
                _db_res.TradeName = RFWITrade.TradeName;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save RFWI General Check List:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteRFWITrade(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_trade_master.First(a => a.TradeID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_rfwi_trade_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete RFWI Trade:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionRFWITradeMasterViewModel> GetAllRFWITrades()
        {
            var res = BInDB.qcinspection_rfwi_trade_master.Where(x => x.IsActive == 1).OrderBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionRFWITradeMasterViewModel>>(res);
            return lists;
        }

        public bool CheckRFWITrade(int TradeID, string RFWITradeName)
        {
            try
            {
                var Module = BInDB.qcinspection_rfwi_trade_master.Where(a => a.TradeID != TradeID && a.TradeName == RFWITradeName).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //RFWI Trade

        //RFWI Trade Item
        public QCInspectionRFWITradeItemDetailViewModel GetRFWITradeItem(int Id)
        {
            var Module = BInDB.qcinspection_rfwi_trade_item_detail.Find(Id);
            return Mapper.Map<QCInspectionRFWITradeItemDetailViewModel>(Module);
        }

        public int CreateRFWITradeItem(QCInspectionRFWITradeItemDetailViewModel RFWITradeItem)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_rfwi_trade_item_detail>(RFWITradeItem);
                _db_res.IsActive = 1;
                BInDB.qcinspection_rfwi_trade_item_detail.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create RFWI Trade Item:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveRFWITradeItem(QCInspectionRFWITradeItemDetailViewModel RFWITradeItem)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_rfwi_trade_item_detail>(RFWITradeItem);
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save RFWI Trade Item:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteRFWITradeItem(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_trade_item_detail.First(a => a.TradeItemID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_rfwi_trade_item_detail.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete RFWI Trade Item:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionRFWITradeItemDetailViewModel> GetAllRFWITradeItems()
        {
            var res = BInDB.qcinspection_rfwi_trade_item_detail.Where(x => x.IsActive == 1).OrderBy(a => a.TradeID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionRFWITradeItemDetailViewModel>>(res);
            return lists;
        }

        public bool CheckRFWITradeItem(int TradeID, int TradeItemID, string ItemName)
        {
            try
            {
                var Module = BInDB.qcinspection_rfwi_trade_item_detail.Where(a => a.TradeID == TradeID && a.TradeItemID != TradeItemID && a.ItemName == ItemName).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //RFWI Trade Item

        //RFWI Trade Detailed CheckList
        public QCInspectionRFWITradeDetailedCheckListDetailViewModel GetRFWITradeDetailedCheckList(int Id)
        {
            var Module = BInDB.qcinspection_rfwi_trade_detailed_checklist_detail.Find(Id);
            return Mapper.Map<QCInspectionRFWITradeDetailedCheckListDetailViewModel>(Module);
        }

        public int CreateRFWITradeDetailedCheckList(QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeItem)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_rfwi_trade_detailed_checklist_detail>(RFWITradeItem);
                _db_res.IsActive = 1;
                BInDB.qcinspection_rfwi_trade_detailed_checklist_detail.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create RFWI Trade Detailed Check List:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveRFWITradeDetailedCheckList(QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeItem)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_rfwi_trade_detailed_checklist_detail>(RFWITradeItem);
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save RFWI Trade Detailed Check List:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteRFWITradeDetailedCheckList(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_trade_detailed_checklist_detail.First(a => a.TradeDetailedCheckListID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.qcinspection_rfwi_trade_detailed_checklist_detail.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete RFWI Trade Item:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<QCInspectionRFWITradeDetailedCheckListDetailViewModel> GetAllRFWITradeDetailedCheckLists()
        {
            var res = BInDB.qcinspection_rfwi_trade_detailed_checklist_detail.Where(x => x.IsActive == 1).OrderBy(a => a.TradeID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<QCInspectionRFWITradeDetailedCheckListDetailViewModel>>(res);
            return lists;
        }

        public bool CheckRFWITradeDetailedCheckList(int TradeID, int TradeDetailedCheckListID, string DetailedCheckListName)
        {
            try
            {
                var Module = BInDB.qcinspection_rfwi_trade_detailed_checklist_detail.Where(a => a.TradeID == TradeID && a.TradeDetailedCheckListID != TradeDetailedCheckListID && a.DetailedCheckListName == DetailedCheckListName).SingleOrDefault();
                if (Module == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //RFWI Trade Detailed CheckList

        public List<MasterSyncViewModel> GetQCInspectionAndRFWIMasterSync()
        {
            var res = BInDB.Database.SqlQuery<MasterSyncViewModel>("SELECT * FROM vw_QCInspectionAndRFWI_Master_Sync").ToList();
            var lists = Mapper.Map<List<MasterSyncViewModel>>(res);
            return lists;
        }


        // Masters

        // Transactions
        // QC Inspection Defect Form
        public QCInspectionDefectFormViewModel GetQCInspectionDefectForm(int Id)
        {
            var Module = BInDB.qcinspection_defect_form.Find(Id);
            return Mapper.Map<QCInspectionDefectFormViewModel>(Module);
        }

        public int CreateQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_defect_form>(QCInspectionDefectForm);
                BInDB.qcinspection_defect_form.Add(_db_res);
                BInDB.SaveChanges();

                if (QCInspectionDefectForm.DefectFormFiles != null && QCInspectionDefectForm.DefectFormFiles?.Count > 0)
                {
                    foreach (var fileBase in QCInspectionDefectForm.DefectFormFiles)
                    {
                        if (fileBase != null)
                        {
                            qcinspection_defect_files defectfile = new qcinspection_defect_files();
                            defectfile.QCInspectionDefectID = _db_res.QCInspectionDefectID;
                            defectfile.FileFor = fileBase.FileFor;
                            defectfile.FileName = fileBase.FileName;
                            defectfile.FilePath = QCInspectionDefectForm.FilePath + "\\" + _db_res.QCInspectionDefectID.ToString() + "\\" + fileBase.FileFor + "\\" + fileBase.FileName;
                            defectfile.FileCaption = fileBase.FileName;
                            BInDB.qcinspection_defect_files.Add(defectfile);
                        }
                    }
                }

                if (QCInspectionDefectForm.DefectFiles != null && QCInspectionDefectForm.DefectFiles?.Count > 0)
                {
                    foreach (var fileBase in QCInspectionDefectForm.DefectFiles)
                    {
                        if (fileBase != null)
                        {
                            qcinspection_defect_files defectfile = new qcinspection_defect_files();
                            defectfile.QCInspectionDefectID = _db_res.QCInspectionDefectID;
                            defectfile.FileFor = "Defect";
                            defectfile.FileName = fileBase.FileName;
                            defectfile.FilePath = QCInspectionDefectForm.FilePath + "\\"+ _db_res.QCInspectionDefectID.ToString() + "\\Defect\\" + fileBase.FileName;
                            defectfile.FileCaption = fileBase.FileName;
                            BInDB.qcinspection_defect_files.Add(defectfile);
                        }
                    }
                }
                BInDB.SaveChanges();
                return _db_res.QCInspectionDefectID;
            }
            catch (Exception ex)
            {
                logger.Debug("Create QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.UnitID = QCInspectionDefectForm.UnitID;
                _db_res.TradeID = QCInspectionDefectForm.TradeID;
                _db_res.DefectTypeID = QCInspectionDefectForm.DefectTypeID;
                _db_res.DefectRemarks = QCInspectionDefectForm.DefectRemarks;
                _db_res.SubcontractorID = QCInspectionDefectForm.SubcontractorID;
                _db_res.ProjectManagerID = QCInspectionDefectForm.ProjectManagerID;
                _db_res.Remarks += "<==Updated on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;

                if (QCInspectionDefectForm.DefectFormFiles != null && QCInspectionDefectForm.DefectFormFiles?.Count > 0)
                {
                    foreach (var fileBase in QCInspectionDefectForm.DefectFormFiles)
                    {
                        if (fileBase != null)
                        {
                            qcinspection_defect_files defectfile = new qcinspection_defect_files();
                            defectfile.QCInspectionDefectID = _db_res.QCInspectionDefectID;
                            defectfile.FileFor = fileBase.FileFor;
                            defectfile.FileName = fileBase.FileName;
                            defectfile.FilePath = QCInspectionDefectForm.FilePath + "\\" + _db_res.QCInspectionDefectID.ToString() + "\\" + fileBase.FileFor + "\\" + fileBase.FileName;
                            defectfile.FileCaption = fileBase.FileName;
                            BInDB.qcinspection_defect_files.Add(defectfile);
                        }
                    }
                }
                if (QCInspectionDefectForm.DefectFiles != null && QCInspectionDefectForm.DefectFiles?.Count > 0 )
                {
                    foreach (var fileBase in QCInspectionDefectForm.DefectFiles)
                    {
                        if (fileBase != null)
                        {
                            qcinspection_defect_files defectfile = new qcinspection_defect_files();
                            defectfile.QCInspectionDefectID = _db_res.QCInspectionDefectID;
                            defectfile.FileFor = "Defect";
                            defectfile.FileName = fileBase.FileName;
                            defectfile.FilePath = QCInspectionDefectForm.FilePath + "\\" + _db_res.QCInspectionDefectID.ToString() + "\\Defect\\" + fileBase.FileName;
                            defectfile.FileCaption = fileBase.FileName;
                            BInDB.qcinspection_defect_files.Add(defectfile);
                        }
                    }
                }

                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteQCInspectionDefectForm(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == Id);
                //_db_res.UpdatedBy = int.Parse(UserID);
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.Status = "Deleted";
                //BInDB.Entry(_db_res).State = EntityState.Modified;
                var _db_res1 = _db_res.qcinspection_defect_files.Where(a => a.QCInspectionDefectID == Id).ToList();
                foreach (var _dbDet in _db_res1)
                {
                    if (File.Exists(_dbDet.FilePath))
                    {
                        File.Delete(_dbDet.FilePath);
                    }
                    BInDB.qcinspection_defect_files.Remove(_dbDet);
                }
                BInDB.qcinspection_defect_form.Remove(_db_res);

                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int ApprovedQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.ApprovedBy = AppSession.GetCurrentUserId();
                _db_res.ApprovedDate = DateTime.Now;
                _db_res.ApprovedRemarks = QCInspectionDefectForm.ApprovedRemarks;
                _db_res.Status = "Approved";
                _db_res.Remarks += "<==Approved on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Approved QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int ReDoQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.ReDoBy = AppSession.GetCurrentUserId();
                _db_res.ReDoDate = DateTime.Now;
                _db_res.ReDoRemarks = QCInspectionDefectForm.ReDoRemarks;
                _db_res.Status = "ReDo";
                _db_res.Remarks += "<==ReDo on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("ReDo QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int ReDoDoneQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.UnitID = QCInspectionDefectForm.UnitID;
                _db_res.TradeID = QCInspectionDefectForm.TradeID;
                _db_res.DefectTypeID = QCInspectionDefectForm.DefectTypeID;
                _db_res.DefectRemarks = QCInspectionDefectForm.DefectRemarks;
                _db_res.SubcontractorID = QCInspectionDefectForm.SubcontractorID;
                _db_res.ReDoDoneBy = AppSession.GetCurrentUserId();
                _db_res.ReDoDoneDate = DateTime.Now;
                _db_res.ReDoDoneRemarks = QCInspectionDefectForm.ReDoDoneRemarks;
                _db_res.Status = "ReDo-Done";
                _db_res.Remarks += "<==ReDo-Done on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                if (QCInspectionDefectForm.DefectFiles != null && QCInspectionDefectForm.DefectFiles.Count > 0)
                {
                    foreach (var fileBase in QCInspectionDefectForm.DefectFiles)
                    {
                        if (fileBase != null)
                        {
                            qcinspection_defect_files defectfile = new qcinspection_defect_files();
                            defectfile.QCInspectionDefectID = _db_res.QCInspectionDefectID;
                            defectfile.FileFor = "Defect";
                            defectfile.FileName = fileBase.FileName;
                            defectfile.FilePath = QCInspectionDefectForm.FilePath + "\\" + _db_res.QCInspectionDefectID.ToString() + "\\Defect\\" + fileBase.FileName;
                            defectfile.FileCaption = fileBase.FileName;
                            BInDB.qcinspection_defect_files.Add(defectfile);
                        }
                    }
                }

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("ReDo QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int RectificationQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.RectificationBy = AppSession.GetCurrentUserId();
                _db_res.RectificationDate = DateTime.Now;
                _db_res.RectificationRemarks = QCInspectionDefectForm.RectificationRemarks;
                _db_res.RectificationSignature = QCInspectionDefectForm.RectificationSignature;
                _db_res.Status = "Rectified";
                _db_res.Remarks += "<==Rectified on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;

                if(QCInspectionDefectForm.RectifyFiles != null && QCInspectionDefectForm.RectifyFiles.Count > 0)
                {
                    foreach (var fileBase in QCInspectionDefectForm.RectifyFiles)
                    {
                        if(fileBase !=null)
                        {
                            qcinspection_defect_files defectfile = new qcinspection_defect_files();
                            defectfile.QCInspectionDefectID = _db_res.QCInspectionDefectID;
                            defectfile.FileFor = "Rectify";
                            defectfile.FileName = fileBase.FileName;
                            defectfile.FilePath = QCInspectionDefectForm.FilePath + "\\" + _db_res.QCInspectionDefectID.ToString() + "\\Rectify\\" + fileBase.FileName;
                            defectfile.FileCaption = fileBase.FileName;
                            BInDB.qcinspection_defect_files.Add(defectfile);
                        }
                    }
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Rectification QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int ReworkQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.ReworkBy = AppSession.GetCurrentUserId();
                _db_res.ReworkDate = DateTime.Now;
                _db_res.ReworkRemarks = QCInspectionDefectForm.ReworkRemarks;
                _db_res.RectificationBy = null;
                _db_res.RectificationDate = null;
                _db_res.RectificationSignature = null;
                _db_res.Status = "Rework";
                _db_res.Remarks += "<==Rework on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Rework QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int ReworkDoneQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.ReworkBy = AppSession.GetCurrentUserId();
                _db_res.ReworkDate = DateTime.Now;
                _db_res.ReworkRemarks = QCInspectionDefectForm.ReworkRemarks;
                _db_res.RectificationBy = null;
                _db_res.RectificationDate = null;
                _db_res.RectificationSignature = null;
                _db_res.Status = "Rework-Done";
                _db_res.Remarks += "<==Rework-Done on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;
                if (QCInspectionDefectForm.RectifyFiles != null && QCInspectionDefectForm.RectifyFiles.Count > 0)
                {
                    foreach (var fileBase in QCInspectionDefectForm.RectifyFiles)
                    {
                        if (fileBase != null)
                        {
                            qcinspection_defect_files defectfile = new qcinspection_defect_files();
                            defectfile.QCInspectionDefectID = _db_res.QCInspectionDefectID;
                            defectfile.FileFor = "Rectify";
                            defectfile.FileName = fileBase.FileName;
                            defectfile.FilePath = QCInspectionDefectForm.FilePath + "\\" + _db_res.QCInspectionDefectID.ToString() + "\\Rectify\\" + fileBase.FileName;
                            defectfile.FileCaption = fileBase.FileName;
                            BInDB.qcinspection_defect_files.Add(defectfile);
                        }
                    }
                }
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Rework QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int CompletedQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.CompletedBy = AppSession.GetCurrentUserId();
                _db_res.CompletedDate = DateTime.Now;
                _db_res.CompletedRemarks = QCInspectionDefectForm.CompletedRemarks;
                _db_res.CompletedSignature = QCInspectionDefectForm.CompletedSignature;
                _db_res.Status = "Completed";
                _db_res.Remarks += "<==Completed on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Completed QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectForms()
        {
            var res = BInDB.qcinspection_defect_form.Where(x => x.Status != "Deleted").OrderBy(a => a.ProjectID).ThenBy(a => a.QCInspectionDefectID).ToList();
            var lists = Mapper.Map<List<QCInspectionDefectFormViewModel>>(res);
            return lists;
        }

        public List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedProjectID(int Id, string BatchID = "")
        {
            if (BatchID.Length == 0)
            {
                var res = BInDB.qcinspection_defect_form.Where(x => x.Status != "Deleted" && x.ProjectID == Id).OrderBy(a => a.QCInspectionDefectID).ToList();
                var lists = Mapper.Map<List<QCInspectionDefectFormViewModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.qcinspection_defect_form.Where(x => x.Status != "Deleted" && x.ProjectID == Id && x.BatchID == BatchID).OrderBy(a => a.QCInspectionDefectID).ToList();
                var lists = Mapper.Map<List<QCInspectionDefectFormViewModel>>(res);
                return lists;
            }
        }

        public List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedUserID(int Id)
        {
            var res = BInDB.qcinspection_defect_form.Where(x => x.Status != "Deleted" && x.CreatedBy == Id).OrderBy(a => a.ProjectID).ThenBy(a => a.QCInspectionDefectID).ToList();
            var lists = Mapper.Map<List<QCInspectionDefectFormViewModel>>(res);
            return lists;
        }

        public List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedSubcontractorID(int Id)
        {
            var res = BInDB.qcinspection_defect_form.Where(x => (x.Status == "Approved" || x.Status == "Rectified" || x.Status == "Completed" || x.Status == "Rework" || x.Status == "Rework-Done") && x.SubcontractorID == Id).OrderBy(a => a.ProjectID).ThenBy(a => a.QCInspectionDefectID).ToList();
            var lists = Mapper.Map<List<QCInspectionDefectFormViewModel>>(res);
            return lists;
        }

        public int MobileSaveQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID);
                _db_res.QCInspectionDefectNo = QCInspectionDefectForm.QCInspectionDefectNo;
                _db_res.ProjectID = QCInspectionDefectForm.ProjectID;
                _db_res.UnitID = QCInspectionDefectForm.UnitID;
                _db_res.TradeID = QCInspectionDefectForm.TradeID;
                _db_res.DefectTypeID = QCInspectionDefectForm.DefectTypeID;
                _db_res.DefectRemarks = QCInspectionDefectForm.DefectRemarks;
                _db_res.SubcontractorID = QCInspectionDefectForm.SubcontractorID;
                _db_res.ProjectManagerID = QCInspectionDefectForm.ProjectManagerID;
                _db_res.ApprovedBy = QCInspectionDefectForm.ApprovedBy;
                _db_res.ApprovedDate = QCInspectionDefectForm.ApprovedDate;
                _db_res.ApprovedRemarks = QCInspectionDefectForm.ApprovedRemarks;
                _db_res.ReDoBy = QCInspectionDefectForm.ReDoBy;
                _db_res.ReDoDate = QCInspectionDefectForm.ReDoDate;
                _db_res.ReDoRemarks = QCInspectionDefectForm.ReDoRemarks;
                _db_res.RectificationBy = QCInspectionDefectForm.RectificationBy;
                _db_res.RectificationDate = QCInspectionDefectForm.RectificationDate;
                _db_res.RectificationRemarks = QCInspectionDefectForm.RectificationRemarks;
                _db_res.RectificationSignature = QCInspectionDefectForm.RectificationSignature;
                _db_res.ReworkBy = QCInspectionDefectForm.ReworkBy;
                _db_res.ReworkDate = QCInspectionDefectForm.ReworkDate;
                _db_res.ReworkRemarks = QCInspectionDefectForm.ReworkRemarks;
                _db_res.CompletedBy = QCInspectionDefectForm.CompletedBy;
                _db_res.CompletedDate = QCInspectionDefectForm.CompletedDate;
                _db_res.CompletedRemarks = QCInspectionDefectForm.CompletedRemarks;
                _db_res.CompletedSignature = QCInspectionDefectForm.CompletedSignature;
                _db_res.Status = QCInspectionDefectForm.Status;
                _db_res.MobileQCInspectionDefectID = QCInspectionDefectForm.MobileQCInspectionDefectID;
                _db_res.BatchID = QCInspectionDefectForm.BatchID;
                _db_res.UpdatedBy = QCInspectionDefectForm.UpdatedBy;
                _db_res.UpdatedDate = QCInspectionDefectForm.UpdatedDate;
                BInDB.Entry(_db_res).State = EntityState.Modified;

                var _db_res1 = BInDB.qcinspection_defect_files.Where(a => a.QCInspectionDefectID == QCInspectionDefectForm.QCInspectionDefectID).ToList();
                foreach (var _dbDet in _db_res1)
                {
                    if(File.Exists(_dbDet.FilePath))
                    {
                        File.Delete(_dbDet.FilePath);
                    }
                    BInDB.qcinspection_defect_files.Remove(_dbDet);
                }

                if (QCInspectionDefectForm.DefectFormFiles != null && QCInspectionDefectForm.DefectFormFiles?.Count > 0)
                {
                    foreach (var fileBase in QCInspectionDefectForm.DefectFormFiles)
                    {
                        if (fileBase != null)
                        {
                            qcinspection_defect_files defectfile = new qcinspection_defect_files();
                            defectfile.QCInspectionDefectID = _db_res.QCInspectionDefectID;
                            defectfile.FileFor = fileBase.FileFor;
                            defectfile.FileName = fileBase.FileName;
                            defectfile.FilePath = QCInspectionDefectForm.FilePath + "\\" + _db_res.QCInspectionDefectID.ToString() + "\\" + fileBase.FileFor + "\\" + fileBase.FileName;
                            defectfile.FileCaption = fileBase.FileName;
                            BInDB.qcinspection_defect_files.Add(defectfile);
                        }
                    }
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Mobile QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int MobileDeleteQCInspectionDefectForm(string Ids, string UserID)
        {
            try
            {
                int Id = 0;
                foreach (string FormId in Ids.Split(','))
                {
                    Id = int.Parse(FormId);
                    var _db_res = BInDB.qcinspection_defect_form.First(a => a.QCInspectionDefectID == Id);
                    if(_db_res.Status == "Pending")
                    {
                        //_db_res.UpdatedBy = int.Parse(UserID);
                        //_db_res.UpdatedDate = DateTime.Now;
                        //_db_res.Status = "Deleted";
                        //BInDB.Entry(_db_res).State = EntityState.Modified;
                        var _db_res1 = _db_res.qcinspection_defect_files.Where(a => a.QCInspectionDefectID == Id).ToList();
                        foreach (var _dbDet in _db_res1)
                        {
                            if (File.Exists(_dbDet.FilePath))
                            {
                                File.Delete(_dbDet.FilePath);
                            }
                            BInDB.qcinspection_defect_files.Remove(_dbDet);
                        }
                        BInDB.qcinspection_defect_form.Remove(_db_res);
                    }
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Mobile QC Inspection Defect Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        // QC Inspection Defect Form

        // RFWI Form
        public QCInspectionRFWIFormViewModel GetRFWIForm(int Id)
        {
            var Module = BInDB.qcinspection_rfwi_form.Find(Id);
            return Mapper.Map<QCInspectionRFWIFormViewModel>(Module);
        }

        public int CreateRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = Mapper.Map<qcinspection_rfwi_form>(RFWIForm);
                _db_res.InspectionStartOn = RFWIForm.InspectionStartOn;
                _db_res.InspectionEndOn = RFWIForm.InspectionEndOn;
                BInDB.qcinspection_rfwi_form.Add(_db_res);
                foreach (var Id in RFWIForm.SelectedGeneralCheckListIds.Split(','))
                {
                    qcinspection_rfwi_form_general_checklist_detail RFWIDetail = new qcinspection_rfwi_form_general_checklist_detail();
                    RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                    RFWIDetail.GeneralCheckListID = int.Parse(Id.ToString());
                    RFWIDetail.CreatedBy = _db_res.CreatedBy;
                    RFWIDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_rfwi_form_general_checklist_detail.Add(RFWIDetail);
                }

                foreach (var Id in RFWIForm.SelectedTradeItemListIds.Split(','))
                {
                    qcinspection_rfwi_form_trade_item_detail RFWIDetail = new qcinspection_rfwi_form_trade_item_detail();
                    RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                    RFWIDetail.TradeItemID = int.Parse(Id.ToString());
                    RFWIDetail.CreatedBy = _db_res.CreatedBy;
                    RFWIDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_rfwi_form_trade_item_detail.Add(RFWIDetail);
                }

                foreach (var Id in RFWIForm.SelectedTradeDetailedCheckListIds.Split(','))
                {
                    qcinspection_rfwi_form_trade_detailed_checklist_detail RFWIDetail = new qcinspection_rfwi_form_trade_detailed_checklist_detail();
                    RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                    RFWIDetail.TradeDetailedCheckListID = int.Parse(Id.ToString());
                    RFWIDetail.CreatedBy = _db_res.CreatedBy;
                    RFWIDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_rfwi_form_trade_detailed_checklist_detail.Add(RFWIDetail);
                }
                
                BInDB.SaveChanges();
                return _db_res.QCInspectionRFWIFormID;
            }
            catch (Exception ex)
            {
                logger.Debug("Create RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == RFWIForm.QCInspectionRFWIFormID);
                _db_res.TradeID = RFWIForm.TradeID;
                _db_res.InspectorID = RFWIForm.InspectorID;
                _db_res.RequestFor = RFWIForm.RequestFor;
                _db_res.OtherTradeClearance_Structure = RFWIForm.OtherTradeClearance_Structure;
                _db_res.OtherTradeClearance_MandE = RFWIForm.OtherTradeClearance_MandE;
                _db_res.OtherTradeClearance_Other = RFWIForm.OtherTradeClearance_Other;
                if(RFWIForm.RequestSignature!=null)
                {
                    _db_res.InspectionOn = RFWIForm.InspectionOn;
                    _db_res.InspectionStartOn = RFWIForm.InspectionStartOn;
                    _db_res.InspectionEndOn = RFWIForm.InspectionEndOn;
                    _db_res.RequestBy = AppSession.GetCurrentUserId();
                    _db_res.RequestOn = DateTime.Now;
                    _db_res.RequestSignature = RFWIForm.RequestSignature;
                    _db_res.Status = "Requested";
                }
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                if (!string.IsNullOrEmpty(RFWIForm.SelectedGeneralCheckListIds))
                {
                    foreach (var _dbDet in _db_res.qcinspection_rfwi_form_general_checklist_detail.ToList())
                    {
                        BInDB.qcinspection_rfwi_form_general_checklist_detail.Remove(_dbDet);
                    }

                    foreach (var Id in RFWIForm.SelectedGeneralCheckListIds.Split(','))
                    {
                        qcinspection_rfwi_form_general_checklist_detail RFWIDetail = new qcinspection_rfwi_form_general_checklist_detail();
                        RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                        RFWIDetail.GeneralCheckListID = int.Parse(Id.ToString());
                        RFWIDetail.CreatedBy = AppSession.GetCurrentUserId();
                        RFWIDetail.CreatedDate = DateTime.Now;
                        BInDB.qcinspection_rfwi_form_general_checklist_detail.Add(RFWIDetail);
                    }
                }
                if (!string.IsNullOrEmpty(RFWIForm.SelectedTradeItemListIds))
                {

                    foreach (var _dbDet in _db_res.qcinspection_rfwi_form_trade_item_detail.ToList())
                    {
                        BInDB.qcinspection_rfwi_form_trade_item_detail.Remove(_dbDet);
                    }

                    foreach (var Id in RFWIForm.SelectedTradeItemListIds.Split(','))
                    {
                        qcinspection_rfwi_form_trade_item_detail RFWIDetail = new qcinspection_rfwi_form_trade_item_detail();
                        RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                        RFWIDetail.TradeItemID = int.Parse(Id.ToString());
                        RFWIDetail.CreatedBy = AppSession.GetCurrentUserId();
                        RFWIDetail.CreatedDate = DateTime.Now;
                        BInDB.qcinspection_rfwi_form_trade_item_detail.Add(RFWIDetail);
                    }
                }
                if (!string.IsNullOrEmpty(RFWIForm.SelectedTradeDetailedCheckListIds))
                {
                    foreach (var _dbDet in _db_res.qcinspection_rfwi_form_trade_detailed_checklist_detail.ToList())
                    {
                        BInDB.qcinspection_rfwi_form_trade_detailed_checklist_detail.Remove(_dbDet);
                    }

                    foreach (var Id in RFWIForm.SelectedTradeDetailedCheckListIds.Split(','))
                    {
                        qcinspection_rfwi_form_trade_detailed_checklist_detail RFWIDetail = new qcinspection_rfwi_form_trade_detailed_checklist_detail();
                        RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                        RFWIDetail.TradeDetailedCheckListID = int.Parse(Id.ToString());
                        RFWIDetail.CreatedBy = AppSession.GetCurrentUserId();
                        RFWIDetail.CreatedDate = DateTime.Now;
                        BInDB.qcinspection_rfwi_form_trade_detailed_checklist_detail.Add(RFWIDetail);
                    }
                }
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteRFWIForm(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == Id);
                
                //_db_res.UpdatedBy = int.Parse(UserID);
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.Status = "Deleted";
                //BInDB.Entry(_db_res).State = EntityState.Modified;
                foreach (var _dbDet in _db_res.qcinspection_rfwi_form_general_checklist_detail.ToList())
                {
                    BInDB.qcinspection_rfwi_form_general_checklist_detail.Remove(_dbDet);
                }
                foreach (var _dbDet in _db_res.qcinspection_rfwi_form_trade_item_detail.ToList())
                {
                    BInDB.qcinspection_rfwi_form_trade_item_detail.Remove(_dbDet);
                }
                foreach (var _dbDet in _db_res.qcinspection_rfwi_form_trade_detailed_checklist_detail.ToList())
                {
                    BInDB.qcinspection_rfwi_form_trade_detailed_checklist_detail.Remove(_dbDet);
                }
                foreach (var _dbDet in _db_res.qcinspection_rfwi_form_location_detail.ToList())
                {
                    BInDB.qcinspection_rfwi_form_location_detail.Remove(_dbDet);
                }
                BInDB.qcinspection_rfwi_form.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int MandESignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == RFWIForm.QCInspectionRFWIFormID);
                _db_res.OtherTradeClearance_MandEBy = AppSession.GetCurrentUserId();
                _db_res.OtherTradeClearance_MandEOn = DateTime.Now;
                _db_res.OtherTradeClearance_MandESignature = RFWIForm.OtherTradeClearance_MandESignature;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;
                bool bolApporved = true;
                
                // 11
                if (_db_res.OtherTradeClearance_Structure == true && _db_res.OtherTradeClearance_Other == true)
                {
                    if (_db_res.OtherTradeClearance_StructureSignature != null && _db_res.OtherTradeClearance_OtherSignature != null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }
                // 10
                else if (_db_res.OtherTradeClearance_Structure == true && _db_res.OtherTradeClearance_Other == false)
                {
                    if (_db_res.OtherTradeClearance_StructureSignature != null && _db_res.OtherTradeClearance_OtherSignature == null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }
                // 01
                else if (_db_res.OtherTradeClearance_Structure == false && _db_res.OtherTradeClearance_Other == true)
                {
                    if (_db_res.OtherTradeClearance_StructureSignature == null && _db_res.OtherTradeClearance_OtherSignature != null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }
               

                if (bolApporved)
                {
                    _db_res.Status = "Approved";
                }
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("MandE - Sign Off RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int StructureSignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == RFWIForm.QCInspectionRFWIFormID);
                _db_res.OtherTradeClearance_StructureBy = AppSession.GetCurrentUserId();
                _db_res.OtherTradeClearance_StructureOn = DateTime.Now;
                _db_res.OtherTradeClearance_StructureSignature = RFWIForm.OtherTradeClearance_StructureSignature;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;
                bool bolApporved = true;

                // 11
                if (_db_res.OtherTradeClearance_MandE == true && _db_res.OtherTradeClearance_Other == true)
                {
                    if (_db_res.OtherTradeClearance_MandESignature != null && _db_res.OtherTradeClearance_OtherSignature != null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }
                // 10
                else if (_db_res.OtherTradeClearance_MandE == true && _db_res.OtherTradeClearance_Other == false)
                {
                    if (_db_res.OtherTradeClearance_MandESignature != null && _db_res.OtherTradeClearance_OtherSignature == null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }
                // 01
                else if (_db_res.OtherTradeClearance_MandE == false && _db_res.OtherTradeClearance_Other == true)
                {
                    if (_db_res.OtherTradeClearance_MandESignature == null && _db_res.OtherTradeClearance_OtherSignature != null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }

                if (bolApporved)
                {
                    _db_res.Status = "Approved";
                }
                
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Structure - Sign Off RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int OtherSignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == RFWIForm.QCInspectionRFWIFormID);
                _db_res.OtherTradeClearance_OtherBy = AppSession.GetCurrentUserId();
                _db_res.OtherTradeClearance_OtherOn = DateTime.Now;
                _db_res.OtherTradeClearance_OtherSignature = RFWIForm.OtherTradeClearance_OtherSignature;
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;
                bool bolApporved = true;

                // 11
                if (_db_res.OtherTradeClearance_MandE == true && _db_res.OtherTradeClearance_Structure == true)
                {
                    if (_db_res.OtherTradeClearance_MandESignature != null && _db_res.OtherTradeClearance_StructureSignature != null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }
                // 10
                else if (_db_res.OtherTradeClearance_MandE == true && _db_res.OtherTradeClearance_Structure == false)
                {
                    if (_db_res.OtherTradeClearance_MandESignature != null && _db_res.OtherTradeClearance_StructureSignature == null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }
                // 01
                else if (_db_res.OtherTradeClearance_MandE == false && _db_res.OtherTradeClearance_Structure == true)
                {
                    if (_db_res.OtherTradeClearance_MandESignature == null && _db_res.OtherTradeClearance_StructureSignature != null)
                    {
                        bolApporved = true;
                    }
                    else
                    {
                        bolApporved = false;
                    }
                }

                if (bolApporved)
                {
                    _db_res.Status = "Approved";
                }
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Other - Sign Off RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int RequestedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == RFWIForm.QCInspectionRFWIFormID);
                _db_res.InspectionOn = RFWIForm.InspectionOn;
                _db_res.InspectionStartOn = RFWIForm.InspectionStartOn;
                _db_res.InspectionEndOn = RFWIForm.InspectionEndOn;
                _db_res.InspectorID = RFWIForm.InspectorID;
                _db_res.RequestBy = AppSession.GetCurrentUserId();
                _db_res.RequestOn = DateTime.Now;
                _db_res.RequestSignature = RFWIForm.RequestSignature;
                _db_res.Status = "Requested";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Request RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int CompletedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == RFWIForm.QCInspectionRFWIFormID);
                _db_res.CompletedBy = AppSession.GetCurrentUserId();
                _db_res.CompletedDate = DateTime.Now;
                _db_res.CompletedRemarks = RFWIForm.CompletedRemarks;
                _db_res.CompletedSignature = RFWIForm.CompletedSignature;
                _db_res.Status = "Completed";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Completed RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int RejectedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == RFWIForm.QCInspectionRFWIFormID);
                _db_res.CompletedBy = AppSession.GetCurrentUserId();
                _db_res.CompletedDate = DateTime.Now;
                _db_res.CompletedRemarks = RFWIForm.CompletedRemarks;
                _db_res.CompletedSignature = RFWIForm.CompletedSignature;
                _db_res.Status = "Rejected";
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Completed RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int ReInspectionRFWIForm(int Id)
        {
            try
            {
                var _db_Data = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == Id);
                qcinspection_rfwi_form _db_res = new qcinspection_rfwi_form();
                _db_res.ProjectID = _db_Data.ProjectID;
                _db_res.TradeID = _db_Data.TradeID;
                _db_res.InspectionNo = _db_Data.InspectionNo;
                _db_res.InspectorID = _db_Data.InspectorID;
                _db_res.RequestFor = _db_Data.RequestFor;
                _db_res.ItemOthers = _db_Data.ItemOthers;
                _db_res.DetailedCheckListOthers = _db_Data.DetailedCheckListOthers;
                _db_res.QCInspectionRFWINo = _db_Data.QCInspectionRFWINo.Substring(0,11) + "-" + _db_res.InspectionNo.ToNullSafeString();
                _db_res.OtherTradeClearance_Structure = _db_Data.OtherTradeClearance_Structure;
                _db_res.OtherTradeClearance_MandE = _db_Data.OtherTradeClearance_MandE;
                _db_res.OtherTradeClearance_Other = _db_Data.OtherTradeClearance_Other;
                _db_res.Status = "Pending";
                _db_res.CreatedBy = AppSession.GetCurrentUserId();
                _db_res.CreatedDate = DateTime.Now;
                _db_res.InspectionNo++;
                BInDB.qcinspection_rfwi_form.Add(_db_res);
                foreach (var GCLDetail in _db_Data.qcinspection_rfwi_form_general_checklist_detail.OrderBy(x=> x.RFWIFormGeneralCheckListDetailID))
                {
                    qcinspection_rfwi_form_general_checklist_detail RFWIDetail = new qcinspection_rfwi_form_general_checklist_detail();
                    RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                    RFWIDetail.GeneralCheckListID = GCLDetail.GeneralCheckListID;
                    RFWIDetail.CreatedBy = AppSession.GetCurrentUserId();
                    RFWIDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_rfwi_form_general_checklist_detail.Add(RFWIDetail);
                }

                foreach (var TrItem in _db_Data.qcinspection_rfwi_form_trade_item_detail.OrderBy(x => x.RFWIFormTradeItemDetailID))
                {
                    qcinspection_rfwi_form_trade_item_detail RFWIDetail = new qcinspection_rfwi_form_trade_item_detail();
                    RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                    RFWIDetail.TradeItemID = TrItem.TradeItemID;
                    RFWIDetail.CreatedBy = AppSession.GetCurrentUserId();
                    RFWIDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_rfwi_form_trade_item_detail.Add(RFWIDetail);
                }

                foreach (var TrDCL in _db_Data.qcinspection_rfwi_form_trade_detailed_checklist_detail.OrderBy(x => x.RFWIFormTradeDetailedCheckListDetailID))
                {
                    qcinspection_rfwi_form_trade_detailed_checklist_detail RFWIDetail = new qcinspection_rfwi_form_trade_detailed_checklist_detail();
                    RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                    RFWIDetail.TradeDetailedCheckListID = TrDCL.TradeDetailedCheckListID;
                    RFWIDetail.CreatedBy = AppSession.GetCurrentUserId();
                    RFWIDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_rfwi_form_trade_detailed_checklist_detail.Add(RFWIDetail);
                }
                foreach (var TrDCL in _db_Data.qcinspection_rfwi_form_location_detail.OrderBy(x => x.RFWIFormLocationDetailID))
                {
                    qcinspection_rfwi_form_location_detail RFWILocationDetail = new qcinspection_rfwi_form_location_detail();
                    RFWILocationDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                    RFWILocationDetail.UnitID = TrDCL.UnitID;
                    RFWILocationDetail.QCInspectionDrawingReferenceFileID = TrDCL.QCInspectionDrawingReferenceFileID;
                    RFWILocationDetail.CreatedBy = AppSession.GetCurrentUserId();
                    RFWILocationDetail.CreatedDate = DateTime.Now;
                    BInDB.qcinspection_rfwi_form_location_detail.Add(RFWILocationDetail);
                }
                BInDB.SaveChanges();
                _db_Data.ReInspectionFormID = _db_res.QCInspectionRFWIFormID;
                BInDB.Entry(_db_Data).State = EntityState.Modified;
                BInDB.SaveChanges();
                return _db_res.QCInspectionRFWIFormID;
            }
            catch (Exception ex)
            {
                logger.Debug("ReInspection Create RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int MobileSaveRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == RFWIForm.QCInspectionRFWIFormID);
                _db_res.TradeID = RFWIForm.TradeID;
                _db_res.RequestBy = RFWIForm.RequestBy;
                _db_res.RequestOn = RFWIForm.RequestOn;
                _db_res.RequestSignature = RFWIForm.RequestSignature;
                _db_res.NotiicationReceivedByName = RFWIForm.NotiicationReceivedByName;
                _db_res.NotiicationReceivedSignature = RFWIForm.NotiicationReceivedSignature;
                _db_res.NotiicationReceivedOn = RFWIForm.NotiicationReceivedOn;
                _db_res.InspectionOn = RFWIForm.InspectionOn;
                _db_res.InspectionStartOn = RFWIForm.InspectionStartOn;
                _db_res.InspectionEndOn = RFWIForm.InspectionEndOn;
                _db_res.InspectorID = RFWIForm.InspectorID;
                _db_res.RequestFor = RFWIForm.RequestFor;
                _db_res.ItemOthers = RFWIForm.ItemOthers;
                _db_res.DetailedCheckListOthers = RFWIForm.DetailedCheckListOthers;
                _db_res.OtherTradeClearance_Structure = RFWIForm.OtherTradeClearance_Structure;
                _db_res.OtherTradeClearance_StructureBy = RFWIForm.OtherTradeClearance_StructureBy;
                _db_res.OtherTradeClearance_StructureOn = RFWIForm.OtherTradeClearance_StructureOn;
                _db_res.OtherTradeClearance_StructureSignature = RFWIForm.OtherTradeClearance_StructureSignature;
                _db_res.OtherTradeClearance_MandE = RFWIForm.OtherTradeClearance_MandE;
                _db_res.OtherTradeClearance_MandEBy = RFWIForm.OtherTradeClearance_MandEBy;
                _db_res.OtherTradeClearance_MandEOn = RFWIForm.OtherTradeClearance_MandEOn;
                _db_res.OtherTradeClearance_MandESignature = RFWIForm.OtherTradeClearance_MandESignature;
                _db_res.OtherTradeClearance_Other = RFWIForm.OtherTradeClearance_Other;
                _db_res.OtherTradeClearance_OtherBy = RFWIForm.OtherTradeClearance_OtherBy;
                _db_res.OtherTradeClearance_OtherOn = RFWIForm.OtherTradeClearance_OtherOn;
                _db_res.OtherTradeClearance_OtherSignature = RFWIForm.OtherTradeClearance_OtherSignature;
                _db_res.CompletedBy = RFWIForm.CompletedBy;
                _db_res.CompletedDate = RFWIForm.CompletedDate;
                _db_res.CompletedRemarks = RFWIForm.CompletedRemarks;
                _db_res.CompletedSignature = RFWIForm.CompletedSignature;
                _db_res.ReInspectionFormID = RFWIForm.ReInspectionFormID;
                _db_res.MobileQCInspectionRFWIFormID = RFWIForm.MobileQCInspectionRFWIFormID;
                _db_res.BatchID = RFWIForm.BatchID;
                _db_res.Status = RFWIForm.Status;
                _db_res.UpdatedBy = RFWIForm.UpdatedBy;
                _db_res.UpdatedDate = RFWIForm.UpdatedDate;
                BInDB.Entry(_db_res).State = EntityState.Modified;
               
                if (RFWIForm.SelectedGeneralCheckListIds != null)
                {
                    foreach (var _dbDet in _db_res.qcinspection_rfwi_form_general_checklist_detail.ToList())
                    {
                        BInDB.qcinspection_rfwi_form_general_checklist_detail.Remove(_dbDet);
                    }

                    foreach (var Id in RFWIForm.SelectedGeneralCheckListIds.Split(','))
                    {
                        qcinspection_rfwi_form_general_checklist_detail RFWIDetail = new qcinspection_rfwi_form_general_checklist_detail();
                        RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                        RFWIDetail.GeneralCheckListID = int.Parse(Id.ToString());
                        RFWIDetail.CreatedBy = RFWIForm.UpdatedBy;
                        RFWIDetail.CreatedDate = RFWIForm.UpdatedDate;
                        BInDB.qcinspection_rfwi_form_general_checklist_detail.Add(RFWIDetail);
                    }
                }
                
                if (RFWIForm.SelectedTradeItemListIds != null)
                {
                    foreach (var _dbDet in _db_res.qcinspection_rfwi_form_trade_item_detail.ToList())
                    {
                        BInDB.qcinspection_rfwi_form_trade_item_detail.Remove(_dbDet);
                    }

                    foreach (var Id in RFWIForm.SelectedTradeItemListIds.Split(','))
                    {
                        qcinspection_rfwi_form_trade_item_detail RFWIDetail = new qcinspection_rfwi_form_trade_item_detail();
                        RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                        RFWIDetail.TradeItemID = int.Parse(Id.ToString());
                        RFWIDetail.CreatedBy = RFWIForm.UpdatedBy;
                        RFWIDetail.CreatedDate = RFWIForm.UpdatedDate;
                        BInDB.qcinspection_rfwi_form_trade_item_detail.Add(RFWIDetail);
                    }
                }
                
                if (RFWIForm.SelectedTradeDetailedCheckListIds != null)
                {
                    foreach (var _dbDet in _db_res.qcinspection_rfwi_form_trade_detailed_checklist_detail.ToList())
                    {
                        BInDB.qcinspection_rfwi_form_trade_detailed_checklist_detail.Remove(_dbDet);
                    }

                    foreach (var Id in RFWIForm.SelectedTradeDetailedCheckListIds.Split(','))
                    {
                        qcinspection_rfwi_form_trade_detailed_checklist_detail RFWIDetail = new qcinspection_rfwi_form_trade_detailed_checklist_detail();
                        RFWIDetail.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                        RFWIDetail.TradeDetailedCheckListID = int.Parse(Id.ToString());
                        RFWIDetail.CreatedBy = RFWIForm.UpdatedBy;
                        RFWIDetail.CreatedDate = RFWIForm.UpdatedDate;
                        BInDB.qcinspection_rfwi_form_trade_detailed_checklist_detail.Add(RFWIDetail);
                    }
                }
                
                if (RFWIForm.qcinspection_rfwi_form_location_detail?.Count > 0)
                {
                    foreach (var _dbDet in _db_res.qcinspection_rfwi_form_location_detail.ToList())
                    {
                        BInDB.qcinspection_rfwi_form_location_detail.Remove(_dbDet);
                    }

                    foreach (var item in RFWIForm.qcinspection_rfwi_form_location_detail)
                    {
                        qcinspection_rfwi_form_location_detail RFWILocation = new qcinspection_rfwi_form_location_detail();
                        RFWILocation.QCInspectionRFWIFormID = _db_res.QCInspectionRFWIFormID;
                        RFWILocation.UnitID = item.UnitID;
                        RFWILocation.QCInspectionDrawingReferenceFileID = item.QCInspectionDrawingReferenceFileID;
                        RFWILocation.CreatedBy = RFWIForm.UpdatedBy;
                        RFWILocation.CreatedDate = RFWIForm.UpdatedDate;
                        BInDB.qcinspection_rfwi_form_location_detail.Add(RFWILocation);
                    }
                }
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int MobileDeleteRFWIForm(string Ids, string UserID)
        {
            try
            {
                int Id = 0;
                foreach (string FormId in Ids.Split(','))
                {
                    Id = int.Parse(FormId);
                    var _db_res = BInDB.qcinspection_rfwi_form.First(a => a.QCInspectionRFWIFormID == Id);
                    if (_db_res.Status == "Pending")
                    {
                        //_db_res.UpdatedBy = int.Parse(UserID);
                        //_db_res.UpdatedDate = DateTime.Now;
                        //_db_res.Status = "Deleted";
                        //BInDB.Entry(_db_res).State = EntityState.Modified;
                        foreach (var _dbDet in _db_res.qcinspection_rfwi_form_general_checklist_detail.ToList())
                        {
                            BInDB.qcinspection_rfwi_form_general_checklist_detail.Remove(_dbDet);
                        }
                        foreach (var _dbDet in _db_res.qcinspection_rfwi_form_trade_item_detail.ToList())
                        {
                            BInDB.qcinspection_rfwi_form_trade_item_detail.Remove(_dbDet);
                        }
                        foreach (var _dbDet in _db_res.qcinspection_rfwi_form_trade_detailed_checklist_detail.ToList())
                        {
                            BInDB.qcinspection_rfwi_form_trade_detailed_checklist_detail.Remove(_dbDet);
                        }
                        foreach (var _dbDet in _db_res.qcinspection_rfwi_form_location_detail.ToList())
                        {
                            BInDB.qcinspection_rfwi_form_location_detail.Remove(_dbDet);
                        }
                        BInDB.qcinspection_rfwi_form.Remove(_db_res);
                    }
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Mobile RFWI Form:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public bool ValidateInspectionDateTimeSlot(DateTime InspectionOn, TimeSpan InspectionStartOn, TimeSpan InspectionEndOn)
        {
            try
            {
                var Module = BInDB.qcinspection_rfwi_form.Where(a => a.InspectionOn.Value.Day == InspectionOn.Day && a.InspectionOn.Value.Month == InspectionOn.Month && a.InspectionOn.Value.Year == InspectionOn.Year && ((a.InspectionStartOn.Value >= InspectionStartOn && a.InspectionStartOn.Value <= InspectionEndOn) || (a.InspectionEndOn.Value >= InspectionStartOn && a.InspectionEndOn.Value <= InspectionEndOn))).ToList();
                if (Module.Count > 0)
                    return true;
                else
                    return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public List<QCInspectionRFWIFormViewModel> GetAllRFWIForms()
        {
            var res = BInDB.qcinspection_rfwi_form.Where(x => x.Status != "Deleted").OrderBy(a => a.ProjectID).ThenBy(a => a.QCInspectionRFWIFormID).ToList();
            var lists = Mapper.Map<List<QCInspectionRFWIFormViewModel>>(res);
            return lists;
        }

        public List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedProjectID(int Id, string BatchID = "")
        {
            if (BatchID.Length == 0)
            {
                var res = BInDB.qcinspection_rfwi_form.Where(x => x.Status != "Deleted" && x.ProjectID == Id).OrderBy(a => a.ProjectID).ThenBy(a => a.QCInspectionRFWIFormID).ToList();
                var lists = Mapper.Map<List<QCInspectionRFWIFormViewModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.qcinspection_rfwi_form.Where(x => x.Status != "Deleted" && x.ProjectID == Id && x.BatchID == BatchID).OrderBy(a => a.ProjectID).ThenBy(a => a.QCInspectionRFWIFormID).ToList();
                var lists = Mapper.Map<List<QCInspectionRFWIFormViewModel>>(res);
                return lists;
            }
        }

        public List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedUserID(int Id)
        {
            var res = BInDB.qcinspection_rfwi_form.Where(x => x.Status != "Deleted" && x.CreatedBy == Id).OrderBy(a => a.ProjectID).ThenBy(a => a.QCInspectionRFWIFormID).ToList();
            var lists = Mapper.Map<List<QCInspectionRFWIFormViewModel>>(res);
            return lists;
        }

        public List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedRTOInspectorID(int Id)
        {
            var res = BInDB.qcinspection_rfwi_form.Where(x => (x.Status == "Requested" || x.Status == "Rejected" || x.Status == "Completed") && x.InspectorID == Id).OrderBy(a => a.ProjectID).ThenBy(a => a.QCInspectionRFWIFormID).ToList();
            var lists = Mapper.Map<List<QCInspectionRFWIFormViewModel>>(res);
            return lists;
        }
        // RFWI Form

        //RFWI Drawings Reference File
        public QCInspectionRFWIFormLocationDetailViewModel GetRFWIFormLocationDetail(int Id)
        {
            var Module = BInDB.qcinspection_rfwi_form_location_detail.Find(Id);
            return Mapper.Map<QCInspectionRFWIFormLocationDetailViewModel>(Module);
        }

        public int CreateRFWIFormLocationDetail(QCInspectionRFWIFormLocationDetailViewModel RFWIFormLocation)
        {
            try
            {
                qcinspection_rfwi_form_location_detail _db_res = new qcinspection_rfwi_form_location_detail();
                _db_res.QCInspectionRFWIFormID = RFWIFormLocation.QCInspectionRFWIFormID;
                _db_res.UnitID = RFWIFormLocation.UnitID;
                _db_res.QCInspectionDrawingReferenceFileID = RFWIFormLocation.QCInspectionDrawingReferenceFileID;
                _db_res.CreatedBy = RFWIFormLocation.CreatedBy;
                _db_res.CreatedDate = RFWIFormLocation.CreatedDate;
                BInDB.qcinspection_rfwi_form_location_detail.Add(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create RFWI Form Location Detail:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteRFWIFormLocationDetail(int Id)
        {
            try
            {
                var _db_res = BInDB.qcinspection_rfwi_form_location_detail.First(a => a.RFWIFormLocationDetailID == Id);
                BInDB.qcinspection_rfwi_form_location_detail.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete RFWI Form Location:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public List<QCInspectionRFWIFormLocationDetailViewModel> GetAllRFWIFormsLocationDetails(int Id)
        {
            var res = BInDB.qcinspection_rfwi_form_location_detail.Where(x => x.QCInspectionRFWIFormID == Id).OrderBy(a => a.QCInspectionDrawingReferenceFileID).ToList();
            var lists = Mapper.Map<List<QCInspectionRFWIFormLocationDetailViewModel>>(res);
            return lists;
        }

        public bool CheckRFWIFormLocationDetail(int QCInspectionRFWIFormID, int RFWIFormLocationDetailID, int UnitID)
        {
            try
            {
                var user = BInDB.qcinspection_rfwi_form_location_detail.Where(a => a.RFWIFormLocationDetailID != RFWIFormLocationDetailID && a.QCInspectionRFWIFormID == QCInspectionRFWIFormID && a.UnitID == UnitID).SingleOrDefault();
                if (user == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //RFWI Drawings Reference File

        // Transactions
    }
}