using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Utility;
using NLog;
using System.Configuration;
using System.Globalization;
using System.IO;


namespace BuildInspect.Controllers
{
    public class MobileLoginController : ApiController
    {
        private readonly IUserServices userService;
        private readonly IEmployeeServices employeeService;
        private readonly IAssessmentServices assessmentService;
        private readonly IQCInspectionServices qcInspectionService;
        Logger logger = LogManager.GetCurrentClassLogger();

        public MobileLoginController(IUserServices _userService, IEmployeeServices _employeeService,
            IAssessmentServices _assessmentService, IQCInspectionServices _qcInspectionService)
        {
            userService = _userService;
            employeeService = _employeeService;
            assessmentService = _assessmentService;
            qcInspectionService = _qcInspectionService;
        }

        [HttpPost]
        public LoginResponseViewModel BQASLogin(string username, string password, string deviceid)
        {
            //logger.Debug(string.Format("Login Username:{0}", username));

            //SecureString secPwd = new SecureString();
            //var deviceid = "";
            LoginResponseViewModel returnResponse = new Models.ViewModel.LoginResponseViewModel();
            try
            {
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, password);

                var returnUser = userService.ValidateUserLoginMobile(username, encWord, deviceid);
                if (returnUser != null)
                {
                    if (returnUser.IsActive == 1)
                    {
                        returnResponse.Credentials = returnUser.Password;
                        returnResponse.Success = true;
                        returnResponse.User = new BInLiteUserViewModel()
                        {
                            GroupID = returnUser.GroupID.ToString(),
                            UserId = returnUser.UserID.ToString(),
                            UserFullName = returnUser.DisplayName,
                            UserName = returnUser.UserName,
                            CompanyID = returnUser.CompanyID.ToString(),
                            Password = password
                        };

                        return returnResponse;
                    }
                    else
                    {
                        returnResponse.Success = false;
                        returnResponse.ErrorMessage = "Invalid login attempt.";
                    }
                }
            }
            catch (Exception ex)
            {
                returnResponse.Success = false;
                returnResponse.ErrorMessage = "Invalid login attempt.";
                return returnResponse;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return new LoginResponseViewModel();
            //return authService.SingleLogin(username, password);
        }

        #region Assessment Projects

