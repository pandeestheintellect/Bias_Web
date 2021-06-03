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
    public class AssessmentRepository : IAssessmentRepository
    {
        BuildInspectEntities BInDB = new BuildInspectEntities();
        Logger logger = LogManager.GetCurrentClassLogger();

        // Project
        public List<AssessmentProjectMasterViewModel> GetAllProjects()
        {
            var users = BInDB.assessment_project_master.ToList();
            var lstUserView = Mapper.Map<List<AssessmentProjectMasterViewModel>>(users);
            return lstUserView;
        }

        public List<AssessmentProjectMasterViewModel> GetAllProjectsBasedCompanyID(int? id)
        {
            var res = BInDB.assessment_project_master.Where(a => a.CompanyID == id).ToList();
            var lists = Mapper.Map<List<AssessmentProjectMasterViewModel>>(res);
            return lists;
        }

        public AssessmentProjectMasterViewModel GetProject(int pid)
        {
            var project = BInDB.assessment_project_master.Find(pid);
            return Mapper.Map<AssessmentProjectMasterViewModel>(project);
        }

        public int CreateProject(AssessmentProjectMasterViewModel project, List<AssessmentProjectAssessorsDetailViewModel> detailViewModels)
        {
            try
            {
                var _db_project = Mapper.Map<assessment_project_master>(project);

                BInDB.assessment_project_master.Add(_db_project);
                foreach (var desc in detailViewModels)
                {
                    assessment_project_assessors_detail projectAssessorsDetail = Mapper.Map<assessment_project_assessors_detail>(desc);
                    projectAssessorsDetail.ProjectID = _db_project.ProjectID;
                    BInDB.assessment_project_assessors_detail.Add(projectAssessorsDetail);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CreateProject:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveProject(AssessmentProjectMasterViewModel project, List<AssessmentProjectAssessorsDetailViewModel> detailViewModels)
        {
            try
            {
                var _db_project = BInDB.assessment_project_master.Find(project.ProjectID);
                _db_project.CompanyID = project.CompanyID;
                _db_project.Project_Name = project.Project_Name;
                _db_project.Project_ID = project.Project_ID;
                _db_project.Project_DocNo = project.Project_DocNo;
                _db_project.Developer_Name = project.Developer_Name;
                _db_project.Contractor_Name = project.Contractor_Name;
                _db_project.Assessment_StartOn = project.Assessment_StartOn;
                _db_project.Assessment_EndOn = project.Assessment_EndOn;
                _db_project.Assessment_Dates = project.Assessment_Dates;
                _db_project.DevelopmentTypeID = project.DevelopmentTypeID;
                _db_project.ArchitecturalWorksWeightage = project.ArchitecturalWorksWeightage;
                _db_project.MEWorksWeightage = project.MEWorksWeightage;
                _db_project.BuildQASScore = project.BuildQASScore;
                _db_project.MinimumCompliancePercentageThreshold = project.MinimumCompliancePercentageThreshold;
                _db_project.FieldWindowWTT_Contractor_Name = project.FieldWindowWTT_Contractor_Name;
                _db_project.FieldWindowWTT_Witness_Name = project.FieldWindowWTT_Witness_Name;
                _db_project.WetAreaWTT_Contractor_Name = project.WetAreaWTT_Contractor_Name;
                _db_project.WetAreaWTT_Witness_Name = project.WetAreaWTT_Witness_Name;
                _db_project.Is_ExternalWallApplicable = project.Is_ExternalWallApplicable;
                _db_project.Is_ExternalWorksApplicable = project.Is_ExternalWorksApplicable;
                _db_project.Is_RoofApplicable = project.Is_RoofApplicable;
                _db_project.Is_FieldWindowWTTApplicable = project.Is_FieldWindowWTTApplicable;
                _db_project.Is_WetAreaWTTApplicable = project.Is_WetAreaWTTApplicable;
                _db_project.Is_Completed = project.Is_Completed;
                _db_project.UpdatedBy = project.UpdatedBy;
                _db_project.UpdatedDate = project.UpdatedDate;
                BInDB.Entry(_db_project).State = EntityState.Modified;

                var _db_res = BInDB.assessment_project_assessors_detail.Where(a => a.ProjectID == project.ProjectID).ToList();
                foreach (var _dbDet in _db_res)
                {
                    BInDB.assessment_project_assessors_detail.Remove(_dbDet);
                }

                foreach (var desc in detailViewModels)
                {
                    assessment_project_assessors_detail projectAssessorsDetail = Mapper.Map<assessment_project_assessors_detail>(desc);
                    projectAssessorsDetail.ProjectID = project.ProjectID;
                    BInDB.assessment_project_assessors_detail.Add(projectAssessorsDetail);
                }
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
                var _db_res1 = BInDB.assessment_project_assessors_detail.Where(a => a.ProjectID == pID).ToList();
                foreach (var _dbDet in _db_res1)
                {
                    BInDB.assessment_project_assessors_detail.Remove(_dbDet);
                }
                var _db_res = BInDB.assessment_project_master.First(a => a.ProjectID == pID);
                BInDB.assessment_project_master.Remove(_db_res);
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

        public bool CheckProject(string projectname)
        {
            try
            {
                var user = BInDB.assessment_project_master.Where(a => a.Project_Name == projectname).SingleOrDefault();
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
                var _db_res = BInDB.assessment_project_master.First(a => a.ProjectID == pid);
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
        //Assessors
        public AssessorsMasterViewModel GetAssessor(int aid)
        {
            var assessor = BInDB.assessors_master.AsNoTracking().Where(x => x.AssessorsID == aid).FirstOrDefault();
            return Mapper.Map<AssessorsMasterViewModel>(assessor);
        }

        public int CreateAssessor(AssessorsMasterViewModel Assessor)
        {
            var _db_res = Mapper.Map<assessors_master>(Assessor);
            _db_res.IsActive = 1;
            BInDB.assessors_master.Add(_db_res);
            BInDB.SaveChanges();
            return _db_res.AssessorsID;
        }

        public int SaveAssessor(AssessorsMasterViewModel Assessor)
        {
            try
            {
                var _db_res = Mapper.Map<assessors_master>(Assessor);
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessor:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteAssessor(int aID)
        {
            try
            {
                var _db_res = BInDB.assessors_master.First(a => a.AssessorsID == aID);
                BInDB.assessors_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Assessor:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }
        public List<AssessorsMasterViewModel> GetAllAssessors()
        {
            var res = BInDB.assessors_master.Where(x => x.IsActive == 1).OrderBy(a => a.AssessorsID).ToList();
            var lists = Mapper.Map<List<AssessorsMasterViewModel>>(res);
            return lists;
        }

        public List<AssessorsMasterViewModel> GetAllAssessorsBasedCompanyID(int? id)
        {
            var res = BInDB.assessors_master.Where(a => a.IsActive == 1 && a.CompanyID == id).ToList();
            var lists = Mapper.Map<List<AssessorsMasterViewModel>>(res);
            return lists;
        }

        public bool CheckAssessor(string Assessorsname)
        {
            try
            {
                var assessor = BInDB.assessors_master.Where(a => a.AssessorsName == Assessorsname).SingleOrDefault();
                if (assessor == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Assessors

        public List<AssessmentDevelopmentTypeMasterViewModel> GetAllDevelopmentTypes()
        {
            var res = BInDB.assessment_development_type_master.OrderBy(a => a.DevelopmentTypeID).ToList();
            var lists = Mapper.Map<List<AssessmentDevelopmentTypeMasterViewModel>>(res);
            return lists;
        }

        public List<AssessmentTypeMasterViewModel> GetAllAssessmentTypes()
        {
            var res = BInDB.assessment_type_master.OrderBy(a => a.AssessmentTypeID).ToList();
            var lists = Mapper.Map<List<AssessmentTypeMasterViewModel>>(res);
            return lists;
        }

        //Locations
        public AssessmentTypeLocationMasterViewModel GetLocation(int Id)
        {
            var Location = BInDB.assessment_type_location_master.Find(Id);
            return Mapper.Map<AssessmentTypeLocationMasterViewModel>(Location);
        }

        public int CreateLocation(AssessmentTypeLocationMasterViewModel Location)
        {
            var _db_res = Mapper.Map<assessment_type_location_master>(Location);
            _db_res.IsActive = 1;
            BInDB.assessment_type_location_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public int SaveLocation(AssessmentTypeLocationMasterViewModel Location)
        {
            var _db_res = Mapper.Map<assessment_type_location_master>(Location);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteLocation(int Id)
        {
            try
            {
                var _db_res = BInDB.assessment_type_location_master.First(a => a.AssessmentTypeLocationID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;
                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.assessment_type_location_master.Remove(_db_res);
                return BInDB.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Debug("Delete Location:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }
        public List<AssessmentTypeLocationMasterViewModel> GetAllAssessmentLocations()
        {
            var res = BInDB.assessment_type_location_master.Where(x => x.IsActive == 1).OrderBy(a => a.AssessmentTypeID).ThenBy(a => a.AssessmentTypeLocationID).ToList();
            var lists = Mapper.Map<List<AssessmentTypeLocationMasterViewModel>>(res);
            return lists;
        }

        public bool CheckLocation(int AssessmentTypeID, string LocationName)
        {
            try
            {
                var Location = BInDB.assessment_type_location_master.Where(a => a.AssessmentTypeID == AssessmentTypeID && a.AssessmentTypeLocationName == LocationName).SingleOrDefault();
                if (Location == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Locations

        //Modules
        public AssessmentTypeModuleMasterViewModel GetModule(int Id)
        {
            var Module = BInDB.assessment_type_module_master.Find(Id);
            return Mapper.Map<AssessmentTypeModuleMasterViewModel>(Module);
        }

        public int CreateModule(AssessmentTypeModuleMasterViewModel Module)
        {
            var _db_res = Mapper.Map<assessment_type_module_master>(Module);
            _db_res.IsActive = 1;
            BInDB.assessment_type_module_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public int SaveModule(AssessmentTypeModuleMasterViewModel Module)
        {
            var _db_res = Mapper.Map<assessment_type_module_master>(Module);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteModule(int Id)
        {
            try
            {
                var _db_res = BInDB.assessment_type_module_master.First(a => a.AssessmentTypeModuleID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.assessment_type_module_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Module:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<AssessmentTypeModuleMasterViewModel> GetAllModules()
        {
            var res = BInDB.assessment_type_module_master.Where(x => x.IsActive == 1).OrderBy(a => a.AssessmentTypeID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<AssessmentTypeModuleMasterViewModel>>(res);
            return lists;
        }

        public bool CheckModule(int? AssessmentTypeID, string ModuleName)
        {
            try
            {
                var Module = BInDB.assessment_type_module_master.Where(a => a.AssessmentTypeID == AssessmentTypeID && a.AssessmentTypeModuleName == ModuleName).SingleOrDefault();
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
        //Modules

        //Module Process
        public AssessmentTypeModuleProcessMasterViewModel GetModuleProcess(int Id)
        {
            var ModuleProcess = BInDB.assessment_type_module_Process_master.Find(Id);
            return Mapper.Map<AssessmentTypeModuleProcessMasterViewModel>(ModuleProcess);
        }

        public int CreateModuleProcess(AssessmentTypeModuleProcessMasterViewModel ModuleProcess)
        {
            var _db_res = Mapper.Map<assessment_type_module_Process_master>(ModuleProcess);
            _db_res.IsActive = 1;
            BInDB.assessment_type_module_Process_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public int SaveModuleProcess(AssessmentTypeModuleProcessMasterViewModel ModuleProcess)
        {
            var _db_res = Mapper.Map<assessment_type_module_Process_master>(ModuleProcess);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteModuleProcess(int Id)
        {
            try
            {
                var _db_res = BInDB.assessment_type_module_Process_master.First(a => a.AssessmentTypeModuleProcessID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.assessment_type_module_Process_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete ModuleProcess:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<AssessmentTypeModuleProcessMasterViewModel> GetAllModuleProcess()
        {
            var res = BInDB.assessment_type_module_Process_master.Where(x => x.IsActive == 1).OrderBy(a => a.AssessmentTypeModuleID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<AssessmentTypeModuleProcessMasterViewModel>>(res);
            return lists;
        }

        public List<AssessmentTypeModuleProcessMasterViewModel> GetAllModuleProcessByModuleIds(List<int> ids)
        {
            var res = BInDB.assessment_type_module_Process_master.Where(a => a.IsActive == 1 && ids.Contains(a.AssessmentTypeModuleID ?? default(int))).OrderBy(a => a.AssessmentTypeModuleID).ThenBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<AssessmentTypeModuleProcessMasterViewModel>>(res);
            return lists;
        }

        public bool CheckModuleProcess(int ModuleID, string ModuleProcessName)
        {
            try
            {
                var ModuleProcess = BInDB.assessment_type_module_Process_master.Where(a => a.AssessmentTypeModuleID == ModuleID && a.AssessmentTypeModuleProcessName == ModuleProcessName).SingleOrDefault();
                if (ModuleProcess == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Module Process


        // Directions
        public List<AssessmentDirectionMasterViewModel> GetAllDirections()
        {
            var res = BInDB.assessment_direction_master.OrderBy(a => a.AssessmentDirectionID).ToList();
            var lists = Mapper.Map<List<AssessmentDirectionMasterViewModel>>(res);
            return lists;
        }
        // Directions


        //Joints
        public AssessmentJointMasterViewModel GetJoint(int Id)
        {
            var Joint = BInDB.assessment_joint_master.Find(Id);
            return Mapper.Map<AssessmentJointMasterViewModel>(Joint);
        }

        public int CreateJoint(AssessmentJointMasterViewModel Joint)
        {
            var _db_res = Mapper.Map<assessment_joint_master>(Joint);
            _db_res.IsActive = 1;
            BInDB.assessment_joint_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public int SaveJoint(AssessmentJointMasterViewModel Joint)
        {
            var _db_res = Mapper.Map<assessment_joint_master>(Joint);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteJoint(int Id)
        {
            try
            {
                var _db_res = BInDB.assessment_joint_master.First(a => a.AssessmentJointID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;

                BInDB.assessment_joint_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Joint:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<AssessmentJointMasterViewModel> GetAllJoints()
        {
            var res = BInDB.assessment_joint_master.Where(x => x.IsActive == 1).OrderBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<AssessmentJointMasterViewModel>>(res);
            return lists;
        }

        public bool CheckJoint(string JointName)
        {
            try
            {
                var Joint = BInDB.assessment_joint_master.Where(a => a.AssessmentJointName == JointName).SingleOrDefault();
                if (Joint == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Joints

        //Leaks
        public AssessmentLeakMasterViewModel GetLeak(int Id)
        {
            var Leak = BInDB.assessment_leak_master.Find(Id);
            return Mapper.Map<AssessmentLeakMasterViewModel>(Leak);
        }

        public int CreateLeak(AssessmentLeakMasterViewModel Leak)
        {
            var _db_res = Mapper.Map<assessment_leak_master>(Leak);
            _db_res.IsActive = 1;
            BInDB.assessment_leak_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public int SaveLeak(AssessmentLeakMasterViewModel Leak)
        {
            var _db_res = Mapper.Map<assessment_leak_master>(Leak);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteLeak(int Id)
        {
            try
            {
                var _db_res = BInDB.assessment_leak_master.First(a => a.AssessmentLeakID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;

                BInDB.assessment_leak_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Leak:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<AssessmentLeakMasterViewModel> GetAllLeaks()
        {
            var res = BInDB.assessment_leak_master.Where(x => x.IsActive == 1).OrderBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<AssessmentLeakMasterViewModel>>(res);
            return lists;
        }

        public bool CheckLeak(string LeakName)
        {
            try
            {
                var Leak = BInDB.assessment_leak_master.Where(a => a.AssessmentLeakName == LeakName).SingleOrDefault();
                if (Leak == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Leaks

        //Walls
        public AssessmentWallMasterViewModel GetWall(int Id)
        {
            var Wall = BInDB.assessment_wall_master.Find(Id);
            return Mapper.Map<AssessmentWallMasterViewModel>(Wall);
        }

        public int CreateWall(AssessmentWallMasterViewModel Wall)
        {
            var _db_res = Mapper.Map<assessment_wall_master>(Wall);
            _db_res.IsActive = 1;
            BInDB.assessment_wall_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public int SaveWall(AssessmentWallMasterViewModel Wall)
        {
            var _db_res = Mapper.Map<assessment_wall_master>(Wall);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteWall(int Id)
        {
            try
            {
                var _db_res = BInDB.assessment_wall_master.First(a => a.AssessmentWallID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.assessment_wall_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Wall:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<AssessmentWallMasterViewModel> GetAllWalls()
        {
            var res = BInDB.assessment_wall_master.Where(x => x.IsActive == 1).OrderBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<AssessmentWallMasterViewModel>>(res);
            return lists;
        }

        public bool CheckWall(string WallName)
        {
            try
            {
                var Wall = BInDB.assessment_wall_master.Where(a => a.AssessmentWallName == WallName).SingleOrDefault();
                if (Wall == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Walls

        //Windows
        public AssessmentWindowMasterViewModel GetWindow(int Id)
        {
            var Window = BInDB.assessment_window_master.Find(Id);
            return Mapper.Map<AssessmentWindowMasterViewModel>(Window);
        }

        public int CreateWindow(AssessmentWindowMasterViewModel Window)
        {
            var _db_res = Mapper.Map<assessment_window_master>(Window);
            _db_res.IsActive = 1;
            BInDB.assessment_window_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public int SaveWindow(AssessmentWindowMasterViewModel Window)
        {
            var _db_res = Mapper.Map<assessment_window_master>(Window);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteWindow(int Id)
        {
            try
            {
                var _db_res = BInDB.assessment_window_master.First(a => a.AssessmentWindowID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.assessment_window_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Window:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<AssessmentWindowMasterViewModel> GetAllWindows()
        {
            var res = BInDB.assessment_window_master.Where(x => x.IsActive == 1).OrderBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<AssessmentWindowMasterViewModel>>(res);
            return lists;
        }

        public bool CheckWindow(string WindowName)
        {
            try
            {
                var Window = BInDB.assessment_window_master.Where(a => a.AssessmentWindowName == WindowName).SingleOrDefault();
                if (Window == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Windows


        //Wet Area Water Tightness Test Result
        public AssessmentWetAreaWaterTightnessTestResultMasterViewModel GetWetAreaWaterTightnessTestResult(int Id)
        {
            var Window = BInDB.assessment_wet_area_water_tightness_test_result_master.Find(Id);
            return Mapper.Map<AssessmentWetAreaWaterTightnessTestResultMasterViewModel>(Window);
        }

        public int CreateWetAreaWaterTightnessTestResult(AssessmentWetAreaWaterTightnessTestResultMasterViewModel WAWTTResult)
        {
            var _db_res = Mapper.Map<assessment_wet_area_water_tightness_test_result_master>(WAWTTResult);
            _db_res.IsActive = 1;
            BInDB.assessment_wet_area_water_tightness_test_result_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public int SaveWetAreaWaterTightnessTestResult(AssessmentWetAreaWaterTightnessTestResultMasterViewModel WAWTTResult)
        {
            var _db_res = Mapper.Map<assessment_wet_area_water_tightness_test_result_master>(WAWTTResult);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteWetAreaWaterTightnessTestResult(int Id)
        {
            try
            {
                var _db_res = BInDB.assessment_wet_area_water_tightness_test_result_master.First(a => a.AssessmentWAWTTResultID == Id);
                //_db_res.UpdatedBy = AppSession.GetCurrentUserId();
                //_db_res.UpdatedDate = DateTime.Now;
                //_db_res.IsActive = 0;

                //BInDB.Entry(_db_res).State = EntityState.Modified;
                BInDB.assessment_wet_area_water_tightness_test_result_master.Remove(_db_res);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Wet Area Water Tightness Test Result:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<AssessmentWetAreaWaterTightnessTestResultMasterViewModel> GetAllWetAreaWaterTightnessTestResults()
        {
            var res = BInDB.assessment_wet_area_water_tightness_test_result_master.Where(x => x.IsActive == 1).OrderBy(a => a.OrderBy).ToList();
            var lists = Mapper.Map<List<AssessmentWetAreaWaterTightnessTestResultMasterViewModel>>(res);
            return lists;
        }

        public bool CheckWetAreaWaterTightnessTestResult(string WAWTTResultName)
        {
            try
            {
                var Window = BInDB.assessment_wet_area_water_tightness_test_result_master.Where(a => a.AssessmentWAWTTResult == WAWTTResultName).SingleOrDefault();
                if (Window == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Wet Area Water Tightness Test Result

        public List<MasterSyncViewModel> GetAssessmentMasterSync()
        {
            var res = BInDB.Database.SqlQuery<MasterSyncViewModel>("SELECT * FROM vw_Assessment_Master_Sync").ToList();
            var lists = Mapper.Map<List<MasterSyncViewModel>>(res);
            return lists;
        }

        // Masters

        // Transactions
        public List<AssessmentSummaryDetailModel> GetAssessmentSummaryByProjectID(int? id)
        {
            var res = BInDB.Database.SqlQuery<AssessmentSummaryDetailModel>("SELECT * FROM vw_Assessment_Summary_List WHERE ProjectID =" + id.ToString() + " ORDER BY ProjectID,AssessmentTypeModuleID,OrderBy").ToList();
            var lists = Mapper.Map<List<AssessmentSummaryDetailModel>>(res);
            return lists;
        }

        // Internal Finishes
        public List<AssessmentInternalFinishesIndexViewModel> GetAllAssessmentInternalFinishes_List(int? id)
        {
            var res = BInDB.Database.SqlQuery<AssessmentInternalFinishesIndexViewModel>("SELECT * FROM vw_Assessment_Internal_Finishes_List WHERE CompanyID =" + id.ToString() + " ORDER BY ProjectID").ToList();
            var lists = Mapper.Map<List<AssessmentInternalFinishesIndexViewModel>>(res);
            return lists;
        }

        public List<AssessmentInternalFinishesTransMasterViewModel> GetAllAssessmentInternalFinishes(int id, string BatchID = "")
        {
            if (BatchID.Length == 0)
            {
                var res = BInDB.assessment_internal_finishes_trn.Where(a => a.ProjectID == id).ToList();
                var lists = Mapper.Map<List<AssessmentInternalFinishesTransMasterViewModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.assessment_internal_finishes_trn.Where(a => a.ProjectID == id && a.BatchID == BatchID).ToList();
                var lists = Mapper.Map<List<AssessmentInternalFinishesTransMasterViewModel>>(res);
                return lists;
            }
        }

        public AssessmentInternalFinishesTransMasterViewModel GetAllAssessmentInternalFinishes_ByID(int id)
        {
            var res = BInDB.assessment_internal_finishes_trn.Where(a => a.AssessmentIFID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentInternalFinishesTransMasterViewModel>(res);
            return lists;
        }

        public List<AssessmentInternalFinishesTransDetailViewModel> GetAllAssessmentInternalFinishes_Detail(List<int> ids)
        {
            var res = BInDB.assessment_internal_finishes_trn_detail.Where(a => ids.Contains(a.AssessmentIFID ?? default(int))).ToList();
            var lists = Mapper.Map<List<AssessmentInternalFinishesTransDetailViewModel>>(res);
            return lists;
        }

        public AssessmentInternalFinishesTransDetailViewModel GetAllAssessmentInternalFinishes_DetailByID(int id)
        {
            var res = BInDB.assessment_internal_finishes_trn_detail.Where(a => a.AssessmentIFDetailID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentInternalFinishesTransDetailViewModel>(res);
            return lists;
        }

        public int CreateAssessmentInternalFinishesMaster(AssessmentInternalFinishesTransMasterViewModel masterViewModel, List<AssessmentInternalFinishesTransDetailViewModel> detailViewModels)
        {
            try
            {
                assessment_internal_finishes_trn _db_res_Master = new assessment_internal_finishes_trn();
                _db_res_Master.ProjectID = masterViewModel.ProjectID;
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.LocationID = masterViewModel.LocationID;
                _db_res_Master.MobileAssessmentIFID = masterViewModel.MobileAssessmentIFID;
                _db_res_Master.BatchID = masterViewModel.BatchID;
                _db_res_Master.CreatedBy = masterViewModel.CreatedBy;
                _db_res_Master.CreatedDate = DateTime.Now;
                foreach (var desc in detailViewModels)
                {
                    assessment_internal_finishes_trn_detail domainIFDetail = Mapper.Map<assessment_internal_finishes_trn_detail>(desc);
                    domainIFDetail.AssessmentIFID = _db_res_Master.AssessmentIFID;
                    BInDB.assessment_internal_finishes_trn_detail.Add(domainIFDetail);
                }
                BInDB.assessment_internal_finishes_trn.Add(_db_res_Master);

                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Assessment Internal Finishes Master:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentInternalFinishes(AssessmentInternalFinishesTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_internal_finishes_trn.Where(a => a.AssessmentIFID == masterViewModel.AssessmentIFID).FirstOrDefault();
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.LocationID = masterViewModel.LocationID;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;

                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                foreach (var detailViewModel in masterViewModel.assessment_internal_finishes_trn_detail)
                {
                    var _db_res_Detail = BInDB.assessment_internal_finishes_trn_detail.Where(a => a.AssessmentIFDetailID == detailViewModel.AssessmentIFDetailID).FirstOrDefault();
                    _db_res_Detail.Result = detailViewModel.Result;
                    _db_res_Detail.UpdatedBy = detailViewModel.UpdatedBy;
                    _db_res_Detail.UpdatedDate = DateTime.Now;
                    BInDB.Entry(_db_res_Detail).State = EntityState.Modified;
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Internal Finishes:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentInternalFinishesDetail(AssessmentInternalFinishesTransDetailViewModel detailViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_internal_finishes_trn_detail.Where(a => a.AssessmentIFDetailID == detailViewModel.AssessmentIFDetailID).FirstOrDefault();
                _db_res.Result = detailViewModel.Result;
                _db_res.UpdatedBy = detailViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Internal Finishes Detail:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteAssessmentInternalFinishes(string Ids)
        {
            try
            {
                int Id = 0;
                foreach (string EWmaster in Ids.Split(','))
                {
                    Id = int.Parse(EWmaster);
                    var _db_res = BInDB.assessment_internal_finishes_trn.Find(Id);
                    var IDids = BInDB.assessment_internal_finishes_trn_detail.Where(a => a.AssessmentIFID == Id).Select(a => a.AssessmentIFDetailID).ToList();
                    foreach (var det in IDids)
                    {
                        var deleteObj = BInDB.assessment_internal_finishes_trn_detail.Find(det);
                        BInDB.assessment_internal_finishes_trn_detail.Remove(deleteObj);
                    }
                    BInDB.assessment_internal_finishes_trn.Remove(_db_res);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Assessment Internal Finishes Master:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public List<AssessmentReportDetailModel> GetAssessmentInternalFinishesByProjectAndModuleID(int? pid, int? mid)
        {
            if (mid == -1)
            {
                var res = BInDB.Database.SqlQuery<AssessmentReportDetailModel>("SELECT * FROM vw_Assessment_Internal_Finishes_Module_Chart_Report WHERE ProjectID =" + pid.ToString() + " ORDER BY ProjectID,AssessmentTypeModuleID,OrderBy").ToList();
                var lists = Mapper.Map<List<AssessmentReportDetailModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.Database.SqlQuery<AssessmentReportDetailModel>("SELECT * FROM vw_Assessment_Internal_Finishes_Process_Chart_Report WHERE ProjectID =" + pid.ToString() + " AND AssessmentTypeModuleID = " + mid + " ORDER BY ProjectID,AssessmentTypeModuleID,OrderBy").ToList();
                var lists = Mapper.Map<List<AssessmentReportDetailModel>>(res);
                return lists;
            }
        }

        public bool CheckAssessmentInternalFinishes(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate)
        {
            try
            {
                var internalFinishe = BInDB.assessment_internal_finishes_trn.Where(a => a.ProjectID == pid && a.AssessmentDate == AssessmentDate && a.LocationID == LocationID && a.Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim() == Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim()).SingleOrDefault();
                if (internalFinishe == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        // Internal Finishes

        // External Wall
        public List<AssessmentExternalWallIndexViewModel> GetAllAssessmentExternalWall_List(int? id)
        {
            var res = BInDB.Database.SqlQuery<AssessmentExternalWallIndexViewModel>("SELECT * FROM vw_Assessment_ExternalWall_List WHERE CompanyID =" + id.ToString() + " ORDER BY ProjectID").ToList();
            var lists = Mapper.Map<List<AssessmentExternalWallIndexViewModel>>(res);
            return lists;
        }

        public List<AssessmentExternalWallTransMasterViewModel> GetAllAssessmentExternalWall(int id, string BatchID = "")
        {
            if (BatchID.Length == 0)
            {
                var res = BInDB.assessment_external_wall_trn.Where(a => a.ProjectID == id).ToList();
                var lists = Mapper.Map<List<AssessmentExternalWallTransMasterViewModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.assessment_external_wall_trn.Where(a => a.ProjectID == id && a.BatchID == BatchID).ToList();
                var lists = Mapper.Map<List<AssessmentExternalWallTransMasterViewModel>>(res);
                return lists;
            }
        }

        public AssessmentExternalWallTransMasterViewModel GetAllAssessmentExternalWall_ByID(int id)
        {
            var res = BInDB.assessment_external_wall_trn.Where(a => a.AssessmentEWID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentExternalWallTransMasterViewModel>(res);
            return lists;
        }

        public List<AssessmentExternalWallTransDetailViewModel> GetAllAssessmentExternalWall_Detail(List<int> ids)
        {
            var res = BInDB.assessment_external_wall_trn_detail.Where(a => ids.Contains(a.AssessmentEWID ?? default(int))).ToList();
            var lists = Mapper.Map<List<AssessmentExternalWallTransDetailViewModel>>(res);
            return lists;
        }

        public AssessmentExternalWallTransDetailViewModel GetAllAssessmentExternalWall_DetailByID(int id)
        {
            var res = BInDB.assessment_external_wall_trn_detail.Where(a => a.AssessmentEWDetailID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentExternalWallTransDetailViewModel>(res);
            return lists;
        }

        public int CreateAssessmentExternalWallMaster(AssessmentExternalWallTransMasterViewModel masterViewModel, List<AssessmentExternalWallTransDetailViewModel> detailViewModels)
        {
            try
            {
                assessment_external_wall_trn _db_res_Master = new assessment_external_wall_trn();
                _db_res_Master.ProjectID = masterViewModel.ProjectID;
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.LocationID = masterViewModel.LocationID;
                _db_res_Master.MobileAssessmentEWID = masterViewModel.MobileAssessmentEWID;
                _db_res_Master.BatchID = masterViewModel.BatchID;
                _db_res_Master.CreatedBy = masterViewModel.CreatedBy;
                _db_res_Master.CreatedDate = DateTime.Now;
                foreach (var desc in detailViewModels)
                {
                    assessment_external_wall_trn_detail domainIFDetail = Mapper.Map<assessment_external_wall_trn_detail>(desc);
                    domainIFDetail.AssessmentEWID = _db_res_Master.AssessmentEWID;
                    BInDB.assessment_external_wall_trn_detail.Add(domainIFDetail);
                }
                BInDB.assessment_external_wall_trn.Add(_db_res_Master);

                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Assessment External Wall:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentExternalWall(AssessmentExternalWallTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_external_wall_trn.Where(a => a.AssessmentEWID == masterViewModel.AssessmentEWID).FirstOrDefault();
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.LocationID = masterViewModel.LocationID;
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                foreach (var detailViewModel in masterViewModel.assessment_external_wall_trn_detail)
                {
                    var _db_res_Detail = BInDB.assessment_external_wall_trn_detail.Where(a => a.AssessmentEWDetailID == detailViewModel.AssessmentEWDetailID).FirstOrDefault();
                    _db_res_Detail.AssessmentTypeModuleProcessID = detailViewModel.AssessmentTypeModuleProcessID;
                    _db_res_Detail.Result = detailViewModel.Result;
                    _db_res_Detail.UpdatedBy = detailViewModel.UpdatedBy;
                    _db_res_Detail.UpdatedDate = DateTime.Now;
                    BInDB.Entry(_db_res_Detail).State = EntityState.Modified;
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment External Wall:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentExternalWallSignature(AssessmentExternalWallTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_external_wall_trn.Where(a => a.AssessmentEWID == masterViewModel.AssessmentEWID).FirstOrDefault();
                _db_res.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Assessment External Wall Signature:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentExternalWallDetail(AssessmentExternalWallTransDetailViewModel detailViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_external_wall_trn_detail.Where(a => a.AssessmentEWDetailID == detailViewModel.AssessmentEWDetailID).FirstOrDefault();
                _db_res.Result = detailViewModel.Result;
                _db_res.UpdatedBy = detailViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment External Wall:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteAssessmentExternalWall(string Ids)
        {
            try
            {
                int Id = 0;
                foreach (string EWmaster in Ids.Split(','))
                {
                    Id = int.Parse(EWmaster);
                    var _db_res = BInDB.assessment_external_wall_trn.Find(Id);
                    var IDids = BInDB.assessment_external_wall_trn_detail.Where(a => a.AssessmentEWID == Id).Select(a => a.AssessmentEWDetailID).ToList();
                    foreach (var det in IDids)
                    {
                        var deleteObj = BInDB.assessment_external_wall_trn_detail.Find(det);
                        BInDB.assessment_external_wall_trn_detail.Remove(deleteObj);
                    }
                    BInDB.assessment_external_wall_trn.Remove(_db_res);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Assessment External Wall:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public bool CheckAssessmentExternalWall(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate)
        {
            try
            {
                var internalFinishe = BInDB.assessment_external_wall_trn.Where(a => a.ProjectID == pid && a.AssessmentDate == AssessmentDate && a.LocationID == LocationID && a.Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim() == Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim()).SingleOrDefault();
                if (internalFinishe == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        // External Wall

        // External Works
        public List<AssessmentExternalWorksIndexViewModel> GetAllAssessmentExternalWorks_List(int? id)
        {
            var res = BInDB.Database.SqlQuery<AssessmentExternalWorksIndexViewModel>("SELECT * FROM vw_Assessment_ExternalWorks_List WHERE CompanyID =" + id.ToString() + " ORDER BY ProjectID").ToList();
            var lists = Mapper.Map<List<AssessmentExternalWorksIndexViewModel>>(res);
            return lists;
        }

        public List<AssessmentExternalWorksTransMasterViewModel> GetAllAssessmentExternalWorks(int id, string BatchID = "")
        {
            if (BatchID.Length == 0)
            {
                var res = BInDB.assessment_external_works_trn.Where(a => a.ProjectID == id).ToList();
                var lists = Mapper.Map<List<AssessmentExternalWorksTransMasterViewModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.assessment_external_works_trn.Where(a => a.ProjectID == id && a.BatchID == BatchID).ToList();
                var lists = Mapper.Map<List<AssessmentExternalWorksTransMasterViewModel>>(res);
                return lists;
            }
        }

        public AssessmentExternalWorksTransMasterViewModel GetAllAssessmentExternalWorks_ByID(int id)
        {
            var res = BInDB.assessment_external_works_trn.Where(a => a.AssessmentEWKID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentExternalWorksTransMasterViewModel>(res);
            return lists;
        }

        public List<AssessmentExternalWorksTransDetailViewModel> GetAllAssessmentExternalWorks_Detail(List<int> ids)
        {
            var res = BInDB.assessment_external_works_trn_detail.Where(a => ids.Contains(a.AssessmentEWKID ?? default(int))).ToList();
            var lists = Mapper.Map<List<AssessmentExternalWorksTransDetailViewModel>>(res);
            return lists;
        }

        public AssessmentExternalWorksTransDetailViewModel GetAllAssessmentExternalWorks_DetailByID(int id)
        {
            var res = BInDB.assessment_external_works_trn_detail.Where(a => a.AssessmentEWKDetailID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentExternalWorksTransDetailViewModel>(res);
            return lists;
        }

        public int CreateAssessmentExternalWorksMaster(AssessmentExternalWorksTransMasterViewModel masterViewModel, List<AssessmentExternalWorksTransDetailViewModel> detailViewModels)
        {
            try
            {
                assessment_external_works_trn _db_res_Master = new assessment_external_works_trn();
                _db_res_Master.ProjectID = masterViewModel.ProjectID;
                _db_res_Master.Remarks = masterViewModel.Remarks;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.LocationID = masterViewModel.LocationID;
                _db_res_Master.MobileAssessmentEWKID = masterViewModel.MobileAssessmentEWKID;
                _db_res_Master.BatchID = masterViewModel.BatchID;
                _db_res_Master.CreatedBy = masterViewModel.CreatedBy;
                _db_res_Master.CreatedDate = DateTime.Now;
                foreach (var desc in detailViewModels)
                {
                    assessment_external_works_trn_detail domainIFDetail = Mapper.Map<assessment_external_works_trn_detail>(desc);
                    domainIFDetail.AssessmentEWKID = _db_res_Master.AssessmentEWKID;
                    _db_res_Master.assessment_external_works_trn_detail.Add(domainIFDetail);
                }
                BInDB.assessment_external_works_trn.Add(_db_res_Master);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Assessment External Works:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentExternalWorks(AssessmentExternalWorksTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_external_works_trn.Where(a => a.AssessmentEWKID == masterViewModel.AssessmentEWKID).FirstOrDefault();
                _db_res_Master.Remarks = masterViewModel.Remarks;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.LocationID = masterViewModel.LocationID;
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                foreach (var detailViewModel in masterViewModel.assessment_external_works_trn_detail)
                {
                    var _db_res_Detail = BInDB.assessment_external_works_trn_detail.Where(a => a.AssessmentEWKDetailID == detailViewModel.AssessmentEWKDetailID).FirstOrDefault();
                    _db_res_Detail.AssessmentTypeModuleProcessID = detailViewModel.AssessmentTypeModuleProcessID;
                    _db_res_Detail.Result = detailViewModel.Result;
                    _db_res_Detail.UpdatedBy = detailViewModel.UpdatedBy;
                    _db_res_Detail.UpdatedDate = DateTime.Now;
                    BInDB.Entry(_db_res_Detail).State = EntityState.Modified;
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment External Works:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentExternalWorksDetail(AssessmentExternalWorksTransDetailViewModel detailViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_external_works_trn_detail.Where(a => a.AssessmentEWKDetailID == detailViewModel.AssessmentEWKDetailID).FirstOrDefault();
                _db_res.Result = detailViewModel.Result;
                _db_res.UpdatedBy = detailViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment External Works:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentExternalWorksSignature(AssessmentExternalWorksTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_external_works_trn.Where(a => a.AssessmentEWKID == masterViewModel.AssessmentEWKID).FirstOrDefault();
                _db_res.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Assessment External Works Signature:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteAssessmentExternalWorks(string Ids)
        {
            try
            {
                int Id = 0;
                foreach (string EWmaster in Ids.Split(','))
                {
                    Id = int.Parse(EWmaster);
                    var _db_res = BInDB.assessment_external_works_trn.Find(Id);
                    var IDids = BInDB.assessment_external_works_trn_detail.Where(a => a.AssessmentEWKID == Id).Select(a => a.AssessmentEWKDetailID).ToList();
                    foreach (var det in IDids)
                    {
                        var deleteObj = BInDB.assessment_external_works_trn_detail.Find(det);
                        BInDB.assessment_external_works_trn_detail.Remove(deleteObj);
                    }
                    BInDB.assessment_external_works_trn.Remove(_db_res);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Assessment External Works:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public bool CheckAssessmentExternalWorks(int pid, int LocationID, string Remarks, DateTime AssessmentDate)
        {
            try
            {
                var internalFinishe = BInDB.assessment_external_works_trn.Where(a => a.Remarks == Remarks && a.ProjectID == pid && a.AssessmentDate == AssessmentDate && a.LocationID == LocationID).SingleOrDefault();
                if (internalFinishe == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        // External Works


        // Roof Construction
        public List<AssessmentRoofConstructionIndexViewModel> GetAllAssessmentRoofConstruction_List(int? id)
        {
            var res = BInDB.Database.SqlQuery<AssessmentRoofConstructionIndexViewModel>("SELECT * FROM vw_Assessment_RoofConstruction_List WHERE CompanyID =" + id.ToString() + " ORDER BY ProjectID").ToList();
            var lists = Mapper.Map<List<AssessmentRoofConstructionIndexViewModel>>(res);
            return lists;
        }

        public List<AssessmentRoofConstructionTransMasterViewModel> GetAllAssessmentRoofConstruction(int id, string BatchID = "")
        {
            if (BatchID.Length == 0)
            {
                var res = BInDB.assessment_roof_construction_trn.Where(a => a.ProjectID == id).ToList();
                var lists = Mapper.Map<List<AssessmentRoofConstructionTransMasterViewModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.assessment_roof_construction_trn.Where(a => a.ProjectID == id && a.BatchID == BatchID).ToList();
                var lists = Mapper.Map<List<AssessmentRoofConstructionTransMasterViewModel>>(res);
                return lists;
            }
        }

        public AssessmentRoofConstructionTransMasterViewModel GetAllAssessmentRoofConstruction_ByID(int id)
        {
            var res = BInDB.assessment_roof_construction_trn.Where(a => a.AssessmentRFCID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentRoofConstructionTransMasterViewModel>(res);
            return lists;
        }

        public List<AssessmentRoofConstructionTransDetailViewModel> GetAllAssessmentRoofConstruction_Detail(List<int> ids)
        {
            var res = BInDB.assessment_roof_construction_trn_detail.Where(a => ids.Contains(a.AssessmentRFCID ?? default(int))).ToList();
            var lists = Mapper.Map<List<AssessmentRoofConstructionTransDetailViewModel>>(res);
            return lists;
        }

        public AssessmentRoofConstructionTransDetailViewModel GetAllAssessmentRoofConstruction_DetailByID(int id)
        {
            var res = BInDB.assessment_roof_construction_trn_detail.Where(a => a.AssessmentRFCDetailID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentRoofConstructionTransDetailViewModel>(res);
            return lists;
        }

        public int CreateAssessmentRoofConstructionMaster(AssessmentRoofConstructionTransMasterViewModel masterViewModel, List<AssessmentRoofConstructionTransDetailViewModel> detailViewModels)
        {
            try
            {
                assessment_roof_construction_trn _db_res_Master = new assessment_roof_construction_trn();
                _db_res_Master.ProjectID = masterViewModel.ProjectID;
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.LocationID = masterViewModel.LocationID;
                _db_res_Master.MobileAssessmentRFCID = masterViewModel.MobileAssessmentRFCID;
                _db_res_Master.BatchID = masterViewModel.BatchID;
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.CreatedBy = masterViewModel.CreatedBy;
                _db_res_Master.CreatedDate = DateTime.Now;
                foreach (var desc in detailViewModels)
                {
                    assessment_roof_construction_trn_detail _db_res_Detail = Mapper.Map<assessment_roof_construction_trn_detail>(desc);
                    _db_res_Detail.AssessmentRFCID = _db_res_Master.AssessmentRFCID;
                    _db_res_Detail.Result = desc.Result;
                    _db_res_Detail.UpdatedBy = masterViewModel.CreatedBy;
                    _db_res_Detail.UpdatedDate = DateTime.Now;
                    _db_res_Master.assessment_roof_construction_trn_detail.Add(_db_res_Detail);
                }
                BInDB.assessment_roof_construction_trn.Add(_db_res_Master);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Assessment Roof Construction Master:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentRoofConstruction(AssessmentRoofConstructionTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_roof_construction_trn.Where(a => a.AssessmentRFCID == masterViewModel.AssessmentRFCID).FirstOrDefault();
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.LocationID = masterViewModel.LocationID;
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                foreach (var detailViewModel in masterViewModel.assessment_roof_construction_trn_detail)
                {
                    var _db_res_Detail = BInDB.assessment_roof_construction_trn_detail.Where(a => a.AssessmentRFCDetailID == detailViewModel.AssessmentRFCDetailID).FirstOrDefault();
                    _db_res_Detail.AssessmentTypeModuleProcessID = detailViewModel.AssessmentTypeModuleProcessID;
                    _db_res_Detail.Result = detailViewModel.Result;
                    _db_res_Detail.UpdatedBy = detailViewModel.UpdatedBy;
                    _db_res_Detail.UpdatedDate = DateTime.Now;
                    BInDB.Entry(_db_res_Detail).State = EntityState.Modified;
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Roof Construction:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentRoofConstructionDetail(AssessmentRoofConstructionTransDetailViewModel detailViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_roof_construction_trn_detail.Where(a => a.AssessmentRFCDetailID == detailViewModel.AssessmentRFCDetailID).FirstOrDefault();
                _db_res.Result = detailViewModel.Result;
                _db_res.UpdatedBy = detailViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Roof Construction Detail:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentRoofConstructionSignature(AssessmentRoofConstructionTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_roof_construction_trn.Where(a => a.AssessmentRFCID == masterViewModel.AssessmentRFCID).FirstOrDefault();
                _db_res.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Roof Construction Signature:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteAssessmentRoofConstruction(string Ids)
        {
            try
            {
                int Id = 0;
                foreach (string EWmaster in Ids.Split(','))
                {
                    Id = int.Parse(EWmaster);
                    var _db_res = BInDB.assessment_roof_construction_trn.Find(Id);
                    var IDids = BInDB.assessment_roof_construction_trn_detail.Where(a => a.AssessmentRFCID == Id).Select(a => a.AssessmentRFCDetailID).ToList();
                    foreach (var det in IDids)
                    {
                        var deleteObj = BInDB.assessment_roof_construction_trn_detail.Find(det);
                        BInDB.assessment_roof_construction_trn_detail.Remove(deleteObj);
                    }
                    BInDB.assessment_roof_construction_trn.Remove(_db_res);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Assessment Roof Construction:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public bool CheckAssessmentRoofConstruction(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate)
        {
            try
            {
                var internalFinishe = BInDB.assessment_roof_construction_trn.Where(a => a.ProjectID == pid && a.AssessmentDate == AssessmentDate && a.LocationID == LocationID && a.Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim() == Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim()).SingleOrDefault();
                if (internalFinishe == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        // Roof Construction

        // Field Window Water Tightness Test
        public List<AssessmentFieldWindowWaterTightnessTestIndexViewModel> GetAllAssessmentFieldWindowWaterTightnessTest_List(int? id)
        {
            var res = BInDB.Database.SqlQuery<AssessmentFieldWindowWaterTightnessTestIndexViewModel>("SELECT * FROM vw_Assessment_FieldWindowWaterTightnessTest_List WHERE CompanyID =" + id.ToString() + " ORDER BY ProjectID").ToList();
            var lists = Mapper.Map<List<AssessmentFieldWindowWaterTightnessTestIndexViewModel>>(res);
            return lists;
        }

        public List<AssessmentFieldWindowWaterTightnessTestTransViewModel> GetAllAssessmentFieldWindowWaterTightnessTest(int id, string BatchID = "")
        {
            if (BatchID.Length == 0)
            {
                var res = BInDB.assessment_field_window_water_tightness_test.Where(a => a.ProjectID == id).ToList();
                var lists = Mapper.Map<List<AssessmentFieldWindowWaterTightnessTestTransViewModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.assessment_field_window_water_tightness_test.Where(a => a.ProjectID == id && a.BatchID == BatchID).ToList();
                var lists = Mapper.Map<List<AssessmentFieldWindowWaterTightnessTestTransViewModel>>(res);
                return lists;
            }
        }

        public AssessmentFieldWindowWaterTightnessTestTransViewModel GetAllAssessmentFieldWindowWaterTightnessTest_ByID(int id)
        {
            var res = BInDB.assessment_field_window_water_tightness_test.Where(a => a.AssessmentFWWTTID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentFieldWindowWaterTightnessTestTransViewModel>(res);
            return lists;
        }

        public int CreateAssessmentFieldWindowWaterTightnessTest(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel)
        {
            try
            {
                assessment_field_window_water_tightness_test _db_res_Master = new assessment_field_window_water_tightness_test();
                _db_res_Master.ProjectID = masterViewModel.ProjectID;
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.AssessmentWallID = masterViewModel.AssessmentWallID;
                _db_res_Master.AssessmentWindowID = masterViewModel.AssessmentWindowID;
                _db_res_Master.AssessmentJointID = masterViewModel.AssessmentJointID;
                _db_res_Master.AssessmentDirectionID = masterViewModel.AssessmentDirectionID;
                _db_res_Master.AssessmentLeakID = masterViewModel.AssessmentLeakID;
                _db_res_Master.Result = masterViewModel.Result;
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.MobileAssessmentFWWTTID = masterViewModel.MobileAssessmentFWWTTID;
                _db_res_Master.BatchID = masterViewModel.BatchID;
                _db_res_Master.CreatedBy = masterViewModel.CreatedBy;
                _db_res_Master.CreatedDate = DateTime.Now;
                BInDB.assessment_field_window_water_tightness_test.Add(_db_res_Master);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Assessment Field Window Water Tightness Test:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentFieldWindowWaterTightnessTest(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_field_window_water_tightness_test.Find(masterViewModel.AssessmentFWWTTID);
                _db_res_Master.ProjectID = masterViewModel.ProjectID;
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.AssessmentWallID = masterViewModel.AssessmentWallID;
                _db_res_Master.AssessmentWindowID = masterViewModel.AssessmentWindowID;
                _db_res_Master.AssessmentJointID = masterViewModel.AssessmentJointID;
                _db_res_Master.AssessmentDirectionID = masterViewModel.AssessmentDirectionID;
                _db_res_Master.AssessmentLeakID = masterViewModel.AssessmentLeakID;
                _db_res_Master.Result = masterViewModel.Result;
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Field Window Water Tightness Test:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentFieldWindowWaterTightnessTestSignature(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_field_window_water_tightness_test.Find(masterViewModel.AssessmentFWWTTID);
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Field Window Water Tightness Test Signature:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteAssessmentFieldWindowWaterTightnessTest(string Ids)
        {
            try
            {
                int Id = 0;
                foreach (string EWmaster in Ids.Split(','))
                {
                    Id = int.Parse(EWmaster);
                    var _db_res = BInDB.assessment_field_window_water_tightness_test.Find(Id);
                    BInDB.assessment_field_window_water_tightness_test.Remove(_db_res);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Assessment Field Window Water Tightness Test:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public bool CheckAssessmentFieldWindowWaterTightnessTest(int pid, string Block_Unit, DateTime AssessmentDate, int WallId, int WindowId, int JointId, int DirectionId, int LeakId)
        {
            try
            {
                var internalFinishe = BInDB.assessment_field_window_water_tightness_test.Where(a => a.ProjectID == pid && a.AssessmentDate == AssessmentDate && a.Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim() == Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim() && a.AssessmentWallID == WallId && a.AssessmentWindowID == WindowId && a.AssessmentJointID == JointId && a.AssessmentDirectionID == DirectionId && a.AssessmentLeakID == LeakId).SingleOrDefault();
                if (internalFinishe == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        // Field Window Water Tightness Test

        // Wet Area Water Tightness Test
        public List<AssessmentWetAreaWaterTightnessTestIndexViewModel> GetAllAssessmentWetAreaWaterTightnessTest_List(int? id)
        {
            var res = BInDB.Database.SqlQuery<AssessmentWetAreaWaterTightnessTestIndexViewModel>("SELECT * FROM vw_Assessment_WetAreaWaterTightnessTest_List WHERE CompanyID =" + id.ToString() + " ORDER BY ProjectID").ToList();
            var lists = Mapper.Map<List<AssessmentWetAreaWaterTightnessTestIndexViewModel>>(res);
            return lists;
        }

        public List<AssessmentWetAreaWaterTightnessTestTransMasterViewModel> GetAllAssessmentWetAreaWaterTightnessTest(int id, string BatchID = "")
        {
            if (BatchID.Length == 0)
            {
                var res = BInDB.assessment_wet_area_water_tightness_test_tran.Where(a => a.ProjectID == id).ToList();
                var lists = Mapper.Map<List<AssessmentWetAreaWaterTightnessTestTransMasterViewModel>>(res);
                return lists;
            }
            else
            {
                var res = BInDB.assessment_wet_area_water_tightness_test_tran.Where(a => a.ProjectID == id && a.BatchID == BatchID).ToList();
                var lists = Mapper.Map<List<AssessmentWetAreaWaterTightnessTestTransMasterViewModel>>(res);
                return lists;
            }
        }

        public AssessmentWetAreaWaterTightnessTestTransMasterViewModel GetAllAssessmentWetAreaWaterTightnessTest_ByID(int id)
        {
            var res = BInDB.assessment_wet_area_water_tightness_test_tran.Where(a => a.AssessmentWAWTTID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentWetAreaWaterTightnessTestTransMasterViewModel>(res);
            return lists;
        }

        public List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> GetAllAssessmentWetAreaWaterTightnessTest_Detail(List<int> ids)
        {
            var res = BInDB.assessment_wet_area_water_tightness_test_tran_detail.Where(a => ids.Contains(a.AssessmentWAWTTID ?? default(int))).ToList();
            var lists = Mapper.Map<List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel>>(res);
            return lists;
        }

        public AssessmentWetAreaWaterTightnessTestTransDetailViewModel GetAllAssessmentWetAreaWaterTightnessTest_DetailByID(int id)
        {
            var res = BInDB.assessment_wet_area_water_tightness_test_tran_detail.Where(a => a.AssessmentWAWTTDetailID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentWetAreaWaterTightnessTestTransDetailViewModel>(res);
            return lists;
        }

        public List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> GetAllAssessmentWetAreaWaterTightnessTest_DetailResult(List<int> ids)
        {
            var res = BInDB.assessment_wet_area_water_tightness_test_tran_detail_result.Where(a => ids.Contains(a.AssessmentWAWTTID ?? default(int))).ToList();
            var lists = Mapper.Map<List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel>>(res);
            return lists;
        }

        public AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel GetAllAssessmentWetAreaWaterTightnessTest_DetailResultByID(int id)
        {
            var res = BInDB.assessment_wet_area_water_tightness_test_tran_detail_result.Where(a => a.AssessmentWAWTTDetailResultID == id).FirstOrDefault();
            var lists = Mapper.Map<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel>(res);
            return lists;
        }

        public int CreateAssessmentWetAreaWaterTightnessTestMaster(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel, List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> detailViewModels, List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> detailResultViewModels)
        {
            try
            {
                assessment_wet_area_water_tightness_test_tran _db_res_Master = new assessment_wet_area_water_tightness_test_tran();
                _db_res_Master.ProjectID = masterViewModel.ProjectID;
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.Other_Result = masterViewModel.Other_Result;
                _db_res_Master.MobileAssessmentWAWTTID = masterViewModel.MobileAssessmentWAWTTID;
                _db_res_Master.BatchID = masterViewModel.BatchID;
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.CreatedBy = masterViewModel.CreatedBy;
                _db_res_Master.CreatedDate = DateTime.Now;
                foreach (var desc in detailViewModels)
                {
                    assessment_wet_area_water_tightness_test_tran_detail _db_res_Detail = Mapper.Map<assessment_wet_area_water_tightness_test_tran_detail>(desc);
                    _db_res_Detail.AssessmentWAWTTID = _db_res_Master.AssessmentWAWTTID;
                    _db_res_Master.assessment_wet_area_water_tightness_test_tran_detail.Add(_db_res_Detail);
                }
                foreach (var desc in detailResultViewModels)
                {
                    assessment_wet_area_water_tightness_test_tran_detail_result _db_res_Detail_result = Mapper.Map<assessment_wet_area_water_tightness_test_tran_detail_result>(desc);
                    _db_res_Detail_result.AssessmentWAWTTID = _db_res_Master.AssessmentWAWTTID;
                    _db_res_Master.assessment_wet_area_water_tightness_test_tran_detail_result.Add(_db_res_Detail_result);
                }

                BInDB.assessment_wet_area_water_tightness_test_tran.Add(_db_res_Master);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Create Assessment Wet Area Water Tightness Test:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentWetAreaWaterTightnessTest(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_wet_area_water_tightness_test_tran.Where(a => a.AssessmentWAWTTID == masterViewModel.AssessmentWAWTTID).FirstOrDefault();
                _db_res_Master.AssessmentDate = masterViewModel.AssessmentDate;
                _db_res_Master.Block_Unit = masterViewModel.Block_Unit;
                _db_res_Master.Other_Result = masterViewModel.Other_Result;
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                foreach (var detailViewModel in masterViewModel.assessment_wet_area_water_tightness_test_tran_detail)
                {
                    var _db_res_Detail = BInDB.assessment_wet_area_water_tightness_test_tran_detail.Where(a => a.AssessmentWAWTTDetailID == detailViewModel.AssessmentWAWTTDetailID).FirstOrDefault();
                    _db_res_Detail.Result = detailViewModel.Result;
                    _db_res_Detail.UpdatedBy = detailViewModel.UpdatedBy;
                    _db_res_Detail.UpdatedDate = DateTime.Now;
                    BInDB.Entry(_db_res_Detail).State = EntityState.Modified;
                }
                foreach (var detailResultViewModel in masterViewModel.assessment_wet_area_water_tightness_test_tran_detail_result)
                {
                    var _db_res_Detail_Result = BInDB.assessment_wet_area_water_tightness_test_tran_detail_result.Where(a => a.AssessmentWAWTTDetailResultID == detailResultViewModel.AssessmentWAWTTDetailResultID).FirstOrDefault();
                    _db_res_Detail_Result.Result = detailResultViewModel.Result;
                    _db_res_Detail_Result.UpdatedBy = detailResultViewModel.UpdatedBy;
                    _db_res_Detail_Result.UpdatedDate = DateTime.Now;
                    BInDB.Entry(_db_res_Detail_Result).State = EntityState.Modified;
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Wet Area Water Tightness Test:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentWetAreaWaterTightnessTestOtherResult(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_wet_area_water_tightness_test_tran.Where(a => a.AssessmentWAWTTID == masterViewModel.AssessmentWAWTTID).FirstOrDefault();
                _db_res_Master.Other_Result = masterViewModel.Other_Result;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Wet Area Water Tightness Test Other Result:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentWetAreaWaterTightnessTestSignature(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel)
        {
            try
            {
                var _db_res_Master = BInDB.assessment_wet_area_water_tightness_test_tran.Where(a => a.AssessmentWAWTTID == masterViewModel.AssessmentWAWTTID).FirstOrDefault();
                _db_res_Master.Drawing_Image = masterViewModel.Drawing_Image;
                _db_res_Master.UpdatedBy = masterViewModel.UpdatedBy;
                _db_res_Master.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res_Master).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Wet Area Water Tightness Test Signature:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentWetAreaWaterTightnessTestDetail(AssessmentWetAreaWaterTightnessTestTransDetailViewModel detailViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_wet_area_water_tightness_test_tran_detail.Where(a => a.AssessmentWAWTTDetailID == detailViewModel.AssessmentWAWTTDetailID).FirstOrDefault();
                _db_res.Result = detailViewModel.Result;
                _db_res.UpdatedBy = detailViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Wet Area Water Tightness Test Detail:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int SaveAssessmentWetAreaWaterTightnessTestDetailResult(AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel detailResultViewModel)
        {
            try
            {
                var _db_res = BInDB.assessment_wet_area_water_tightness_test_tran_detail_result.Where(a => a.AssessmentWAWTTDetailResultID == detailResultViewModel.AssessmentWAWTTDetailResultID).FirstOrDefault();
                _db_res.Result = detailResultViewModel.Result;
                _db_res.UpdatedBy = detailResultViewModel.UpdatedBy;
                _db_res.UpdatedDate = DateTime.Now;
                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Save Assessment Wet Area Water Tightness Test Detail Result:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public int DeleteAssessmentWetAreaWaterTightnessTest(string Ids)
        {
            try
            {
                int Id = 0;
                foreach (string EWmaster in Ids.Split(','))
                {
                    Id = int.Parse(EWmaster);
                    var _db_res = BInDB.assessment_wet_area_water_tightness_test_tran.Find(Id);
                    var IDids = BInDB.assessment_wet_area_water_tightness_test_tran_detail.Where(a => a.AssessmentWAWTTID == Id).Select(a => a.AssessmentWAWTTDetailID).ToList();
                    foreach (var det in IDids)
                    {
                        var deleteObj = BInDB.assessment_wet_area_water_tightness_test_tran_detail.Find(det);
                        BInDB.assessment_wet_area_water_tightness_test_tran_detail.Remove(deleteObj);
                    }
                    IDids = BInDB.assessment_wet_area_water_tightness_test_tran_detail_result.Where(a => a.AssessmentWAWTTID == Id).Select(a => a.AssessmentWAWTTDetailResultID).ToList();
                    foreach (var det in IDids)
                    {
                        var deleteObj = BInDB.assessment_wet_area_water_tightness_test_tran_detail_result.Find(det);
                        BInDB.assessment_wet_area_water_tightness_test_tran_detail_result.Remove(deleteObj);
                    }
                    BInDB.assessment_wet_area_water_tightness_test_tran.Remove(_db_res);
                }
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Delete Assessment Wet Area Water Tightness Test:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }

        public bool CheckAssessmentWetAreaWaterTightnessTest(int pid, string Block_Unit, DateTime AssessmentDate)
        {
            try
            {
                var internalFinishe = BInDB.assessment_wet_area_water_tightness_test_tran.Where(a => a.ProjectID == pid && a.AssessmentDate == AssessmentDate && a.Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim() == Block_Unit.Replace(" ", "").Replace("#", "").Replace("-", "").Replace(",", "").Trim()).SingleOrDefault();
                if (internalFinishe == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        // Wet Area Water Tightness Test

    }
}