        [HttpGet]
        public List<AssessmentProjectMasterViewModel> GetAllAssessmentProjects(int CompanyID)
        {
            try
            {
                return assessmentService.GetAllProjects().Where(a => a.CompanyID == CompanyID).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            //return new List<CheckListItemMasterViewModel>();
        }

        [HttpGet]
        public List<AssessmentProjectMasterViewModel> GetAllAssessmentProjectsByAssessorUserID(int UserID)
        {
            try
            {
                var user = userService.GetUser(UserID);
                if(user!=null)
                {
                    return assessmentService.GetAllProjects().Where(a => a.assessment_project_assessors_detail.Where(w => w.AssessorsID == user.Assessor_ID).Count() > 0).ToList();
                }
                else
                {
                    return new List<AssessmentProjectMasterViewModel>();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            //return new List<CheckListItemMasterViewModel>();
        }

        [HttpGet]
        public AssessmentSummaryViewModel GetAssessmentProjectSummary(int ProjectID)
        {
            AssessmentSummaryViewModel assessmentSummaryViewModel = new AssessmentSummaryViewModel();
            try
            {
                assessmentSummaryViewModel.projectMasterViewModel = assessmentService.GetProject(ProjectID);
                assessmentSummaryViewModel.assessmentSummaryDetailModels = assessmentService.GetAssessmentSummaryByProjectID(ProjectID);
                assessmentSummaryViewModel.ArchitecturalWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).Select(x => x.Weightage)?.Sum();
                if (assessmentSummaryViewModel.ArchitecturalWorksWeightage != 100)
                {
                    int Applicablecount = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).ToList().Count();
                    decimal AdjustWeightage = Math.Round(((Convert.ToDecimal(100) - Convert.ToDecimal(assessmentSummaryViewModel.ArchitecturalWorksWeightage)) / Applicablecount), 2);
                    foreach (var item in assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1))
                    {
                        item.Weightage = item.Weightage + AdjustWeightage;
                        item.WeightedScore = Math.Round((Convert.ToDecimal(item.NoofCompliances) / Convert.ToDecimal(item.NoofChecks)) * item.Weightage, 2);
                    }
                    assessmentSummaryViewModel.ArchitecturalWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).Select(x => x.Weightage)?.Sum();
                    if (assessmentSummaryViewModel.ArchitecturalWorksWeightage != 100)
                    {
                        AdjustWeightage = Math.Round(Convert.ToDecimal(100) - Convert.ToDecimal(assessmentSummaryViewModel.ArchitecturalWorksWeightage), 2);
                        var item1 = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleID == 1).FirstOrDefault();
                        item1.Weightage = item1.Weightage + AdjustWeightage;
                        item1.WeightedScore = Math.Round((Convert.ToDecimal(item1.NoofCompliances) / Convert.ToDecimal(item1.NoofChecks)) * item1.Weightage, 2);
                        assessmentSummaryViewModel.ArchitecturalWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).Select(x => x.Weightage)?.Sum();
                    }
                }
                assessmentSummaryViewModel.ArchitecturalWorksWeightedScore = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).Select(x => x.WeightedScore)?.Sum();
                assessmentSummaryViewModel.InternalFinishesWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeID == 1 && x.AssessmentTypeModuleName != "M&E").Select(x => x.Weightage)?.Sum();
                assessmentSummaryViewModel.InternalFinishesWorksWeightedScore = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeID == 1 && x.AssessmentTypeModuleName != "M&E").Select(x => x.WeightedScore)?.Sum();
                assessmentSummaryViewModel.MEWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeID == 1 && x.AssessmentTypeModuleName == "M&E").Select(x => x.Weightage)?.Sum();
                assessmentSummaryViewModel.MEWorksWeightedScore = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeID == 1 && x.AssessmentTypeModuleName == "M&E").Select(x => x.WeightedScore)?.Sum();

                if (assessmentSummaryViewModel.ArchitecturalWorksWeightage > 0)
                {
                    assessmentSummaryViewModel.ArchitecturalWorksTotalScore = ((assessmentSummaryViewModel.ArchitecturalWorksWeightedScore / assessmentSummaryViewModel.ArchitecturalWorksWeightage) * assessmentSummaryViewModel.projectMasterViewModel.assessment_development_type_master.ArchitecturalWorksWeightage);
                }
                if (assessmentSummaryViewModel.MEWorksWeightage > 0)
                {
                    assessmentSummaryViewModel.MEWorksTotalScore = ((assessmentSummaryViewModel.MEWorksWeightedScore / assessmentSummaryViewModel.MEWorksWeightage) * assessmentSummaryViewModel.projectMasterViewModel.assessment_development_type_master.MEWorksWeightage);
                }
                assessmentSummaryViewModel.ArchitecturalWorksTotalScore = Math.Round(Convert.ToDecimal(assessmentSummaryViewModel.ArchitecturalWorksTotalScore), 2);
                assessmentSummaryViewModel.MEWorksTotalScore = Math.Round(Convert.ToDecimal(assessmentSummaryViewModel.MEWorksTotalScore), 2);
                assessmentSummaryViewModel.FinalScore = assessmentSummaryViewModel.ArchitecturalWorksTotalScore + assessmentSummaryViewModel.MEWorksTotalScore;
                return assessmentSummaryViewModel;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            //return new List<CheckListItemMasterViewModel>();
        }

        #endregion Assessment Projects

        #region Assessment Masters
        [HttpGet]
        public List<AssessmentTypeLocationMasterViewModel> GetAllAssessmentLocations()
        {
            try
            {
                return assessmentService.GetAllAssessmentLocations().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<AssessmentTypeModuleMasterViewModel> GetAllModules()
        {
            try
            {
                return assessmentService.GetAllModules().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<AssessmentTypeModuleProcessMasterViewModel> GetAllModuleProcess()
        {
            try
            {
                return assessmentService.GetAllModuleProcess().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<AssessmentDirectionMasterViewModel> GetAllDirections()
        {
            try
            {
                return assessmentService.GetAllDirections().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<AssessmentJointMasterViewModel> GetAllJoints()
        {
            try
            {
                return assessmentService.GetAllJoints().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<AssessmentLeakMasterViewModel> GetAllLeaks()
        {
            try
            {
                return assessmentService.GetAllLeaks().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<AssessmentWallMasterViewModel> GetAllWalls()
        {
            try
            {
                return assessmentService.GetAllWalls().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<AssessmentWindowMasterViewModel> GetAllWindows()
        {
            try
            {
                return assessmentService.GetAllWindows().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<AssessmentWetAreaWaterTightnessTestResultMasterViewModel> GetAllWetAreaWaterTightnessTestResults()
        {
            try
            {
                return assessmentService.GetAllWetAreaWaterTightnessTestResults().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<MasterSyncViewModel> GetAssessmentMasterSync()
        {
            try
            {
                return assessmentService.GetAssessmentMasterSync().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }
        #endregion Assessment Masters

        #region Internal Finishes
        [HttpGet]
        public AssessmentInternalFinishesMobileHeaderViewModel GetInternalFinishesHeader(int ProjectID)
        {
            AssessmentInternalFinishesMobileHeaderViewModel AssessmentInternalFinishesMobileHeaderView = new AssessmentInternalFinishesMobileHeaderViewModel();
            //Project 
            AssessmentProjectMasterViewModel assessmentProjectMasterViewModel = new AssessmentProjectMasterViewModel();
            assessmentProjectMasterViewModel = assessmentService.GetProject(ProjectID);
            AssessmentInternalFinishesMobileHeaderView.ProjectMasterMobileView = new AssessmentProjectMasterMobileViewModel();
            assessmentProjectMasterViewModel.CopyProperties(AssessmentInternalFinishesMobileHeaderView.ProjectMasterMobileView);
            AssessmentInternalFinishesMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView = new AssessmentDevelopmentTypeMasterViewModel();
            assessmentProjectMasterViewModel.assessment_development_type_master.CopyProperties(AssessmentInternalFinishesMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView);
            AssessmentInternalFinishesMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels = new List<AssessmentProjectAssessorsDetailMobileViewModel>();
            foreach (var assessors in assessmentProjectMasterViewModel.assessment_project_assessors_detail)
            {
                AssessmentInternalFinishesMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels.Add(new AssessmentProjectAssessorsDetailMobileViewModel
                {
                    AssessorsID = assessors.AssessorsID,
                    AssessorsName = assessors.assessors_master.AssessorsName,
                    RowNo = assessors.RowNo
                });
            }
            //Project 

            // Locations
            AssessmentInternalFinishesMobileHeaderView.AssessmentTypeLocationMasterMobileViews = new List<AssessmentTypeLocationMasterMobileViewModel>();
            foreach (var location in assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 1).ToList())
            {
                AssessmentInternalFinishesMobileHeaderView.AssessmentTypeLocationMasterMobileViews.Add(new AssessmentTypeLocationMasterMobileViewModel()
                {
                    AssessmentTypeID = location.AssessmentTypeID,
                    AssessmentTypeLocationID = location.AssessmentTypeLocationID,
                    AssessmentTypeLocationName = location.AssessmentTypeLocationName,
                    AssessmentTypeLocationType = location.AssessmentTypeLocationType,
                });
            }
            // Location

            // Modules And Process
            AssessmentInternalFinishesMobileHeaderView.AssessmentTypeModuleMasterMobileViewModels = new List<AssessmentTypeModuleMasterMobileViewModel>();
            foreach (var module in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 1).ToList())
            {
                AssessmentTypeModuleMasterMobileViewModel assessmentTypeModuleMasterMobileViewModel = new AssessmentTypeModuleMasterMobileViewModel();
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeID = module.AssessmentTypeID;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleID = module.AssessmentTypeModuleID;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleName = module.AssessmentTypeModuleName;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleShortName = module.AssessmentTypeModuleShortName;
                assessmentTypeModuleMasterMobileViewModel.OrderBy = module.OrderBy;
                assessmentTypeModuleMasterMobileViewModel.NoOfRow = module.NoOfRow;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels = new List<AssessmentTypeModuleProcessMasterMobileViewModel>();
                foreach (var process in assessmentService.GetAllModuleProcessByModuleIds(new List<int> { module.AssessmentTypeModuleID }).ToList())
                {
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels.Add(new AssessmentTypeModuleProcessMasterMobileViewModel()
                    {
                        AssessmentTypeModuleID = process.AssessmentTypeModuleID,
                        AssessmentTypeModuleProcessID = process.AssessmentTypeModuleProcessID,
                        AssessmentTypeModuleProcessName = process.AssessmentTypeModuleProcessName,
                        OrderBy = process.OrderBy
                    });
                }
                AssessmentInternalFinishesMobileHeaderView.AssessmentTypeModuleMasterMobileViewModels.Add(assessmentTypeModuleMasterMobileViewModel);
            }
            // Modules And Process

            return AssessmentInternalFinishesMobileHeaderView;
        }

        [HttpGet]
        public List<AssessmentInternalFinishesTransMasterMobileViewModel> GetInternalFinishesDetail(int ProjectID)
        {
            List<AssessmentInternalFinishesTransMasterMobileViewModel> assessmentInternalFinishesTransMasterViewModels = new List<AssessmentInternalFinishesTransMasterMobileViewModel>();
            foreach (var InternalFinishesMaster in assessmentService.GetAllAssessmentInternalFinishes(ProjectID).ToList())
            {
                AssessmentInternalFinishesTransMasterMobileViewModel assessmentInternalFinishesTransMasterMobileViewModel = new AssessmentInternalFinishesTransMasterMobileViewModel();
                assessmentInternalFinishesTransMasterMobileViewModel.AssessmentIFID = InternalFinishesMaster.AssessmentIFID;
                assessmentInternalFinishesTransMasterMobileViewModel.ProjectID = InternalFinishesMaster.ProjectID;
                assessmentInternalFinishesTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", InternalFinishesMaster.AssessmentDate);
                assessmentInternalFinishesTransMasterMobileViewModel.Block_Unit = InternalFinishesMaster.Block_Unit;
                assessmentInternalFinishesTransMasterMobileViewModel.LocationID = InternalFinishesMaster.LocationID;
                assessmentInternalFinishesTransMasterMobileViewModel.LocationName = InternalFinishesMaster.assessment_type_location_master.AssessmentTypeLocationName;
                assessmentInternalFinishesTransMasterMobileViewModel.MobileAssessmentIFID = InternalFinishesMaster.MobileAssessmentIFID;
                assessmentInternalFinishesTransMasterMobileViewModel.BatchID = InternalFinishesMaster.BatchID;
                assessmentInternalFinishesTransMasterMobileViewModel.Status = 0;
                assessmentInternalFinishesTransMasterMobileViewModel.AssessmentInternalFinishesTransDetailMobileViewModels = new List<AssessmentInternalFinishesTransDetailMobileViewModel>();
                foreach (var InternalFinishesDetail in InternalFinishesMaster.assessment_internal_finishes_trn_detail)
                {
                    assessmentInternalFinishesTransMasterMobileViewModel.AssessmentInternalFinishesTransDetailMobileViewModels.Add(new AssessmentInternalFinishesTransDetailMobileViewModel()
                    {
                        AssessmentIFDetailID = InternalFinishesDetail.AssessmentIFDetailID,
                        AssessmentTypeModuleProcessID = InternalFinishesDetail.AssessmentTypeModuleProcessID,
                        Result = (string.IsNullOrEmpty(InternalFinishesDetail.Result) ? 3 : int.Parse(InternalFinishesDetail.Result)),
                        RowNo = InternalFinishesDetail.RowNo,
                    });
                }
                assessmentInternalFinishesTransMasterViewModels.Add(assessmentInternalFinishesTransMasterMobileViewModel);
            }
            return assessmentInternalFinishesTransMasterViewModels;
        }

        [HttpPost]
        public InternalFinishesResponseViewModel SaveInternalFinishes(List<AssessmentInternalFinishesTransMasterMobileViewModel> AssessmentInternalFinishesTransMasterMobileViewModels)
        {
            InternalFinishesResponseViewModel InternalFinishesResponseViewModel = new InternalFinishesResponseViewModel();
            InternalFinishesResponseViewModel.AssessmentInternalFinishesTransMasterMobileViewModels = new List<AssessmentInternalFinishesTransMasterMobileViewModel>();
            int ProjectID = Convert.ToInt32(AssessmentInternalFinishesTransMasterMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                foreach (var EWMasterview in AssessmentInternalFinishesTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 1)
                    {
                        AssessmentInternalFinishesTransMasterViewModel masterViewModel = new AssessmentInternalFinishesTransMasterViewModel();
                        masterViewModel.ProjectID = EWMasterview.ProjectID;
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.LocationID = EWMasterview.LocationID;
                        masterViewModel.MobileAssessmentIFID = EWMasterview.MobileAssessmentIFID;
                        masterViewModel.BatchID = BatchID;
                        masterViewModel.CreatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        List<AssessmentInternalFinishesTransDetailViewModel> detailViewModels = new List<AssessmentInternalFinishesTransDetailViewModel>();
                        foreach (var EWDetailView in EWMasterview.AssessmentInternalFinishesTransDetailMobileViewModels)
                        {
                            detailViewModels.Add(new AssessmentInternalFinishesTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = EWDetailView.AssessmentTypeModuleProcessID,
                                Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString()),
                                RowNo = EWDetailView.RowNo,
                                UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId,
                                UpdatedDate = DateTime.Now
                            });
                        }
                        var result = assessmentService.CreateAssessmentInternalFinishesMaster(masterViewModel, detailViewModels);
                    }
                }


                foreach (var InternalFinishesMaster in assessmentService.GetAllAssessmentInternalFinishes(ProjectID, BatchID).ToList())
                {
                    AssessmentInternalFinishesTransMasterMobileViewModel assessmentInternalFinishesTransMasterMobileViewModel = new AssessmentInternalFinishesTransMasterMobileViewModel();
                    assessmentInternalFinishesTransMasterMobileViewModel.AssessmentIFID = InternalFinishesMaster.AssessmentIFID;
                    assessmentInternalFinishesTransMasterMobileViewModel.ProjectID = InternalFinishesMaster.ProjectID;
                    assessmentInternalFinishesTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", InternalFinishesMaster.AssessmentDate);
                    assessmentInternalFinishesTransMasterMobileViewModel.Block_Unit = InternalFinishesMaster.Block_Unit;
                    assessmentInternalFinishesTransMasterMobileViewModel.LocationID = InternalFinishesMaster.LocationID;
                    assessmentInternalFinishesTransMasterMobileViewModel.LocationName = InternalFinishesMaster.assessment_type_location_master.AssessmentTypeLocationName;
                    assessmentInternalFinishesTransMasterMobileViewModel.MobileAssessmentIFID = InternalFinishesMaster.MobileAssessmentIFID;
                    assessmentInternalFinishesTransMasterMobileViewModel.BatchID = InternalFinishesMaster.BatchID;
                    assessmentInternalFinishesTransMasterMobileViewModel.Status = 0;
                    assessmentInternalFinishesTransMasterMobileViewModel.AssessmentInternalFinishesTransDetailMobileViewModels = new List<AssessmentInternalFinishesTransDetailMobileViewModel>();
                    foreach (var InternalFinishesDetail in InternalFinishesMaster.assessment_internal_finishes_trn_detail)
                    {
                        assessmentInternalFinishesTransMasterMobileViewModel.AssessmentInternalFinishesTransDetailMobileViewModels.Add(new AssessmentInternalFinishesTransDetailMobileViewModel()
                        {
                            AssessmentIFDetailID = InternalFinishesDetail.AssessmentIFDetailID,
                            AssessmentTypeModuleProcessID = InternalFinishesDetail.AssessmentTypeModuleProcessID,
                            Result = (string.IsNullOrEmpty(InternalFinishesDetail.Result) ? 3 : int.Parse(InternalFinishesDetail.Result)),
                            RowNo = InternalFinishesDetail.RowNo,
                        });
                    }
                    InternalFinishesResponseViewModel.AssessmentInternalFinishesTransMasterMobileViewModels.Add(assessmentInternalFinishesTransMasterMobileViewModel);
                }
                InternalFinishesResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return InternalFinishesResponseViewModel;
        }

        [HttpPost]
        public InternalFinishesResponseViewModel UpdateInternalFinishes(List<AssessmentInternalFinishesTransMasterMobileViewModel> AssessmentInternalFinishesTransMasterMobileViewModels)
        {
            InternalFinishesResponseViewModel InternalFinishesResponseViewModel = new InternalFinishesResponseViewModel();
            InternalFinishesResponseViewModel.AssessmentInternalFinishesTransMasterMobileViewModels = new List<AssessmentInternalFinishesTransMasterMobileViewModel>();
            try
            {
                foreach (var EWMasterview in AssessmentInternalFinishesTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 2)
                    {
                        AssessmentInternalFinishesTransMasterViewModel masterViewModel = assessmentService.GetAllAssessmentInternalFinishes_ByID(EWMasterview.AssessmentIFID);
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.LocationID = EWMasterview.LocationID;
                        masterViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        foreach (var EWDetailView in EWMasterview.AssessmentInternalFinishesTransDetailMobileViewModels)
                        {
                            AssessmentInternalFinishesTransDetailViewModel detailViewModel = masterViewModel.assessment_internal_finishes_trn_detail.Where(a => a.AssessmentIFDetailID == EWDetailView.AssessmentIFDetailID).FirstOrDefault();
                            detailViewModel.Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString());
                            detailViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                            detailViewModel.UpdatedDate = DateTime.Now;
                        }
                        var result = assessmentService.SaveAssessmentInternalFinishes(masterViewModel);
                    }
                }
                InternalFinishesResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return InternalFinishesResponseViewModel;
        }

        [HttpPost]
        public InternalFinishesResponseViewModel DeleteInternalFinishes(List<AssessmentInternalFinishesTransMasterMobileDeleteViewModel> AssessmentInternalFinishesTransMasterMobileDeleteViewModels)
        {
            InternalFinishesResponseViewModel InternalFinishesResponseViewModel = new InternalFinishesResponseViewModel();
            InternalFinishesResponseViewModel.AssessmentInternalFinishesTransMasterMobileViewModels = new List<AssessmentInternalFinishesTransMasterMobileViewModel>();
            try
            {
                string EWIds = "";
                foreach (var EWMasterview in AssessmentInternalFinishesTransMasterMobileDeleteViewModels)
                {
                    EWIds = EWIds + EWMasterview.AssessmentIFID.ToString() + ",";
                }
                var result = assessmentService.DeleteAssessmentInternalFinishes(EWIds.Substring(0, EWIds.Length - 1));
                if (result > 0)
                {
                    InternalFinishesResponseViewModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return InternalFinishesResponseViewModel;
        }

        #endregion 

        #region  External Wall

        [HttpGet]
        public AssessmentExternalWallMobileHeaderViewModel GetExternalWallHeader(int ProjectID)
        {
            AssessmentExternalWallMobileHeaderViewModel AssessmentExternalWallMobileHeaderView = new AssessmentExternalWallMobileHeaderViewModel();
            //Project 
            AssessmentProjectMasterViewModel assessmentProjectMasterViewModel = new AssessmentProjectMasterViewModel();
            assessmentProjectMasterViewModel = assessmentService.GetProject(ProjectID);
            AssessmentExternalWallMobileHeaderView.ProjectMasterMobileView = new AssessmentProjectMasterMobileViewModel();
            assessmentProjectMasterViewModel.CopyProperties(AssessmentExternalWallMobileHeaderView.ProjectMasterMobileView);
            AssessmentExternalWallMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView = new AssessmentDevelopmentTypeMasterViewModel();
            assessmentProjectMasterViewModel.assessment_development_type_master.CopyProperties(AssessmentExternalWallMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView);
            AssessmentExternalWallMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels = new List<AssessmentProjectAssessorsDetailMobileViewModel>();
            foreach (var assessors in assessmentProjectMasterViewModel.assessment_project_assessors_detail)
            {
                AssessmentExternalWallMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels.Add(new AssessmentProjectAssessorsDetailMobileViewModel
                {
                    AssessorsID = assessors.AssessorsID,
                    AssessorsName = assessors.assessors_master.AssessorsName,
                    RowNo = assessors.RowNo
                });
            }
            //Project 

            // Locations
            AssessmentExternalWallMobileHeaderView.AssessmentTypeLocationMasterMobileViews = new List<AssessmentTypeLocationMasterMobileViewModel>();
            foreach (var location in assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 2).ToList())
            {
                AssessmentExternalWallMobileHeaderView.AssessmentTypeLocationMasterMobileViews.Add(new AssessmentTypeLocationMasterMobileViewModel()
                {
                    AssessmentTypeID = location.AssessmentTypeID,
                    AssessmentTypeLocationID = location.AssessmentTypeLocationID,
                    AssessmentTypeLocationName = location.AssessmentTypeLocationName,
                    AssessmentTypeLocationType = location.AssessmentTypeLocationType,
                });
            }
            // Location

            // Modules And Process
            AssessmentExternalWallMobileHeaderView.AssessmentTypeModuleMasterMobileViewModels = new List<AssessmentTypeModuleMasterMobileViewModel>();
            foreach (var module in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 2).ToList())
            {
                AssessmentTypeModuleMasterMobileViewModel assessmentTypeModuleMasterMobileViewModel = new AssessmentTypeModuleMasterMobileViewModel();
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeID = module.AssessmentTypeID;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleID = module.AssessmentTypeModuleID;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleName = module.AssessmentTypeModuleName;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleShortName = module.AssessmentTypeModuleShortName;
                assessmentTypeModuleMasterMobileViewModel.OrderBy = module.OrderBy;
                assessmentTypeModuleMasterMobileViewModel.NoOfRow = module.NoOfRow;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels = new List<AssessmentTypeModuleProcessMasterMobileViewModel>();
                foreach (var process in assessmentService.GetAllModuleProcessByModuleIds(new List<int> { module.AssessmentTypeModuleID }).ToList())
                {
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels.Add(new AssessmentTypeModuleProcessMasterMobileViewModel()
                    {
                        AssessmentTypeModuleID = process.AssessmentTypeModuleID,
                        AssessmentTypeModuleProcessID = process.AssessmentTypeModuleProcessID,
                        AssessmentTypeModuleProcessName = process.AssessmentTypeModuleProcessName,
                        OrderBy = process.OrderBy
                    });
                }
                AssessmentExternalWallMobileHeaderView.AssessmentTypeModuleMasterMobileViewModels.Add(assessmentTypeModuleMasterMobileViewModel);
            }
            // Modules And Process

            return AssessmentExternalWallMobileHeaderView;
        }

        [HttpGet]
        public List<AssessmentExternalWallTransMasterMobileViewModel> GetExternalWallDetail(int ProjectID)
        {
            List<AssessmentExternalWallTransMasterMobileViewModel> assessmentExternalWallTransMasterViewModels = new List<AssessmentExternalWallTransMasterMobileViewModel>();
            foreach (var ExternalWallMaster in assessmentService.GetAllAssessmentExternalWall(ProjectID).ToList())
            {
                AssessmentExternalWallTransMasterMobileViewModel assessmentExternalWallTransMasterMobileViewModel = new AssessmentExternalWallTransMasterMobileViewModel();
                assessmentExternalWallTransMasterMobileViewModel.AssessmentEWID = ExternalWallMaster.AssessmentEWID;
                assessmentExternalWallTransMasterMobileViewModel.ProjectID = ExternalWallMaster.ProjectID;
                assessmentExternalWallTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", ExternalWallMaster.AssessmentDate);
                assessmentExternalWallTransMasterMobileViewModel.Block_Unit = ExternalWallMaster.Block_Unit;
                assessmentExternalWallTransMasterMobileViewModel.LocationID = ExternalWallMaster.LocationID;
                assessmentExternalWallTransMasterMobileViewModel.LocationName = ExternalWallMaster.assessment_type_location_master.AssessmentTypeLocationName;
                assessmentExternalWallTransMasterMobileViewModel.Drawing_Image = ExternalWallMaster.Drawing_Image;
                assessmentExternalWallTransMasterMobileViewModel.MobileAssessmentEWID = ExternalWallMaster.MobileAssessmentEWID;
                assessmentExternalWallTransMasterMobileViewModel.BatchID = ExternalWallMaster.BatchID;
                assessmentExternalWallTransMasterMobileViewModel.Status = 0;
                assessmentExternalWallTransMasterMobileViewModel.AssessmentExternalWallTransDetailMobileViewModels = new List<AssessmentExternalWallTransDetailMobileViewModel>();
                foreach (var ExternalWallDetail in ExternalWallMaster.assessment_external_wall_trn_detail)
                {
                    assessmentExternalWallTransMasterMobileViewModel.AssessmentExternalWallTransDetailMobileViewModels.Add(new AssessmentExternalWallTransDetailMobileViewModel()
                    {
                        AssessmentEWDetailID = ExternalWallDetail.AssessmentEWDetailID,
                        AssessmentTypeModuleProcessID = ExternalWallDetail.AssessmentTypeModuleProcessID,
                        Result = (string.IsNullOrEmpty(ExternalWallDetail.Result) ? 3 : int.Parse(ExternalWallDetail.Result)),
                        RowNo = ExternalWallDetail.RowNo,
                    });
                }
                assessmentExternalWallTransMasterViewModels.Add(assessmentExternalWallTransMasterMobileViewModel);
            }
            return assessmentExternalWallTransMasterViewModels;
        }

        [HttpPost]
        public ExternalWallResponseViewModel SaveExternalWall(List<AssessmentExternalWallTransMasterMobileViewModel> AssessmentExternalWallTransMasterMobileViewModels)
        {
            ExternalWallResponseViewModel externalWallResponseViewModel = new ExternalWallResponseViewModel();
            externalWallResponseViewModel.AssessmentExternalWallTransMasterViewModels = new List<AssessmentExternalWallTransMasterMobileViewModel>();
            int ProjectID = Convert.ToInt32(AssessmentExternalWallTransMasterMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                foreach (var EWMasterview in AssessmentExternalWallTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 1)
                    {
                        AssessmentExternalWallTransMasterViewModel masterViewModel = new AssessmentExternalWallTransMasterViewModel();
                        masterViewModel.ProjectID = EWMasterview.ProjectID;
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.LocationID = EWMasterview.LocationID;
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.MobileAssessmentEWID = EWMasterview.MobileAssessmentEWID;
                        masterViewModel.BatchID = BatchID;
                        masterViewModel.CreatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        List<AssessmentExternalWallTransDetailViewModel> detailViewModels = new List<AssessmentExternalWallTransDetailViewModel>();
                        foreach (var EWDetailView in EWMasterview.AssessmentExternalWallTransDetailMobileViewModels)
                        {
                            detailViewModels.Add(new AssessmentExternalWallTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = EWDetailView.AssessmentTypeModuleProcessID,
                                Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString()),
                                RowNo = EWDetailView.RowNo,
                                UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId,
                                UpdatedDate = DateTime.Now
                            });
                        }
                        var result = assessmentService.CreateAssessmentExternalWallMaster(masterViewModel, detailViewModels);
                    }
                }


                foreach (var ExternalWallMaster in assessmentService.GetAllAssessmentExternalWall(ProjectID, BatchID).ToList())
                {
                    AssessmentExternalWallTransMasterMobileViewModel assessmentExternalWallTransMasterMobileViewModel = new AssessmentExternalWallTransMasterMobileViewModel();
                    assessmentExternalWallTransMasterMobileViewModel.AssessmentEWID = ExternalWallMaster.AssessmentEWID;
                    assessmentExternalWallTransMasterMobileViewModel.ProjectID = ExternalWallMaster.ProjectID;
                    assessmentExternalWallTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", ExternalWallMaster.AssessmentDate);
                    assessmentExternalWallTransMasterMobileViewModel.Block_Unit = ExternalWallMaster.Block_Unit;
                    assessmentExternalWallTransMasterMobileViewModel.LocationID = ExternalWallMaster.LocationID;
                    //assessmentExternalWallTransMasterMobileViewModel.LocationName = ExternalWallMaster.assessment_type_location_master.AssessmentTypeLocationName;
                    assessmentExternalWallTransMasterMobileViewModel.Drawing_Image = ExternalWallMaster.Drawing_Image;
                    assessmentExternalWallTransMasterMobileViewModel.MobileAssessmentEWID = ExternalWallMaster.MobileAssessmentEWID;
                    assessmentExternalWallTransMasterMobileViewModel.BatchID = ExternalWallMaster.BatchID;
                    assessmentExternalWallTransMasterMobileViewModel.Status = 0;
                    assessmentExternalWallTransMasterMobileViewModel.AssessmentExternalWallTransDetailMobileViewModels = new List<AssessmentExternalWallTransDetailMobileViewModel>();
                    foreach (var ExternalWallDetail in ExternalWallMaster.assessment_external_wall_trn_detail)
                    {
                        assessmentExternalWallTransMasterMobileViewModel.AssessmentExternalWallTransDetailMobileViewModels.Add(new AssessmentExternalWallTransDetailMobileViewModel()
                        {
                            AssessmentEWDetailID = ExternalWallDetail.AssessmentEWDetailID,
                            AssessmentTypeModuleProcessID = ExternalWallDetail.AssessmentTypeModuleProcessID,
                            Result = (string.IsNullOrEmpty(ExternalWallDetail.Result) ? 3 : int.Parse(ExternalWallDetail.Result)),
                            RowNo = ExternalWallDetail.RowNo,
                        });
                    }
                    externalWallResponseViewModel.AssessmentExternalWallTransMasterViewModels.Add(assessmentExternalWallTransMasterMobileViewModel);
                }
                externalWallResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return externalWallResponseViewModel;
        }

        [HttpPost]
        public ExternalWallResponseViewModel UpdateExternalWall(List<AssessmentExternalWallTransMasterMobileViewModel> AssessmentExternalWallTransMasterMobileViewModels)
        {
            ExternalWallResponseViewModel externalWallResponseViewModel = new ExternalWallResponseViewModel();
            externalWallResponseViewModel.AssessmentExternalWallTransMasterViewModels = new List<AssessmentExternalWallTransMasterMobileViewModel>();
            try
            {
                foreach (var EWMasterview in AssessmentExternalWallTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 2)
                    {
                        AssessmentExternalWallTransMasterViewModel masterViewModel = assessmentService.GetAllAssessmentExternalWall_ByID(EWMasterview.AssessmentEWID);
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.LocationID = EWMasterview.LocationID;
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        foreach (var EWDetailView in EWMasterview.AssessmentExternalWallTransDetailMobileViewModels)
                        {
                            AssessmentExternalWallTransDetailViewModel detailViewModel = masterViewModel.assessment_external_wall_trn_detail.Where(a => a.AssessmentEWDetailID == EWDetailView.AssessmentEWDetailID).FirstOrDefault();
                            detailViewModel.Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString());
                            detailViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                            detailViewModel.UpdatedDate = DateTime.Now;
                        }
                        var result = assessmentService.SaveAssessmentExternalWall(masterViewModel);
                    }
                }
                externalWallResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return externalWallResponseViewModel;
        }

        [HttpPost]
        public ExternalWallResponseViewModel DeleteExternalWall(List<AssessmentExternalWallTransMasterMobileDeleteViewModel> AssessmentExternalWallTransMasterMobileDeleteViewModels)
        {
            ExternalWallResponseViewModel externalWallResponseViewModel = new ExternalWallResponseViewModel();
            externalWallResponseViewModel.AssessmentExternalWallTransMasterViewModels = new List<AssessmentExternalWallTransMasterMobileViewModel>();
            try
            {
                string EWIds = "";
                foreach (var EWMasterview in AssessmentExternalWallTransMasterMobileDeleteViewModels)
                {
                    EWIds = EWIds + EWMasterview.AssessmentEWID.ToString() + ",";
                }
                var result = assessmentService.DeleteAssessmentExternalWall(EWIds.Substring(0, EWIds.Length - 1));
                if (result > 0)
                {
                    externalWallResponseViewModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return externalWallResponseViewModel;
        }

        #endregion

        #region External Works

        [HttpGet]
        public AssessmentExternalWorksMobileHeaderViewModel GetExternalWorksHeader(int ProjectID)
        {
            AssessmentExternalWorksMobileHeaderViewModel AssessmentExternalWorksMobileHeaderView = new AssessmentExternalWorksMobileHeaderViewModel();
            //Project 
            AssessmentProjectMasterViewModel assessmentProjectMasterViewModel = new AssessmentProjectMasterViewModel();
            assessmentProjectMasterViewModel = assessmentService.GetProject(ProjectID);
            AssessmentExternalWorksMobileHeaderView.ProjectMasterMobileView = new AssessmentProjectMasterMobileViewModel();
            assessmentProjectMasterViewModel.CopyProperties(AssessmentExternalWorksMobileHeaderView.ProjectMasterMobileView);
            AssessmentExternalWorksMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView = new AssessmentDevelopmentTypeMasterViewModel();
            assessmentProjectMasterViewModel.assessment_development_type_master.CopyProperties(AssessmentExternalWorksMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView);
            AssessmentExternalWorksMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels = new List<AssessmentProjectAssessorsDetailMobileViewModel>();
            foreach (var assessors in assessmentProjectMasterViewModel.assessment_project_assessors_detail)
            {
                AssessmentExternalWorksMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels.Add(new AssessmentProjectAssessorsDetailMobileViewModel
                {
                    AssessorsID = assessors.AssessorsID,
                    AssessorsName = assessors.assessors_master.AssessorsName,
                    RowNo = assessors.RowNo
                });
            }
            //Project 

            // Locations
            AssessmentExternalWorksMobileHeaderView.AssessmentTypeLocationMasterMobileViews = new List<AssessmentTypeLocationMasterMobileViewModel>();
            foreach (var location in assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 3).ToList())
            {
                AssessmentTypeLocationMasterMobileViewModel assessmentTypeLocationMasterMobileViewModel = new AssessmentTypeLocationMasterMobileViewModel();
                assessmentTypeLocationMasterMobileViewModel.AssessmentTypeID = location.AssessmentTypeID;
                assessmentTypeLocationMasterMobileViewModel.AssessmentTypeLocationID = location.AssessmentTypeLocationID;
                assessmentTypeLocationMasterMobileViewModel.AssessmentTypeLocationName = location.AssessmentTypeLocationName;
                assessmentTypeLocationMasterMobileViewModel.AssessmentTypeLocationType = location.AssessmentTypeLocationType;
                assessmentTypeLocationMasterMobileViewModel.AssessmentTypeModuleMasterMobileViewModels = new List<AssessmentTypeModuleMasterMobileViewModel>();
                // Modules And Process
                foreach (var module in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 3).ToList())
                {
                    AssessmentTypeModuleMasterMobileViewModel assessmentTypeModuleMasterMobileViewModel = new AssessmentTypeModuleMasterMobileViewModel();
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeID = module.AssessmentTypeID;
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleID = module.AssessmentTypeModuleID;
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleName = module.AssessmentTypeModuleName;
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleShortName = module.AssessmentTypeModuleShortName;
                    assessmentTypeModuleMasterMobileViewModel.OrderBy = module.OrderBy;
                    assessmentTypeModuleMasterMobileViewModel.NoOfRow = module.NoOfRow;
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels = new List<AssessmentTypeModuleProcessMasterMobileViewModel>();
                    foreach (var process in assessmentService.GetAllModuleProcessByModuleIds(new List<int> { module.AssessmentTypeModuleID }).Where(x => x.AssessmentTypeLocationID == location.AssessmentTypeLocationID).ToList())
                    {
                        assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels.Add(new AssessmentTypeModuleProcessMasterMobileViewModel()
                        {
                            AssessmentTypeModuleID = process.AssessmentTypeModuleID,
                            AssessmentTypeModuleProcessID = process.AssessmentTypeModuleProcessID,
                            AssessmentTypeModuleProcessName = process.AssessmentTypeModuleProcessName,
                            OrderBy = process.OrderBy
                        });
                    }
                    assessmentTypeLocationMasterMobileViewModel.AssessmentTypeModuleMasterMobileViewModels.Add(assessmentTypeModuleMasterMobileViewModel);
                }
                // Modules And Process
                AssessmentExternalWorksMobileHeaderView.AssessmentTypeLocationMasterMobileViews.Add(assessmentTypeLocationMasterMobileViewModel);
            }
            // Location

            return AssessmentExternalWorksMobileHeaderView;
        }

        [HttpGet]
        public List<AssessmentExternalWorksTransMasterMobileViewModel> GetExternalWorksDetail(int ProjectID)
        {
            List<AssessmentExternalWorksTransMasterMobileViewModel> assessmentExternalWorksTransMasterViewModels = new List<AssessmentExternalWorksTransMasterMobileViewModel>();
            foreach (var ExternalWorksMaster in assessmentService.GetAllAssessmentExternalWorks(ProjectID).ToList())
            {
                AssessmentExternalWorksTransMasterMobileViewModel assessmentExternalWorksTransMasterMobileViewModel = new AssessmentExternalWorksTransMasterMobileViewModel();
                assessmentExternalWorksTransMasterMobileViewModel.AssessmentEWKID = ExternalWorksMaster.AssessmentEWKID;
                assessmentExternalWorksTransMasterMobileViewModel.ProjectID = ExternalWorksMaster.ProjectID;
                assessmentExternalWorksTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", ExternalWorksMaster.AssessmentDate);
                assessmentExternalWorksTransMasterMobileViewModel.Remarks = ExternalWorksMaster.Remarks;
                assessmentExternalWorksTransMasterMobileViewModel.LocationID = ExternalWorksMaster.LocationID;
                assessmentExternalWorksTransMasterMobileViewModel.LocationName = ExternalWorksMaster.assessment_type_location_master.AssessmentTypeLocationName;
                assessmentExternalWorksTransMasterMobileViewModel.Drawing_Image = ExternalWorksMaster.Drawing_Image;
                assessmentExternalWorksTransMasterMobileViewModel.MobileAssessmentEWKID = ExternalWorksMaster.MobileAssessmentEWKID;
                assessmentExternalWorksTransMasterMobileViewModel.BatchID = ExternalWorksMaster.BatchID;
                assessmentExternalWorksTransMasterMobileViewModel.Status = 0;
                assessmentExternalWorksTransMasterMobileViewModel.AssessmentExternalWorksTransDetailMobileViewModels = new List<AssessmentExternalWorksTransDetailMobileViewModel>();
                foreach (var ExternalWorksDetail in ExternalWorksMaster.assessment_external_works_trn_detail)
                {
                    assessmentExternalWorksTransMasterMobileViewModel.AssessmentExternalWorksTransDetailMobileViewModels.Add(new AssessmentExternalWorksTransDetailMobileViewModel()
                    {
                        AssessmentEWKDetailID = ExternalWorksDetail.AssessmentEWKDetailID,
                        AssessmentTypeModuleProcessID = ExternalWorksDetail.AssessmentTypeModuleProcessID,
                        Result = (string.IsNullOrEmpty(ExternalWorksDetail.Result) ? 3 : int.Parse(ExternalWorksDetail.Result)),
                        RowNo = ExternalWorksDetail.RowNo,
                    });
                }
                assessmentExternalWorksTransMasterViewModels.Add(assessmentExternalWorksTransMasterMobileViewModel);
            }
            return assessmentExternalWorksTransMasterViewModels;
        }

        [HttpPost]
        public ExternalWorksResponseViewModel SaveExternalWorks(List<AssessmentExternalWorksTransMasterMobileViewModel> AssessmentExternalWorksTransMasterMobileViewModels)
        {
            ExternalWorksResponseViewModel externalWorksResponseViewModel = new ExternalWorksResponseViewModel();
            externalWorksResponseViewModel.AssessmentExternalWorksTransMasterMobileViewModels = new List<AssessmentExternalWorksTransMasterMobileViewModel>();
            int ProjectID = Convert.ToInt32(AssessmentExternalWorksTransMasterMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                foreach (var EWMasterview in AssessmentExternalWorksTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 1)
                    {
                        AssessmentExternalWorksTransMasterViewModel masterViewModel = new AssessmentExternalWorksTransMasterViewModel();
                        masterViewModel.ProjectID = EWMasterview.ProjectID;
                        masterViewModel.Remarks = EWMasterview.Remarks;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.LocationID = EWMasterview.LocationID;
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.MobileAssessmentEWKID = EWMasterview.MobileAssessmentEWKID;
                        masterViewModel.BatchID = BatchID;
                        masterViewModel.CreatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        List<AssessmentExternalWorksTransDetailViewModel> detailViewModels = new List<AssessmentExternalWorksTransDetailViewModel>();
                        foreach (var EWDetailView in EWMasterview.AssessmentExternalWorksTransDetailMobileViewModels)
                        {
                            detailViewModels.Add(new AssessmentExternalWorksTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = EWDetailView.AssessmentTypeModuleProcessID,
                                Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString()),
                                RowNo = EWDetailView.RowNo,
                                UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId,
                                UpdatedDate = DateTime.Now
                            });
                        }
                        var result = assessmentService.CreateAssessmentExternalWorksMaster(masterViewModel, detailViewModels);
                    }
                }


                foreach (var ExternalWorksMaster in assessmentService.GetAllAssessmentExternalWorks(ProjectID, BatchID).ToList())
                {
                    AssessmentExternalWorksTransMasterMobileViewModel assessmentExternalWorksTransMasterMobileViewModel = new AssessmentExternalWorksTransMasterMobileViewModel();
                    assessmentExternalWorksTransMasterMobileViewModel.AssessmentEWKID = ExternalWorksMaster.AssessmentEWKID;
                    assessmentExternalWorksTransMasterMobileViewModel.ProjectID = ExternalWorksMaster.ProjectID;
                    assessmentExternalWorksTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", ExternalWorksMaster.AssessmentDate);
                    assessmentExternalWorksTransMasterMobileViewModel.Remarks = ExternalWorksMaster.Remarks;
                    assessmentExternalWorksTransMasterMobileViewModel.LocationID = ExternalWorksMaster.LocationID;
                    //assessmentExternalWorksTransMasterMobileViewModel.LocationName = ExternalWorksMaster.assessment_type_location_master.AssessmentTypeLocationName;
                    assessmentExternalWorksTransMasterMobileViewModel.Drawing_Image = ExternalWorksMaster.Drawing_Image;
                    assessmentExternalWorksTransMasterMobileViewModel.MobileAssessmentEWKID = ExternalWorksMaster.MobileAssessmentEWKID;
                    assessmentExternalWorksTransMasterMobileViewModel.BatchID = ExternalWorksMaster.BatchID;
                    assessmentExternalWorksTransMasterMobileViewModel.Status = 0;
                    assessmentExternalWorksTransMasterMobileViewModel.AssessmentExternalWorksTransDetailMobileViewModels = new List<AssessmentExternalWorksTransDetailMobileViewModel>();
                    foreach (var ExternalWorksDetail in ExternalWorksMaster.assessment_external_works_trn_detail)
                    {
                        assessmentExternalWorksTransMasterMobileViewModel.AssessmentExternalWorksTransDetailMobileViewModels.Add(new AssessmentExternalWorksTransDetailMobileViewModel()
                        {
                            AssessmentEWKDetailID = ExternalWorksDetail.AssessmentEWKDetailID,
                            AssessmentTypeModuleProcessID = ExternalWorksDetail.AssessmentTypeModuleProcessID,
                            Result = (string.IsNullOrEmpty(ExternalWorksDetail.Result) ? 3 : int.Parse(ExternalWorksDetail.Result)),
                            RowNo = ExternalWorksDetail.RowNo,
                        });
                    }
                    externalWorksResponseViewModel.AssessmentExternalWorksTransMasterMobileViewModels.Add(assessmentExternalWorksTransMasterMobileViewModel);
                }
                externalWorksResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return externalWorksResponseViewModel;
        }

        [HttpPost]
        public ExternalWorksResponseViewModel UpdateExternalWorks(List<AssessmentExternalWorksTransMasterMobileViewModel> AssessmentExternalWorksTransMasterMobileViewModels)
        {
            ExternalWorksResponseViewModel externalWorksResponseViewModel = new ExternalWorksResponseViewModel();
            externalWorksResponseViewModel.AssessmentExternalWorksTransMasterMobileViewModels = new List<AssessmentExternalWorksTransMasterMobileViewModel>();
            try
            {
                foreach (var EWMasterview in AssessmentExternalWorksTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 2)
                    {
                        AssessmentExternalWorksTransMasterViewModel masterViewModel = assessmentService.GetAllAssessmentExternalWorks_ByID(EWMasterview.AssessmentEWKID);
                        masterViewModel.Remarks = EWMasterview.Remarks;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.LocationID = EWMasterview.LocationID;
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        foreach (var EWDetailView in EWMasterview.AssessmentExternalWorksTransDetailMobileViewModels)
                        {
                            AssessmentExternalWorksTransDetailViewModel detailViewModel = masterViewModel.assessment_external_works_trn_detail.Where(a => a.AssessmentEWKDetailID == EWDetailView.AssessmentEWKDetailID).FirstOrDefault();
                            detailViewModel.Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString());
                            detailViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                            detailViewModel.UpdatedDate = DateTime.Now;
                        }
                        var result = assessmentService.SaveAssessmentExternalWorks(masterViewModel);
                    }
                }
                externalWorksResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return externalWorksResponseViewModel;
        }

        [HttpPost]
        public ExternalWorksResponseViewModel DeleteExternalWorks(List<AssessmentExternalWorksTransMasterMobileDeleteViewModel> AssessmentExternalWorksTransMasterMobileDeleteViewModels)
        {
            ExternalWorksResponseViewModel externalWorksResponseViewModel = new ExternalWorksResponseViewModel();
            externalWorksResponseViewModel.AssessmentExternalWorksTransMasterMobileViewModels = new List<AssessmentExternalWorksTransMasterMobileViewModel>();
            try
            {
                string EWIds = "";
                foreach (var EWMasterview in AssessmentExternalWorksTransMasterMobileDeleteViewModels)
                {
                    EWIds = EWIds + EWMasterview.AssessmentEWKID.ToString() + ",";
                }
                var result = assessmentService.DeleteAssessmentExternalWorks(EWIds.Substring(0, EWIds.Length - 1));
                if (result > 0)
                {
                    externalWorksResponseViewModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return externalWorksResponseViewModel;
        }
        #endregion

        #region Roof Construction
        [HttpGet]
        public AssessmentRoofConstructionMobileHeaderViewModel GetRoofConstructionHeader(int ProjectID)
        {
            AssessmentRoofConstructionMobileHeaderViewModel AssessmentRoofConstructionMobileHeaderView = new AssessmentRoofConstructionMobileHeaderViewModel();
            //Project 
            AssessmentProjectMasterViewModel assessmentProjectMasterViewModel = new AssessmentProjectMasterViewModel();
            assessmentProjectMasterViewModel = assessmentService.GetProject(ProjectID);
            AssessmentRoofConstructionMobileHeaderView.ProjectMasterMobileView = new AssessmentProjectMasterMobileViewModel();
            assessmentProjectMasterViewModel.CopyProperties(AssessmentRoofConstructionMobileHeaderView.ProjectMasterMobileView);
            AssessmentRoofConstructionMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView = new AssessmentDevelopmentTypeMasterViewModel();
            assessmentProjectMasterViewModel.assessment_development_type_master.CopyProperties(AssessmentRoofConstructionMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView);
            AssessmentRoofConstructionMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels = new List<AssessmentProjectAssessorsDetailMobileViewModel>();
            foreach (var assessors in assessmentProjectMasterViewModel.assessment_project_assessors_detail)
            {
                AssessmentRoofConstructionMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels.Add(new AssessmentProjectAssessorsDetailMobileViewModel
                {
                    AssessorsID = assessors.AssessorsID,
                    AssessorsName = assessors.assessors_master.AssessorsName,
                    RowNo = assessors.RowNo
                });
            }
            //Project 

            // Locations
            AssessmentRoofConstructionMobileHeaderView.AssessmentTypeLocationMasterMobileViews = new List<AssessmentTypeLocationMasterMobileViewModel>();
            foreach (var location in assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 4).ToList())
            {
                AssessmentRoofConstructionMobileHeaderView.AssessmentTypeLocationMasterMobileViews.Add(new AssessmentTypeLocationMasterMobileViewModel()
                {
                    AssessmentTypeID = location.AssessmentTypeID,
                    AssessmentTypeLocationID = location.AssessmentTypeLocationID,
                    AssessmentTypeLocationName = location.AssessmentTypeLocationName,
                    AssessmentTypeLocationType = location.AssessmentTypeLocationType,
                });
            }
            // Location

            // Modules And Process
            AssessmentRoofConstructionMobileHeaderView.AssessmentTypeModuleMasterMobileViewModels = new List<AssessmentTypeModuleMasterMobileViewModel>();
            foreach (var module in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 4).ToList())
            {
                AssessmentTypeModuleMasterMobileViewModel assessmentTypeModuleMasterMobileViewModel = new AssessmentTypeModuleMasterMobileViewModel();
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeID = module.AssessmentTypeID;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleID = module.AssessmentTypeModuleID;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleName = module.AssessmentTypeModuleName;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleShortName = module.AssessmentTypeModuleShortName;
                assessmentTypeModuleMasterMobileViewModel.OrderBy = module.OrderBy;
                assessmentTypeModuleMasterMobileViewModel.NoOfRow = module.NoOfRow;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels = new List<AssessmentTypeModuleProcessMasterMobileViewModel>();
                foreach (var process in assessmentService.GetAllModuleProcessByModuleIds(new List<int> { module.AssessmentTypeModuleID }).ToList())
                {
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels.Add(new AssessmentTypeModuleProcessMasterMobileViewModel()
                    {
                        AssessmentTypeModuleID = process.AssessmentTypeModuleID,
                        AssessmentTypeModuleProcessID = process.AssessmentTypeModuleProcessID,
                        AssessmentTypeModuleProcessName = process.AssessmentTypeModuleProcessName,
                        OrderBy = process.OrderBy
                    });
                }
                AssessmentRoofConstructionMobileHeaderView.AssessmentTypeModuleMasterMobileViewModels.Add(assessmentTypeModuleMasterMobileViewModel);
            }
            // Modules And Process

            return AssessmentRoofConstructionMobileHeaderView;
        }

        [HttpGet]
        public List<AssessmentRoofConstructionTransMasterMobileViewModel> GetRoofConstructionDetail(int ProjectID)
        {
            List<AssessmentRoofConstructionTransMasterMobileViewModel> assessmentRoofConstructionTransMasterViewModels = new List<AssessmentRoofConstructionTransMasterMobileViewModel>();
            foreach (var RoofConstructionMaster in assessmentService.GetAllAssessmentRoofConstruction(ProjectID).ToList())
            {
                AssessmentRoofConstructionTransMasterMobileViewModel assessmentRoofConstructionTransMasterMobileViewModel = new AssessmentRoofConstructionTransMasterMobileViewModel();
                assessmentRoofConstructionTransMasterMobileViewModel.AssessmentRFCID = RoofConstructionMaster.AssessmentRFCID;
                assessmentRoofConstructionTransMasterMobileViewModel.ProjectID = RoofConstructionMaster.ProjectID;
                assessmentRoofConstructionTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", RoofConstructionMaster.AssessmentDate);
                assessmentRoofConstructionTransMasterMobileViewModel.Block_Unit = RoofConstructionMaster.Block_Unit;
                assessmentRoofConstructionTransMasterMobileViewModel.LocationID = RoofConstructionMaster.LocationID;
                assessmentRoofConstructionTransMasterMobileViewModel.LocationName = RoofConstructionMaster.assessment_type_location_master.AssessmentTypeLocationName;
                assessmentRoofConstructionTransMasterMobileViewModel.Drawing_Image = RoofConstructionMaster.Drawing_Image;
                assessmentRoofConstructionTransMasterMobileViewModel.MobileAssessmentRFCID = RoofConstructionMaster.MobileAssessmentRFCID;
                assessmentRoofConstructionTransMasterMobileViewModel.BatchID = RoofConstructionMaster.BatchID;
                assessmentRoofConstructionTransMasterMobileViewModel.Status = 0;
                assessmentRoofConstructionTransMasterMobileViewModel.AssessmentRoofConstructionTransDetailMobileViewModels = new List<AssessmentRoofConstructionTransDetailMobileViewModel>();
                foreach (var RoofConstructionDetail in RoofConstructionMaster.assessment_roof_construction_trn_detail)
                {
                    assessmentRoofConstructionTransMasterMobileViewModel.AssessmentRoofConstructionTransDetailMobileViewModels.Add(new AssessmentRoofConstructionTransDetailMobileViewModel()
                    {
                        AssessmentRFCDetailID = RoofConstructionDetail.AssessmentRFCDetailID,
                        AssessmentTypeModuleProcessID = RoofConstructionDetail.AssessmentTypeModuleProcessID,
                        Result = (string.IsNullOrEmpty(RoofConstructionDetail.Result) ? 3 : int.Parse(RoofConstructionDetail.Result)),
                        RowNo = RoofConstructionDetail.RowNo,
                    });
                }
                assessmentRoofConstructionTransMasterViewModels.Add(assessmentRoofConstructionTransMasterMobileViewModel);
            }
            return assessmentRoofConstructionTransMasterViewModels;
        }

        [HttpPost]
        public RoofConstructionResponseViewModel SaveRoofConstruction(List<AssessmentRoofConstructionTransMasterMobileViewModel> AssessmentRoofConstructionTransMasterMobileViewModels)
        {
            RoofConstructionResponseViewModel RoofConstructionResponseViewModel = new RoofConstructionResponseViewModel();
            RoofConstructionResponseViewModel.AssessmentRoofConstructionTransMasterMobileViewModels = new List<AssessmentRoofConstructionTransMasterMobileViewModel>();
            int ProjectID = Convert.ToInt32(AssessmentRoofConstructionTransMasterMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                foreach (var EWMasterview in AssessmentRoofConstructionTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 1)
                    {
                        AssessmentRoofConstructionTransMasterViewModel masterViewModel = new AssessmentRoofConstructionTransMasterViewModel();
                        masterViewModel.ProjectID = EWMasterview.ProjectID;
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.LocationID = EWMasterview.LocationID;
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.MobileAssessmentRFCID = EWMasterview.MobileAssessmentRFCID;
                        masterViewModel.BatchID = BatchID;
                        masterViewModel.CreatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        List<AssessmentRoofConstructionTransDetailViewModel> detailViewModels = new List<AssessmentRoofConstructionTransDetailViewModel>();
                        foreach (var EWDetailView in EWMasterview.AssessmentRoofConstructionTransDetailMobileViewModels)
                        {
                            detailViewModels.Add(new AssessmentRoofConstructionTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = EWDetailView.AssessmentTypeModuleProcessID,
                                Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString()),
                                RowNo = EWDetailView.RowNo,
                                UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId,
                                UpdatedDate = DateTime.Now
                            });
                        }
                        var result = assessmentService.CreateAssessmentRoofConstructionMaster(masterViewModel, detailViewModels);
                    }
                }


                foreach (var RoofConstructionMaster in assessmentService.GetAllAssessmentRoofConstruction(ProjectID, BatchID).ToList())
                {
                    AssessmentRoofConstructionTransMasterMobileViewModel assessmentRoofConstructionTransMasterMobileViewModel = new AssessmentRoofConstructionTransMasterMobileViewModel();
                    assessmentRoofConstructionTransMasterMobileViewModel.AssessmentRFCID = RoofConstructionMaster.AssessmentRFCID;
                    assessmentRoofConstructionTransMasterMobileViewModel.ProjectID = RoofConstructionMaster.ProjectID;
                    assessmentRoofConstructionTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", RoofConstructionMaster.AssessmentDate);
                    assessmentRoofConstructionTransMasterMobileViewModel.Block_Unit = RoofConstructionMaster.Block_Unit;
                    assessmentRoofConstructionTransMasterMobileViewModel.LocationID = RoofConstructionMaster.LocationID;
                    //assessmentRoofConstructionTransMasterMobileViewModel.LocationName = RoofConstructionMaster.assessment_type_location_master.AssessmentTypeLocationName;
                    assessmentRoofConstructionTransMasterMobileViewModel.Drawing_Image = RoofConstructionMaster.Drawing_Image;
                    assessmentRoofConstructionTransMasterMobileViewModel.MobileAssessmentRFCID = RoofConstructionMaster.MobileAssessmentRFCID;
                    assessmentRoofConstructionTransMasterMobileViewModel.BatchID = RoofConstructionMaster.BatchID;
                    assessmentRoofConstructionTransMasterMobileViewModel.Status = 0;
                    assessmentRoofConstructionTransMasterMobileViewModel.AssessmentRoofConstructionTransDetailMobileViewModels = new List<AssessmentRoofConstructionTransDetailMobileViewModel>();
                    foreach (var RoofConstructionDetail in RoofConstructionMaster.assessment_roof_construction_trn_detail)
                    {
                        assessmentRoofConstructionTransMasterMobileViewModel.AssessmentRoofConstructionTransDetailMobileViewModels.Add(new AssessmentRoofConstructionTransDetailMobileViewModel()
                        {
                            AssessmentRFCDetailID = RoofConstructionDetail.AssessmentRFCDetailID,
                            AssessmentTypeModuleProcessID = RoofConstructionDetail.AssessmentTypeModuleProcessID,
                            Result = (string.IsNullOrEmpty(RoofConstructionDetail.Result) ? 3 : int.Parse(RoofConstructionDetail.Result)),
                            RowNo = RoofConstructionDetail.RowNo,
                        });
                    }
                    RoofConstructionResponseViewModel.AssessmentRoofConstructionTransMasterMobileViewModels.Add(assessmentRoofConstructionTransMasterMobileViewModel);
                }
                RoofConstructionResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return RoofConstructionResponseViewModel;
        }

        [HttpPost]
        public RoofConstructionResponseViewModel UpdateRoofConstruction(List<AssessmentRoofConstructionTransMasterMobileViewModel> AssessmentRoofConstructionTransMasterMobileViewModels)
        {
            RoofConstructionResponseViewModel RoofConstructionResponseViewModel = new RoofConstructionResponseViewModel();
            RoofConstructionResponseViewModel.AssessmentRoofConstructionTransMasterMobileViewModels = new List<AssessmentRoofConstructionTransMasterMobileViewModel>();
            try
            {
                foreach (var EWMasterview in AssessmentRoofConstructionTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 2)
                    {
                        AssessmentRoofConstructionTransMasterViewModel masterViewModel = assessmentService.GetAllAssessmentRoofConstruction_ByID(EWMasterview.AssessmentRFCID);
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.LocationID = EWMasterview.LocationID;
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        foreach (var EWDetailView in EWMasterview.AssessmentRoofConstructionTransDetailMobileViewModels)
                        {
                            AssessmentRoofConstructionTransDetailViewModel detailViewModel = masterViewModel.assessment_roof_construction_trn_detail.Where(a => a.AssessmentRFCDetailID == EWDetailView.AssessmentRFCDetailID).FirstOrDefault();
                            detailViewModel.Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString());
                            detailViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                            detailViewModel.UpdatedDate = DateTime.Now;
                        }
                        var result = assessmentService.SaveAssessmentRoofConstruction(masterViewModel);
                    }
                }
                RoofConstructionResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return RoofConstructionResponseViewModel;
        }

        [HttpPost]
        public RoofConstructionResponseViewModel DeleteRoofConstruction(List<AssessmentRoofConstructionTransMasterMobileDeleteViewModel> AssessmentRoofConstructionTransMasterMobileDeleteViewModels)
        {
            RoofConstructionResponseViewModel RoofConstructionResponseViewModel = new RoofConstructionResponseViewModel();
            RoofConstructionResponseViewModel.AssessmentRoofConstructionTransMasterMobileViewModels = new List<AssessmentRoofConstructionTransMasterMobileViewModel>();
            try
            {
                string EWIds = "";
                foreach (var EWMasterview in AssessmentRoofConstructionTransMasterMobileDeleteViewModels)
                {
                    EWIds = EWIds + EWMasterview.AssessmentRFCID.ToString() + ",";
                }
                var result = assessmentService.DeleteAssessmentRoofConstruction(EWIds.Substring(0, EWIds.Length - 1));
                if (result > 0)
                {
                    RoofConstructionResponseViewModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return RoofConstructionResponseViewModel;
        }

        #endregion

        #region Field Window Water Tightness Test
        [HttpGet]
        public AssessmentFieldWindowWaterTightnessTestMobileHeaderViewModel GetFieldWindowWaterTightnessTestHeader(int ProjectID)
        {
            AssessmentFieldWindowWaterTightnessTestMobileHeaderViewModel AssessmentFieldWindowWaterTightnessTestMobileHeaderView = new AssessmentFieldWindowWaterTightnessTestMobileHeaderViewModel();
            //Project 
            AssessmentProjectMasterViewModel assessmentProjectMasterViewModel = new AssessmentProjectMasterViewModel();
            assessmentProjectMasterViewModel = assessmentService.GetProject(ProjectID);
            AssessmentFieldWindowWaterTightnessTestMobileHeaderView.ProjectMasterMobileView = new AssessmentProjectMasterMobileViewModel();
            assessmentProjectMasterViewModel.CopyProperties(AssessmentFieldWindowWaterTightnessTestMobileHeaderView.ProjectMasterMobileView);
            AssessmentFieldWindowWaterTightnessTestMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView = new AssessmentDevelopmentTypeMasterViewModel();
            assessmentProjectMasterViewModel.assessment_development_type_master.CopyProperties(AssessmentFieldWindowWaterTightnessTestMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView);
            AssessmentFieldWindowWaterTightnessTestMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels = new List<AssessmentProjectAssessorsDetailMobileViewModel>();
            foreach (var assessors in assessmentProjectMasterViewModel.assessment_project_assessors_detail)
            {
                AssessmentFieldWindowWaterTightnessTestMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels.Add(new AssessmentProjectAssessorsDetailMobileViewModel
                {
                    AssessorsID = assessors.AssessorsID,
                    AssessorsName = assessors.assessors_master.AssessorsName,
                    RowNo = assessors.RowNo
                });
            }
            //Project 

            // Directions
            AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentDirectionMasterMobileViewModels = new List<AssessmentDirectionMasterMobileViewModel>();
            foreach (var direction in assessmentService.GetAllDirections().ToList())
            {
                AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentDirectionMasterMobileViewModels.Add(new AssessmentDirectionMasterMobileViewModel()
                {
                    AssessmentDirectionID = direction.AssessmentDirectionID,
                    AssessmentDirectionName = direction.AssessmentDirectionName
                });
            }
            // Directions

            // Joints
            AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentJointMasterMobileViewModels = new List<AssessmentJointMasterMobileViewModel>();
            foreach (var joint in assessmentService.GetAllJoints().ToList())
            {
                AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentJointMasterMobileViewModels.Add(new AssessmentJointMasterMobileViewModel()
                {
                    AssessmentJointID = joint.AssessmentJointID,
                    AssessmentJointName = joint.AssessmentJointName
                });
            }
            // Joints

            // Windows
            AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentWindowMasterMobileViewModels = new List<AssessmentWindowMasterMobileViewModel>();
            foreach (var window in assessmentService.GetAllWindows().ToList())
            {
                AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentWindowMasterMobileViewModels.Add(new AssessmentWindowMasterMobileViewModel()
                {
                    AssessmentWindowID = window.AssessmentWindowID,
                    AssessmentWindowName = window.AssessmentWindowName
                });
            }
            // Windows

            // Walls
            AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentWallMasterMobileViewModels = new List<AssessmentWallMasterMobileViewModel>();
            foreach (var wall in assessmentService.GetAllWalls().ToList())
            {
                AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentWallMasterMobileViewModels.Add(new AssessmentWallMasterMobileViewModel()
                {
                    AssessmentWallID = wall.AssessmentWallID,
                    AssessmentWallName = wall.AssessmentWallName
                });
            }
            // Walls

            // Leaks
            AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentLeakMasterMobileViewModels = new List<AssessmentLeakMasterMobileViewModel>();
            foreach (var leak in assessmentService.GetAllLeaks().ToList())
            {
                AssessmentFieldWindowWaterTightnessTestMobileHeaderView.AssessmentLeakMasterMobileViewModels.Add(new AssessmentLeakMasterMobileViewModel()
                {
                    AssessmentLeakID = leak.AssessmentLeakID,
                    AssessmentLeakName = leak.AssessmentLeakName
                });
            }
            // Leaks
            return AssessmentFieldWindowWaterTightnessTestMobileHeaderView;
        }

        [HttpGet]
        public List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel> GetFieldWindowWaterTightnessTestDetail(int ProjectID)
        {
            List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel> AssessmentFieldWindowWaterTightnessTestTransMobileViewModels = new List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel>();
            foreach (var FieldWindowWaterTightnessTestMaster in assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest(ProjectID).ToList())
            {
                AssessmentFieldWindowWaterTightnessTestTransMobileViewModel assessmentFieldWindowWaterTightnessTestTransMobileViewModel = new AssessmentFieldWindowWaterTightnessTestTransMobileViewModel();
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentFWWTTID = FieldWindowWaterTightnessTestMaster.AssessmentFWWTTID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentWallID = FieldWindowWaterTightnessTestMaster.AssessmentWallID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentWindowID = FieldWindowWaterTightnessTestMaster.AssessmentWindowID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentJointID = FieldWindowWaterTightnessTestMaster.AssessmentJointID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentDirectionID = FieldWindowWaterTightnessTestMaster.AssessmentDirectionID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentLeakID = FieldWindowWaterTightnessTestMaster.AssessmentLeakID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.ProjectID = FieldWindowWaterTightnessTestMaster.ProjectID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", FieldWindowWaterTightnessTestMaster.AssessmentDate);
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.Block_Unit = FieldWindowWaterTightnessTestMaster.Block_Unit;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.Result = (string.IsNullOrEmpty(FieldWindowWaterTightnessTestMaster.Result) ? 3 : int.Parse(FieldWindowWaterTightnessTestMaster.Result));
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.Drawing_Image = FieldWindowWaterTightnessTestMaster.Drawing_Image;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.MobileAssessmentFWWTTID = FieldWindowWaterTightnessTestMaster.MobileAssessmentFWWTTID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.BatchID = FieldWindowWaterTightnessTestMaster.BatchID;
                assessmentFieldWindowWaterTightnessTestTransMobileViewModel.Status = 0;
                AssessmentFieldWindowWaterTightnessTestTransMobileViewModels.Add(assessmentFieldWindowWaterTightnessTestTransMobileViewModel);
            }
            return AssessmentFieldWindowWaterTightnessTestTransMobileViewModels;
        }

        [HttpPost]
        public FieldWindowWaterTightnessTestResponseViewModel SaveFieldWindowWaterTightnessTest(List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel> AssessmentFieldWindowWaterTightnessTestTransMobileViewModels)
        {
            FieldWindowWaterTightnessTestResponseViewModel FieldWindowWaterTightnessTestResponseViewModel = new FieldWindowWaterTightnessTestResponseViewModel();
            FieldWindowWaterTightnessTestResponseViewModel.AssessmentFieldWindowWaterTightnessTestTransMobileViewModels = new List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel>();
            int ProjectID = Convert.ToInt32(AssessmentFieldWindowWaterTightnessTestTransMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                foreach (var EWMasterview in AssessmentFieldWindowWaterTightnessTestTransMobileViewModels)
                {
                    if (EWMasterview.Status == 1)
                    {
                        AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel = new AssessmentFieldWindowWaterTightnessTestTransViewModel();
                        masterViewModel.ProjectID = EWMasterview.ProjectID;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentWallID = EWMasterview.AssessmentWallID;
                        masterViewModel.AssessmentWindowID = EWMasterview.AssessmentWindowID;
                        masterViewModel.AssessmentJointID = EWMasterview.AssessmentJointID;
                        masterViewModel.AssessmentDirectionID = EWMasterview.AssessmentDirectionID;
                        masterViewModel.AssessmentLeakID = EWMasterview.AssessmentLeakID;
                        masterViewModel.Result = EWMasterview.Result.ToString();
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.MobileAssessmentFWWTTID = EWMasterview.MobileAssessmentFWWTTID;
                        masterViewModel.BatchID = BatchID;
                        masterViewModel.CreatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        var result = assessmentService.CreateAssessmentFieldWindowWaterTightnessTest(masterViewModel);
                    }
                }

                foreach (var FieldWindowWaterTightnessTestMaster in assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest(ProjectID, BatchID).ToList())
                {
                    AssessmentFieldWindowWaterTightnessTestTransMobileViewModel assessmentFieldWindowWaterTightnessTestTransMobileViewModel = new AssessmentFieldWindowWaterTightnessTestTransMobileViewModel();
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentFWWTTID = FieldWindowWaterTightnessTestMaster.AssessmentFWWTTID;
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.ProjectID = FieldWindowWaterTightnessTestMaster.ProjectID;
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", FieldWindowWaterTightnessTestMaster.AssessmentDate);
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.Block_Unit = FieldWindowWaterTightnessTestMaster.Block_Unit;
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.Drawing_Image = FieldWindowWaterTightnessTestMaster.Drawing_Image;
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.MobileAssessmentFWWTTID = FieldWindowWaterTightnessTestMaster.MobileAssessmentFWWTTID;
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.BatchID = FieldWindowWaterTightnessTestMaster.BatchID;
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.Status = 0;
                    assessmentFieldWindowWaterTightnessTestTransMobileViewModel.Result = int.Parse(FieldWindowWaterTightnessTestMaster.Result);
                    FieldWindowWaterTightnessTestResponseViewModel.AssessmentFieldWindowWaterTightnessTestTransMobileViewModels.Add(assessmentFieldWindowWaterTightnessTestTransMobileViewModel);
                }
                FieldWindowWaterTightnessTestResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return FieldWindowWaterTightnessTestResponseViewModel;
        }

        [HttpPost]
        public FieldWindowWaterTightnessTestResponseViewModel UpdateFieldWindowWaterTightnessTest(List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel> AssessmentFieldWindowWaterTightnessTestTransMobileViewModels)
        {
            FieldWindowWaterTightnessTestResponseViewModel FieldWindowWaterTightnessTestResponseViewModel = new FieldWindowWaterTightnessTestResponseViewModel();
            FieldWindowWaterTightnessTestResponseViewModel.AssessmentFieldWindowWaterTightnessTestTransMobileViewModels = new List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel>();
            try
            {
                foreach (var EWMasterview in AssessmentFieldWindowWaterTightnessTestTransMobileViewModels)
                {
                    if (EWMasterview.Status == 2)
                    {
                        AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_ByID(EWMasterview.AssessmentFWWTTID);
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentWallID = EWMasterview.AssessmentWallID;
                        masterViewModel.AssessmentWindowID = EWMasterview.AssessmentWindowID;
                        masterViewModel.AssessmentJointID = EWMasterview.AssessmentJointID;
                        masterViewModel.AssessmentDirectionID = EWMasterview.AssessmentDirectionID;
                        masterViewModel.AssessmentLeakID = EWMasterview.AssessmentLeakID;
                        masterViewModel.Result = (EWMasterview.Result == 3 ? "" : EWMasterview.Result.ToString());
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.MobileAssessmentFWWTTID = EWMasterview.MobileAssessmentFWWTTID;
                        masterViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.UpdatedDate = DateTime.Now;
                        var result = assessmentService.SaveAssessmentFieldWindowWaterTightnessTest(masterViewModel);
                    }
                }
                FieldWindowWaterTightnessTestResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return FieldWindowWaterTightnessTestResponseViewModel;
        }

        [HttpPost]
        public FieldWindowWaterTightnessTestResponseViewModel DeleteFieldWindowWaterTightnessTest(List<AssessmentFieldWindowWaterTightnessTestTransMobileDeleteViewModel> AssessmentFieldWindowWaterTightnessTestTransMobileDeleteViewModels)
        {
            FieldWindowWaterTightnessTestResponseViewModel FieldWindowWaterTightnessTestResponseViewModel = new FieldWindowWaterTightnessTestResponseViewModel();
            FieldWindowWaterTightnessTestResponseViewModel.AssessmentFieldWindowWaterTightnessTestTransMobileViewModels = new List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel>();
            try
            {
                string EWIds = "";
                foreach (var EWMasterview in AssessmentFieldWindowWaterTightnessTestTransMobileDeleteViewModels)
                {
                    EWIds = EWIds + EWMasterview.AssessmentFWWTTID.ToString() + ",";
                }
                var result = assessmentService.DeleteAssessmentFieldWindowWaterTightnessTest(EWIds.Substring(0, EWIds.Length - 1));
                if (result > 0)
                {
                    FieldWindowWaterTightnessTestResponseViewModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return FieldWindowWaterTightnessTestResponseViewModel;
        }
        #endregion

        #region Wet Area Water Tightness Test
        [HttpGet]
        public AssessmentWetAreaWaterTightnessTestMobileHeaderViewModel GetWetAreaWaterTightnessTestHeader(int ProjectID)
        {
            AssessmentWetAreaWaterTightnessTestMobileHeaderViewModel AssessmentWetAreaWaterTightnessTestMobileHeaderView = new AssessmentWetAreaWaterTightnessTestMobileHeaderViewModel();
            //Project 
            AssessmentProjectMasterViewModel assessmentProjectMasterViewModel = new AssessmentProjectMasterViewModel();
            assessmentProjectMasterViewModel = assessmentService.GetProject(ProjectID);
            AssessmentWetAreaWaterTightnessTestMobileHeaderView.ProjectMasterMobileView = new AssessmentProjectMasterMobileViewModel();
            assessmentProjectMasterViewModel.CopyProperties(AssessmentWetAreaWaterTightnessTestMobileHeaderView.ProjectMasterMobileView);
            AssessmentWetAreaWaterTightnessTestMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView = new AssessmentDevelopmentTypeMasterViewModel();
            assessmentProjectMasterViewModel.assessment_development_type_master.CopyProperties(AssessmentWetAreaWaterTightnessTestMobileHeaderView.ProjectMasterMobileView.AssessmentDevelopmentTypeMasterMobileView);
            AssessmentWetAreaWaterTightnessTestMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels = new List<AssessmentProjectAssessorsDetailMobileViewModel>();
            foreach (var assessors in assessmentProjectMasterViewModel.assessment_project_assessors_detail)
            {
                AssessmentWetAreaWaterTightnessTestMobileHeaderView.ProjectMasterMobileView.AssessmentProjectAssessorsDetailMobileViewModels.Add(new AssessmentProjectAssessorsDetailMobileViewModel
                {
                    AssessorsID = assessors.AssessorsID,
                    AssessorsName = assessors.assessors_master.AssessorsName,
                    RowNo = assessors.RowNo
                });
            }
            //Project 

            // Modules And Process
            AssessmentWetAreaWaterTightnessTestMobileHeaderView.AssessmentTypeModuleMasterMobileViewModels = new List<AssessmentTypeModuleMasterMobileViewModel>();
            foreach (var module in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 6).ToList())
            {
                AssessmentTypeModuleMasterMobileViewModel assessmentTypeModuleMasterMobileViewModel = new AssessmentTypeModuleMasterMobileViewModel();
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeID = module.AssessmentTypeID;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleID = module.AssessmentTypeModuleID;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleName = module.AssessmentTypeModuleName;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleShortName = module.AssessmentTypeModuleShortName;
                assessmentTypeModuleMasterMobileViewModel.OrderBy = module.OrderBy;
                assessmentTypeModuleMasterMobileViewModel.NoOfRow = module.NoOfRow;
                assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels = new List<AssessmentTypeModuleProcessMasterMobileViewModel>();
                foreach (var process in assessmentService.GetAllModuleProcessByModuleIds(new List<int> { module.AssessmentTypeModuleID }).ToList())
                {
                    assessmentTypeModuleMasterMobileViewModel.AssessmentTypeModuleProcessMasterViewModels.Add(new AssessmentTypeModuleProcessMasterMobileViewModel()
                    {
                        AssessmentTypeModuleID = process.AssessmentTypeModuleID,
                        AssessmentTypeModuleProcessID = process.AssessmentTypeModuleProcessID,
                        AssessmentTypeModuleProcessName = process.AssessmentTypeModuleProcessName,
                        OrderBy = process.OrderBy
                    });
                }
                AssessmentWetAreaWaterTightnessTestMobileHeaderView.AssessmentTypeModuleMasterMobileViewModels.Add(assessmentTypeModuleMasterMobileViewModel);
            }
            // Modules And Process    

            return AssessmentWetAreaWaterTightnessTestMobileHeaderView;
        }

        [HttpGet]
        public List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel> GetWetAreaWaterTightnessTestDetail(int ProjectID)
        {
            List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel> AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels = new List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel>();
            foreach (var WetAreaWaterTightnessTestMaster in assessmentService.GetAllAssessmentWetAreaWaterTightnessTest(ProjectID).ToList())
            {
                AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel = new AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel();
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWAWTTID = WetAreaWaterTightnessTestMaster.AssessmentWAWTTID;
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.ProjectID = WetAreaWaterTightnessTestMaster.ProjectID;
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", WetAreaWaterTightnessTestMaster.AssessmentDate);
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.Drawing_Image = WetAreaWaterTightnessTestMaster.Drawing_Image;
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.Block_Unit = WetAreaWaterTightnessTestMaster.Block_Unit;
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.MobileAssessmentWAWTTID = WetAreaWaterTightnessTestMaster.MobileAssessmentWAWTTID;
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.BatchID = WetAreaWaterTightnessTestMaster.BatchID;
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.Status = 0;
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.Other_Result = WetAreaWaterTightnessTestMaster.Other_Result;
                AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels.Add(assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel);
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModels = new List<AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModel>();
                foreach (var WetAreaWaterTightnessTestDetail in WetAreaWaterTightnessTestMaster.assessment_wet_area_water_tightness_test_tran_detail)
                {
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModels.Add(new AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModel()
                    {
                        AssessmentWAWTTDetailID = WetAreaWaterTightnessTestDetail.AssessmentWAWTTDetailID,
                        AssessmentTypeModuleProcessID = WetAreaWaterTightnessTestDetail.AssessmentTypeModuleProcessID,
                        Result = (string.IsNullOrEmpty(WetAreaWaterTightnessTestDetail.Result) ? 3 : int.Parse(WetAreaWaterTightnessTestDetail.Result)),
                        RowNo = WetAreaWaterTightnessTestDetail.RowNo,
                    });
                }
                assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModels = new List<AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModel>();
                foreach (var WetAreaWaterTightnessTestDetailResult in WetAreaWaterTightnessTestMaster.assessment_wet_area_water_tightness_test_tran_detail_result)
                {
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModels.Add(new AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModel()
                    {
                        AssessmentWAWTTDetailResultID = WetAreaWaterTightnessTestDetailResult.AssessmentWAWTTDetailResultID,
                        AssessmentTypeModuleProcessID = WetAreaWaterTightnessTestDetailResult.AssessmentTypeModuleProcessID,
                        AssessmentWAWTTResultID = WetAreaWaterTightnessTestDetailResult.AssessmentWAWTTResultID,
                        Result = (string.IsNullOrEmpty(WetAreaWaterTightnessTestDetailResult.Result) ? 3 : int.Parse(WetAreaWaterTightnessTestDetailResult.Result)),
                    });
                }
            }
            return AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels;
        }

        [HttpPost]
        public WetAreaWaterTightnessTestResponseViewModel SaveWetAreaWaterTightnessTest(List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel> AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels)
        {
            WetAreaWaterTightnessTestResponseViewModel WetAreaWaterTightnessTestResponseView = new WetAreaWaterTightnessTestResponseViewModel();
            WetAreaWaterTightnessTestResponseView.AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels = new List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel>();
            int ProjectID = Convert.ToInt32(AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                foreach (var EWMasterview in AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 1)
                    {
                        AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel = new AssessmentWetAreaWaterTightnessTestTransMasterViewModel();
                        masterViewModel.ProjectID = EWMasterview.ProjectID;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.MobileAssessmentWAWTTID = EWMasterview.MobileAssessmentWAWTTID;
                        masterViewModel.BatchID = BatchID;
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.Other_Result = EWMasterview.Other_Result;
                        masterViewModel.CreatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.CreatedDate = DateTime.Now;
                        List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> detailViewModels = new List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel>();
                        foreach (var EWDetailView in EWMasterview.AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModels)
                        {
                            detailViewModels.Add(new AssessmentWetAreaWaterTightnessTestTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = EWDetailView.AssessmentTypeModuleProcessID,
                                Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString()),
                                RowNo = EWDetailView.RowNo,
                                UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId,
                                UpdatedDate = DateTime.Now
                            });
                        }
                        List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> detailResultViewModels = new List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel>();
                        foreach (var EWDetailResultView in EWMasterview.AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModels)
                        {
                            detailResultViewModels.Add(new AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel
                            {
                                AssessmentTypeModuleProcessID = EWDetailResultView.AssessmentTypeModuleProcessID,
                                AssessmentWAWTTResultID = EWDetailResultView.AssessmentWAWTTResultID,
                                Result = (EWDetailResultView.Result == 3 ? "" : EWDetailResultView.Result.ToString()),
                                UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId,
                                UpdatedDate = DateTime.Now
                            });
                        }
                        var result = assessmentService.CreateAssessmentWetAreaWaterTightnessTestMaster(masterViewModel, detailViewModels, detailResultViewModels );
                    }
                }

                foreach (var WetAreaWaterTightnessTestMaster in assessmentService.GetAllAssessmentWetAreaWaterTightnessTest(ProjectID, BatchID).ToList())
                {
                    AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel = new AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel();
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWAWTTID = WetAreaWaterTightnessTestMaster.AssessmentWAWTTID;
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.ProjectID = WetAreaWaterTightnessTestMaster.ProjectID;
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentDate = string.Format("{0:dd/MM/yyyy}", WetAreaWaterTightnessTestMaster.AssessmentDate);
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.Drawing_Image = WetAreaWaterTightnessTestMaster.Drawing_Image;
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.Block_Unit = WetAreaWaterTightnessTestMaster.Block_Unit;
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.MobileAssessmentWAWTTID = WetAreaWaterTightnessTestMaster.MobileAssessmentWAWTTID;
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.BatchID = WetAreaWaterTightnessTestMaster.BatchID;
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.Status = 0;
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.Other_Result = WetAreaWaterTightnessTestMaster.Other_Result;
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModels = new List<AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModel>();
                    foreach (var WetAreaWaterTightnessTestDetail in WetAreaWaterTightnessTestMaster.assessment_wet_area_water_tightness_test_tran_detail)
                    {
                        assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModels.Add(new AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModel()
                        {
                            AssessmentWAWTTDetailID = WetAreaWaterTightnessTestDetail.AssessmentWAWTTDetailID,
                            AssessmentTypeModuleProcessID = WetAreaWaterTightnessTestDetail.AssessmentTypeModuleProcessID,
                            Result = (string.IsNullOrEmpty(WetAreaWaterTightnessTestDetail.Result) ? 3 : int.Parse(WetAreaWaterTightnessTestDetail.Result)),
                            RowNo = WetAreaWaterTightnessTestDetail.RowNo,
                        });
                    }
                    assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModels = new List<AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModel>();
                    foreach (var WetAreaWaterTightnessTestDetailResult in WetAreaWaterTightnessTestMaster.assessment_wet_area_water_tightness_test_tran_detail_result)
                    {
                        assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel.AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModels.Add(new AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModel()
                        {
                            AssessmentWAWTTDetailResultID = WetAreaWaterTightnessTestDetailResult.AssessmentWAWTTDetailResultID,
                            AssessmentTypeModuleProcessID = WetAreaWaterTightnessTestDetailResult.AssessmentTypeModuleProcessID,
                            AssessmentWAWTTResultID = WetAreaWaterTightnessTestDetailResult.AssessmentWAWTTResultID,
                            Result = (string.IsNullOrEmpty(WetAreaWaterTightnessTestDetailResult.Result) ? 3 : int.Parse(WetAreaWaterTightnessTestDetailResult.Result)),
                        });
                    }
                    WetAreaWaterTightnessTestResponseView.AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels.Add(assessmentWetAreaWaterTightnessTestTransMasterMobileViewModel);
                }
                WetAreaWaterTightnessTestResponseView.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return WetAreaWaterTightnessTestResponseView;
        }

        [HttpPost]
        public WetAreaWaterTightnessTestResponseViewModel UpdateWetAreaWaterTightnessTest(List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel> AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels)
        {
            WetAreaWaterTightnessTestResponseViewModel WetAreaWaterTightnessTestResponseViewModel = new WetAreaWaterTightnessTestResponseViewModel();
            WetAreaWaterTightnessTestResponseViewModel.AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels = new List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel>();
            try
            {
                foreach (var EWMasterview in AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels)
                {
                    if (EWMasterview.Status == 2)
                    {
                        AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_ByID(EWMasterview.AssessmentWAWTTID);
                        masterViewModel.Block_Unit = EWMasterview.Block_Unit;
                        masterViewModel.AssessmentDate = DateTime.ParseExact(EWMasterview.AssessmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        masterViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                        masterViewModel.Drawing_Image = EWMasterview.Drawing_Image;
                        masterViewModel.Other_Result = EWMasterview.Other_Result;
                        masterViewModel.CreatedDate = DateTime.Now;
                        foreach (var EWDetailView in EWMasterview.AssessmentWetAreaWaterTightnessTestTransDetailMobileViewModels)
                        {
                            AssessmentWetAreaWaterTightnessTestTransDetailViewModel detailViewModel = masterViewModel.assessment_wet_area_water_tightness_test_tran_detail.Where(a => a.AssessmentWAWTTDetailID == EWDetailView.AssessmentWAWTTDetailID).FirstOrDefault();
                            detailViewModel.Result = (EWDetailView.Result == 3 ? "" : EWDetailView.Result.ToString());
                            detailViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                            detailViewModel.UpdatedDate = DateTime.Now;
                        }
                        foreach (var EWDetailResultView in EWMasterview.AssessmentWetAreaWaterTightnessTestTransDetailResultMobileViewModels)
                        {
                            AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel detailResultViewModel = masterViewModel.assessment_wet_area_water_tightness_test_tran_detail_result.Where(a => a.AssessmentWAWTTDetailResultID == EWDetailResultView.AssessmentWAWTTDetailResultID).FirstOrDefault();
                            detailResultViewModel.Result = (EWDetailResultView.Result == 3 ? "" : EWDetailResultView.Result.ToString());
                            detailResultViewModel.UpdatedBy = EWMasterview.CreatedOrUpdatedByUserId;
                            detailResultViewModel.UpdatedDate = DateTime.Now;
                        }
                        var result = assessmentService.SaveAssessmentWetAreaWaterTightnessTest(masterViewModel);
                    }
                }
                WetAreaWaterTightnessTestResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return WetAreaWaterTightnessTestResponseViewModel;
        }

        [HttpPost]
        public WetAreaWaterTightnessTestResponseViewModel DeleteWetAreaWaterTightnessTest(List<AssessmentWetAreaWaterTightnessTestTransMasterMobileDeleteViewModel> AssessmentWetAreaWaterTightnessTestTransMobileDeleteViewModels)
        {
            WetAreaWaterTightnessTestResponseViewModel WetAreaWaterTightnessTestResponseViewModel = new WetAreaWaterTightnessTestResponseViewModel();
            WetAreaWaterTightnessTestResponseViewModel.AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels = new List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel>();
            try
            {
                string EWIds = "";
                foreach (var EWMasterview in AssessmentWetAreaWaterTightnessTestTransMobileDeleteViewModels)
                {
                    EWIds = EWIds + EWMasterview.AssessmentWAWTTID.ToString() + ",";
                }
                var result = assessmentService.DeleteAssessmentWetAreaWaterTightnessTest(EWIds.Substring(0, EWIds.Length - 1));
                if (result > 0)
                {
                    WetAreaWaterTightnessTestResponseViewModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return WetAreaWaterTightnessTestResponseViewModel;
        }
        #endregion

        #region QC Inspection Projects
        [HttpGet]
        public List<QCInspectionProjectMasterMobileViewModel> GetAllQCInspectionProjects(int CompanyID)
        {
            try
            {
                return FormatQCInspectionProject(qcInspectionService.GetAllProjects().Where(a => a.CompanyID == CompanyID).ToList());
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<QCInspectionProjectMasterMobileViewModel> GetAllQCInspectionProjectsByGroupID(int GroupID, int UserID)
        {
            List<QCInspectionProjectMasterViewModel> projectMasterViewModels = new List<QCInspectionProjectMasterViewModel>();
            try
            {
                if (GroupID == 4)
                {
                    projectMasterViewModels = qcInspectionService.GetAllProjects().Where(a => a.Is_Completed == 0 && a.qcinspection_project_PM_detail.Where(w => w.UserID == UserID).Count() > 0).ToList();
                }
                else if (GroupID == 5)
                {
                    projectMasterViewModels = qcInspectionService.GetAllProjects().Where(a => a.Is_Completed == 0 && a.qcinspection_project_Supervisor_detail.Where(w => w.UserID == UserID).Count() > 0).ToList();
                }
                else if (GroupID == 9)
                {
                    projectMasterViewModels = qcInspectionService.GetAllProjects().Where(a => a.Is_Completed == 0 && a.qcinspection_project_MEInspector_detail.Where(w => w.UserID == UserID).Count() > 0).ToList();
                }
                else if (GroupID == 10)
                {
                    projectMasterViewModels = qcInspectionService.GetAllProjects().Where(a => a.Is_Completed == 0 && a.qcinspection_project_StructureInspector_detail.Where(w => w.UserID == UserID).Count() > 0).ToList();
                }
                else if (GroupID == 11)
                {
                    projectMasterViewModels = qcInspectionService.GetAllProjects().Where(a => a.Is_Completed == 0 && a.qcinspection_project_OtherInspector_detail.Where(w => w.UserID == UserID).Count() > 0).ToList();
                }
                else
                {
                    projectMasterViewModels = qcInspectionService.GetAllProjects().Where(a => a.Is_Completed == 0).ToList();
                }
                return FormatQCInspectionProject(projectMasterViewModels);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        public List<QCInspectionProjectMasterMobileViewModel> FormatQCInspectionProject(List<QCInspectionProjectMasterViewModel> qCInspectionProjectMasterViewModels)
        {
            List<QCInspectionProjectMasterMobileViewModel> qCInspectionProjectMasterMobileViewModels = new List<QCInspectionProjectMasterMobileViewModel>();
            if (qCInspectionProjectMasterViewModels.Count > 0)
            {
                int iCnt = 1;
                QCInspectionProjectMasterMobileViewModel qCInspectionProjectMasterMobileViewModel;
                foreach (var projectMaster in qCInspectionProjectMasterViewModels)
                {
                    qCInspectionProjectMasterMobileViewModel = new QCInspectionProjectMasterMobileViewModel();
                    qCInspectionProjectMasterMobileViewModel.CompanyID = projectMaster.CompanyID;
                    qCInspectionProjectMasterMobileViewModel.ProjectID = projectMaster.ProjectID;
                    qCInspectionProjectMasterMobileViewModel.Project_ID = projectMaster.Project_ID;
                    qCInspectionProjectMasterMobileViewModel.Project_Name = projectMaster.Project_Name;
                    qCInspectionProjectMasterMobileViewModel.StartOn = projectMaster.StartOn;
                    qCInspectionProjectMasterMobileViewModel.EndOn = projectMaster.EndOn;
                    qCInspectionProjectMasterMobileViewModel.Is_Completed = projectMaster.Is_Completed;
                    qCInspectionProjectMasterMobileViewModel.ProjectManagerID = projectMaster.ProjectManagerID;
                    qCInspectionProjectMasterMobileViewModel.ProjectManager_Name = userService.GetUser((int)projectMaster.ProjectManagerID)?.DisplayName;
                    qCInspectionProjectMasterMobileViewModel.Locations = new List<QCInspectionLocationMobileViewModel>();
                    qCInspectionProjectMasterMobileViewModel.PMDetails = new List<QCInspectionProjectPMDetailMobileViewModel>();
                    qCInspectionProjectMasterMobileViewModel.SupervisorDetails = new List<QCInspectionProjectSupervisorDetailMobileViewModel>();
                    qCInspectionProjectMasterMobileViewModel.MEInspectorDetails = new List<QCInspectionProjectMEInspectorDetailMobileViewModel>();
                    qCInspectionProjectMasterMobileViewModel.StructureInspectorDetails = new List<QCInspectionProjectStructureInspectorDetailMobileViewModel>();
                    qCInspectionProjectMasterMobileViewModel.OtherInspectorDetails = new List<QCInspectionProjectOtherInspectorDetailMobileViewModel>();
                    qCInspectionProjectMasterMobileViewModel.RFWIDrawingReferenceFiles = new List<QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel>();
                    qCInspectionProjectMasterMobileViewModel.ProjectFiles = new List<QCInspectionProjectFilesMobileViewModel>();
                    foreach (var PMDetail in projectMaster.qcinspection_project_PM_detail)
                    {
                        qCInspectionProjectMasterMobileViewModel.PMDetails.Add(new QCInspectionProjectPMDetailMobileViewModel()
                        {
                            UserID = PMDetail.UserID,
                            UserName = PMDetail.user.DisplayName,
                            RowNo = iCnt
                        });
                        iCnt++;
                    }
                    iCnt = 1;
                    foreach (var SupervisorDetail in projectMaster.qcinspection_project_Supervisor_detail)
                    {
                        qCInspectionProjectMasterMobileViewModel.SupervisorDetails.Add(new QCInspectionProjectSupervisorDetailMobileViewModel()
                        {
                            UserID = SupervisorDetail.UserID,
                            UserName = SupervisorDetail.user.DisplayName,
                            RowNo = iCnt
                        });
                        iCnt++;
                    }
                    iCnt = 1;
                    foreach (var MEDetail in projectMaster.qcinspection_project_MEInspector_detail)
                    {
                        qCInspectionProjectMasterMobileViewModel.MEInspectorDetails.Add(new QCInspectionProjectMEInspectorDetailMobileViewModel()
                        {
                            UserID = MEDetail.UserID,
                            UserName = MEDetail.user.DisplayName,
                            RowNo = iCnt
                        });
                        iCnt++;
                    }
                    iCnt = 1;
                    foreach (var StructureDetail in projectMaster.qcinspection_project_StructureInspector_detail)
                    {
                        qCInspectionProjectMasterMobileViewModel.StructureInspectorDetails.Add(new QCInspectionProjectStructureInspectorDetailMobileViewModel()
                        {
                            UserID = StructureDetail.UserID,
                            UserName = StructureDetail.user.DisplayName,
                            RowNo = iCnt
                        });
                        iCnt++;
                    }
                    iCnt = 1;
                    foreach (var OtherDetail in projectMaster.qcinspection_project_OtherInspector_detail)
                    {
                        qCInspectionProjectMasterMobileViewModel.OtherInspectorDetails.Add(new QCInspectionProjectOtherInspectorDetailMobileViewModel()
                        {
                            UserID = OtherDetail.UserID,
                            UserName = OtherDetail.user.DisplayName,
                            RowNo = iCnt
                        });
                        iCnt++;
                    }

                    iCnt = 1;
                    foreach (var block in qcInspectionService.GetAllBlocks().Where(x => x.ProjectID == projectMaster.ProjectID).OrderBy(x => x.OrderBy).ToList())
                    {
                        foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                        {
                            foreach (var unit in qcInspectionService.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                            {
                                qCInspectionProjectMasterMobileViewModel.Locations.Add(new QCInspectionLocationMobileViewModel()
                                {
                                    UnitID = unit.UnitID,
                                    UnitName = block.BlockName + " " + level.LevelName + "-" + unit.UnitName,
                                    OrderBy = iCnt
                                });
                                iCnt++;
                            }
                        }
                    }
                    foreach (var RefFile in projectMaster.qcinspection_project_rfwi_drawing_reference_files)
                    {
                        qCInspectionProjectMasterMobileViewModel.RFWIDrawingReferenceFiles.Add(new QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel()
                        {
                            QCInspectionDrawingReferenceFileID = RefFile.QCInspectionDrawingReferenceFileID,
                            FileCaption = RefFile.FileCaption,
                            FileName = RefFile.FileName,
                            //FileBase64 = AppSettings.ConvertFileToBase64String(RefFile.FilePath, RefFile.FileName)
                        });
                    }
                    foreach (var RefFile in projectMaster.qcinspection_project_files)
                    {
                        qCInspectionProjectMasterMobileViewModel.ProjectFiles.Add(new QCInspectionProjectFilesMobileViewModel()
                        {
                            QCInspectionProjectFileID = RefFile.QCInspectionProjectFileID,
                            FileCaption = RefFile.FileCaption,
                            FileName = RefFile.FileName
                            //FileBase64 = AppSettings.ConvertFileToBase64String(RefFile.FilePath, RefFile.FileName)
                        });
                    }
                    qCInspectionProjectMasterMobileViewModels.Add(qCInspectionProjectMasterMobileViewModel);
                }
            }
            return qCInspectionProjectMasterMobileViewModels;
        }

        [HttpGet]
        public List<QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel> GetAllRFWIDrawingReferenceFiles(int ProjectID = 0, int DrawingReferenceFileID = 0)
        {
            try
            {
                List<QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel> RFWIDrawingReferenceFiles = new List<QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel>();
                if(ProjectID > 0 && DrawingReferenceFileID == 0)
                {
                    foreach (var RefFile in qcInspectionService.GetAllRFWIDrawingsReferenceFiles(ProjectID))
                    {
                        RFWIDrawingReferenceFiles.Add(new QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel()
                        {

                            QCInspectionDrawingReferenceFileID = RefFile.QCInspectionDrawingReferenceFileID,
                            FileCaption = RefFile.FileCaption,
                            FileName = RefFile.FileName,
                            FileBase64 = AppSettings.ConvertFileToBase64String(RefFile.FilePath, RefFile.FileName)
                        });
                    }
                }
                else
                {
                    var RefFile = qcInspectionService.GetRFWIDrawingsReferenceFile(DrawingReferenceFileID);
                    RFWIDrawingReferenceFiles.Add(new QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel()
                    {

                        QCInspectionDrawingReferenceFileID = RefFile.QCInspectionDrawingReferenceFileID,
                        FileCaption = RefFile.FileCaption,
                        FileName = RefFile.FileName,
                        FileBase64 = AppSettings.ConvertFileToBase64String(RefFile.FilePath, RefFile.FileName)
                    });
                }
                return RFWIDrawingReferenceFiles;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }


        [HttpGet]
        public List<QCInspectionProjectFilesMobileViewModel> GetAllProjectFiles(int ProjectID = 0, int FileID = 0)
        {
            try
            {
                List<QCInspectionProjectFilesMobileViewModel> RFWIDrawingReferenceFiles = new List<QCInspectionProjectFilesMobileViewModel>();
                if (ProjectID > 0 && FileID == 0)
                {
                    foreach (var RefFile in qcInspectionService.GetAllProjectFiles(ProjectID))
                    {
                        RFWIDrawingReferenceFiles.Add(new QCInspectionProjectFilesMobileViewModel()
                        {

                            QCInspectionProjectFileID = RefFile.QCInspectionProjectFileID,
                            FileCaption = RefFile.FileCaption,
                            FileName = RefFile.FileName,
                            FileBase64 = AppSettings.ConvertFileToBase64String(RefFile.FilePath, RefFile.FileName)
                        });
                    }
                }
                else
                {
                    var RefFile = qcInspectionService.GetProjectFile(FileID);
                    RFWIDrawingReferenceFiles.Add(new QCInspectionProjectFilesMobileViewModel()
                    {
                        QCInspectionProjectFileID = RefFile.QCInspectionProjectFileID,
                        FileCaption = RefFile.FileCaption,
                        FileName = RefFile.FileName,
                        FileBase64 = AppSettings.ConvertFileToBase64String(RefFile.FilePath, RefFile.FileName)
                    });

                }
                return RFWIDrawingReferenceFiles;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }


        #endregion QC Inspection Projects

        #region QC Inspection Masters
        [HttpGet]
        public List<QCInspectionTradeMasterViewModel> GetAllQCInspectionTrades()
        {
            try
            {
                return qcInspectionService.GetAllTrades().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<QCInspectionSubcontractorMasterViewModel> GetAllQCInspectionSubcontractors()
        {
            try
            {
                return qcInspectionService.GetAllSubcontractors().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<QCInspectionDefectTypeMasterViewModel> GetAllQCInspectionDefectTypes()
        {
            try
            {
                return qcInspectionService.GetAllDefectTypes().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<MasterSyncViewModel> GetQCInspectionAndRFWIMasterSync()
        {
            try
            {
                return qcInspectionService.GetQCInspectionAndRFWIMasterSync().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        #endregion QC Inspection Masters

        #region RFWI Masters
        [HttpGet]
        public List<QCInspectionRFWITradeMasterViewModel> GetAllRFWITrades()
        {
            try
            {
                return qcInspectionService.GetAllRFWITrades().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<QCInspectionRFWIGeneralCheckListMasterViewModel> GetAllQCInspectionGeneralCheckLists()
        {
            try
            {
                return qcInspectionService.GetAllRFWIGeneralCheckLists().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<InspectorModel> GetAllRTOInspectors()
        {
            try
            {
                List<InspectorModel> InspectorList = new List<InspectorModel>();
                foreach (var DType in userService.getAllUsers().Where(x => x.GroupID == 8 && x.IsActive == 1).OrderBy(x => x.UserID).ToList())
                {
                    InspectorList.Add(new InspectorModel() { InspectorName = DType.DisplayName, InspectorID = DType.UserID });
                }
                return InspectorList;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<InspectorModel> GetAllMEInspectors()
        {
            try
            {
                List<InspectorModel> InspectorList = new List<InspectorModel>();
                foreach (var DType in userService.getAllUsers().Where(x => x.GroupID == 9 && x.IsActive == 1).OrderBy(x => x.UserID).ToList())
                {
                    InspectorList.Add(new InspectorModel() { InspectorName = DType.DisplayName, InspectorID = DType.UserID });
                }
                return InspectorList;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<InspectorModel> GetAllStructureInspectors()
        {
            try
            {
                List<InspectorModel> InspectorList = new List<InspectorModel>();
                foreach (var DType in userService.getAllUsers().Where(x => x.GroupID == 10 && x.IsActive == 1).OrderBy(x => x.UserID).ToList())
                {
                    InspectorList.Add(new InspectorModel() { InspectorName = DType.DisplayName, InspectorID = DType.UserID });
                }
                return InspectorList;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }

        [HttpGet]
        public List<InspectorModel> GetAllOtherInspectors()
        {
            try
            {
                List<InspectorModel> InspectorList = new List<InspectorModel>();
                foreach (var DType in userService.getAllUsers().Where(x => x.GroupID == 11 && x.IsActive == 1).OrderBy(x => x.UserID).ToList())
                {
                    InspectorList.Add(new InspectorModel() { InspectorName = DType.DisplayName, InspectorID = DType.UserID });
                }
                return InspectorList;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
        }
        #endregion RFWI Masters

        #region QC Inspection Form
        [HttpGet]
        public List<QCInspectionDefectFormMobileViewModel> GetQCInspectionDefectFormList(int GroupID, int UserID)
        {
            List<QCInspectionDefectFormViewModel> qCInspectionDefectFormViewModels = new List<QCInspectionDefectFormViewModel>();
            if (GroupID == 1)
            {
                qCInspectionDefectFormViewModels = qcInspectionService.GetAllQCInspectionDefectForms().ToList();
            }
            else if (GroupID == 4)
            {
                qCInspectionDefectFormViewModels = qcInspectionService.GetAllQCInspectionDefectForms().Where(x => x.ProjectManagerID == UserID).ToList();
            }
            else if (GroupID == 5)
            {
                qCInspectionDefectFormViewModels = qcInspectionService.GetAllQCInspectionDefectFormsBasedUserID(UserID).ToList();
            }
            else if (GroupID == 6)
            {
                int subid = Convert.ToInt32(userService.GetUser(UserID).SubCon_ID);
                qCInspectionDefectFormViewModels = qcInspectionService.GetAllQCInspectionDefectFormsBasedSubcontractorID(subid).ToList();
            }
            else
            {
                qCInspectionDefectFormViewModels = qcInspectionService.GetAllQCInspectionDefectForms().ToList();
            }
            int iCnt = 1;
            List<QCInspectionLocationMobileViewModel> QCInspectionLocationMobileViewModels = new List<QCInspectionLocationMobileViewModel>();
            foreach (var block in qcInspectionService.GetAllBlocks().OrderBy(x => x.OrderBy).ToList())
            {
                foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                {
                    foreach (var unit in qcInspectionService.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                    {
                        QCInspectionLocationMobileViewModels.Add(new QCInspectionLocationMobileViewModel()
                        {
                            UnitID = unit.UnitID,
                            UnitName = block.BlockName + " " + level.LevelName + "-" + unit.UnitName,
                            OrderBy = iCnt
                        });
                        iCnt++;
                    }
                }
            }
            List<QCInspectionProjectMasterViewModel> projectMasterViewModels = qcInspectionService.GetAllProjects().Where(a => a.Is_Completed == 0).ToList();
            List<UserViewModel> userViewModels = userService.getAllUsers();

            List<QCInspectionDefectFormMobileViewModel> QCInspectionDefectFormMobileViewModels = new List<QCInspectionDefectFormMobileViewModel>();
            foreach (var DefectForm in qCInspectionDefectFormViewModels)
            {
                QCInspectionDefectFormMobileViewModel qCInspectionDefectFormMobileViewModel = new QCInspectionDefectFormMobileViewModel();
                qCInspectionDefectFormMobileViewModel.QCInspectionDefectID = DefectForm.QCInspectionDefectID;
                qCInspectionDefectFormMobileViewModel.QCInspectionDefectNo = DefectForm.QCInspectionDefectNo;
                qCInspectionDefectFormMobileViewModel.ProjectID = DefectForm.ProjectID;
                qCInspectionDefectFormMobileViewModel.ProjectName = projectMasterViewModels.Where(x=> x.ProjectID == DefectForm.ProjectID).FirstOrDefault().Project_Name;
                qCInspectionDefectFormMobileViewModel.UnitID = DefectForm.UnitID;
                qCInspectionDefectFormMobileViewModel.UnitName = QCInspectionLocationMobileViewModels.Where(x=> x.UnitID == DefectForm.UnitID).FirstOrDefault().UnitName;
                qCInspectionDefectFormMobileViewModel.TradeID = DefectForm.TradeID;
                qCInspectionDefectFormMobileViewModel.TradeName = DefectForm.qcinspection_trade_master.TradeName;
                qCInspectionDefectFormMobileViewModel.DefectTypeID = DefectForm.DefectTypeID;
                qCInspectionDefectFormMobileViewModel.DefectTypeName = DefectForm.qcinspection_defect_type_master.DefectName;
                qCInspectionDefectFormMobileViewModel.DefectRemarks = DefectForm.DefectRemarks;
                qCInspectionDefectFormMobileViewModel.SubcontractorID = DefectForm.SubcontractorID;
                qCInspectionDefectFormMobileViewModel.SubcontractorName = DefectForm.qcinspection_subcontractor_master.Name;
                qCInspectionDefectFormMobileViewModel.ProjectManagerID = DefectForm.ProjectManagerID;
                qCInspectionDefectFormMobileViewModel.ProjectManagerName = DefectForm.user.DisplayName;

                qCInspectionDefectFormMobileViewModel.ApprovedBy = DefectForm.ApprovedBy;
                if (DefectForm.ApprovedDate != null)
                {
                    qCInspectionDefectFormMobileViewModel.ApprovedByName = userViewModels.Where(x => x.UserID == DefectForm.ApprovedBy).FirstOrDefault().DisplayName;
                    qCInspectionDefectFormMobileViewModel.ApprovedDate = string.Format("{0:dd/MM/yyyy}", DefectForm.ApprovedDate);
                }
                qCInspectionDefectFormMobileViewModel.ApprovedRemarks = DefectForm.ApprovedRemarks;

                qCInspectionDefectFormMobileViewModel.ReDoBy = DefectForm.ReDoBy;
                if (DefectForm.ReDoDate != null)
                {
                    qCInspectionDefectFormMobileViewModel.ReDoByName = userViewModels.Where(x => x.UserID == DefectForm.ReDoBy).FirstOrDefault().DisplayName;
                    qCInspectionDefectFormMobileViewModel.ReDoDate = string.Format("{0:dd/MM/yyyy}", DefectForm.ReDoDate);
                }
                qCInspectionDefectFormMobileViewModel.ReDoRemarks = DefectForm.ReDoRemarks;

                qCInspectionDefectFormMobileViewModel.ReDoDoneBy = DefectForm.ReDoDoneBy;
                if (DefectForm.ReDoDoneDate != null)
                {
                    qCInspectionDefectFormMobileViewModel.ReDoDoneByName = userViewModels.Where(x => x.UserID == DefectForm.ReDoDoneBy).FirstOrDefault().DisplayName;
                    qCInspectionDefectFormMobileViewModel.ReDoDoneDate = string.Format("{0:dd/MM/yyyy}", DefectForm.ReDoDoneDate);
                }
                qCInspectionDefectFormMobileViewModel.ReDoDoneRemarks = DefectForm.ReDoDoneRemarks;
                
                qCInspectionDefectFormMobileViewModel.RectificationBy = DefectForm.RectificationBy;
                if (DefectForm.RectificationDate != null)
                {
                    qCInspectionDefectFormMobileViewModel.RectificationByName = userViewModels.Where(x => x.UserID == DefectForm.RectificationBy).FirstOrDefault().DisplayName;
                    qCInspectionDefectFormMobileViewModel.RectificationDate = string.Format("{0:dd/MM/yyyy}", DefectForm.RectificationDate);
                }
                qCInspectionDefectFormMobileViewModel.RectificationRemarks = DefectForm.RectificationRemarks;
                qCInspectionDefectFormMobileViewModel.RectificationSignature = DefectForm.RectificationSignature;
                qCInspectionDefectFormMobileViewModel.ReworkBy = DefectForm.ReworkBy;
                if (DefectForm.ReworkDate != null)
                {
                    qCInspectionDefectFormMobileViewModel.ReworkByName = userViewModels.Where(x => x.UserID == DefectForm.ReworkBy).FirstOrDefault().DisplayName;
                    qCInspectionDefectFormMobileViewModel.ReworkDate = string.Format("{0:dd/MM/yyyy}", DefectForm.ReworkDate);
                }
                qCInspectionDefectFormMobileViewModel.ReworkRemarks = DefectForm.ReworkRemarks;
                qCInspectionDefectFormMobileViewModel.CompletedBy = DefectForm.CompletedBy;
                if (DefectForm.CompletedDate != null)
                {
                    qCInspectionDefectFormMobileViewModel.CompletedByName = userViewModels.Where(x => x.UserID == DefectForm.CompletedBy).FirstOrDefault().DisplayName;
                    qCInspectionDefectFormMobileViewModel.CompletedDate = string.Format("{0:dd/MM/yyyy}", DefectForm.CompletedDate);
                }
                qCInspectionDefectFormMobileViewModel.CompletedRemarks = DefectForm.CompletedRemarks;
                qCInspectionDefectFormMobileViewModel.CompletedSignature = DefectForm.CompletedSignature;
                qCInspectionDefectFormMobileViewModel.Status = DefectForm.Status;
                qCInspectionDefectFormMobileViewModel.MobileQCInspectionDefectID = DefectForm.MobileQCInspectionDefectID;
                qCInspectionDefectFormMobileViewModel.BatchID = DefectForm.BatchID;
                qCInspectionDefectFormMobileViewModel.MobileStatus = 0;
                qCInspectionDefectFormMobileViewModel.CreatedOrUpdatedByUserId = 0;
                qCInspectionDefectFormMobileViewModel.CreatedOrUpdatedDate = string.Format("{0:dd/MM/yyyy}", DefectForm.CreatedDate);
                qCInspectionDefectFormMobileViewModel.DefectFiles = new List<QCInspectionDefectFormFilesMobileViewModel>();
                //foreach (var Defectfile in DefectForm.qcinspection_defect_files)
                //{
                //    if(Defectfile.FilePath!=null)
                //    {
                //        qCInspectionDefectFormMobileViewModel.DefectFiles.Add(new QCInspectionDefectFormFilesMobileViewModel()
                //        {
                //            QCInspectionDefectFileID = Defectfile.QCInspectionDefectFileID,
                //            FileFor = Defectfile.FileFor,
                //            FileCaption = Defectfile.FileCaption,
                //            FileName = Defectfile.FileName,
                //            FileBase64 = AppSettings.ConvertFileToBase64String(Defectfile.FilePath, Defectfile.FileName)
                //        });
                //    }
                //}
                QCInspectionDefectFormMobileViewModels.Add(qCInspectionDefectFormMobileViewModel);
            }
            return QCInspectionDefectFormMobileViewModels;
        }

        [HttpGet]
        public List<QCInspectionDefectFormFilesMobileViewModel> GetQCInspectionDefectFormFileList(int QCInspectionDefectID)
        {
            List<QCInspectionDefectFormFilesMobileViewModel> QCInspectionDefectFiles = new List<QCInspectionDefectFormFilesMobileViewModel>();
            var QCInspectionDefectForm = qcInspectionService.GetQCInspectionDefectForm(QCInspectionDefectID);
            if(QCInspectionDefectForm != null)
            {
                foreach (var Defectfile in QCInspectionDefectForm.qcinspection_defect_files)
                {
                    if (Defectfile.FilePath != null)
                    {
                        QCInspectionDefectFiles.Add(new QCInspectionDefectFormFilesMobileViewModel()
                        {
                            QCInspectionDefectFileID = Defectfile.QCInspectionDefectFileID,
                            FileFor = Defectfile.FileFor,
                            FileCaption = Defectfile.FileCaption,
                            FileName = Defectfile.FileName,
                            FileBase64 = AppSettings.ConvertFileToBase64String(Defectfile.FilePath, Defectfile.FileName)
                        });
                    }
                }
            }
            return QCInspectionDefectFiles;
        }

        [HttpPost]
        public QCInspectionDefectFormResponseViewModel SaveQCInspectionDefectForm(List<QCInspectionDefectFormMobileViewModel> QCInspectionDefectFormMobileViewModelS)
        {
            QCInspectionDefectFormResponseViewModel qCInspectionDefectFormResponseViewModel = new QCInspectionDefectFormResponseViewModel();
            qCInspectionDefectFormResponseViewModel.QCInspectionDefectFormMobileViewModels = new List<QCInspectionDefectFormMobileViewModel>();
            int ProjectID = Convert.ToInt32(QCInspectionDefectFormMobileViewModelS.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                QCInspectionDefectFormViewModel QCInspection;
                foreach (var QCDefectForm in QCInspectionDefectFormMobileViewModelS)
                {
                    if (QCDefectForm.MobileStatus == 1)
                    {
                        QCInspection = new QCInspectionDefectFormViewModel();
                        QCInspection.QCInspectionDefectNo = QCDefectForm.QCInspectionDefectNo;
                        QCInspection.ProjectID = QCDefectForm.ProjectID;
                        QCInspection.UnitID = QCDefectForm.UnitID;
                        QCInspection.TradeID = QCDefectForm.TradeID;
                        QCInspection.DefectTypeID = QCDefectForm.DefectTypeID;
                        QCInspection.DefectRemarks = QCDefectForm.DefectRemarks;
                        QCInspection.SubcontractorID = QCDefectForm.SubcontractorID;
                        QCInspection.ProjectManagerID = QCDefectForm.ProjectManagerID;
                        QCInspection.ApprovedBy = QCDefectForm.ApprovedBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.ApprovedDate))
                        {
                            QCInspection.ApprovedDate = DateTime.ParseExact(QCDefectForm.ApprovedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.ApprovedRemarks = QCDefectForm.ApprovedRemarks;
                        QCInspection.ReDoBy = QCDefectForm.ReDoBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.ReDoDate))
                        {
                            QCInspection.ReDoDate = DateTime.ParseExact(QCDefectForm.ReDoDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.ReDoRemarks = QCDefectForm.ReDoRemarks;
                        QCInspection.ReDoDoneBy = QCDefectForm.ReDoDoneBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.ReDoDoneDate))
                        {
                            QCInspection.ReDoDoneDate = DateTime.ParseExact(QCDefectForm.ReDoDoneDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.ReDoDoneRemarks = QCDefectForm.ReDoDoneRemarks;
                        QCInspection.RectificationBy = QCDefectForm.RectificationBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.RectificationDate))
                        {
                            QCInspection.RectificationDate = DateTime.ParseExact(QCDefectForm.RectificationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.RectificationRemarks = QCDefectForm.RectificationRemarks;
                        QCInspection.RectificationSignature = QCDefectForm.RectificationSignature;
                        QCInspection.ReworkBy = QCDefectForm.ReworkBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.ReworkDate))
                        {
                            QCInspection.ReworkDate = DateTime.ParseExact(QCDefectForm.ReworkDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.ReworkRemarks = QCDefectForm.ReworkRemarks;
                        QCInspection.CompletedBy = QCDefectForm.CompletedBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.CompletedDate))
                        {
                            QCInspection.CompletedDate = DateTime.ParseExact(QCDefectForm.CompletedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.CompletedRemarks = QCDefectForm.CompletedRemarks;
                        QCInspection.CompletedSignature = QCDefectForm.CompletedSignature;
                        QCInspection.Status = QCDefectForm.Status;
                        QCInspection.MobileQCInspectionDefectID = QCDefectForm.MobileQCInspectionDefectID;
                        QCInspection.BatchID = BatchID;
                        QCInspection.CreatedBy = QCDefectForm.CreatedOrUpdatedByUserId;
                        QCInspection.CreatedDate = DateTime.ParseExact(QCDefectForm.CreatedOrUpdatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        QCInspection.FilePath = System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/");
                        QCInspection.DefectFormFiles = new List<QCInspectionDefectFormFilesMobileViewModel>();
                        QCInspection.DefectFormFiles = QCDefectForm.DefectFiles;
                        var result = qcInspectionService.CreateQCInspectionDefectForm(QCInspection);
                        if (result > 0)
                        {
                            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/") + result.ToString() + "/Defect"))
                            {
                                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/") + result.ToString() + "/Defect");
                                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/") + result.ToString() + "/Rectify");
                            }
                            foreach (var DefectFile in QCDefectForm.DefectFiles)
                            {
                                if (DefectFile?.FileBase64 != null)
                                {
                                    AppSettings.ConvertBase64StringToFile(DefectFile.FileBase64, System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/") + result.ToString() + "/" + DefectFile.FileFor + "/" + DefectFile.FileName);
                                }
                            }
                        }
                    }
                }

                List<QCInspectionDefectFormViewModel> qCInspectionDefectFormViewModels = qcInspectionService.GetAllQCInspectionDefectFormsBasedProjectID(ProjectID, BatchID).ToList();
                if(qCInspectionDefectFormViewModels?.Count > 0)
                {
                    int iCnt = 1;
                    List<QCInspectionLocationMobileViewModel> QCInspectionLocationMobileViewModels = new List<QCInspectionLocationMobileViewModel>();
                    foreach (var block in qcInspectionService.GetAllBlocks().Where(x=> x.ProjectID == ProjectID).OrderBy(x => x.OrderBy).ToList())
                    {
                        foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                        {
                            foreach (var unit in qcInspectionService.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                            {
                                QCInspectionLocationMobileViewModels.Add(new QCInspectionLocationMobileViewModel()
                                {
                                    UnitID = unit.UnitID,
                                    UnitName = block.BlockName + " " + level.LevelName + "-" + unit.UnitName,
                                    OrderBy = iCnt
                                });
                                iCnt++;
                            }
                        }
                    }
                    QCInspectionProjectMasterViewModel projectMasterViewModel = qcInspectionService.GetProject(ProjectID);
                    List<UserViewModel> userViewModels = userService.getAllUsers();
                    List<QCInspectionDefectTypeMasterViewModel> QCInspectionDefectTypeMasterViewModels = qcInspectionService.GetAllDefectTypes();
                    List<QCInspectionTradeMasterViewModel> QCInspectionTradeMasterViewModels = qcInspectionService.GetAllTrades();
                    List<QCInspectionSubcontractorMasterViewModel> QCInspectionSubcontractorMasterViewModels = qcInspectionService.GetAllSubcontractorsBasedCompanyID(int.Parse(projectMasterViewModel.CompanyID.ToString()));

                    foreach (var DefectForm in qCInspectionDefectFormViewModels)
                    {
                        QCInspectionDefectFormMobileViewModel qCInspectionDefectFormMobileViewModel = new QCInspectionDefectFormMobileViewModel();
                        qCInspectionDefectFormMobileViewModel.QCInspectionDefectID = DefectForm.QCInspectionDefectID;
                        qCInspectionDefectFormMobileViewModel.QCInspectionDefectNo = DefectForm.QCInspectionDefectNo;
                        qCInspectionDefectFormMobileViewModel.ProjectID = DefectForm.ProjectID;
                        qCInspectionDefectFormMobileViewModel.ProjectName = projectMasterViewModel.Project_Name;
                        qCInspectionDefectFormMobileViewModel.UnitID = DefectForm.UnitID;
                        qCInspectionDefectFormMobileViewModel.UnitName = QCInspectionLocationMobileViewModels.Where(x => x.UnitID == DefectForm.UnitID).FirstOrDefault().UnitName;
                        qCInspectionDefectFormMobileViewModel.TradeID = DefectForm.TradeID;
                        qCInspectionDefectFormMobileViewModel.TradeName = QCInspectionTradeMasterViewModels.Where(x => x.TradeID == DefectForm.TradeID).FirstOrDefault().TradeName;
                        qCInspectionDefectFormMobileViewModel.DefectTypeID = DefectForm.DefectTypeID;
                        qCInspectionDefectFormMobileViewModel.DefectTypeName = QCInspectionDefectTypeMasterViewModels.Where(x => x.DefectTypeID == DefectForm.DefectTypeID).FirstOrDefault().DefectName;
                        qCInspectionDefectFormMobileViewModel.DefectRemarks = DefectForm.DefectRemarks;
                        qCInspectionDefectFormMobileViewModel.SubcontractorID = DefectForm.SubcontractorID;
                        qCInspectionDefectFormMobileViewModel.SubcontractorName = QCInspectionSubcontractorMasterViewModels.Where(x => x.SubcontractorID == DefectForm.SubcontractorID).FirstOrDefault().Name;
                        qCInspectionDefectFormMobileViewModel.ProjectManagerID = DefectForm.ProjectManagerID;
                        qCInspectionDefectFormMobileViewModel.ProjectManagerName = userViewModels.Where(x => x.UserID == DefectForm.ProjectManagerID).FirstOrDefault().DisplayName;
                        qCInspectionDefectFormMobileViewModel.ApprovedBy = DefectForm.ApprovedBy;
                        if (DefectForm.ApprovedDate != null)
                        {
                            qCInspectionDefectFormMobileViewModel.ApprovedByName = userViewModels.Where(x => x.UserID == DefectForm.ApprovedBy).FirstOrDefault().DisplayName;
                            qCInspectionDefectFormMobileViewModel.ApprovedDate = string.Format("{0:dd/MM/yyyy}", DefectForm.ApprovedDate);
                        }
                        qCInspectionDefectFormMobileViewModel.ApprovedRemarks = DefectForm.ApprovedRemarks;

                        qCInspectionDefectFormMobileViewModel.ReDoBy = DefectForm.ReDoBy;
                        if (DefectForm.ReDoDate != null)
                        {
                            qCInspectionDefectFormMobileViewModel.ReDoByName = userViewModels.Where(x => x.UserID == DefectForm.ReDoBy).FirstOrDefault().DisplayName;
                            qCInspectionDefectFormMobileViewModel.ReDoDate = string.Format("{0:dd/MM/yyyy}", DefectForm.ReDoDate);
                        }
                        qCInspectionDefectFormMobileViewModel.ReDoRemarks = DefectForm.ReDoRemarks;

                        qCInspectionDefectFormMobileViewModel.ReDoDoneBy = DefectForm.ReDoDoneBy;
                        if (DefectForm.ReDoDoneDate != null)
                        {
                            qCInspectionDefectFormMobileViewModel.ReDoDoneByName = userViewModels.Where(x => x.UserID == DefectForm.ReDoDoneBy).FirstOrDefault().DisplayName;
                            qCInspectionDefectFormMobileViewModel.ReDoDoneDate = string.Format("{0:dd/MM/yyyy}", DefectForm.ReDoDoneDate);
                        }
                        qCInspectionDefectFormMobileViewModel.ReDoDoneRemarks = DefectForm.ReDoDoneRemarks;

                        qCInspectionDefectFormMobileViewModel.RectificationBy = DefectForm.RectificationBy;
                        if (DefectForm.RectificationDate != null)
                        {
                            qCInspectionDefectFormMobileViewModel.RectificationByName = userViewModels.Where(x => x.UserID == DefectForm.RectificationBy).FirstOrDefault().DisplayName;
                            qCInspectionDefectFormMobileViewModel.RectificationDate = string.Format("{0:dd/MM/yyyy}", DefectForm.RectificationDate);
                        }
                        qCInspectionDefectFormMobileViewModel.RectificationRemarks = DefectForm.RectificationRemarks;
                        qCInspectionDefectFormMobileViewModel.RectificationSignature = DefectForm.RectificationSignature;
                        qCInspectionDefectFormMobileViewModel.ReworkBy = DefectForm.ReworkBy;
                        if (DefectForm.ReworkDate != null)
                        {
                            qCInspectionDefectFormMobileViewModel.ReworkByName = userViewModels.Where(x => x.UserID == DefectForm.ReworkBy).FirstOrDefault().DisplayName;
                            qCInspectionDefectFormMobileViewModel.ReworkDate = string.Format("{0:dd/MM/yyyy}", DefectForm.ReworkDate);
                        }
                        qCInspectionDefectFormMobileViewModel.ReworkRemarks = DefectForm.ReworkRemarks;
                        qCInspectionDefectFormMobileViewModel.CompletedBy = DefectForm.CompletedBy;
                        if (DefectForm.CompletedDate != null)
                        {
                            qCInspectionDefectFormMobileViewModel.CompletedByName = userViewModels.Where(x => x.UserID == DefectForm.CompletedBy).FirstOrDefault().DisplayName;
                            qCInspectionDefectFormMobileViewModel.CompletedDate = string.Format("{0:dd/MM/yyyy}", DefectForm.CompletedDate);
                        }
                        qCInspectionDefectFormMobileViewModel.CompletedRemarks = DefectForm.CompletedRemarks;
                        qCInspectionDefectFormMobileViewModel.CompletedSignature = DefectForm.CompletedSignature;
                        qCInspectionDefectFormMobileViewModel.Status = DefectForm.Status;
                        qCInspectionDefectFormMobileViewModel.MobileQCInspectionDefectID = DefectForm.MobileQCInspectionDefectID;
                        qCInspectionDefectFormMobileViewModel.BatchID = DefectForm.BatchID;
                        qCInspectionDefectFormMobileViewModel.MobileStatus = 0;
                        qCInspectionDefectFormMobileViewModel.CreatedOrUpdatedByUserId = 0;
                        qCInspectionDefectFormMobileViewModel.CreatedOrUpdatedDate = string.Format("{0:dd/MM/yyyy}", DefectForm.CreatedDate);
                        qCInspectionDefectFormMobileViewModel.DefectFiles = new List<QCInspectionDefectFormFilesMobileViewModel>();
                        //foreach (var Defectfile in DefectForm.qcinspection_defect_files)
                        //{
                        //    if (Defectfile.FilePath != null)
                        //    {
                        //        qCInspectionDefectFormMobileViewModel.DefectFiles.Add(new QCInspectionDefectFormFilesMobileViewModel()
                        //        {
                        //            QCInspectionDefectFileID = Defectfile.QCInspectionDefectFileID,
                        //            FileFor = Defectfile.FileFor,
                        //            FileCaption = Defectfile.FileCaption,
                        //            FileName = Defectfile.FileName,
                        //            FileBase64 = AppSettings.ConvertFileToBase64String(Defectfile.FilePath, Defectfile.FileName)
                        //        });
                        //    }
                        //}
                        qCInspectionDefectFormResponseViewModel.QCInspectionDefectFormMobileViewModels.Add(qCInspectionDefectFormMobileViewModel);
                    }
                }
                qCInspectionDefectFormResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return qCInspectionDefectFormResponseViewModel;
        }

        [HttpPost]
        public QCInspectionDefectFormResponseViewModel UpdateQCInspectionDefectForm(List<QCInspectionDefectFormMobileViewModel> QCInspectionDefectFormMobileViewModels)
        {
            QCInspectionDefectFormResponseViewModel qCInspectionDefectFormResponseViewModel = new QCInspectionDefectFormResponseViewModel();
            qCInspectionDefectFormResponseViewModel.QCInspectionDefectFormMobileViewModels = new List<QCInspectionDefectFormMobileViewModel>();
            int ProjectID = Convert.ToInt32(QCInspectionDefectFormMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                QCInspectionDefectFormViewModel QCInspection;
                foreach (var QCDefectForm in QCInspectionDefectFormMobileViewModels)
                {
                    if (QCDefectForm.MobileStatus == 2)
                    {
                        QCInspection = new QCInspectionDefectFormViewModel();
                        QCInspection.QCInspectionDefectID = QCDefectForm.QCInspectionDefectID;
                        QCInspection.QCInspectionDefectNo = QCDefectForm.QCInspectionDefectNo;
                        QCInspection.ProjectID = QCDefectForm.ProjectID;
                        QCInspection.UnitID = QCDefectForm.UnitID;
                        QCInspection.TradeID = QCDefectForm.TradeID;
                        QCInspection.DefectTypeID = QCDefectForm.DefectTypeID;
                        QCInspection.DefectRemarks = QCDefectForm.DefectRemarks;
                        QCInspection.SubcontractorID = QCDefectForm.SubcontractorID;
                        QCInspection.ProjectManagerID = QCDefectForm.ProjectManagerID;
                        QCInspection.ApprovedBy = QCDefectForm.ApprovedBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.ApprovedDate))
                        {
                            QCInspection.ApprovedDate = DateTime.ParseExact(QCDefectForm.ApprovedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.ApprovedRemarks = QCDefectForm.ApprovedRemarks;
                        QCInspection.ReDoBy = QCDefectForm.ReDoBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.ReDoDate))
                        {
                            QCInspection.ReDoDate = DateTime.ParseExact(QCDefectForm.ReDoDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.ReDoRemarks = QCDefectForm.ReDoRemarks;
                        QCInspection.ReDoDoneBy = QCDefectForm.ReDoDoneBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.ReDoDoneDate))
                        {
                            QCInspection.ReDoDoneDate = DateTime.ParseExact(QCDefectForm.ReDoDoneDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.ReDoDoneRemarks = QCDefectForm.ReDoDoneRemarks;
                        QCInspection.RectificationBy = QCDefectForm.RectificationBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.RectificationDate))
                        {
                            QCInspection.RectificationDate = DateTime.ParseExact(QCDefectForm.RectificationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.RectificationRemarks = QCDefectForm.RectificationRemarks;
                        QCInspection.RectificationSignature = QCDefectForm.RectificationSignature;
                        QCInspection.ReworkBy = QCDefectForm.ReworkBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.ReworkDate))
                        {
                            QCInspection.ReworkDate = DateTime.ParseExact(QCDefectForm.ReworkDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.ReworkRemarks = QCDefectForm.ReworkRemarks;
                        QCInspection.CompletedBy = QCDefectForm.CompletedBy;
                        if (!string.IsNullOrEmpty(QCDefectForm.CompletedDate))
                        {
                            QCInspection.CompletedDate = DateTime.ParseExact(QCDefectForm.CompletedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        QCInspection.CompletedRemarks = QCDefectForm.CompletedRemarks;
                        QCInspection.CompletedSignature = QCDefectForm.CompletedSignature;
                        QCInspection.Status = QCDefectForm.Status;
                        QCInspection.MobileQCInspectionDefectID = QCDefectForm.MobileQCInspectionDefectID;
                        QCInspection.BatchID = QCDefectForm.BatchID;
                        QCInspection.UpdatedBy = QCDefectForm.CreatedOrUpdatedByUserId;
                        QCInspection.UpdatedDate = DateTime.ParseExact(QCDefectForm.CreatedOrUpdatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        QCInspection.FilePath = System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/");
                        QCInspection.DefectFormFiles = new List<QCInspectionDefectFormFilesMobileViewModel>();
                        QCInspection.DefectFormFiles = QCDefectForm.DefectFiles;
                        var result = qcInspectionService.MobileSaveQCInspectionDefectForm(QCInspection);
                        if (result > 0)
                        {
                            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/") + QCDefectForm.QCInspectionDefectID.ToString() + "/Defect"))
                            {
                                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/") + QCDefectForm.QCInspectionDefectID.ToString() + "/Defect");
                                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/") + QCDefectForm.QCInspectionDefectID.ToString() + "/Rectify");
                            }
                            foreach (var DefectFile in QCDefectForm.DefectFiles)
                            {
                                if (DefectFile?.FileBase64 != null)
                                {
                                    AppSettings.ConvertBase64StringToFile(DefectFile.FileBase64, System.Web.HttpContext.Current.Server.MapPath("~/images/QCInspection/") + QCDefectForm.QCInspectionDefectID.ToString() + "/" + DefectFile.FileFor + "/" + DefectFile.FileName);
                                }
                            }
                        }
                    }
                }
                qCInspectionDefectFormResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return qCInspectionDefectFormResponseViewModel;
        }

        [HttpPost]
        public QCInspectionDefectFormResponseViewModel DeleteQCInspectionDefectForm(List<QCInspectionDefectFormMobileDeleteViewModel> QCInspectionDefectFormMobileDeleteViewModels,string UserID)
        {
            QCInspectionDefectFormResponseViewModel qCInspectionDefectFormResponseViewModel = new QCInspectionDefectFormResponseViewModel();
            qCInspectionDefectFormResponseViewModel.QCInspectionDefectFormMobileViewModels = new List<QCInspectionDefectFormMobileViewModel>();
            try
            {
                string QCIDIds = "";
                foreach (var DefectForm in QCInspectionDefectFormMobileDeleteViewModels)
                {
                    QCIDIds = QCIDIds + DefectForm.QCInspectionDefectID.ToString() + ",";
                }
                var result = qcInspectionService.MobileDeleteQCInspectionDefectForm(QCIDIds.Substring(0, QCIDIds.Length - 1), UserID);
                if (result > 0)
                {
                    qCInspectionDefectFormResponseViewModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return qCInspectionDefectFormResponseViewModel;
        }

        #endregion

        #region RFWI Form
        [HttpGet]
        public List<AvailableSlotsModel> GetRFWIFormAvailableSlots(int InspectorID, string InspectionDate)
        {
            List<AvailableSlotsModel> availableSlotsModels = new List<AvailableSlotsModel>();
            foreach (var objIns in qcInspectionService.GetAllRFWIFormsBasedRTOInspectorID(InspectorID).Where(x => x.Status != "Rejected" && x.InspectionOn == DateTime.ParseExact(InspectionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) && x.InspectionStartOn != null && x.InspectionEndOn != null).OrderBy(w => w.InspectionStartOn).ToList())
            {
                availableSlotsModels.Add(new AvailableSlotsModel()
                {
                    ProjectName = objIns.qcinspection_project_master.Project_Name,
                    QCInspectionRFWINo = objIns.QCInspectionRFWINo + "  " + DateTime.Today.Add(objIns.InspectionStartOn.Value).ToString("hh:mm tt") + " to " + DateTime.Today.Add(objIns.InspectionEndOn.Value).ToString("hh:mm tt"),
                    InspectionStartOn = objIns.InspectionStartOn,
                    InspectionEndOn = objIns.InspectionEndOn
                });
            }
            return availableSlotsModels;
        }

        [HttpGet]
        public List<QCInspectionRFWIFormMobileViewModel> GetRFWIFormList(int GroupID, int UserID, int ProjectID = 0)
        {
            List<QCInspectionRFWIFormViewModel> qCInspectionRFWIFormViewModels = new List<QCInspectionRFWIFormViewModel>();
            List<QCInspectionProjectMasterViewModel> projectMasterViewModels = qcInspectionService.GetAllProjects().Where(a => a.Is_Completed == 0).ToList();
            List<UserViewModel> userViewModels = userService.getAllUsers();
            List<QCInspectionRFWITradeMasterViewModel> QCInspectionRFWITradeMasterViewModels = qcInspectionService.GetAllRFWITrades();

            List<int> Projectids = new List<int>();
            int? CompanyID = userViewModels.Find(x=> x.UserID == UserID).CompanyID;
            if (GroupID < 5)
            {
                qCInspectionRFWIFormViewModels = qcInspectionService.GetAllRFWIForms().ToList();
                Projectids = projectMasterViewModels.Where(a => a.CompanyID == CompanyID).Select(a => a.ProjectID).ToList();
            }
            else if (GroupID == 5)
            {
                qCInspectionRFWIFormViewModels = qcInspectionService.GetAllRFWIFormsBasedUserID(UserID).ToList();
                Projectids = projectMasterViewModels.Where(a => a.CompanyID == CompanyID).Select(a => a.ProjectID).ToList();
            }
            // RTO Inspector
            else if (GroupID == 8)
            {
                qCInspectionRFWIFormViewModels = qcInspectionService.GetAllRFWIFormsBasedRTOInspectorID(UserID).ToList();
                Projectids = qcInspectionService.GetAllProjects().Where(a => a.CompanyID == CompanyID).Select(a => a.ProjectID).ToList();
            }
            // M&E Inspector
            else if (GroupID == 9)
            {
                qCInspectionRFWIFormViewModels = qcInspectionService.GetAllRFWIForms().Where(a => ((a.OtherTradeClearance_MandE == true && a.OtherTradeClearance_MandESignature == null && a.Status == "Pending") || a.OtherTradeClearance_MandEBy == UserID) && a.qcinspection_rfwi_form_location_detail.Count() > 0).ToList();
                Projectids = projectMasterViewModels.Where(a => a.CompanyID == CompanyID && a.qcinspection_project_MEInspector_detail.Where(w => w.UserID == UserID).Count() > 0).Select(a => a.ProjectID).ToList();
            }
            // Structure Inspector
            else if (GroupID == 10)
            {
                qCInspectionRFWIFormViewModels = qcInspectionService.GetAllRFWIForms().Where(a => ((a.OtherTradeClearance_Structure == true && a.OtherTradeClearance_StructureSignature == null && a.Status == "Pending") || a.OtherTradeClearance_StructureBy == UserID) && a.qcinspection_rfwi_form_location_detail.Count() > 0).ToList();
                Projectids = projectMasterViewModels.Where(a => a.CompanyID == CompanyID && a.qcinspection_project_StructureInspector_detail.Where(w => w.UserID == UserID).Count() > 0).Select(a => a.ProjectID).ToList();
            }
            // Other Inspector
            else if (GroupID == 11)
            {
                qCInspectionRFWIFormViewModels = qcInspectionService.GetAllRFWIForms().Where(a => ((a.OtherTradeClearance_Other == true && a.OtherTradeClearance_OtherSignature == null && a.Status == "Pending") || a.OtherTradeClearance_OtherBy == UserID) && a.qcinspection_rfwi_form_location_detail.Count() > 0).ToList();
                Projectids = projectMasterViewModels.Where(a => a.CompanyID == CompanyID && a.qcinspection_project_OtherInspector_detail.Where(w => w.UserID == UserID).Count() > 0).Select(a => a.ProjectID).ToList();
            }
            else
            {
                qCInspectionRFWIFormViewModels = qcInspectionService.GetAllRFWIForms().ToList();
                Projectids = projectMasterViewModels.Where(a => a.CompanyID == CompanyID).Select(a => a.ProjectID).ToList();
            }

            if(ProjectID > 0)
            {
                qCInspectionRFWIFormViewModels = qCInspectionRFWIFormViewModels.Where(x => x.ProjectID == ProjectID).ToList();
            }
            else
            {
                qCInspectionRFWIFormViewModels = qCInspectionRFWIFormViewModels.Where(x => Projectids.Contains(x.ProjectID)).ToList();
            }

            int iCnt = 1;
            List<QCInspectionLocationMobileViewModel> QCInspectionLocationMobileViewModels = new List<QCInspectionLocationMobileViewModel>();
            foreach (var block in qcInspectionService.GetAllBlocks().OrderBy(x => x.OrderBy).ToList())
            {
                foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                {
                    foreach (var unit in qcInspectionService.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                    {
                        QCInspectionLocationMobileViewModels.Add(new QCInspectionLocationMobileViewModel()
                        {
                            UnitID = unit.UnitID,
                            UnitName = block.BlockName + " " + level.LevelName + "-" + unit.UnitName,
                            OrderBy = iCnt
                        });
                        iCnt++;
                    }
                }
            }
            
            List<QCInspectionRFWIFormMobileViewModel> QCInspectionRFWIFormMobileViewModels = new List<QCInspectionRFWIFormMobileViewModel>();
            foreach (var RFWIForm in qCInspectionRFWIFormViewModels)
            {
                QCInspectionRFWIFormMobileViewModel qCInspectionRFWIFormMobileViewModel = new QCInspectionRFWIFormMobileViewModel();
                qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormID = RFWIForm.QCInspectionRFWIFormID;
                qCInspectionRFWIFormMobileViewModel.QCInspectionRFWINo = RFWIForm.QCInspectionRFWINo;
                qCInspectionRFWIFormMobileViewModel.ProjectID = RFWIForm.ProjectID;
                qCInspectionRFWIFormMobileViewModel.ProjectName = projectMasterViewModels.Where(x => x.ProjectID == RFWIForm.ProjectID).FirstOrDefault().Project_Name;
                qCInspectionRFWIFormMobileViewModel.TradeID = RFWIForm.TradeID;
                qCInspectionRFWIFormMobileViewModel.TradeName = QCInspectionRFWITradeMasterViewModels.Where(x=> x.TradeID == RFWIForm.TradeID).FirstOrDefault().TradeName;
                qCInspectionRFWIFormMobileViewModel.RequestBy = RFWIForm.RequestBy;
                if (RFWIForm.RequestOn != null)
                {
                    qCInspectionRFWIFormMobileViewModel.RequestByName = userViewModels.Where(x => x.UserID == RFWIForm.RequestBy).FirstOrDefault()?.DisplayName;
                    qCInspectionRFWIFormMobileViewModel.RequestOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.RequestOn);
                }
                qCInspectionRFWIFormMobileViewModel.RequestSignature = RFWIForm.RequestSignature;
                qCInspectionRFWIFormMobileViewModel.NotiicationReceivedByName = RFWIForm.NotiicationReceivedByName;
                qCInspectionRFWIFormMobileViewModel.NotiicationReceivedSignature = RFWIForm.NotiicationReceivedSignature;
                if (RFWIForm.NotiicationReceivedOn != null)
                {
                    qCInspectionRFWIFormMobileViewModel.NotiicationReceivedOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.NotiicationReceivedOn);
                }
                qCInspectionRFWIFormMobileViewModel.InspectionNo = RFWIForm.InspectionNo;

                if (RFWIForm.InspectionOn != null)
                {
                    qCInspectionRFWIFormMobileViewModel.InspectionOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.InspectionOn);
                }
                if (RFWIForm.InspectionStartOn != null)
                {
                    qCInspectionRFWIFormMobileViewModel.InspectionStartOn = DateTime.Today.Add(RFWIForm.InspectionStartOn.Value).ToString("hh:mm tt").ToUpper();
                }
                if (RFWIForm.InspectionEndOn != null)
                {
                    qCInspectionRFWIFormMobileViewModel.InspectionEndOn = DateTime.Today.Add(RFWIForm.InspectionEndOn.Value).ToString("hh:mm tt").ToUpper();
                }
                qCInspectionRFWIFormMobileViewModel.InspectorID = RFWIForm.InspectorID;
                qCInspectionRFWIFormMobileViewModel.InspectorName = userViewModels.Where(x => x.UserID == RFWIForm.InspectorID).FirstOrDefault().DisplayName;
                qCInspectionRFWIFormMobileViewModel.RequestFor = RFWIForm.RequestFor;

                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_Structure = RFWIForm.OtherTradeClearance_Structure;
                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureBy = RFWIForm.OtherTradeClearance_StructureBy;
                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureSignature = RFWIForm.OtherTradeClearance_StructureSignature;
                if (RFWIForm.OtherTradeClearance_StructureOn != null)
                {
                    if (qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureBy != null)
                    {
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureByName = userViewModels.Where(x => x.UserID == RFWIForm.OtherTradeClearance_StructureBy).FirstOrDefault().DisplayName;
                    }
                    qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.OtherTradeClearance_StructureOn);
                }
                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandE = RFWIForm.OtherTradeClearance_MandE;
                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandEBy = RFWIForm.OtherTradeClearance_MandEBy;
                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandESignature = RFWIForm.OtherTradeClearance_MandESignature;
                if (RFWIForm.OtherTradeClearance_MandEOn != null)
                {
                    if (qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandEBy != null)
                    {
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandEByName = userViewModels.Where(x => x.UserID == RFWIForm.OtherTradeClearance_MandEBy).FirstOrDefault().DisplayName;
                    }
                    qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandEOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.OtherTradeClearance_MandEOn);
                }

                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_Other = RFWIForm.OtherTradeClearance_Other;
                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherBy = RFWIForm.OtherTradeClearance_OtherBy;
                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherSignature = RFWIForm.OtherTradeClearance_OtherSignature;
                if (RFWIForm.OtherTradeClearance_OtherOn != null)
                {
                    if (qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherBy != null)
                    {
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherByName = userViewModels.Where(x => x.UserID == RFWIForm.OtherTradeClearance_OtherBy).FirstOrDefault().DisplayName;
                    }
                    qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.OtherTradeClearance_OtherOn);
                }

                qCInspectionRFWIFormMobileViewModel.CompletedBy = RFWIForm.CompletedBy;
                if (RFWIForm.CompletedDate != null)
                {
                    if(qCInspectionRFWIFormMobileViewModel.CompletedBy!=null)
                    {
                        qCInspectionRFWIFormMobileViewModel.CompletedByName = userViewModels.Where(x => x.UserID == RFWIForm.CompletedBy).FirstOrDefault().DisplayName;
                    }
                    qCInspectionRFWIFormMobileViewModel.CompletedDate = string.Format("{0:dd/MM/yyyy}", RFWIForm.CompletedDate);
                }
                qCInspectionRFWIFormMobileViewModel.CompletedRemarks = RFWIForm.CompletedRemarks;
                qCInspectionRFWIFormMobileViewModel.CompletedSignature = RFWIForm.CompletedSignature;
                qCInspectionRFWIFormMobileViewModel.ReInspectionFormID = RFWIForm.ReInspectionFormID;
                qCInspectionRFWIFormMobileViewModel.Status = RFWIForm.Status;
                qCInspectionRFWIFormMobileViewModel.MobileQCInspectionRFWIFormID = RFWIForm.MobileQCInspectionRFWIFormID;
                qCInspectionRFWIFormMobileViewModel.BatchID = RFWIForm.BatchID;
                qCInspectionRFWIFormMobileViewModel.MobileStatus  = 0;
                qCInspectionRFWIFormMobileViewModel.CreatedOrUpdatedByUserId = 0;
                qCInspectionRFWIFormMobileViewModel.CreatedOrUpdatedDate = string.Format("{0:dd/MM/yyyy}", RFWIForm.CreatedDate);
                qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels = new List<QCInspectionRFWIFormGeneralCheckListDetailMobileViewModel>();
                qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels = new List<QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModel>();
                qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormTradeItemDetailMobileViewModels = new List<QCInspectionRFWIFormTradeItemDetailMobileViewModel>();
                qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormLocationDetailMobileViewModels = new List<QCInspectionRFWIFormLocationDetailMobileViewModel>();

                qCInspectionRFWIFormMobileViewModel.ProceedRequest = false;
                if (!string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature) || !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature) || !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                {
                    qCInspectionRFWIFormMobileViewModel.OtherSigned = true;
                }
                else
                {
                    qCInspectionRFWIFormMobileViewModel.OtherSigned = false;
                }
                // SMO
                // 111
                if (RFWIForm.OtherTradeClearance_Structure == true && RFWIForm.OtherTradeClearance_MandE == true && RFWIForm.OtherTradeClearance_Other == true)
                {
                    if (RFWIForm.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature) && RFWIForm.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature) && RFWIForm.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                    {
                        RFWIForm.ProceedRequest = true;
                    }
                }
                // 000
                else if (RFWIForm.OtherTradeClearance_Structure == false && RFWIForm.OtherTradeClearance_MandE == false && RFWIForm.OtherTradeClearance_Other == false)
                {
                    qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                }
                // 101
                else if (RFWIForm.OtherTradeClearance_Structure == true && RFWIForm.OtherTradeClearance_MandE == false && RFWIForm.OtherTradeClearance_Other == true)
                {
                    if (RFWIForm.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature) && RFWIForm.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                    {
                        qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                    }
                }
                // 110
                else if (RFWIForm.OtherTradeClearance_Structure == true && RFWIForm.OtherTradeClearance_MandE == true && RFWIForm.OtherTradeClearance_Other == false)
                {
                    if (RFWIForm.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature) && RFWIForm.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature))
                    {
                        qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                    }
                }
                // 100
                else if (RFWIForm.OtherTradeClearance_Structure == true && RFWIForm.OtherTradeClearance_MandE == false && RFWIForm.OtherTradeClearance_Other == false)
                {
                    if (RFWIForm.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature))
                    {
                        qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                    }
                }
                // 001
                else if (RFWIForm.OtherTradeClearance_Structure == false && RFWIForm.OtherTradeClearance_MandE == false && RFWIForm.OtherTradeClearance_Other == true)
                {
                    if (RFWIForm.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                    {
                        qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                    }
                }
                // 010
                else if (RFWIForm.OtherTradeClearance_Structure == false && RFWIForm.OtherTradeClearance_MandE == true && RFWIForm.OtherTradeClearance_Other == false)
                {
                    if (RFWIForm.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature))
                    {
                        qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                    }
                }
                // 011
                else if (RFWIForm.OtherTradeClearance_Structure == false && RFWIForm.OtherTradeClearance_MandE == true && RFWIForm.OtherTradeClearance_Other == true)
                {
                    if (RFWIForm.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature) && RFWIForm.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                    {
                        qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                    }
                }

                iCnt = 1;
                foreach (var general_checklist in RFWIForm.qcinspection_rfwi_form_general_checklist_detail)
                {
                    qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels.Add(new QCInspectionRFWIFormGeneralCheckListDetailMobileViewModel()
                    {
                        GeneralCheckListID = general_checklist.GeneralCheckListID,
                        GeneralCheckListName = general_checklist.qcinspection_rfwi_general_checklist_master.GeneralCheckListName,
                        OrderBy = iCnt
                    });
                    iCnt++;
                }
                iCnt = 1;
                foreach (var trade_detailed_checklist in RFWIForm.qcinspection_rfwi_form_trade_detailed_checklist_detail)
                {
                    qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels.Add(new QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModel()
                    {
                        TradeDetailedCheckListID = trade_detailed_checklist.TradeDetailedCheckListID,
                        DetailedCheckListName = trade_detailed_checklist.qcinspection_rfwi_trade_detailed_checklist_detail.DetailedCheckListName,
                        OrderBy = iCnt
                    });
                    iCnt++;
                }
                iCnt = 1;
                foreach (var trade_item_detail in RFWIForm.qcinspection_rfwi_form_trade_item_detail)
                {
                    qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormTradeItemDetailMobileViewModels.Add(new QCInspectionRFWIFormTradeItemDetailMobileViewModel()
                    {
                        TradeItemID = trade_item_detail.TradeItemID,
                        ItemName = trade_item_detail.qcinspection_rfwi_trade_item_detail.ItemName,
                        OrderBy = iCnt
                    });
                    iCnt++;
                }

                foreach (var location_detail in RFWIForm.qcinspection_rfwi_form_location_detail)
                {
                    qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormLocationDetailMobileViewModels.Add(new QCInspectionRFWIFormLocationDetailMobileViewModel()
                    {
                        UnitID = location_detail.UnitID,
                        UnitName = QCInspectionLocationMobileViewModels.Where(x=> x.UnitID == location_detail.UnitID).FirstOrDefault().UnitName,
                        QCInspectionDrawingReferenceFileID = location_detail.QCInspectionDrawingReferenceFileID,
                        FileCaption = location_detail.qcinspection_project_rfwi_drawing_reference_files.FileCaption,
                        FileName = location_detail.qcinspection_project_rfwi_drawing_reference_files.FileName
                        //FileBase64 = AppSettings.ConvertFileToBase64String(location_detail.qcinspection_project_rfwi_drawing_reference_files.FilePath, location_detail.qcinspection_project_rfwi_drawing_reference_files.FileName)
                    });
                }
                QCInspectionRFWIFormMobileViewModels.Add(qCInspectionRFWIFormMobileViewModel);
            }
            return QCInspectionRFWIFormMobileViewModels;
        }
        
        [HttpPost]
        public QCInspectionRFWIFormResponseViewModel SaveRFWIForm(List<QCInspectionRFWIFormMobileViewModel> QCInspectionRFWIFormMobileViewModels)
        {
            QCInspectionRFWIFormResponseViewModel qCInspectionRFWIFormResponseViewModel = new QCInspectionRFWIFormResponseViewModel();
            qCInspectionRFWIFormResponseViewModel.QCInspectionRFWIFormMobileViewModels = new List<QCInspectionRFWIFormMobileViewModel>();
            int ProjectID = Convert.ToInt32(QCInspectionRFWIFormMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            int iCnt = 1;
            try
            {
                QCInspectionRFWIFormViewModel RFWIFormModel;
                foreach (var RFWIForm in QCInspectionRFWIFormMobileViewModels)
                {
                    if (RFWIForm.MobileStatus == 1)
                    {
                        RFWIFormModel = new QCInspectionRFWIFormViewModel();
                        RFWIFormModel.QCInspectionRFWINo = RFWIForm.QCInspectionRFWINo;
                        RFWIFormModel.InspectionNo = RFWIForm.InspectionNo;
                        RFWIFormModel.ProjectID = RFWIForm.ProjectID;
                        RFWIFormModel.TradeID = RFWIForm.TradeID;
                        RFWIFormModel.RequestBy = RFWIForm.RequestBy;
                        if (!string.IsNullOrEmpty(RFWIForm.RequestOn))
                        {
                           RFWIFormModel.RequestOn = DateTime.ParseExact(RFWIForm.RequestOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.RequestSignature = RFWIForm.RequestSignature;
                        RFWIFormModel.NotiicationReceivedByName = RFWIForm.NotiicationReceivedByName;
                        RFWIFormModel.NotiicationReceivedSignature = RFWIForm.NotiicationReceivedSignature;
                        if (!string.IsNullOrEmpty(RFWIForm.NotiicationReceivedOn))
                        {
                            RFWIFormModel.NotiicationReceivedOn = DateTime.ParseExact(RFWIForm.NotiicationReceivedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        if (!string.IsNullOrEmpty(RFWIForm.InspectionOn))
                        {
                            RFWIFormModel.InspectionOn = DateTime.ParseExact(RFWIForm.InspectionOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        if (string.IsNullOrEmpty(RFWIForm.InspectionStartOn))
                        {
                            RFWIFormModel.InspectionStartOn = null;
                        }
                        else
                        {
                            RFWIFormModel.InspectionStartOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionStartOn));
                        }
                        if (string.IsNullOrEmpty(RFWIForm.InspectionEndOn))
                        {
                            RFWIFormModel.InspectionEndOn = null;
                        }
                        else
                        {
                            RFWIFormModel.InspectionEndOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionEndOn));
                        }
                        RFWIFormModel.InspectorID = RFWIForm.InspectorID;
                        RFWIFormModel.RequestFor = RFWIForm.RequestFor;
                        RFWIFormModel.ItemOthers = RFWIForm.ItemOthers;
                        RFWIFormModel.DetailedCheckListOthers = RFWIForm.DetailedCheckListOthers;

                        RFWIFormModel.OtherTradeClearance_Structure = RFWIForm.OtherTradeClearance_Structure;
                        RFWIFormModel.OtherTradeClearance_StructureBy = RFWIForm.OtherTradeClearance_StructureBy;
                        if (!string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureOn))
                        {
                            RFWIFormModel.OtherTradeClearance_StructureOn = DateTime.ParseExact(RFWIForm.OtherTradeClearance_StructureOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.OtherTradeClearance_StructureSignature = RFWIForm.OtherTradeClearance_StructureSignature;

                        RFWIFormModel.OtherTradeClearance_MandE = RFWIForm.OtherTradeClearance_MandE;
                        RFWIFormModel.OtherTradeClearance_MandEBy = RFWIForm.OtherTradeClearance_MandEBy;
                        if (!string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandEOn))
                        {
                            RFWIFormModel.OtherTradeClearance_MandEOn = DateTime.ParseExact(RFWIForm.OtherTradeClearance_MandEOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.OtherTradeClearance_MandESignature = RFWIForm.OtherTradeClearance_MandESignature;

                        RFWIFormModel.OtherTradeClearance_Other = RFWIForm.OtherTradeClearance_Other;
                        RFWIFormModel.OtherTradeClearance_OtherBy = RFWIForm.OtherTradeClearance_OtherBy;
                        if (!string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherOn))
                        {
                            RFWIFormModel.OtherTradeClearance_OtherOn = DateTime.ParseExact(RFWIForm.OtherTradeClearance_OtherOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.OtherTradeClearance_OtherSignature = RFWIForm.OtherTradeClearance_OtherSignature;

                        RFWIFormModel.CompletedBy = RFWIForm.CompletedBy;
                        if (!string.IsNullOrEmpty(RFWIForm.CompletedDate))
                        {
                            RFWIFormModel.CompletedDate = DateTime.ParseExact(RFWIForm.CompletedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.CompletedRemarks = RFWIForm.CompletedRemarks;
                        RFWIFormModel.CompletedSignature = RFWIForm.CompletedSignature;
                        RFWIFormModel.ReInspectionFormID = RFWIForm.ReInspectionFormID;
                        RFWIFormModel.Status = RFWIForm.Status;
                        RFWIFormModel.MobileQCInspectionRFWIFormID = RFWIForm.MobileQCInspectionRFWIFormID;
                        RFWIFormModel.BatchID = BatchID;
                        RFWIFormModel.CreatedBy = RFWIForm.CreatedOrUpdatedByUserId;
                        RFWIFormModel.CreatedDate = DateTime.ParseExact(RFWIForm.CreatedOrUpdatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (RFWIForm.QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels?.Count > 0)
                        {
                            RFWIFormModel.SelectedGeneralCheckListIds = "";
                            foreach (var item in RFWIForm.QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels)
                            {
                                RFWIFormModel.SelectedGeneralCheckListIds += item.GeneralCheckListID.ToString() + ",";
                            }
                            RFWIFormModel.SelectedGeneralCheckListIds = RFWIFormModel.SelectedGeneralCheckListIds.Substring(0, RFWIFormModel.SelectedGeneralCheckListIds.Length - 1);
                        }
                        if (RFWIForm.QCInspectionRFWIFormTradeItemDetailMobileViewModels?.Count > 0)
                        {
                            RFWIFormModel.SelectedTradeItemListIds = "";
                            foreach (var item in RFWIForm.QCInspectionRFWIFormTradeItemDetailMobileViewModels)
                            {
                                RFWIFormModel.SelectedTradeItemListIds += item.TradeItemID.ToString() + ",";
                            }
                            RFWIFormModel.SelectedTradeItemListIds = RFWIFormModel.SelectedTradeItemListIds.Substring(0, RFWIFormModel.SelectedTradeItemListIds.Length - 1);
                        }
                        if (RFWIForm.QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels?.Count > 0)
                        {
                            RFWIFormModel.SelectedTradeDetailedCheckListIds = "";
                            foreach (var item in RFWIForm.QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels)
                            {
                                RFWIFormModel.SelectedTradeDetailedCheckListIds += item.TradeDetailedCheckListID.ToString() + ",";
                            }
                            RFWIFormModel.SelectedTradeDetailedCheckListIds = RFWIFormModel.SelectedTradeDetailedCheckListIds.Substring(0, RFWIFormModel.SelectedTradeDetailedCheckListIds.Length - 1);
                        }
                        if (RFWIForm.QCInspectionRFWIFormLocationDetailMobileViewModels?.Count > 0)
                        {
                            RFWIFormModel.qcinspection_rfwi_form_location_detail = new List<QCInspectionRFWIFormLocationDetailViewModel>();
                            foreach (var item in RFWIForm.QCInspectionRFWIFormLocationDetailMobileViewModels)
                            {
                                RFWIFormModel.qcinspection_rfwi_form_location_detail.Add(new QCInspectionRFWIFormLocationDetailViewModel()
                                {
                                    UnitID = item.UnitID,
                                    QCInspectionDrawingReferenceFileID = item.QCInspectionDrawingReferenceFileID,
                                    CreatedBy = RFWIForm.CreatedOrUpdatedByUserId,
                                    CreatedDate = DateTime.Now
                                });
                            }
                        }
                        var result = qcInspectionService.CreateRFWIForm(RFWIFormModel);
                    }
                }

                List<QCInspectionRFWIFormViewModel> qCInspectionRFWIFormViewModels = new List<QCInspectionRFWIFormViewModel>();
                qCInspectionRFWIFormViewModels = qcInspectionService.GetAllRFWIFormsBasedProjectID(ProjectID, BatchID).ToList();
                if(qCInspectionRFWIFormViewModels?.Count > 0)
                {
                    List<QCInspectionLocationMobileViewModel> QCInspectionLocationMobileViewModels = new List<QCInspectionLocationMobileViewModel>();
                    foreach (var block in qcInspectionService.GetAllBlocks().Where(x => x.ProjectID == ProjectID).OrderBy(x => x.OrderBy).ToList())
                    {
                        foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                        {
                            foreach (var unit in qcInspectionService.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                            {
                                QCInspectionLocationMobileViewModels.Add(new QCInspectionLocationMobileViewModel()
                                {
                                    UnitID = unit.UnitID,
                                    UnitName = block.BlockName + " " + level.LevelName + "-" + unit.UnitName,
                                    OrderBy = iCnt
                                });
                                iCnt++;
                            }
                        }
                    }
                    QCInspectionProjectMasterViewModel projectMasterViewModel = qcInspectionService.GetProject(ProjectID);
                    List<UserViewModel> userViewModels = userService.getAllUsers();
                    List<QCInspectionRFWITradeMasterViewModel> QCInspectionRFWITradeMasterViewModels = qcInspectionService.GetAllRFWITrades();
                    List<QCInspectionRFWITradeItemDetailViewModel> qCInspectionRFWITradeItemDetailViewModels = qcInspectionService.GetAllRFWITradeItems();
                    List<QCInspectionRFWITradeDetailedCheckListDetailViewModel> qCInspectionRFWITradeDetailedCheckListDetailViewModels = qcInspectionService.GetAllRFWITradeDetailedCheckLists();
                    List<QCInspectionRFWIGeneralCheckListMasterViewModel> qCInspectionRFWIGeneralCheckListMasterViewModels = qcInspectionService.GetAllRFWIGeneralCheckLists();
                    List<QCInspectionProjectRFWIDrawingReferenceFilesViewModel> qCInspectionProjectRFWIDrawingReferenceFilesViewModels = qcInspectionService.GetAllRFWIDrawingsReferenceFiles(ProjectID);

                    foreach (var RFWIForm in qCInspectionRFWIFormViewModels)
                    {
                        QCInspectionRFWIFormMobileViewModel qCInspectionRFWIFormMobileViewModel = new QCInspectionRFWIFormMobileViewModel();
                        qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormID = RFWIForm.QCInspectionRFWIFormID;
                        qCInspectionRFWIFormMobileViewModel.QCInspectionRFWINo = RFWIForm.QCInspectionRFWINo;
                        qCInspectionRFWIFormMobileViewModel.ProjectID = RFWIForm.ProjectID;
                        qCInspectionRFWIFormMobileViewModel.ProjectName = projectMasterViewModel.Project_Name;
                        qCInspectionRFWIFormMobileViewModel.TradeID = RFWIForm.TradeID;
                        qCInspectionRFWIFormMobileViewModel.TradeName = QCInspectionRFWITradeMasterViewModels.Where(x => x.TradeID == RFWIForm.TradeID).FirstOrDefault().TradeName;
                        qCInspectionRFWIFormMobileViewModel.RequestBy = RFWIForm.RequestBy;
                        if (RFWIForm.RequestOn != null)
                        {
                            qCInspectionRFWIFormMobileViewModel.RequestByName = userViewModels.Where(x => x.UserID == RFWIForm.RequestBy).FirstOrDefault().DisplayName;
                            qCInspectionRFWIFormMobileViewModel.RequestOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.RequestOn);
                        }
                        qCInspectionRFWIFormMobileViewModel.RequestSignature = RFWIForm.RequestSignature;
                        qCInspectionRFWIFormMobileViewModel.NotiicationReceivedByName = RFWIForm.NotiicationReceivedByName;
                        qCInspectionRFWIFormMobileViewModel.NotiicationReceivedSignature = RFWIForm.NotiicationReceivedSignature;
                        if (RFWIForm.NotiicationReceivedOn != null)
                        {
                            qCInspectionRFWIFormMobileViewModel.NotiicationReceivedOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.NotiicationReceivedOn);
                        }
                        qCInspectionRFWIFormMobileViewModel.InspectionNo = RFWIForm.InspectionNo;
                        if (RFWIForm.InspectionOn != null)
                        {
                            qCInspectionRFWIFormMobileViewModel.InspectionOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.InspectionOn);
                        }
                        if (RFWIForm.InspectionStartOn != null)
                        {
                            qCInspectionRFWIFormMobileViewModel.InspectionStartOn = DateTime.Today.Add(RFWIForm.InspectionStartOn.Value).ToString("hh:mm tt").ToUpper();
                        }
                        if (RFWIForm.InspectionEndOn != null)
                        {
                            qCInspectionRFWIFormMobileViewModel.InspectionEndOn = DateTime.Today.Add(RFWIForm.InspectionEndOn.Value).ToString("hh:mm tt").ToUpper();
                        }
                        qCInspectionRFWIFormMobileViewModel.InspectorID = RFWIForm.InspectorID;
                        qCInspectionRFWIFormMobileViewModel.InspectorName = userViewModels.Where(x => x.UserID == RFWIForm.InspectorID).FirstOrDefault().DisplayName;
                        qCInspectionRFWIFormMobileViewModel.RequestFor = RFWIForm.RequestFor;

                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_Structure = RFWIForm.OtherTradeClearance_Structure;
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureBy = RFWIForm.OtherTradeClearance_StructureBy;
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureSignature = RFWIForm.OtherTradeClearance_StructureSignature;
                        if (RFWIForm.OtherTradeClearance_StructureOn != null)
                        {
                            if (qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureBy != null)
                            {
                                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureByName = userViewModels.Where(x => x.UserID == RFWIForm.OtherTradeClearance_StructureBy).FirstOrDefault().DisplayName;
                            }
                            qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_StructureOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.OtherTradeClearance_StructureOn);
                        }
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandE = RFWIForm.OtherTradeClearance_MandE;
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandEBy = RFWIForm.OtherTradeClearance_MandEBy;
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandESignature = RFWIForm.OtherTradeClearance_MandESignature;
                        if (RFWIForm.OtherTradeClearance_MandEOn != null)
                        {
                            if (qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandEBy != null)
                            {
                                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandEByName = userViewModels.Where(x => x.UserID == RFWIForm.OtherTradeClearance_MandEBy).FirstOrDefault().DisplayName;
                            }
                            qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_MandEOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.OtherTradeClearance_MandEOn);
                        }

                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_Other = RFWIForm.OtherTradeClearance_Other;
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherBy = RFWIForm.OtherTradeClearance_OtherBy;
                        qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherSignature = RFWIForm.OtherTradeClearance_OtherSignature;
                        if (RFWIForm.OtherTradeClearance_OtherOn != null)
                        {
                            if (qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherBy != null)
                            {
                                qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherByName = userViewModels.Where(x => x.UserID == RFWIForm.OtherTradeClearance_OtherBy).FirstOrDefault().DisplayName;
                            }
                            qCInspectionRFWIFormMobileViewModel.OtherTradeClearance_OtherOn = string.Format("{0:dd/MM/yyyy}", RFWIForm.OtherTradeClearance_OtherOn);
                        }

                        qCInspectionRFWIFormMobileViewModel.CompletedBy = RFWIForm.CompletedBy;
                        if (RFWIForm.CompletedDate != null)
                        {
                            qCInspectionRFWIFormMobileViewModel.CompletedByName = userViewModels.Where(x => x.UserID == RFWIForm.CompletedBy).FirstOrDefault().DisplayName;
                            qCInspectionRFWIFormMobileViewModel.CompletedDate = string.Format("{0:dd/MM/yyyy}", RFWIForm.CompletedDate);
                        }
                        qCInspectionRFWIFormMobileViewModel.CompletedRemarks = RFWIForm.CompletedRemarks;
                        qCInspectionRFWIFormMobileViewModel.CompletedSignature = RFWIForm.CompletedSignature;
                        qCInspectionRFWIFormMobileViewModel.ReInspectionFormID = RFWIForm.ReInspectionFormID;
                        qCInspectionRFWIFormMobileViewModel.Status = RFWIForm.Status;
                        qCInspectionRFWIFormMobileViewModel.MobileQCInspectionRFWIFormID = RFWIForm.MobileQCInspectionRFWIFormID;
                        qCInspectionRFWIFormMobileViewModel.BatchID = RFWIForm.BatchID;
                        qCInspectionRFWIFormMobileViewModel.MobileStatus = 0;
                        qCInspectionRFWIFormMobileViewModel.CreatedOrUpdatedByUserId = 0;
                        qCInspectionRFWIFormMobileViewModel.CreatedOrUpdatedDate = string.Format("{0:dd/MM/yyyy}", RFWIForm.CreatedDate);
                        qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels = new List<QCInspectionRFWIFormGeneralCheckListDetailMobileViewModel>();
                        qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels = new List<QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModel>();
                        qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormTradeItemDetailMobileViewModels = new List<QCInspectionRFWIFormTradeItemDetailMobileViewModel>();
                        qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormLocationDetailMobileViewModels = new List<QCInspectionRFWIFormLocationDetailMobileViewModel>();

                        qCInspectionRFWIFormMobileViewModel.ProceedRequest = false;
                        if (!string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature) || !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature) || !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                        {
                            qCInspectionRFWIFormMobileViewModel.OtherSigned = true;
                        }
                        else
                        {
                            qCInspectionRFWIFormMobileViewModel.OtherSigned = false;
                        }
                        // SMO
                        // 111
                        if (RFWIForm.OtherTradeClearance_Structure == true && RFWIForm.OtherTradeClearance_MandE == true && RFWIForm.OtherTradeClearance_Other == true)
                        {
                            if (RFWIForm.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature) && RFWIForm.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature) && RFWIForm.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                            {
                                RFWIForm.ProceedRequest = true;
                            }
                        }
                        // 000
                        else if (RFWIForm.OtherTradeClearance_Structure == false && RFWIForm.OtherTradeClearance_MandE == false && RFWIForm.OtherTradeClearance_Other == false)
                        {
                            qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                        }
                        // 101
                        else if (RFWIForm.OtherTradeClearance_Structure == true && RFWIForm.OtherTradeClearance_MandE == false && RFWIForm.OtherTradeClearance_Other == true)
                        {
                            if (RFWIForm.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature) && RFWIForm.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                            {
                                qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                            }
                        }
                        // 110
                        else if (RFWIForm.OtherTradeClearance_Structure == true && RFWIForm.OtherTradeClearance_MandE == true && RFWIForm.OtherTradeClearance_Other == false)
                        {
                            if (RFWIForm.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature) && RFWIForm.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature))
                            {
                                qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                            }
                        }
                        // 100
                        else if (RFWIForm.OtherTradeClearance_Structure == true && RFWIForm.OtherTradeClearance_MandE == false && RFWIForm.OtherTradeClearance_Other == false)
                        {
                            if (RFWIForm.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureSignature))
                            {
                                qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                            }
                        }
                        // 001
                        else if (RFWIForm.OtherTradeClearance_Structure == false && RFWIForm.OtherTradeClearance_MandE == false && RFWIForm.OtherTradeClearance_Other == true)
                        {
                            if (RFWIForm.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                            {
                                qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                            }
                        }
                        // 010
                        else if (RFWIForm.OtherTradeClearance_Structure == false && RFWIForm.OtherTradeClearance_MandE == true && RFWIForm.OtherTradeClearance_Other == false)
                        {
                            if (RFWIForm.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature))
                            {
                                qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                            }
                        }
                        // 011
                        else if (RFWIForm.OtherTradeClearance_Structure == false && RFWIForm.OtherTradeClearance_MandE == true && RFWIForm.OtherTradeClearance_Other == true)
                        {
                            if (RFWIForm.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandESignature) && RFWIForm.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherSignature))
                            {
                                qCInspectionRFWIFormMobileViewModel.ProceedRequest = true;
                            }
                        }

                        iCnt = 1;
                        foreach (var general_checklist in RFWIForm.qcinspection_rfwi_form_general_checklist_detail)
                        {
                            qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels.Add(new QCInspectionRFWIFormGeneralCheckListDetailMobileViewModel()
                            {
                                GeneralCheckListID = general_checklist.GeneralCheckListID,
                                GeneralCheckListName = qCInspectionRFWIGeneralCheckListMasterViewModels.Where(x => x.GeneralCheckListID == general_checklist.GeneralCheckListID).FirstOrDefault().GeneralCheckListName,
                                OrderBy = iCnt
                            });
                            iCnt++;
                        }
                        iCnt = 1;
                        foreach (var trade_detailed_checklist in RFWIForm.qcinspection_rfwi_form_trade_detailed_checklist_detail)
                        {
                            qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels.Add(new QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModel()
                            {
                                TradeDetailedCheckListID = trade_detailed_checklist.TradeDetailedCheckListID,
                                DetailedCheckListName = qCInspectionRFWITradeDetailedCheckListDetailViewModels.Where(x => x.TradeDetailedCheckListID == trade_detailed_checklist.TradeDetailedCheckListID).FirstOrDefault().DetailedCheckListName,
                                OrderBy = iCnt
                            });
                            iCnt++;
                        }
                        iCnt = 1;
                        foreach (var trade_item_detail in RFWIForm.qcinspection_rfwi_form_trade_item_detail)
                        {
                            qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormTradeItemDetailMobileViewModels.Add(new QCInspectionRFWIFormTradeItemDetailMobileViewModel()
                            {
                                TradeItemID = trade_item_detail.TradeItemID,
                                ItemName = qCInspectionRFWITradeItemDetailViewModels.Where(x => x.TradeItemID == trade_item_detail.TradeItemID).FirstOrDefault().ItemName,
                                OrderBy = iCnt
                            });
                            iCnt++;
                        }

                        foreach (var location_detail in RFWIForm.qcinspection_rfwi_form_location_detail)
                        {
                            qCInspectionRFWIFormMobileViewModel.QCInspectionRFWIFormLocationDetailMobileViewModels.Add(new QCInspectionRFWIFormLocationDetailMobileViewModel()
                            {
                                UnitID = location_detail.UnitID,
                                UnitName = QCInspectionLocationMobileViewModels.Where(x => x.UnitID == location_detail.UnitID).FirstOrDefault().UnitName,
                                QCInspectionDrawingReferenceFileID = location_detail.QCInspectionDrawingReferenceFileID,
                                FileCaption = qCInspectionProjectRFWIDrawingReferenceFilesViewModels.Where(x=> x.QCInspectionDrawingReferenceFileID == location_detail.QCInspectionDrawingReferenceFileID).FirstOrDefault().FileCaption,
                                FileName = qCInspectionProjectRFWIDrawingReferenceFilesViewModels.Where(x => x.QCInspectionDrawingReferenceFileID == location_detail.QCInspectionDrawingReferenceFileID).FirstOrDefault().FileName
                                //FileBase64 = AppSettings.ConvertFileToBase64String(qCInspectionProjectRFWIDrawingReferenceFilesViewModels.Where(x => x.QCInspectionDrawingReferenceFileID == location_detail.QCInspectionDrawingReferenceFileID).FirstOrDefault().FilePath, qCInspectionProjectRFWIDrawingReferenceFilesViewModels.Where(x => x.QCInspectionDrawingReferenceFileID == location_detail.QCInspectionDrawingReferenceFileID).FirstOrDefault().FileName)
                            });
                        }
                        qCInspectionRFWIFormResponseViewModel.QCInspectionRFWIFormMobileViewModels.Add(qCInspectionRFWIFormMobileViewModel);
                    }
                }
                
                qCInspectionRFWIFormResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return qCInspectionRFWIFormResponseViewModel;
        }

        [HttpPost]
        public QCInspectionRFWIFormResponseViewModel UpdateRFWIForm(List<QCInspectionRFWIFormMobileViewModel> QCInspectionRFWIFormMobileViewModels)
        {
            QCInspectionRFWIFormResponseViewModel qCInspectionRFWIFormResponseViewModel = new QCInspectionRFWIFormResponseViewModel();
            qCInspectionRFWIFormResponseViewModel.QCInspectionRFWIFormMobileViewModels = new List<QCInspectionRFWIFormMobileViewModel>();
            int ProjectID = Convert.ToInt32(QCInspectionRFWIFormMobileViewModels.FirstOrDefault().ProjectID);
            string BatchID = DateTime.Now.ToString("ddMMyyyyHHmmss");
            try
            {
                QCInspectionRFWIFormViewModel RFWIFormModel;
                foreach (var RFWIForm in QCInspectionRFWIFormMobileViewModels)
                {
                    if (RFWIForm.MobileStatus == 2)
                    {
                        RFWIFormModel = new QCInspectionRFWIFormViewModel();
                        RFWIFormModel.QCInspectionRFWIFormID = RFWIForm.QCInspectionRFWIFormID;
                        RFWIFormModel.QCInspectionRFWINo = RFWIForm.QCInspectionRFWINo;
                        RFWIFormModel.InspectionNo = RFWIForm.InspectionNo;
                        RFWIFormModel.ProjectID = RFWIForm.ProjectID;
                        RFWIFormModel.TradeID = RFWIForm.TradeID;
                        RFWIFormModel.RequestBy = RFWIForm.RequestBy;
                        if (!string.IsNullOrEmpty(RFWIForm.RequestOn))
                        {

                            RFWIFormModel.RequestOn = DateTime.ParseExact(RFWIForm.RequestOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.RequestSignature = RFWIForm.RequestSignature;
                        RFWIFormModel.NotiicationReceivedByName = RFWIForm.NotiicationReceivedByName;
                        RFWIFormModel.NotiicationReceivedSignature = RFWIForm.NotiicationReceivedSignature;
                        if (!string.IsNullOrEmpty(RFWIForm.NotiicationReceivedOn))
                        {
                            RFWIFormModel.NotiicationReceivedOn = DateTime.ParseExact(RFWIForm.NotiicationReceivedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        if (!string.IsNullOrEmpty(RFWIForm.InspectionOn))
                        {
                            RFWIFormModel.InspectionOn = DateTime.ParseExact(RFWIForm.InspectionOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        if (string.IsNullOrEmpty(RFWIForm.InspectionStartOn))
                        {
                            RFWIFormModel.InspectionStartOn = null;
                        }
                        else
                        {
                            RFWIFormModel.InspectionStartOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionStartOn));
                        }
                        if (string.IsNullOrEmpty(RFWIForm.InspectionEndOn))
                        {
                            RFWIFormModel.InspectionEndOn = null;
                        }
                        else
                        {
                            RFWIFormModel.InspectionEndOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionEndOn));
                        }
                        RFWIFormModel.InspectorID = RFWIForm.InspectorID;
                        RFWIFormModel.RequestFor = RFWIForm.RequestFor;
                        RFWIFormModel.ItemOthers = RFWIForm.ItemOthers;
                        RFWIFormModel.DetailedCheckListOthers = RFWIForm.DetailedCheckListOthers;

                        RFWIFormModel.OtherTradeClearance_Structure = RFWIForm.OtherTradeClearance_Structure;
                        RFWIFormModel.OtherTradeClearance_StructureBy = RFWIForm.OtherTradeClearance_StructureBy;
                        if (!string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_StructureOn))
                        {
                            RFWIFormModel.OtherTradeClearance_StructureOn = DateTime.ParseExact(RFWIForm.OtherTradeClearance_StructureOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.OtherTradeClearance_StructureSignature = RFWIForm.OtherTradeClearance_StructureSignature;

                        RFWIFormModel.OtherTradeClearance_MandE = RFWIForm.OtherTradeClearance_MandE;
                        RFWIFormModel.OtherTradeClearance_MandEBy = RFWIForm.OtherTradeClearance_MandEBy;
                        if (!string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_MandEOn))
                        {
                            RFWIFormModel.OtherTradeClearance_MandEOn = DateTime.ParseExact(RFWIForm.OtherTradeClearance_MandEOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.OtherTradeClearance_MandESignature = RFWIForm.OtherTradeClearance_MandESignature;

                        RFWIFormModel.OtherTradeClearance_Other = RFWIForm.OtherTradeClearance_Other;
                        RFWIFormModel.OtherTradeClearance_OtherBy = RFWIForm.OtherTradeClearance_OtherBy;
                        if (!string.IsNullOrEmpty(RFWIForm.OtherTradeClearance_OtherOn))
                        {
                            RFWIFormModel.OtherTradeClearance_OtherOn = DateTime.ParseExact(RFWIForm.OtherTradeClearance_OtherOn, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.OtherTradeClearance_OtherSignature = RFWIForm.OtherTradeClearance_OtherSignature;


                        RFWIFormModel.CompletedBy = RFWIForm.CompletedBy;
                        if (!string.IsNullOrEmpty(RFWIForm.CompletedDate))
                        {
                            RFWIFormModel.CompletedDate = DateTime.ParseExact(RFWIForm.CompletedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        RFWIFormModel.CompletedRemarks = RFWIForm.CompletedRemarks;
                        RFWIFormModel.CompletedSignature = RFWIForm.CompletedSignature;
                        RFWIFormModel.ReInspectionFormID = RFWIForm.ReInspectionFormID;
                        RFWIFormModel.Status = RFWIForm.Status;
                        RFWIFormModel.MobileQCInspectionRFWIFormID = RFWIForm.MobileQCInspectionRFWIFormID;
                        RFWIFormModel.BatchID = BatchID;
                        RFWIFormModel.UpdatedBy = RFWIForm.CreatedOrUpdatedByUserId;
                        RFWIFormModel.UpdatedDate = DateTime.ParseExact(RFWIForm.CreatedOrUpdatedDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        if (RFWIForm.QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels?.Count > 0)
                        {
                            RFWIFormModel.SelectedGeneralCheckListIds = "";
                            foreach (var item in RFWIForm.QCInspectionRFWIFormGeneralCheckListDetailMobileViewModels)
                            {
                                RFWIFormModel.SelectedGeneralCheckListIds += item.GeneralCheckListID.ToString() + ",";
                            }
                            RFWIFormModel.SelectedGeneralCheckListIds = RFWIFormModel.SelectedGeneralCheckListIds.Substring(0, RFWIFormModel.SelectedGeneralCheckListIds.Length - 1);
                        }
                        if (RFWIForm.QCInspectionRFWIFormTradeItemDetailMobileViewModels?.Count > 0)
                        {
                            RFWIFormModel.SelectedTradeItemListIds = "";
                            foreach (var item in RFWIForm.QCInspectionRFWIFormTradeItemDetailMobileViewModels)
                            {
                                RFWIFormModel.SelectedTradeItemListIds += item.TradeItemID.ToString() + ",";
                            }
                            RFWIFormModel.SelectedTradeItemListIds = RFWIFormModel.SelectedTradeItemListIds.Substring(0, RFWIFormModel.SelectedTradeItemListIds.Length - 1);
                        }
                        if (RFWIForm.QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels?.Count > 0)
                        {
                            RFWIFormModel.SelectedTradeDetailedCheckListIds = "";
                            foreach (var item in RFWIForm.QCInspectionRFWIFormTradeDetailedCheckListDetailMobileViewModels)
                            {
                                RFWIFormModel.SelectedTradeDetailedCheckListIds += item.TradeDetailedCheckListID.ToString() + ",";
                            }
                            RFWIFormModel.SelectedTradeDetailedCheckListIds = RFWIFormModel.SelectedTradeDetailedCheckListIds.Substring(0, RFWIFormModel.SelectedTradeDetailedCheckListIds.Length - 1);
                        }
                        if (RFWIForm.QCInspectionRFWIFormLocationDetailMobileViewModels.Count > 0)
                        {
                            RFWIFormModel.qcinspection_rfwi_form_location_detail = new List<QCInspectionRFWIFormLocationDetailViewModel>();
                            foreach (var item in RFWIForm.QCInspectionRFWIFormLocationDetailMobileViewModels)
                            {
                                RFWIFormModel.qcinspection_rfwi_form_location_detail.Add(new QCInspectionRFWIFormLocationDetailViewModel()
                                {
                                    UnitID = item.UnitID,
                                    QCInspectionDrawingReferenceFileID = item.QCInspectionDrawingReferenceFileID,
                                    CreatedBy = RFWIForm.CreatedOrUpdatedByUserId,
                                    CreatedDate = DateTime.Now
                                });
                            }
                        }
                        var result = qcInspectionService.MobileSaveRFWIForm(RFWIFormModel);
                    }
                }
                qCInspectionRFWIFormResponseViewModel.Success = true;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return qCInspectionRFWIFormResponseViewModel;
        }

        [HttpPost]
        public QCInspectionRFWIFormResponseViewModel DeleteRFWIForm(List<QCInspectionRFWIFormMobileDeleteViewModel> QCInspectionRFWIFormMobileDeleteViewModels, string UserID)
        {
            QCInspectionRFWIFormResponseViewModel qCInspectionRFWIFormResponseViewModel = new QCInspectionRFWIFormResponseViewModel();
            qCInspectionRFWIFormResponseViewModel.QCInspectionRFWIFormMobileViewModels = new List<QCInspectionRFWIFormMobileViewModel>();
            try
            {
                string RFWIIds = "";
                foreach (var DefectForm in QCInspectionRFWIFormMobileDeleteViewModels)
                {
                    RFWIIds = RFWIIds + DefectForm.QCInspectionRFWIFormID.ToString() + ",";
                }
                var result = qcInspectionService.MobileDeleteRFWIForm(RFWIIds.Substring(0, RFWIIds.Length - 1), UserID);
                if (result > 0)
                {
                    qCInspectionRFWIFormResponseViewModel.Success = true;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return qCInspectionRFWIFormResponseViewModel;
        }
        #endregion
    }
}
