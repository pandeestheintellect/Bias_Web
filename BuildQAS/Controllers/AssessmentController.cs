using BuildInspect.Models.Domain;
using BuildInspect.Models.Security;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Utility;
using BuildInspect.Models.ViewModel;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Globalization;
using Rotativa;

namespace BuildInspect.Controllers
{
    [AccessDeniedAuthorize]
    public class AssessmentController : SuperBaseController
    {
        private BuildInspectEntities db = new BuildInspectEntities();
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IAssessmentServices assessmentService;
        private readonly IUserServices userService;

        public AssessmentController(IAssessmentServices _assessmentService, IUserServices _userService)
        {
            assessmentService = _assessmentService;
            userService = _userService;
        }

        // GET: Assessment
        public ActionResult Index()
        {
            AppSession.SetCurrentPage("/Assessment/Index");
            AppSession.SetCurrentMenu("Assessment", "", "Index");
            var gid = AppSession.GetCurrentUserId();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(assessmentService.GetAllProjects().ToList());
            }
            else
            {
                return View(assessmentService.GetAllProjects().Where(a => a.CompanyID == cid).ToList());
            }
        }

        public ActionResult AssessmentIndex()
        {
            AppSession.SetCurrentPage("/Assessment/AssessmentIndex");
            AppSession.SetCurrentMenu("Assessment", "Assessments", "Assessments");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            List<AssessmentProjectMasterViewModel> Projects = new List<AssessmentProjectMasterViewModel>();
            if (gid == 1)
            {
                Projects = assessmentService.GetAllProjects().ToList();
            }
            else if (gid == 7)
            {
                var user = userService.GetUser(uid);
                if (user != null)
                {
                    Projects = assessmentService.GetAllProjects().Where(a => a.assessment_project_assessors_detail.Where(w => w.AssessorsID == user.Assessor_ID).Count() > 0).ToList();
                }
            }
            else
            {
                Projects = assessmentService.GetAllProjects().Where(a => a.CompanyID == cid).ToList();
            }
            return View(Projects);
        }

        #region Project

        // GET: Project
        public ActionResult ProjectIndex()
        {
            AppSession.SetCurrentPage("/Assessment/ProjectIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Project");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(assessmentService.GetAllProjects().ToList());
            }
            else
            {
                return View(assessmentService.GetAllProjects().Where(a => a.CompanyID == cid).ToList());
            }
        }

        public ActionResult ProjectCreate()
        {
            AppSession.SetCurrentPage("/Assessment/ProjectCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Project");
            var cpyID = AppSession.GetCompanyId();
            var grp = assessmentService.GetAllDevelopmentTypes().ToList();
            //grp.Insert(0, new AssessmentDevelopmentTypeMasterViewModel() { DevelopmentTypeID = 0, DevelopmentTypeName = "-Select-" });
            ViewBag.DevelopmentTypeID = new SelectList(grp, "DevelopmentTypeID", "DevelopmentTypeName");
            ViewBag.AssessorsList = assessmentService.GetAllAssessors().Where(a => a.CompanyID == cpyID).ToList();

            AssessmentProjectMasterViewModel projectMasterViewModel = new AssessmentProjectMasterViewModel();
            projectMasterViewModel.CompanyID = cpyID;
            projectMasterViewModel.Is_Completed = 0;
            projectMasterViewModel.Is_ExternalWallApplicable = 1;
            projectMasterViewModel.Is_ExternalWorksApplicable = 1;
            projectMasterViewModel.Is_RoofApplicable = 1;
            projectMasterViewModel.Is_FieldWindowWTTApplicable = 0;
            projectMasterViewModel.Is_WetAreaWTTApplicable = 0;
            projectMasterViewModel.StartDate = string.Format("{0:dd/MM/yyyy}", DateTime.Now);
            projectMasterViewModel.EndDate = string.Format("{0:dd/MM/yyyy}", DateTime.Now);
            return View(projectMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectCreate(AssessmentProjectMasterViewModel Project)
        {
            if (assessmentService.CheckProject(Project.Project_Name) == false)
            {
                Project.Assessment_StartOn = DateTime.ParseExact(Project.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Project.Assessment_EndOn = DateTime.ParseExact(Project.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Project.Assessment_Dates = string.Format("{0:dd/MM/yyyy}", Project.Assessment_StartOn) + "-" + string.Format("{0:dd/MM/yyyy}", Project.Assessment_EndOn);
                Project.CompanyID = AppSession.GetCompanyId();
                Project.CreatedBy = AppSession.GetCurrentUserId();
                Project.Is_Completed = 0;
                Project.CreatedDate = DateTime.Now;

                int i = 1;
                List<AssessmentProjectAssessorsDetailViewModel> detailViewModels = new List<AssessmentProjectAssessorsDetailViewModel>();
                foreach (var Id in Project.Assessors.Split(','))
                {
                    detailViewModels.Add(new AssessmentProjectAssessorsDetailViewModel()
                    {
                        AssessorsID = int.Parse(Id.ToString()),
                        RowNo = i,
                        CreatedBy = AppSession.GetCurrentUserId(),
                        CreatedDate = DateTime.Now
                    });
                    i++;
                }

                var result = assessmentService.CreateProject(Project, detailViewModels);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            else
            {
                return getFailedOperation("Project Name Already exits!");
            }
        }

        public ActionResult ProjectEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/ProjectEdit/" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "Project");
            var cpy = assessmentService.GetProject(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            cpy.StartDate = string.Format("{0:dd/MM/yyyy}", cpy.Assessment_StartOn);
            cpy.EndDate = string.Format("{0:dd/MM/yyyy}", cpy.Assessment_EndOn);
            var grp = assessmentService.GetAllDevelopmentTypes().ToList();
            //grp.Insert(0, new AssessmentDevelopmentTypeMasterViewModel() { DevelopmentTypeID = 0, DevelopmentTypeName = "-Select-" });
            ViewBag.DevelopmentTypeID = new SelectList(grp, "DevelopmentTypeID", "DevelopmentTypeName", cpy.DevelopmentTypeID);
            ViewBag.AssessorsList = assessmentService.GetAllAssessors().Where(a => a.CompanyID == cpy.CompanyID).ToList();
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectEdit(AssessmentProjectMasterViewModel Project)
        {
            Project.Assessment_StartOn = DateTime.ParseExact(Project.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Project.Assessment_EndOn = DateTime.ParseExact(Project.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Project.Assessment_Dates = string.Format("{0:dd/MM/yyyy}", Project.Assessment_StartOn) + "-" + string.Format("{0:dd/MM/yyyy}", Project.Assessment_EndOn);
            Project.UpdatedBy = AppSession.GetCurrentUserId();
            Project.UpdatedDate = DateTime.Now;
            int i = 1;
            List<AssessmentProjectAssessorsDetailViewModel> detailViewModels = new List<AssessmentProjectAssessorsDetailViewModel>();
            foreach (var Id in Project.Assessors.Split(','))
            {
                detailViewModels.Add(new AssessmentProjectAssessorsDetailViewModel()
                {
                    AssessorsID = int.Parse(Id.ToString()),
                    RowNo = i,
                    CreatedBy = AppSession.GetCurrentUserId(),
                    CreatedDate = DateTime.Now
                });
                i++;
            }
            var result = assessmentService.SaveProject(Project, detailViewModels);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }


        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteProject")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProject(int id)
        {
            var result = assessmentService.DeleteProject(id);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public JsonResult JSGetTypeofDevelopmentInfo(string Id)
        {
            var result = assessmentService.GetAllDevelopmentTypes().Where(x => x.DevelopmentTypeID == int.Parse(Id));
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("CompletedProject")]
        [ValidateAntiForgeryToken]
        public ActionResult CompletedProject(AssessmentProjectMasterViewModel Project)
        {
            var result = assessmentService.CompletedProject(Project.ProjectID);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Assessor

        // GET: Assessor
        public ActionResult AssessorIndex()
        {
            AppSession.SetCurrentPage("/Assessment/AssessorIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Assessor");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(assessmentService.GetAllAssessors().ToList());
            }
            else
            {
                return View(assessmentService.GetAllAssessorsBasedCompanyID(cid).ToList());
            }
        }

        public ActionResult AssessorCreate()
        {
            AppSession.SetCurrentPage("/Assessment/AssessorCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Assessor");
            var cpyID = AppSession.GetCompanyId();
            AssessorsMasterViewModel AssessorMasterViewModel = new AssessorsMasterViewModel();
            AssessorMasterViewModel.CompanyID = cpyID;
            var users = userService.getAllUsers().Where(a => a.GroupID == 7 && a.Assessor_ID == null).ToList();
            //users.Insert(0, new UserViewModel() { UserID = 0, DisplayName = "-Select-" });
            ViewBag.Assessor_ID = new SelectList(users, "UserID", "DisplayName", AssessorMasterViewModel.Assessor_ID);
            return View(AssessorMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssessorCreate(AssessorsMasterViewModel Assessor)
        {
            if (ModelState.IsValid)
            {
                if (assessmentService.CheckAssessor(Assessor.AssessorsName) == false)
                {
                    Assessor.CompanyID = AppSession.GetCompanyId();
                    Assessor.CreatedBy = AppSession.GetCurrentUserId();
                    Assessor.CreatedDate = DateTime.Now;
                    Assessor.IsActive = 1;
                    var result = assessmentService.CreateAssessor(Assessor);
                    if (result > 0)
                    {
                        // Existing Assessor User
                        var user = userService.getAllUsers().Where(x => x.Assessor_ID == result).FirstOrDefault();
                        if (user != null)
                        {
                            user.Assessor_ID = null;
                            user.UpdatedBy = AppSession.GetCurrentUserId();
                            user.UpdatedDate = DateTime.Now;
                            result = userService.SaveUser(user);
                        }
                        // Existing Assessor User
                        if (Assessor.Assessor_ID > 0)
                        {
                            user = userService.GetUser(Assessor.Assessor_ID);
                            if (user != null)
                            {
                                user.Assessor_ID = result;
                                user.UpdatedBy = AppSession.GetCurrentUserId();
                                user.UpdatedDate = DateTime.Now;
                                result = userService.SaveUser(user);
                            }
                        }
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("Assessor Name Already exits!");
                }
            }
            return View(Assessor);
        }

        public ActionResult AssessorEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/AssessorEdit/" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "Assessor");
            var cpy = assessmentService.GetAssessor(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            // Existing Assessor User
            var user = userService.getAllUsers().Where(x => x.Assessor_ID == cpy.AssessorsID).FirstOrDefault();
            if (user != null)
            {
                cpy.Assessor_ID = user.UserID;
                cpy.User_Name = user.DisplayName;
            }
            // Existing Assessor User
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssessorEdit(AssessorsMasterViewModel Assessor)
        {
            if (ModelState.IsValid)
            {
                Assessor.UpdatedBy = AppSession.GetCurrentUserId();
                Assessor.UpdatedDate = DateTime.Now;
                var result = assessmentService.SaveAssessor(Assessor);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(Assessor);
        }


        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteAssessor")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAssessor(int id)
        {
            // Existing Assessor User
            var user = userService.getAllUsers().Where(x => x.Assessor_ID == id).FirstOrDefault();
            if (user != null)
            {
                user.Assessor_ID = null;
                user.UpdatedBy = AppSession.GetCurrentUserId();
                user.UpdatedDate = DateTime.Now;
                userService.SaveUser(user);

            }
            // Existing Assessor User

            var result = assessmentService.DeleteAssessor(id);
            if (result > 0)
            {
                
                return getSuccessfulOperation();
            }
            else
            {
                // Existing Assessor User
                user = userService.getAllUsers().Where(x => x.Assessor_ID == id).FirstOrDefault();
                if (user != null)
                {
                    user.Assessor_ID = id;
                    user.UpdatedBy = AppSession.GetCurrentUserId();
                    user.UpdatedDate = DateTime.Now;
                    result = userService.SaveUser(user);
                }
                // Existing Assessor User
                return getFailedOperation();
            }
        }

        #endregion

        #region Location

        // GET: Location
        public ActionResult LocationIndex()
        {
            AppSession.SetCurrentPage("/Assessment/LocationIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Location");
            return View(assessmentService.GetAllAssessmentLocations().ToList());
        }

        public ActionResult LocationCreate()
        {
            AppSession.SetCurrentPage("/Assessment/LocationCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Location");
            var grp = assessmentService.GetAllAssessmentTypes().ToList();
            ViewBag.AssessmentTypeID = new SelectList(grp, "AssessmentTypeID", "AssessmentTypeName");

            List<SelectListItem> AssessmentTypeLocationTypeList = new List<SelectListItem>();
            AssessmentTypeLocationTypeList.Add(new SelectListItem() { Value = "P", Text = "P" });
            AssessmentTypeLocationTypeList.Add(new SelectListItem() { Value = "S", Text = "S" });
            AssessmentTypeLocationTypeList.Add(new SelectListItem() { Value = "C", Text = "C" });
            ViewBag.AssessmentTypeLocationType = new SelectList(AssessmentTypeLocationTypeList, "Text", "Value");

            AssessmentTypeLocationMasterViewModel LocationMasterViewModel = new AssessmentTypeLocationMasterViewModel();
            return View(LocationMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocationCreate(AssessmentTypeLocationMasterViewModel Location)
        {
            if (ModelState.IsValid)
            {
                if (assessmentService.CheckLocation(Location.AssessmentTypeID, Location.AssessmentTypeLocationName) == false)
                {
                    Location.CreatedBy = AppSession.GetCurrentUserId();
                    Location.CreatedDate = DateTime.Now;
                    Location.IsActive = 1;
                    var result = assessmentService.CreateLocation(Location);
                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("Location Name Already exits!");
                }
            }
            return View(Location);
        }

        public ActionResult LocationEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/LocationEdit/" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "Location");
            var grp = assessmentService.GetAllAssessmentTypes().ToList();
            ViewBag.AssessmentTypeID = new SelectList(grp, "AssessmentTypeID", "AssessmentTypeName");

            List<SelectListItem> AssessmentTypeLocationTypeList = new List<SelectListItem>();
            AssessmentTypeLocationTypeList.Add(new SelectListItem() { Value = "P", Text = "P" });
            AssessmentTypeLocationTypeList.Add(new SelectListItem() { Value = "S", Text = "S" });
            AssessmentTypeLocationTypeList.Add(new SelectListItem() { Value = "C", Text = "C" });
            ViewBag.AssessmentTypeLocationType = new SelectList(AssessmentTypeLocationTypeList, "Text", "Value");

            var cpy = assessmentService.GetLocation(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocationEdit(AssessmentTypeLocationMasterViewModel Location)
        {
            if (ModelState.IsValid)
            {
                Location.UpdatedBy = AppSession.GetCurrentUserId();
                Location.UpdatedDate = DateTime.Now;
                var result = assessmentService.SaveLocation(Location);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(Location);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteLocation")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLocation(int id)
        {
            var result = assessmentService.DeleteLocation(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Module

        // GET: Module
        public ActionResult ModuleIndex()
        {
            AppSession.SetCurrentPage("/Assessment/ModuleIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Module");
            return View(assessmentService.GetAllModules().ToList());
        }

        public ActionResult ModuleCreate()
        {
            AppSession.SetCurrentPage("/Assessment/ModuleCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Module");
            ViewBag.AssessmentTypeList = assessmentService.GetAllAssessmentTypes().ToList();
            AssessmentTypeModuleMasterViewModel ModuleMasterViewModel = new AssessmentTypeModuleMasterViewModel();
            return View(ModuleMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModuleCreate(AssessmentTypeModuleMasterViewModel Module)
        {
            if (ModelState.IsValid)
            {
                if (assessmentService.CheckModule(Module.AssessmentTypeID, Module.AssessmentTypeModuleName) == false)
                {
                    Module.CreatedBy = AppSession.GetCurrentUserId();
                    Module.CreatedDate = DateTime.Now;
                    Module.IsActive = 1;
                    var result = assessmentService.CreateModule(Module);
                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("Module Name Already exits!");
                }
            }
            return View(Module);
        }

        public ActionResult ModuleEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/ModuleEdit/" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "Module");
            ViewBag.AssessmentTypeList = assessmentService.GetAllAssessmentTypes().ToList();

            var cpy = assessmentService.GetModule(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModuleEdit(AssessmentTypeModuleMasterViewModel Module)
        {
            if (ModelState.IsValid)
            {
                Module.UpdatedBy = AppSession.GetCurrentUserId();
                Module.UpdatedDate = DateTime.Now;
                var result = assessmentService.SaveModule(Module);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(Module);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteModule")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteModule(int id)
        {
            var result = assessmentService.DeleteModule(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Module Process

        // GET: ModuleProcess
        public ActionResult ModuleProcessIndex()
        {
            AppSession.SetCurrentPage("/Assessment/ModuleProcessIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "ModuleProcess");
            return View(assessmentService.GetAllModuleProcess().ToList());
        }

        public ActionResult ModuleProcessCreate()
        {
            AppSession.SetCurrentPage("/Assessment/ModuleProcessCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "ModuleProcess");
            ViewBag.AssessmentTypeModuleList = assessmentService.GetAllModules().ToList().Select(o => new { AssessmentTypeModuleID = o.AssessmentTypeModuleID, AssessmentTypeModuleName = (o.AssessmentTypeID == 1 ? o.assessment_type_master.AssessmentTypeName + " / " + o.AssessmentTypeModuleName : o.AssessmentTypeModuleName) });
            int Id = assessmentService.GetAllModules().FirstOrDefault().AssessmentTypeModuleID;

            var AssessmentTypeLocationList = assessmentService.GetAllAssessmentLocations().Where(o => o.AssessmentTypeID == Id).ToList();
            //AssessmentTypeLocationList.Insert(0, new AssessmentTypeLocationMasterViewModel() { AssessmentTypeLocationID = 0, AssessmentTypeLocationName = "N/A" });
            ViewBag.AssessmentTypeLocationList = AssessmentTypeLocationList;

            AssessmentTypeModuleProcessMasterViewModel ModuleProcessMasterViewModel = new AssessmentTypeModuleProcessMasterViewModel();
            return View(ModuleProcessMasterViewModel);
        }

        public ActionResult PartialModuleProcessLocation(int ID)
        {
            var aid = assessmentService.GetModule(ID);
            if (aid != null)
            {
                var AssessmentTypeLocationList = assessmentService.GetAllAssessmentLocations().Where(o => o.AssessmentTypeID == aid.AssessmentTypeID).ToList();
                //AssessmentTypeLocationList.Insert(0, new AssessmentTypeLocationMasterViewModel() { AssessmentTypeLocationID = 0, AssessmentTypeLocationName = "N/A" });
                ViewBag.AssessmentTypeLocationList = AssessmentTypeLocationList;
            }
            AssessmentTypeModuleProcessMasterViewModel ModuleProcessMasterViewModel = new AssessmentTypeModuleProcessMasterViewModel();
            return PartialView(ModuleProcessMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModuleProcessCreate(AssessmentTypeModuleProcessMasterViewModel ModuleProcess)
        {
            if (ModelState.IsValid)
            {
                if (assessmentService.CheckModuleProcess(ModuleProcess.AssessmentTypeModuleID, ModuleProcess.AssessmentTypeModuleProcessName) == false)
                {
                    if (ModuleProcess?.AssessmentTypeModuleID != 9)
                    {
                        ModuleProcess.AssessmentTypeLocationID = null;
                    }
                    ModuleProcess.CreatedBy = AppSession.GetCurrentUserId();
                    ModuleProcess.CreatedDate = DateTime.Now;
                    ModuleProcess.IsActive = 1;
                    var result = assessmentService.CreateModuleProcess(ModuleProcess);
                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("Process Name Already exits!");
                }
            }
            return View(ModuleProcess);
        }

        public ActionResult ModuleProcessEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/ModuleProcessEdit" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "ModuleProcess");
            ViewBag.AssessmentTypeModuleList = assessmentService.GetAllModules().ToList().Select(o => new { AssessmentTypeModuleID = o.AssessmentTypeModuleID, AssessmentTypeModuleName = (o.AssessmentTypeID == 1 ? o.assessment_type_master.AssessmentTypeName + " / " + o.AssessmentTypeModuleName : o.AssessmentTypeModuleName) });
            int Id = assessmentService.GetAllModules().FirstOrDefault().AssessmentTypeModuleID;

            var AssessmentTypeLocationList = assessmentService.GetAllAssessmentLocations().Where(o => o.AssessmentTypeID == Id).ToList();
            //AssessmentTypeLocationList.Insert(0, new AssessmentTypeLocationMasterViewModel() { AssessmentTypeLocationID = 0, AssessmentTypeLocationName = "N/A" });
            ViewBag.AssessmentTypeLocationList = AssessmentTypeLocationList;

            var cpy = assessmentService.GetModuleProcess(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            //if (cpy?.AssessmentTypeLocationID == 0)
            //{

            //}
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModuleProcessEdit(AssessmentTypeModuleProcessMasterViewModel ModuleProcess)
        {
            if (ModelState.IsValid)
            {
                if (ModuleProcess?.AssessmentTypeModuleID != 9)
                {
                    ModuleProcess.AssessmentTypeLocationID = null;
                }
                ModuleProcess.UpdatedBy = AppSession.GetCurrentUserId();
                ModuleProcess.UpdatedDate = DateTime.Now;
                var result = assessmentService.SaveModuleProcess(ModuleProcess);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(ModuleProcess);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteModuleProcess")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteModuleProcess(int id)
        {
            var result = assessmentService.DeleteModuleProcess(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Joint

        // GET: Joint
        public ActionResult JointIndex()
        {
            AppSession.SetCurrentPage("/Assessment/JointIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Joint");
            return View(assessmentService.GetAllJoints().ToList());
        }

        public ActionResult JointCreate()
        {
            AppSession.SetCurrentPage("/Assessment/JointCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Joint");
            AssessmentJointMasterViewModel JointMasterViewModel = new AssessmentJointMasterViewModel();
            return View(JointMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JointCreate(AssessmentJointMasterViewModel Joint)
        {
            if (ModelState.IsValid)
            {
                if (assessmentService.CheckJoint(Joint.AssessmentJointName) == false)
                {
                    Joint.CreatedBy = AppSession.GetCurrentUserId();
                    Joint.CreatedDate = DateTime.Now;
                    Joint.IsActive = 1;
                    var result = assessmentService.CreateJoint(Joint);
                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("Joint Name Already exits!");
                }
            }
            return View(Joint);
        }

        public ActionResult JointEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/JointEdit/" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "Joint");
            var cpy = assessmentService.GetJoint(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JointEdit(AssessmentJointMasterViewModel Joint)
        {
            if (ModelState.IsValid)
            {
                Joint.UpdatedBy = AppSession.GetCurrentUserId();
                Joint.UpdatedDate = DateTime.Now;
                var result = assessmentService.SaveJoint(Joint);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(Joint);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteJoint")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteJoint(int id)
        {
            var result = assessmentService.DeleteJoint(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Leak

        // GET: Leak
        public ActionResult LeakIndex()
        {
            AppSession.SetCurrentPage("/Assessment/LeakIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Leak");
            return View(assessmentService.GetAllLeaks().ToList());
        }

        public ActionResult LeakCreate()
        {
            AppSession.SetCurrentPage("/Assessment/LeakCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Leak");
            AssessmentLeakMasterViewModel LeakMasterViewModel = new AssessmentLeakMasterViewModel();
            return View(LeakMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeakCreate(AssessmentLeakMasterViewModel Leak)
        {
            if (ModelState.IsValid)
            {
                if (assessmentService.CheckLeak(Leak.AssessmentLeakName) == false)
                {
                    Leak.CreatedBy = AppSession.GetCurrentUserId();
                    Leak.CreatedDate = DateTime.Now;
                    Leak.IsActive = 1;
                    var result = assessmentService.CreateLeak(Leak);
                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("Leak Name Already exits!");
                }
            }
            return View(Leak);
        }

        public ActionResult LeakEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/LeakEdit/" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "Leak");
            var cpy = assessmentService.GetLeak(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeakEdit(AssessmentLeakMasterViewModel Leak)
        {
            if (ModelState.IsValid)
            {
                Leak.UpdatedBy = AppSession.GetCurrentUserId();
                Leak.UpdatedDate = DateTime.Now;
                var result = assessmentService.SaveLeak(Leak);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(Leak);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteLeak")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLeak(int id)
        {
            var result = assessmentService.DeleteLeak(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Wall

        // GET: Wall
        public ActionResult WallIndex()
        {
            AppSession.SetCurrentPage("/Assessment/WallIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Wall");
            return View(assessmentService.GetAllWalls().ToList());
        }

        public ActionResult WallCreate()
        {
            AppSession.SetCurrentPage("/Assessment/WallCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Wall");
            AssessmentWallMasterViewModel WallMasterViewModel = new AssessmentWallMasterViewModel();
            return View(WallMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WallCreate(AssessmentWallMasterViewModel Wall)
        {
            if (ModelState.IsValid)
            {
                if (assessmentService.CheckWall(Wall.AssessmentWallName) == false)
                {
                    Wall.CreatedBy = AppSession.GetCurrentUserId();
                    Wall.CreatedDate = DateTime.Now;
                    Wall.IsActive = 1;
                    var result = assessmentService.CreateWall(Wall);
                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("Wall Name Already exits!");
                }
            }
            return View(Wall);
        }

        public ActionResult WallEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/WallEdit/" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "Wall");
            var cpy = assessmentService.GetWall(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WallEdit(AssessmentWallMasterViewModel Wall)
        {
            if (ModelState.IsValid)
            {
                Wall.UpdatedBy = AppSession.GetCurrentUserId();
                Wall.UpdatedDate = DateTime.Now;
                var result = assessmentService.SaveWall(Wall);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(Wall);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteWall")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteWall(int id)
        {
            var result = assessmentService.DeleteWall(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Window

        // GET: Window
        public ActionResult WindowIndex()
        {
            AppSession.SetCurrentPage("/Assessment/WindowIndex");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Window");
            return View(assessmentService.GetAllWindows().ToList());
        }

        public ActionResult WindowCreate()
        {
            AppSession.SetCurrentPage("/Assessment/WindowCreate");
            AppSession.SetCurrentMenu("Assessment", "Masters", "Window");
            AssessmentWindowMasterViewModel WindowMasterViewModel = new AssessmentWindowMasterViewModel();
            return View(WindowMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WindowCreate(AssessmentWindowMasterViewModel Window)
        {
            if (ModelState.IsValid)
            {
                if (assessmentService.CheckWindow(Window.AssessmentWindowName) == false)
                {
                    Window.CreatedBy = AppSession.GetCurrentUserId();
                    Window.CreatedDate = DateTime.Now;
                    Window.IsActive = 1;
                    var result = assessmentService.CreateWindow(Window);
                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("Window Name Already exits!");
                }
            }
            return View(Window);
        }

        public ActionResult WindowEdit(int? id)
        {
            AppSession.SetCurrentPage("/Assessment/WindowEdit/" + id.ToString());
            AppSession.SetCurrentMenu("Assessment", "Masters", "Window");
            var cpy = assessmentService.GetWindow(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WindowEdit(AssessmentWindowMasterViewModel Window)
        {
            if (ModelState.IsValid)
            {
                Window.UpdatedBy = AppSession.GetCurrentUserId();
                Window.UpdatedDate = DateTime.Now;
                var result = assessmentService.SaveWindow(Window);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(Window);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteWindow")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteWindow(int id)
        {
            var result = assessmentService.DeleteWindow(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Assessment Summary
        public ActionResult AssessmentSummary(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/AssessmentSummary/" + ID.ToString());
            AssessmentSummaryViewModel assessmentSummaryViewModel = new AssessmentSummaryViewModel();
            ViewBag.Title = "BUILDQAS ASSESSMENT REPORT";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentSummaryViewModel.projectMasterViewModel = assessmentService.GetProject(ID);
                assessmentSummaryViewModel.assessmentSummaryDetailModels = assessmentService.GetAssessmentSummaryByProjectID(ID);
                assessmentSummaryViewModel.ArchitecturalWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).Select(x => x.Weightage)?.Sum();
                if (assessmentSummaryViewModel.ArchitecturalWorksWeightage != 100)
                {
                    int Applicablecount = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).ToList().Count();
                    decimal AdjustWeightage = Math.Round(((Convert.ToDecimal(100) - Convert.ToDecimal(assessmentSummaryViewModel.ArchitecturalWorksWeightage)) / Applicablecount), 2);
                    foreach (var item in assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1))
                    {
                        item.Weightage = item.Weightage + AdjustWeightage;
                        item.WeightedScore = Math.Round((Convert.ToDecimal(item.NoofCompliances) / Convert.ToDecimal(item.NoofChecks)) * item.Weightage,2);
                    }
                    assessmentSummaryViewModel.ArchitecturalWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).Select(x => x.Weightage)?.Sum();
                    if (assessmentSummaryViewModel.ArchitecturalWorksWeightage != 100)
                    {
                        AdjustWeightage = Math.Round(Convert.ToDecimal(100) - Convert.ToDecimal(assessmentSummaryViewModel.ArchitecturalWorksWeightage), 2);
                        var item1 = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleID == 1).FirstOrDefault();
                        item1.Weightage = item1.Weightage + AdjustWeightage;
                        item1.WeightedScore = Math.Round((Convert.ToDecimal(item1.NoofCompliances) / Convert.ToDecimal(item1.NoofChecks)) * item1.Weightage,2);
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
            }
            catch (Exception ex)
            {
                logger.Debug("Assessment Summary :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentSummaryViewModel);
        }
        #endregion Assessment Summary 

        #region Internal Finishes

        public ActionResult InternalFinishesList()
        {
            AppSession.SetCurrentPage("/Assessment/InternalFinishesList");
            List<AssessmentInternalFinishesIndexViewModel> objIFList = new List<AssessmentInternalFinishesIndexViewModel>();
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                objIFList = assessmentService.GetAllAssessmentInternalFinishes_List(user.CompanyID).ToList();
                return View(objIFList);
            }
            catch (Exception ex)
            {
                logger.Debug("InternalFinishesList:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }

        public ActionResult PartialAddInternalFinishes(string ID)
        {
            AssessmentInternalFinishesEntryViewModel masterViewModel = new AssessmentInternalFinishesEntryViewModel();
            ModelState.Clear();
            masterViewModel.assessmentTypeLocationMasterViews = assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 1).ToList();
            masterViewModel.ProjectID = int.Parse(ID);
            AssessmentInternalFinishesTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentInternalFinishes(int.Parse(ID)).ToList().LastOrDefault();
            if (transMasterViewModel != null)
            {
                masterViewModel.Block_Unit = transMasterViewModel?.Block_Unit;
                //masterViewModel.AssessmentDate = transMasterViewModel?.AssessmentDate;
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.LocationID = transMasterViewModel?.LocationID;
            }
            else
            {
                masterViewModel.Block_Unit = "";
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.LocationID = 1;
            }
            return PartialView(masterViewModel);
        }

        public ActionResult InternalFinishes(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/InternalFinishes/" + ID.ToString());
            AssessmentInternalFinishesEntryViewModel assessmentInternalFinishesEntryViewModel = new AssessmentInternalFinishesEntryViewModel();
            ViewBag.Title = "INTERNAL FINISHES";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentInternalFinishesEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesIndexViewModel = assessmentService.GetAllAssessmentInternalFinishes_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentInternalFinishesEntryViewModel.assessmentTypeLocationMasterViews = new List<AssessmentTypeLocationMasterViewModel>();
                assessmentInternalFinishesEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 1).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentInternalFinishesEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentInternalFinishesEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentInternalFinishesEntryViewModel.moduleAndProcessesList = new List<ModuleAndProcessModel>();
                ModuleAndProcessModel moduleAndProcess = null;
                foreach (AssessmentTypeModuleMasterViewModel mod in assessmentInternalFinishesEntryViewModel.assessmentTypeModuleMasterViewModels)
                {
                    moduleAndProcess = new ModuleAndProcessModel();
                    moduleAndProcess.ModuleNames = mod.AssessmentTypeModuleShortName.Replace("&", "");
                    moduleAndProcess.ProcessIds = assessmentInternalFinishesEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).Select(a => a.AssessmentTypeModuleProcessID.ToString()).ToList();
                    assessmentInternalFinishesEntryViewModel.moduleAndProcessesList.Add(moduleAndProcess);
                    mod.ProcessCount = assessmentInternalFinishesEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID)?.Count();
                }
                assessmentInternalFinishesEntryViewModel.ModuleAndProcess = JsonConvert.SerializeObject(assessmentInternalFinishesEntryViewModel.moduleAndProcessesList);
                assessmentInternalFinishesEntryViewModel.PCount = 0;
                assessmentInternalFinishesEntryViewModel.SCount = 0;
                assessmentInternalFinishesEntryViewModel.CCount = 0;
                assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransMasterViewModels = assessmentService.GetAllAssessmentInternalFinishes(ID).ToList();
                if (assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransMasterViewModels.Count > 0)
                {
                    foreach (var tran in assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransMasterViewModels)
                    {
                        if (tran.assessment_type_location_master.AssessmentTypeLocationType == "P")
                        {
                            assessmentInternalFinishesEntryViewModel.PCount++;
                        }
                        else if (tran.assessment_type_location_master.AssessmentTypeLocationType == "S")
                        {
                            assessmentInternalFinishesEntryViewModel.SCount++;
                        }
                        else
                        {
                            assessmentInternalFinishesEntryViewModel.CCount++;
                        }
                    }
                    List<int> InternalFinishesids = new List<int>();
                    InternalFinishesids = assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransMasterViewModels.Select(a => a.AssessmentIFID).ToList();
                    //assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransDetailViewModels = assessmentService.GetAllAssessmentInternalFinishes_Detail(InternalFinishesids).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("InternalFinishesForm :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentInternalFinishesEntryViewModel);
        }


        public JsonResult JSSaveInternalFinishes(string PID, string Block_Unit, string LID, string ADate)
        {
            var data = "Error";
            if (assessmentService.CheckAssessmentInternalFinishes(int.Parse(PID), Block_Unit, int.Parse(LID), DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture)) == false)
            {
                AssessmentInternalFinishesTransMasterViewModel masterViewModel = new AssessmentInternalFinishesTransMasterViewModel();
                masterViewModel.ProjectID = int.Parse(PID);
                masterViewModel.Block_Unit = Block_Unit;
                masterViewModel.AssessmentDate = DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                masterViewModel.LocationID = int.Parse(LID);
                masterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                masterViewModel.CreatedDate = DateTime.Now;

                List<AssessmentInternalFinishesTransDetailViewModel> detailViewModels = new List<AssessmentInternalFinishesTransDetailViewModel>();
                foreach (var mod in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 1).OrderBy(x => x.OrderBy))
                {
                    foreach (var proc in assessmentService.GetAllModuleProcess().Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).OrderBy(x => x.OrderBy))
                    {
                        for (int i = 1; i <= mod.NoOfRow; i++)
                        {
                            detailViewModels.Add(new AssessmentInternalFinishesTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = proc.AssessmentTypeModuleProcessID,
                                Result = "1",
                                RowNo = i,
                                UpdatedBy = AppSession.GetCurrentUserId(),
                                UpdatedDate = DateTime.Now
                            });
                        }
                    }
                }
                var result = assessmentService.CreateAssessmentInternalFinishesMaster(masterViewModel, detailViewModels);
                if (result > 0)
                {
                    data = "Success";
                }
            }
            else
            {
                data = "Entry already exists!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JSSaveInternalFinishesDetail(int IFDId, string Result)
        {
            var data = "Error";
            AssessmentInternalFinishesTransDetailViewModel detailViewModel = assessmentService.GetAllAssessmentInternalFinishes_DetailByID(IFDId);
            if (!string.IsNullOrEmpty(Result))
            {
                detailViewModel.Result = Result;
                detailViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                detailViewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentInternalFinishesDetail(detailViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSDeleteInternalFinishes(string IFIds)
        {
            var data = "Error";
            var result = assessmentService.DeleteAssessmentInternalFinishes(IFIds.Substring(0, IFIds.Length - 1));
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion InternalFinishes

        #region External Wall
        public ActionResult ExternalWallList()
        {
            AppSession.SetCurrentPage("/Assessment/ExternalWallList");
            List<AssessmentExternalWallIndexViewModel> objIFList = new List<AssessmentExternalWallIndexViewModel>();
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                objIFList = assessmentService.GetAllAssessmentExternalWall_List(user.CompanyID).ToList();
                return View(objIFList);
            }
            catch (Exception ex)
            {
                logger.Debug("External Wall List:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }

        public ActionResult PartialAddExternalWall(string ID)
        {
            AssessmentExternalWallEntryViewModel masterViewModel = new AssessmentExternalWallEntryViewModel();
            ModelState.Clear();
            masterViewModel.assessmentTypeLocationMasterViews = assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 2).ToList();
            masterViewModel.ProjectID = int.Parse(ID);
            AssessmentExternalWallTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentExternalWall(int.Parse(ID)).ToList().LastOrDefault();
            if (transMasterViewModel != null)
            {
                masterViewModel.Block_Unit = transMasterViewModel?.Block_Unit;
                //masterViewModel.AssessmentDate = transMasterViewModel?.AssessmentDate;
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.LocationID = transMasterViewModel?.LocationID;
            }
            else
            {
                masterViewModel.Block_Unit = "";
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.LocationID = 1;
            }
            return PartialView(masterViewModel);
        }

        public ActionResult PartialExternalWallSignature(string ID)
        {
            AssessmentExternalWallEntryViewModel masterViewModel = new AssessmentExternalWallEntryViewModel();
            ModelState.Clear();
            masterViewModel.AssessmentEWID = int.Parse(ID);
            AssessmentExternalWallTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentExternalWall_ByID(int.Parse(ID));
            if (transMasterViewModel != null)
            {
                masterViewModel.Drawing_Image = transMasterViewModel?.Drawing_Image;
                masterViewModel.projectMasterView = transMasterViewModel.assessment_project_master;
            }
            else
            {
                masterViewModel.Drawing_Image = "";
            }
            return PartialView(masterViewModel);
        }

        [HttpPost]
        public ActionResult PartialExternalWallSignature(AssessmentExternalWallEntryViewModel masterViewModel)
        {
            var data = "Error";
            AssessmentExternalWallTransMasterViewModel viewModel = assessmentService.GetAllAssessmentExternalWall_ByID(masterViewModel.AssessmentEWID);
            if (!string.IsNullOrEmpty(masterViewModel.Drawing_Image))
            {
                viewModel.Drawing_Image = masterViewModel.Drawing_Image;
                viewModel.UpdatedBy = AppSession.GetCurrentUserId();
                viewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentExternalWallSignature(viewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExternalWall(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/ExternalWall/" + ID.ToString());
            AssessmentExternalWallEntryViewModel assessmentExternalWallEntryViewModel = new AssessmentExternalWallEntryViewModel();
            ViewBag.Title = "EXTERNAL WALL";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentExternalWallEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentExternalWallEntryViewModel.assessmentExternalWallIndexViewModel = assessmentService.GetAllAssessmentExternalWall_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentExternalWallEntryViewModel.assessmentTypeLocationMasterViews = new List<AssessmentTypeLocationMasterViewModel>();
                assessmentExternalWallEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 2).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentExternalWallEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentExternalWallEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentExternalWallEntryViewModel.moduleAndProcessesList = new List<ModuleAndProcessModel>();
                ModuleAndProcessModel moduleAndProcess = null;
                foreach (AssessmentTypeModuleMasterViewModel mod in assessmentExternalWallEntryViewModel.assessmentTypeModuleMasterViewModels)
                {
                    moduleAndProcess = new ModuleAndProcessModel();
                    moduleAndProcess.ModuleNames = mod.AssessmentTypeModuleShortName.Replace("&", "");
                    moduleAndProcess.ProcessIds = assessmentExternalWallEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).Select(a => a.AssessmentTypeModuleProcessID.ToString()).ToList();
                    assessmentExternalWallEntryViewModel.moduleAndProcessesList.Add(moduleAndProcess);
                    mod.ProcessCount = assessmentExternalWallEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID)?.Count();
                }
                assessmentExternalWallEntryViewModel.ModuleAndProcess = JsonConvert.SerializeObject(assessmentExternalWallEntryViewModel.moduleAndProcessesList);
                assessmentExternalWallEntryViewModel.assessmentExternalWallTransMasterViewModels = assessmentService.GetAllAssessmentExternalWall(ID).ToList();
                //if (assessmentExternalWallEntryViewModel.assessmentExternalWallTransMasterViewModels.Count > 0)
                //{
                //    List<int> ExternalWallids = new List<int>();
                //    ExternalWallids = assessmentExternalWallEntryViewModel.assessmentExternalWallTransMasterViewModels.Select(a => a.AssessmentEWID).ToList();
                //    assessmentExternalWallEntryViewModel.assessmentExternalWallTransDetailViewModels = assessmentService.GetAllAssessmentExternalWall_Detail(ExternalWallids).ToList();
                //}
            }
            catch (Exception ex)
            {
                logger.Debug("External Wall Form :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentExternalWallEntryViewModel);
        }

        public JsonResult JSSaveExternalWall(string PID, string Block_Unit, string LID, string ADate)
        {
            var data = "Error";
            if (assessmentService.CheckAssessmentExternalWall(int.Parse(PID), Block_Unit, int.Parse(LID), DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture)) == false)
            {
                AssessmentExternalWallTransMasterViewModel masterViewModel = new AssessmentExternalWallTransMasterViewModel();
                masterViewModel.ProjectID = int.Parse(PID);
                masterViewModel.Block_Unit = Block_Unit;
                masterViewModel.AssessmentDate = DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                masterViewModel.LocationID = int.Parse(LID);
                masterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                masterViewModel.CreatedDate = DateTime.Now;
                List<AssessmentExternalWallTransDetailViewModel> detailViewModels = new List<AssessmentExternalWallTransDetailViewModel>();
                foreach (var mod in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 2).OrderBy(x => x.OrderBy))
                {
                    foreach (var proc in assessmentService.GetAllModuleProcess().Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).OrderBy(x => x.OrderBy))
                    {
                        for (int i = 1; i <= mod.NoOfRow; i++)
                        {
                            detailViewModels.Add(new AssessmentExternalWallTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = proc.AssessmentTypeModuleProcessID,
                                Result = "1",
                                RowNo = i,
                                UpdatedBy = AppSession.GetCurrentUserId(),
                                UpdatedDate = DateTime.Now
                            });
                        }
                    }
                }
                var result = assessmentService.CreateAssessmentExternalWallMaster(masterViewModel, detailViewModels);
                if (result > 0)
                {
                    data = "Success";
                }
            }
            else
            {
                data = "Entry already exists!";
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSSaveExternalWallDetail(int EWDId, string Result)
        {
            var data = "Error";
            AssessmentExternalWallTransDetailViewModel detailViewModel = assessmentService.GetAllAssessmentExternalWall_DetailByID(EWDId);
            if (!string.IsNullOrEmpty(Result))
            {
                detailViewModel.Result = Result;
                detailViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                detailViewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentExternalWallDetail(detailViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSDeleteExternalWall(string EWIds)
        {
            var data = "Error";
            var result = assessmentService.DeleteAssessmentExternalWall(EWIds.Substring(0, EWIds.Length - 1));
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion  External Wall

        #region External Works
        public ActionResult ExternalWorksList()
        {
            AppSession.SetCurrentPage("/Assessment/ExternalWorksList");
            List<AssessmentExternalWorksIndexViewModel> objList = new List<AssessmentExternalWorksIndexViewModel>();
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                objList = assessmentService.GetAllAssessmentExternalWorks_List(user.CompanyID).ToList();
                return View(objList);
            }
            catch (Exception ex)
            {
                logger.Debug("External Works List:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }

        public ActionResult PartialAddExternalWorks(string ID)
        {
            AssessmentExternalWorksEntryViewModel masterViewModel = new AssessmentExternalWorksEntryViewModel();
            ModelState.Clear();
            masterViewModel.assessmentTypeLocationMasterViews = assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 3).ToList();
            masterViewModel.ProjectID = int.Parse(ID);
            AssessmentExternalWorksTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentExternalWorks(masterViewModel.ProjectID).ToList().LastOrDefault();
            if (transMasterViewModel != null)
            {
                masterViewModel.Remarks = transMasterViewModel?.Remarks;
                //masterViewModel.AssessmentDate = transMasterViewModel?.AssessmentDate;
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.LocationID = transMasterViewModel?.LocationID;
            }
            else
            {
                masterViewModel.Remarks = "";
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.LocationID = 1;
            }
            return PartialView(masterViewModel);
        }

        public ActionResult PartialExternalWorksSignature(string ID)
        {
            AssessmentExternalWorksEntryViewModel masterViewModel = new AssessmentExternalWorksEntryViewModel();
            ModelState.Clear();
            masterViewModel.AssessmentEWKID = int.Parse(ID);
            AssessmentExternalWorksTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentExternalWorks_ByID(int.Parse(ID));
            if (transMasterViewModel != null)
            {
                masterViewModel.Drawing_Image = transMasterViewModel?.Drawing_Image;
                masterViewModel.projectMasterView = transMasterViewModel.assessment_project_master;
            }
            else
            {
                masterViewModel.Drawing_Image = "";
            }
            return PartialView(masterViewModel);
        }

        [HttpPost]
        public ActionResult PartialExternalWorksSignature(AssessmentExternalWorksEntryViewModel masterViewModel)
        {
            var data = "Error";
            AssessmentExternalWorksTransMasterViewModel viewModel = assessmentService.GetAllAssessmentExternalWorks_ByID(masterViewModel.AssessmentEWKID);
            if (!string.IsNullOrEmpty(masterViewModel.Drawing_Image))
            {
                viewModel.Drawing_Image = masterViewModel.Drawing_Image;
                viewModel.UpdatedBy = AppSession.GetCurrentUserId();
                viewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentExternalWorksSignature(viewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExternalWorks(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/ExternalWorks/" + ID.ToString());
            AssessmentExternalWorksEntryViewModel assessmentExternalWorksEntryViewModel = new AssessmentExternalWorksEntryViewModel();
            ViewBag.Title = "EXTERNAL WORKS";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentExternalWorksEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentExternalWorksEntryViewModel.assessmentExternalWorksIndexViewModel = assessmentService.GetAllAssessmentExternalWorks_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentExternalWorksEntryViewModel.assessmentTypeLocationMasterViews = assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 3).ToList();
                assessmentExternalWorksEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 3).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentExternalWorksEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentExternalWorksEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentExternalWorksEntryViewModel.assessmentExternalWorksTransMasterViewModels = assessmentService.GetAllAssessmentExternalWorks(ID).ToList();
                //if (assessmentExternalWorksEntryViewModel.assessmentExternalWorksTransMasterViewModels.Count > 0)
                //{
                //    List<int> ExternalWorksids = new List<int>();
                //    ExternalWorksids = assessmentExternalWorksEntryViewModel.assessmentExternalWorksTransMasterViewModels.Select(a => a.AssessmentEWKID).ToList();
                //    assessmentExternalWorksEntryViewModel.assessmentExternalWorksTransDetailViewModels = assessmentService.GetAllAssessmentExternalWorks_Detail(ExternalWorksids).ToList();
                //}
                assessmentExternalWorksEntryViewModel.MaxProcessCount = assessmentExternalWorksEntryViewModel.assessmentTypeModuleProcessMasterViewModels.GroupBy(s => s.AssessmentTypeLocationID).Select(x => x.Count()).Max();
                //assessmentExternalWorksEntryViewModel.MaxProcessCount = (assessmentExternalWorksEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Select(x=> x.AssessmentTypeModuleProcessID).Count() / assessmentExternalWorksEntryViewModel.assessmentTypeLocationMasterViews.Select(x => x.AssessmentTypeLocationID).Count());
            }
            catch (Exception ex)
            {
                logger.Debug("External Works Form :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentExternalWorksEntryViewModel);
        }

        public JsonResult JSSaveExternalWorks(string PID, string Remarks, string LID, string ADate)
        {
            var data = "Error";
            if (assessmentService.CheckAssessmentExternalWorks(int.Parse(PID), int.Parse(LID), Remarks, DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture)) == false)
            {
                AssessmentExternalWorksTransMasterViewModel masterViewModel = new AssessmentExternalWorksTransMasterViewModel();
                masterViewModel.ProjectID = int.Parse(PID);
                masterViewModel.Remarks = Remarks;
                masterViewModel.AssessmentDate = DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                masterViewModel.LocationID = int.Parse(LID);
                masterViewModel.MobileAssessmentEWKID = 0;
                masterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                masterViewModel.CreatedDate = DateTime.Now;
                List<AssessmentExternalWorksTransDetailViewModel> detailViewModels = new List<AssessmentExternalWorksTransDetailViewModel>();
                foreach (var mod in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 3).OrderBy(x => x.OrderBy))
                {
                    foreach (var proc in assessmentService.GetAllModuleProcess().Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID && x.AssessmentTypeLocationID == int.Parse(LID)).OrderBy(x => x.OrderBy))
                    {
                        for (int i = 1; i <= mod.NoOfRow; i++)
                        {
                            detailViewModels.Add(new AssessmentExternalWorksTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = proc.AssessmentTypeModuleProcessID,
                                Result = "1",
                                RowNo = i,
                                UpdatedBy = AppSession.GetCurrentUserId(),
                                UpdatedDate = DateTime.Now
                            });
                        }
                    }
                }
                var result = assessmentService.CreateAssessmentExternalWorksMaster(masterViewModel, detailViewModels);
                if (result > 0)
                {
                    data = "Success";
                }
            }
            else
            {
                data = "Entry already exists!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JSSaveExternalWorksDetail(int EWKDId, string Result)
        {
            var data = "Error";
            AssessmentExternalWorksTransDetailViewModel detailViewModel = assessmentService.GetAllAssessmentExternalWorks_DetailByID(EWKDId);
            if (!string.IsNullOrEmpty(Result))
            {
                detailViewModel.Result = Result;
                detailViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                detailViewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentExternalWorksDetail(detailViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSDeleteExternalWorks(string EWKIds)
        {
            var data = "Error";
            var result = assessmentService.DeleteAssessmentExternalWorks(EWKIds.Substring(0, EWKIds.Length - 1));
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion  External Works

        #region Roof Construction
        public ActionResult RoofConstructionList()
        {
            AppSession.SetCurrentPage("/Assessment/RoofConstructionList");
            List<AssessmentRoofConstructionIndexViewModel> objList = new List<AssessmentRoofConstructionIndexViewModel>();
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                objList = assessmentService.GetAllAssessmentRoofConstruction_List(user.CompanyID).ToList();
                return View(objList);
            }
            catch (Exception ex)
            {
                logger.Debug("Roof Construction List:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }

        public ActionResult PartialAddRoofConstruction(string ID)
        {
            AssessmentRoofConstructionEntryViewModel masterViewModel = new AssessmentRoofConstructionEntryViewModel();
            ModelState.Clear();
            masterViewModel.assessmentTypeLocationMasterViews = assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 4).ToList();
            masterViewModel.ProjectID = int.Parse(ID);
            AssessmentRoofConstructionTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentRoofConstruction(int.Parse(ID)).ToList().LastOrDefault();
            if (transMasterViewModel != null)
            {
                masterViewModel.Block_Unit = transMasterViewModel?.Block_Unit;
                //masterViewModel.AssessmentDate = transMasterViewModel?.AssessmentDate;
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.LocationID = transMasterViewModel?.LocationID;
            }
            else
            {
                masterViewModel.Block_Unit = "";
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.LocationID = 1;
            }
            return PartialView(masterViewModel);
        }

        public ActionResult PartialRoofConstructionSignature(string ID)
        {
            AssessmentRoofConstructionEntryViewModel masterViewModel = new AssessmentRoofConstructionEntryViewModel();
            ModelState.Clear();
            masterViewModel.AssessmentRFCID = int.Parse(ID);
            AssessmentRoofConstructionTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentRoofConstruction_ByID(int.Parse(ID));
            if (transMasterViewModel != null)
            {
                masterViewModel.Drawing_Image = transMasterViewModel?.Drawing_Image;
                masterViewModel.projectMasterView = transMasterViewModel.assessment_project_master;

            }
            else
            {
                masterViewModel.Drawing_Image = "";
            }
            return PartialView(masterViewModel);
        }

        [HttpPost]
        public ActionResult PartialRoofConstructionSignature(AssessmentRoofConstructionEntryViewModel masterViewModel)
        {
            var data = "Error";
            AssessmentRoofConstructionTransMasterViewModel viewModel = assessmentService.GetAllAssessmentRoofConstruction_ByID(masterViewModel.AssessmentRFCID);
            if (!string.IsNullOrEmpty(masterViewModel.Drawing_Image))
            {
                viewModel.Drawing_Image = masterViewModel.Drawing_Image;
                viewModel.UpdatedBy = AppSession.GetCurrentUserId();
                viewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentRoofConstructionSignature(viewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RoofConstruction(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/RoofConstruction/" + ID.ToString());
            AssessmentRoofConstructionEntryViewModel assessmentRoofConstructionEntryViewModel = new AssessmentRoofConstructionEntryViewModel();
            ViewBag.Title = "ROOF CONSTRUCTION";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentRoofConstructionEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionIndexViewModel = assessmentService.GetAllAssessmentRoofConstruction_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentRoofConstructionEntryViewModel.assessmentTypeLocationMasterViews = new List<AssessmentTypeLocationMasterViewModel>();
                assessmentRoofConstructionEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 4).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentRoofConstructionEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentRoofConstructionEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentRoofConstructionEntryViewModel.moduleAndProcessesList = new List<ModuleAndProcessModel>();
                ModuleAndProcessModel moduleAndProcess = null;
                foreach (AssessmentTypeModuleMasterViewModel mod in assessmentRoofConstructionEntryViewModel.assessmentTypeModuleMasterViewModels)
                {
                    moduleAndProcess = new ModuleAndProcessModel();
                    moduleAndProcess.ModuleNames = mod.AssessmentTypeModuleShortName.Replace("&", "");
                    moduleAndProcess.ProcessIds = assessmentRoofConstructionEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).Select(a => a.AssessmentTypeModuleProcessID.ToString()).ToList();
                    assessmentRoofConstructionEntryViewModel.moduleAndProcessesList.Add(moduleAndProcess);
                    mod.ProcessCount = assessmentRoofConstructionEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID)?.Count();
                }
                assessmentRoofConstructionEntryViewModel.ModuleAndProcess = JsonConvert.SerializeObject(assessmentRoofConstructionEntryViewModel.moduleAndProcessesList);
                assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionTransMasterViewModels = assessmentService.GetAllAssessmentRoofConstruction(ID).ToList();
                if (assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionTransMasterViewModels.Count > 0)
                {
                    List<int> RoofConstructionids = new List<int>();
                    RoofConstructionids = assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionTransMasterViewModels.Select(a => a.AssessmentRFCID).ToList();
                    assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionTransDetailViewModels = assessmentService.GetAllAssessmentRoofConstruction_Detail(RoofConstructionids).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Roof Construction Form :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentRoofConstructionEntryViewModel);
        }

        public JsonResult JSSaveRoofConstruction(string PID, string Block_Unit, string LID, string ADate)
        {
            var data = "Error";
            if (assessmentService.CheckAssessmentRoofConstruction(int.Parse(PID), Block_Unit, int.Parse(LID), DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture)) == false)
            {
                AssessmentRoofConstructionTransMasterViewModel masterViewModel = new AssessmentRoofConstructionTransMasterViewModel();
                masterViewModel.ProjectID = int.Parse(PID);
                masterViewModel.Block_Unit = Block_Unit;
                masterViewModel.AssessmentDate = DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                masterViewModel.LocationID = int.Parse(LID);
                masterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                masterViewModel.CreatedDate = DateTime.Now;
                List<AssessmentRoofConstructionTransDetailViewModel> detailViewModels = new List<AssessmentRoofConstructionTransDetailViewModel>();
                foreach (var mod in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 4).OrderBy(x => x.OrderBy))
                {
                    foreach (var proc in assessmentService.GetAllModuleProcess().Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).OrderBy(x => x.OrderBy))
                    {
                        for (int i = 1; i <= mod.NoOfRow; i++)
                        {
                            detailViewModels.Add(new AssessmentRoofConstructionTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = proc.AssessmentTypeModuleProcessID,
                                Result = "1",
                                RowNo = i,
                                UpdatedBy = AppSession.GetCurrentUserId(),
                                UpdatedDate = DateTime.Now
                            });
                        }
                    }
                }
                var result = assessmentService.CreateAssessmentRoofConstructionMaster(masterViewModel, detailViewModels);
                if (result > 0)
                {
                    data = "Success";
                }
            }
            else
            {
                data = "Entry already exists!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JSSaveRoofConstructionDetail(int RCDId, string Result)
        {
            var data = "Error";
            AssessmentRoofConstructionTransDetailViewModel detailViewModel = assessmentService.GetAllAssessmentRoofConstruction_DetailByID(RCDId);
            if (!string.IsNullOrEmpty(Result))
            {
                detailViewModel.Result = Result;
                detailViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                detailViewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentRoofConstructionDetail(detailViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSDeleteRoofConstruction(string RCIds)
        {
            var data = "Error";
            var result = assessmentService.DeleteAssessmentRoofConstruction(RCIds.Substring(0, RCIds.Length - 1));
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion Roof Construction

        #region Field Window WTT
        public ActionResult FieldWindowWTTList()
        {
            AppSession.SetCurrentPage("/Assessment/FieldWindowWTTList");
            List<AssessmentFieldWindowWaterTightnessTestIndexViewModel> objList = new List<AssessmentFieldWindowWaterTightnessTestIndexViewModel>();
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                objList = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_List(user.CompanyID).ToList();
                return View(objList);
            }
            catch (Exception ex)
            {
                logger.Debug("Field Window WTT List:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }

        public ActionResult PartialAddFieldWindowWTT(string ID)
        {
            AssessmentFieldWindowWaterTightnessTestEntryViewModel masterViewModel = new AssessmentFieldWindowWaterTightnessTestEntryViewModel();
            ModelState.Clear();
            masterViewModel.assessmentWallMasterViewModels = assessmentService.GetAllWalls();
            masterViewModel.assessmentWindowMasterViewModels = assessmentService.GetAllWindows();
            masterViewModel.assessmentJointMasterViewModels = assessmentService.GetAllJoints();
            masterViewModel.assessmentDirectionMasterViewModels = assessmentService.GetAllDirections();
            masterViewModel.assessmentLeakMasterViewModels = assessmentService.GetAllLeaks();
            masterViewModel.ProjectID = int.Parse(ID);
            AssessmentFieldWindowWaterTightnessTestTransViewModel transMasterViewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest(int.Parse(ID)).ToList().LastOrDefault();
            if (transMasterViewModel != null)
            {
                masterViewModel.Block_Unit = transMasterViewModel?.Block_Unit;
                //masterViewModel.AssessmentDate = transMasterViewModel?.AssessmentDate;
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.WallID = transMasterViewModel?.AssessmentWallID;
                masterViewModel.WindowID = transMasterViewModel?.AssessmentWindowID;
                masterViewModel.JointID = transMasterViewModel?.AssessmentJointID;
                masterViewModel.DirectionID = transMasterViewModel?.AssessmentDirectionID;
                masterViewModel.LeakID = transMasterViewModel?.AssessmentLeakID;
            }
            else
            {
                masterViewModel.Block_Unit = "";
                masterViewModel.AssessmentDate = DateTime.Now;
                masterViewModel.WallID = 1;
                masterViewModel.WindowID = 1;
                masterViewModel.JointID = 1;
                masterViewModel.DirectionID = 1;
                masterViewModel.LeakID = 1;
            }
            return PartialView(masterViewModel);
        }

        public ActionResult PartialFieldWindowWTTLeakThru(string Id)
        {
            AssessmentFieldWindowWaterTightnessTestTransViewModel transMasterViewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_ByID(int.Parse(Id));
            ViewBag.LeakThruList = assessmentService.GetAllLeaks();
            return PartialView(transMasterViewModel);
        }

        public ActionResult PartialFieldWindowWTTSignature(string ID)
        {
            AssessmentFieldWindowWaterTightnessTestEntryViewModel masterViewModel = new AssessmentFieldWindowWaterTightnessTestEntryViewModel();
            ModelState.Clear();
            masterViewModel.AssessmentFWWTTID = int.Parse(ID);
            AssessmentFieldWindowWaterTightnessTestTransViewModel transMasterViewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_ByID(int.Parse(ID));
            if (transMasterViewModel != null)
            {
                masterViewModel.Drawing_Image = transMasterViewModel?.Drawing_Image;
                masterViewModel.projectMasterView = transMasterViewModel.assessment_project_master;
            }
            else
            {
                masterViewModel.Drawing_Image = "";
            }
            return PartialView(masterViewModel);
        }

        [HttpPost]
        public ActionResult PartialFieldWindowWTTSignature(AssessmentFieldWindowWaterTightnessTestEntryViewModel masterViewModel)
        {
            var data = "Error";
            AssessmentFieldWindowWaterTightnessTestTransViewModel viewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_ByID(masterViewModel.AssessmentFWWTTID);
            if (!string.IsNullOrEmpty(masterViewModel.Drawing_Image))
            {
                viewModel.Drawing_Image = masterViewModel.Drawing_Image;
                viewModel.UpdatedBy = AppSession.GetCurrentUserId();
                viewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentFieldWindowWaterTightnessTestSignature(viewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FieldWindowWTT(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/FieldWindowWTT/" + ID.ToString());
            AssessmentFieldWindowWaterTightnessTestEntryViewModel assessmentFieldWindowWaterTightnessTestEntryViewModel = new AssessmentFieldWindowWaterTightnessTestEntryViewModel();
            ViewBag.Title = "Field Window Water-Tightness Test (WTT) (Third Party Test)";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentFieldWindowWaterTightnessTestEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentWallMasterViewModels = new List<AssessmentWallMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentWindowMasterViewModels = new List<AssessmentWindowMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentJointMasterViewModels = new List<AssessmentJointMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentDirectionMasterViewModels = new List<AssessmentDirectionMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentLeakMasterViewModels = new List<AssessmentLeakMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentFieldWindowWaterTightnessTestIndexViewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentFieldWindowWaterTightnessTestTransViewModels = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest(ID).ToList();
            }
            catch (Exception ex)
            {
                logger.Debug("Field Window WTT Form :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentFieldWindowWaterTightnessTestEntryViewModel);
        }

        public JsonResult JSSaveFieldWindowWaterTightnessTest(string PID, string Block_Unit, string ADate, string WID, string WNID, string JID, string DID, string LID)
        {
            var data = "Error";
            if (assessmentService.CheckAssessmentFieldWindowWaterTightnessTest(int.Parse(PID), Block_Unit, DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture), int.Parse(WID), int.Parse(WNID), int.Parse(JID), int.Parse(DID), int.Parse(LID)) == false)
            {
                AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel = new AssessmentFieldWindowWaterTightnessTestTransViewModel();
                masterViewModel.ProjectID = int.Parse(PID);
                masterViewModel.Block_Unit = Block_Unit;
                masterViewModel.AssessmentDate = DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                masterViewModel.AssessmentWallID = int.Parse(WID);
                masterViewModel.AssessmentWindowID = int.Parse(WNID);
                masterViewModel.AssessmentJointID = int.Parse(JID);
                masterViewModel.AssessmentDirectionID = int.Parse(DID);
                masterViewModel.AssessmentLeakID = int.Parse(LID);
                masterViewModel.Result = "1";
                masterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                masterViewModel.CreatedDate = DateTime.Now;
                var result = assessmentService.CreateAssessmentFieldWindowWaterTightnessTest(masterViewModel);
                if (result > 0)
                {
                    data = "Success";
                }
            }
            else
            {
                data = "Entry already exists!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JSUpdateFieldWindowWaterTightnessTest(int FWWTTId, string Result)
        {
            var data = "Error";
            AssessmentFieldWindowWaterTightnessTestTransViewModel detailViewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_ByID(FWWTTId);
            if (!string.IsNullOrEmpty(Result))
            {
                detailViewModel.Result = Result;
                if(Result != "2")
                {
                    detailViewModel.AssessmentLeakID = 1;
                }
                detailViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                detailViewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentFieldWindowWaterTightnessTest(detailViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSSaveFieldWindowWTTLeakThru(int FWWTTId, int LeakThruID)
        {
            var data = "Error";
            AssessmentFieldWindowWaterTightnessTestTransViewModel detailViewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_ByID(FWWTTId);
            detailViewModel.AssessmentLeakID = LeakThruID;
            detailViewModel.UpdatedBy = AppSession.GetCurrentUserId();
            detailViewModel.UpdatedDate = DateTime.Now;
            var result = assessmentService.SaveAssessmentFieldWindowWaterTightnessTest(detailViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        

        public JsonResult JSDeleteFieldWindowWaterTightnessTest(string FWWTTIds)
        {
            var data = "Error";
            var result = assessmentService.DeleteAssessmentFieldWindowWaterTightnessTest(FWWTTIds.Substring(0, FWWTTIds.Length - 1));
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion Field Window WTT

        #region Wet Area WTT
        public ActionResult WetAreaWTTList()
        {
            AppSession.SetCurrentPage("/Assessment/WetAreaWTTList");
            List<AssessmentWetAreaWaterTightnessTestIndexViewModel> objList = new List<AssessmentWetAreaWaterTightnessTestIndexViewModel>();
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                objList = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_List(user.CompanyID).ToList();
                return View(objList);
            }
            catch (Exception ex)
            {
                logger.Debug("Wet Area WTT List:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }

        public ActionResult PartialAddWetAreaWTT(string ID)
        {
            AssessmentWetAreaWaterTightnessTestEntryViewModel masterViewModel = new AssessmentWetAreaWaterTightnessTestEntryViewModel();
            ModelState.Clear();
            masterViewModel.ProjectID = int.Parse(ID);
            AssessmentWetAreaWaterTightnessTestTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest(int.Parse(ID)).ToList().LastOrDefault();
            if (transMasterViewModel != null)
            {
                masterViewModel.Block_Unit = transMasterViewModel?.Block_Unit;
                //masterViewModel.AssessmentDate = transMasterViewModel?.AssessmentDate;
                masterViewModel.AssessmentDate = DateTime.Now;
            }
            else
            {
                masterViewModel.Block_Unit = "";
                masterViewModel.AssessmentDate = DateTime.Now;
            }
            return PartialView(masterViewModel);
        }

        public ActionResult PartialWetAreaWTTOtherResult(string Id)
        {
            AssessmentWetAreaWaterTightnessTestTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_ByID(int.Parse(Id));
            return PartialView(transMasterViewModel);
        }

        public ActionResult PartialWetAreaWTTSignature(string ID)
        {
            AssessmentWetAreaWaterTightnessTestEntryViewModel masterViewModel = new AssessmentWetAreaWaterTightnessTestEntryViewModel();
            ModelState.Clear();
            masterViewModel.AssessmentWAWTTID = int.Parse(ID);
            AssessmentWetAreaWaterTightnessTestTransMasterViewModel transMasterViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_ByID(int.Parse(ID));
            if (transMasterViewModel != null)
            {
                masterViewModel.Drawing_Image = transMasterViewModel?.Drawing_Image;
                masterViewModel.projectMasterView = transMasterViewModel.assessment_project_master;
            }
            else
            {
                masterViewModel.Drawing_Image = "";
            }
            return PartialView(masterViewModel);
        }

        [HttpPost]
        public ActionResult PartialWetAreaWTTSignature(AssessmentWetAreaWaterTightnessTestEntryViewModel masterViewModel)
        {
            var data = "Error";
            AssessmentWetAreaWaterTightnessTestTransMasterViewModel viewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_ByID(masterViewModel.AssessmentWAWTTID);
            if (!string.IsNullOrEmpty(masterViewModel.Drawing_Image))
            {
                viewModel.Drawing_Image = masterViewModel.Drawing_Image;
                viewModel.UpdatedBy = AppSession.GetCurrentUserId();
                viewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentWetAreaWaterTightnessTestSignature(viewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WetAreaWTT(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/WetAreaWTT/" + ID.ToString());
            AssessmentWetAreaWaterTightnessTestEntryViewModel assessmentWetAreaWaterTightnessTestEntryViewModel = new AssessmentWetAreaWaterTightnessTestEntryViewModel();
            ViewBag.Title = "Wet Area Water-Tightness Test (WTT) (Third Party Test)";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentWetAreaWaterTightnessTestEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestIndexViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestResultMasterViewModels = assessmentService.GetAllWetAreaWaterTightnessTestResults().Where(x => x.IsActive == 1).OrderBy(x => x.OrderBy).ToList();
                assessmentWetAreaWaterTightnessTestEntryViewModel.ResultCount = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestResultMasterViewModels.Count;
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 6).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentWetAreaWaterTightnessTestEntryViewModel.moduleAndProcessesList = new List<ModuleAndProcessModel>();
                ModuleAndProcessModel moduleAndProcess = null;
                foreach (AssessmentTypeModuleMasterViewModel mod in assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleMasterViewModels)
                {
                    moduleAndProcess = new ModuleAndProcessModel();
                    moduleAndProcess.ModuleNames = mod.AssessmentTypeModuleShortName.Replace("&", "");
                    moduleAndProcess.ProcessIds = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).Select(a => a.AssessmentTypeModuleProcessID.ToString()).ToList();
                    assessmentWetAreaWaterTightnessTestEntryViewModel.moduleAndProcessesList.Add(moduleAndProcess);
                    mod.ProcessCount = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID)?.Count();
                }
                assessmentWetAreaWaterTightnessTestEntryViewModel.ModuleAndProcess = JsonConvert.SerializeObject(assessmentWetAreaWaterTightnessTestEntryViewModel.moduleAndProcessesList);
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransMasterViewModels = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest(ID).ToList();
                if (assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransMasterViewModels.Count > 0)
                {
                    List<int> WetAreaWaterTightnessTestids = new List<int>();
                    WetAreaWaterTightnessTestids = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransMasterViewModels.Select(a => a.AssessmentWAWTTID).ToList();
                    assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransDetailViewModels = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_Detail(WetAreaWaterTightnessTestids).ToList();
                    assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransDetailResultViewModels = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_DetailResult(WetAreaWaterTightnessTestids).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Wet Area Water Tightness Test Form :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentWetAreaWaterTightnessTestEntryViewModel);
        }

        public JsonResult JSSaveWetAreaWaterTightnessTest(string PID, string Block_Unit, string ADate, string other)
        {
            var data = "Error";
            if (assessmentService.CheckAssessmentWetAreaWaterTightnessTest(int.Parse(PID), Block_Unit, DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture)) == false)
            {
                AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel = new AssessmentWetAreaWaterTightnessTestTransMasterViewModel();
                masterViewModel.ProjectID = int.Parse(PID);
                masterViewModel.Block_Unit = Block_Unit;
                masterViewModel.Other_Result = other;
                masterViewModel.AssessmentDate = DateTime.ParseExact(ADate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                masterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                masterViewModel.CreatedDate = DateTime.Now;
                List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> detailResultViewModels = new List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel>();
                List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> detailViewModels = new List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel>();
                foreach (var mod in assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 6).OrderBy(x => x.OrderBy))
                {
                    foreach (var proc in assessmentService.GetAllModuleProcess().Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).OrderBy(x => x.OrderBy))
                    {
                        for (int i = 1; i <= mod.NoOfRow; i++)
                        {
                            detailViewModels.Add(new AssessmentWetAreaWaterTightnessTestTransDetailViewModel
                            {
                                AssessmentTypeModuleProcessID = proc.AssessmentTypeModuleProcessID,
                                Result = "1",
                                RowNo = i,
                                UpdatedBy = AppSession.GetCurrentUserId(),
                                UpdatedDate = DateTime.Now
                            });
                        }

                        foreach (var TestResult in assessmentService.GetAllWetAreaWaterTightnessTestResults().Where(x => x.IsActive == 1).OrderBy(x => x.OrderBy))
                        {
                            detailResultViewModels.Add(new AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel
                            {
                                AssessmentWAWTTResultID = TestResult.AssessmentWAWTTResultID,
                                AssessmentTypeModuleProcessID = proc.AssessmentTypeModuleProcessID,
                                Result = "",
                                UpdatedBy = AppSession.GetCurrentUserId(),
                                UpdatedDate = DateTime.Now
                            });
                        }
                    }
                }
                var result = assessmentService.CreateAssessmentWetAreaWaterTightnessTestMaster(masterViewModel, detailViewModels, detailResultViewModels);
                if (result > 0)
                {
                    data = "Success";
                }
            }
            else
            {
                data = "Entry already exists!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSSaveWetAreaWaterTightnessTestDetail(int WAWTTDId, string Result)
        {
            var data = "Error";
            AssessmentWetAreaWaterTightnessTestTransDetailViewModel detailViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_DetailByID(WAWTTDId);
            if (!string.IsNullOrEmpty(Result))
            {
                detailViewModel.Result = Result;
                detailViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                detailViewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentWetAreaWaterTightnessTestDetail(detailViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSSaveWetAreaWaterTightnessTestDetailResult(int WAWTTDRId, string Result)
        {
            var data = "Error";
            AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel detailResultViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_DetailResultByID(WAWTTDRId);
            if (!string.IsNullOrEmpty(Result))
            {
                detailResultViewModel.Result = Result;
                detailResultViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                detailResultViewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentWetAreaWaterTightnessTestDetailResult(detailResultViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JSSaveWetAreaWaterTightnessTestOtherResult(int WAWTTId, string OtherResult)
        {
            var data = "Error";
            AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_ByID(WAWTTId);
            if (!string.IsNullOrEmpty(OtherResult))
            {
                masterViewModel.Other_Result = OtherResult;
                masterViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                masterViewModel.UpdatedDate = DateTime.Now;
            }
            var result = assessmentService.SaveAssessmentWetAreaWaterTightnessTestOtherResult(masterViewModel);
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JSDeleteWetAreaWaterTightnessTest(string WAWTTIds)
        {
            var data = "Error";
            var result = assessmentService.DeleteAssessmentWetAreaWaterTightnessTest(WAWTTIds.Substring(0, WAWTTIds.Length - 1));
            if (result > 0)
            {
                data = "Success";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion Wet Area WTT

        #region Assessment Internal Finishes Report
        public ActionResult AssessmentInternalFinishesReport(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/AssessmentInternalFinishesReport/" + ID.ToString());
            AssessmentReportViewModel assessmentReportViewModel = new AssessmentReportViewModel();
            ViewBag.Title = "BUILDQAS ASSESSMENT INTERNAL FINISHES REPORT";
            ViewBag.Module = "Report";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentReportViewModel.projectMasterViewModel = assessmentService.GetProject(ID);
                assessmentReportViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 1).ToList();
            }
            catch (Exception ex)
            {
                logger.Debug("Assessment Internal Finishes Report :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentReportViewModel);
        }

        public JsonResult JSGetInternalFinishesChart(string PId, string MId)
        {
            Dictionary<string, decimal> data = new Dictionary<string, decimal>();
            var result = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(int.Parse(PId), int.Parse(MId));
            if (result.Count > 0)
            {
                //[['FLOOR', 45.0], ['WALL', 45.0], ['CEILING', 45.0], ['DOOR', 45.0], ['WINDOW', 45.0], ['COMPONENT', 45.0], ['M & E FITTING', 45.0]]
                foreach (var obj in result)
                {
                    if (MId == "-1")
                    {
                        data.Add(obj.AssessmentTypeModuleName, obj.NonCompliances);
                    }
                    else
                    {
                        data.Add(obj.AssessmentTypeModuleProcessName, obj.NonCompliances);
                    }
                }
            }
            return Json(data.ToArray(), JsonRequestBehavior.AllowGet);
        }

        #endregion Assessment Internal Finishes Report  

        #region Assessment Report

        public ActionResult AssessmentReport(int ID)
        {
            AppSession.SetCurrentPage("/Assessment/AssessmentReport/" + ID.ToString());
            AssessmentReportViewModel assessmentReportViewModel = new AssessmentReportViewModel();
            ViewBag.Title = "BUILDQAS ASSESSMENTS REPORT";
            ViewBag.Module = "Report";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentReportViewModel.projectMasterViewModel = assessmentService.GetProject(ID);
                assessmentReportViewModel.PDFFilename = "Assessment_Reports_" + assessmentReportViewModel.projectMasterViewModel.Project_ID + ".pdf";
            }
            catch (Exception ex)
            {
                logger.Debug("Assessment Report :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentReportViewModel);
        }

        public ActionResult AssessmentReportPDF(int ID, string Filter)
        {
            AssessmentReportViewModel assessmentReportViewModel = new AssessmentReportViewModel();
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentReportViewModel.projectMasterViewModel = assessmentService.GetProject(ID);
                int Icnt = 1;
                assessmentReportViewModel.PageCount = 0;
                foreach (var module in Filter.Split(','))
                {
                    if (Icnt == 1)
                    {
                        if (module == "true")
                        {
                            assessmentReportViewModel.DAR1IFList = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(ID, -1);
                            assessmentReportViewModel.DAR1IF = true;
                            assessmentReportViewModel.PageCount++;
                        }
                        else
                        {
                            assessmentReportViewModel.DAR1IFList = new List<AssessmentReportDetailModel>();
                            assessmentReportViewModel.DAR1IF = false;
                        }
                    }
                    else if (Icnt == 2)
                    {
                        if (module == "true")
                        {
                            assessmentReportViewModel.DAR2FLOORList = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(ID, 1);
                            assessmentReportViewModel.DAR2FLOOR = true;
                            assessmentReportViewModel.PageCount++;
                        }
                        else
                        {
                            assessmentReportViewModel.DAR2FLOORList = new List<AssessmentReportDetailModel>();
                            assessmentReportViewModel.DAR2FLOOR = false;
                        }
                    }
                    else if (Icnt == 3)
                    {
                        if (module == "true")
                        {
                            assessmentReportViewModel.DAR2WALLList = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(ID, 2);
                            assessmentReportViewModel.DAR2WALL = true;
                            assessmentReportViewModel.PageCount++;
                        }
                        else
                        {
                            assessmentReportViewModel.DAR2WALLList = new List<AssessmentReportDetailModel>();
                            assessmentReportViewModel.DAR2WALL = false;
                        }
                    }
                    else if (Icnt == 4)
                    {
                        if (module == "true")
                        {
                            assessmentReportViewModel.DAR2CEILINGList = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(ID, 3);
                            assessmentReportViewModel.DAR2CEILING = true;
                            assessmentReportViewModel.PageCount++;
                        }
                        else
                        {
                            assessmentReportViewModel.DAR2CEILINGList = new List<AssessmentReportDetailModel>();
                            assessmentReportViewModel.DAR2CEILING = false;
                        }
                    }
                    else if (Icnt == 5)
                    {
                        if (module == "true")
                        {
                            assessmentReportViewModel.DAR2DOORList = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(ID, 4);
                            assessmentReportViewModel.DAR2DOOR = true;
                            assessmentReportViewModel.PageCount++;
                        }
                        else
                        {
                            assessmentReportViewModel.DAR2DOORList = new List<AssessmentReportDetailModel>();
                            assessmentReportViewModel.DAR2DOOR = false;
                        }
                    }
                    else if (Icnt == 6)
                    {
                        if (module == "true")
                        {
                            assessmentReportViewModel.DAR2WINDOWList = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(ID, 5);
                            assessmentReportViewModel.DAR2WINDOW = true;
                            assessmentReportViewModel.PageCount++;
                        }
                        else
                        {
                            assessmentReportViewModel.DAR2WINDOWList = new List<AssessmentReportDetailModel>();
                            assessmentReportViewModel.DAR2WINDOW = false;
                        }
                    }
                    else if (Icnt == 7)
                    {
                        if (module == "true")
                        {
                            assessmentReportViewModel.DAR2COMPONENTList = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(ID, 6);
                            assessmentReportViewModel.DAR2COMPONENT = true;
                            assessmentReportViewModel.PageCount++;
                        }
                        else
                        {
                            assessmentReportViewModel.DAR2COMPONENTList = new List<AssessmentReportDetailModel>();
                            assessmentReportViewModel.DAR2COMPONENT = false;
                        }
                    }
                    else if (Icnt == 8)
                    {
                        if (module == "true")
                        {
                            assessmentReportViewModel.DAR2MEList = assessmentService.GetAssessmentInternalFinishesByProjectAndModuleID(ID, 7);
                            assessmentReportViewModel.DAR2ME = true;
                            assessmentReportViewModel.PageCount++;
                        }
                        else
                        {
                            assessmentReportViewModel.DAR2MEList = new List<AssessmentReportDetailModel>();
                            assessmentReportViewModel.DAR2ME = false;
                        }
                    }
                    Icnt++;
                }
                assessmentReportViewModel.PageCount = (assessmentReportViewModel.PageCount * 2) + 1;
            }
            catch (Exception ex)
            {
                logger.Debug("Assessment Report :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentReportViewModel);
        }

        public ActionResult AssessmentSummaryReportPDF(int ID)
        {
            AssessmentSummaryViewModel assessmentSummaryViewModel = new AssessmentSummaryViewModel();
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentSummaryViewModel.projectMasterViewModel = assessmentService.GetProject(ID);
                assessmentSummaryViewModel.assessmentSummaryDetailModels = assessmentService.GetAssessmentSummaryByProjectID(ID);
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
                assessmentSummaryViewModel.InternalFinishesWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeID == 1 && x.AssessmentTypeModuleName != "M&E").Select(x => x.Weightage)?.Sum();
                assessmentSummaryViewModel.InternalFinishesWorksWeightedScore = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeID == 1 && x.AssessmentTypeModuleName != "M&E").Select(x => x.WeightedScore)?.Sum();
                assessmentSummaryViewModel.MEWorksWeightage = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeID == 1 && x.AssessmentTypeModuleName == "M&E").Select(x => x.Weightage)?.Sum();
                assessmentSummaryViewModel.MEWorksWeightedScore = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeID == 1 && x.AssessmentTypeModuleName == "M&E").Select(x => x.WeightedScore)?.Sum();
                assessmentSummaryViewModel.ArchitecturalWorksWeightedScore = assessmentSummaryViewModel.assessmentSummaryDetailModels.Where(x => x.AssessmentTypeModuleName != "M&E" && x.Is_Applicable == 1).Select(x => x.WeightedScore)?.Sum();
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
            }
            catch (Exception ex)
            {
                logger.Debug("Assessment Summary :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentSummaryViewModel);
        }

        public ActionResult AssessmentInternalFinishesReportPDF(int ID)
        {
            AssessmentInternalFinishesEntryViewModel assessmentInternalFinishesEntryViewModel = new AssessmentInternalFinishesEntryViewModel();
            ViewBag.Title = "INTERNAL FINISHES";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentInternalFinishesEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesIndexViewModel = assessmentService.GetAllAssessmentInternalFinishes_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentInternalFinishesEntryViewModel.assessmentTypeLocationMasterViews = new List<AssessmentTypeLocationMasterViewModel>();
                assessmentInternalFinishesEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 1).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentInternalFinishesEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentInternalFinishesEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentInternalFinishesEntryViewModel.moduleAndProcessesList = new List<ModuleAndProcessModel>();
                ModuleAndProcessModel moduleAndProcess = null;
                foreach (AssessmentTypeModuleMasterViewModel mod in assessmentInternalFinishesEntryViewModel.assessmentTypeModuleMasterViewModels)
                {
                    moduleAndProcess = new ModuleAndProcessModel();
                    moduleAndProcess.ModuleNames = mod.AssessmentTypeModuleShortName.Replace("&", "");
                    moduleAndProcess.ProcessIds = assessmentInternalFinishesEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).Select(a => a.AssessmentTypeModuleProcessID.ToString()).ToList();
                    assessmentInternalFinishesEntryViewModel.moduleAndProcessesList.Add(moduleAndProcess);
                    mod.ProcessCount = assessmentInternalFinishesEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID)?.Count();
                }
                assessmentInternalFinishesEntryViewModel.ModuleAndProcess = JsonConvert.SerializeObject(assessmentInternalFinishesEntryViewModel.moduleAndProcessesList);
                assessmentInternalFinishesEntryViewModel.PCount = 0;
                assessmentInternalFinishesEntryViewModel.SCount = 0;
                assessmentInternalFinishesEntryViewModel.CCount = 0;
                assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransMasterViewModels = assessmentService.GetAllAssessmentInternalFinishes(ID).ToList();
                if (assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransMasterViewModels.Count > 0)
                {
                    foreach (var tran in assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransMasterViewModels)
                    {
                        if (tran.assessment_type_location_master.AssessmentTypeLocationType == "P")
                        {
                            assessmentInternalFinishesEntryViewModel.PCount++;
                        }
                        else if (tran.assessment_type_location_master.AssessmentTypeLocationType == "S")
                        {
                            assessmentInternalFinishesEntryViewModel.SCount++;
                        }
                        else
                        {
                            assessmentInternalFinishesEntryViewModel.CCount++;
                        }
                    }
                    List<int> InternalFinishesids = new List<int>();
                    InternalFinishesids = assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransMasterViewModels.Select(a => a.AssessmentIFID).ToList();
                    assessmentInternalFinishesEntryViewModel.assessmentInternalFinishesTransDetailViewModels = assessmentService.GetAllAssessmentInternalFinishes_Detail(InternalFinishesids).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("AssessmentInternalFinishesReportPDF :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentInternalFinishesEntryViewModel);
        }

        public ActionResult AssessmentExternalWallReportPDF(int ID)
        {
            AssessmentExternalWallEntryViewModel assessmentExternalWallEntryViewModel = new AssessmentExternalWallEntryViewModel();
            ViewBag.Title = "EXTERNAL WALL";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentExternalWallEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentExternalWallEntryViewModel.assessmentExternalWallIndexViewModel = assessmentService.GetAllAssessmentExternalWall_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentExternalWallEntryViewModel.assessmentTypeLocationMasterViews = new List<AssessmentTypeLocationMasterViewModel>();
                assessmentExternalWallEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 2).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentExternalWallEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentExternalWallEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentExternalWallEntryViewModel.moduleAndProcessesList = new List<ModuleAndProcessModel>();
                ModuleAndProcessModel moduleAndProcess = null;
                foreach (AssessmentTypeModuleMasterViewModel mod in assessmentExternalWallEntryViewModel.assessmentTypeModuleMasterViewModels)
                {
                    moduleAndProcess = new ModuleAndProcessModel();
                    moduleAndProcess.ModuleNames = mod.AssessmentTypeModuleShortName.Replace("&", "");
                    moduleAndProcess.ProcessIds = assessmentExternalWallEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).Select(a => a.AssessmentTypeModuleProcessID.ToString()).ToList();
                    assessmentExternalWallEntryViewModel.moduleAndProcessesList.Add(moduleAndProcess);
                    mod.ProcessCount = assessmentExternalWallEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID)?.Count();
                }
                assessmentExternalWallEntryViewModel.ModuleAndProcess = JsonConvert.SerializeObject(assessmentExternalWallEntryViewModel.moduleAndProcessesList);
                assessmentExternalWallEntryViewModel.assessmentExternalWallTransMasterViewModels = assessmentService.GetAllAssessmentExternalWall(ID).ToList();
                //if (assessmentExternalWallEntryViewModel.assessmentExternalWallTransMasterViewModels.Count > 0)
                //{
                //    List<int> ExternalWallids = new List<int>();
                //    ExternalWallids = assessmentExternalWallEntryViewModel.assessmentExternalWallTransMasterViewModels.Select(a => a.AssessmentEWID).ToList();
                //    assessmentExternalWallEntryViewModel.assessmentExternalWallTransDetailViewModels = assessmentService.GetAllAssessmentExternalWall_Detail(ExternalWallids).ToList();
                //}
            }
            catch (Exception ex)
            {
                logger.Debug("AssessmentExternalWallReportPDF :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentExternalWallEntryViewModel);
        }

        public ActionResult AssessmentExternalWorksReportPDF(int ID)
        {
            AssessmentExternalWorksEntryViewModel assessmentExternalWorksEntryViewModel = new AssessmentExternalWorksEntryViewModel();
            ViewBag.Title = "EXTERNAL WORKS";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentExternalWorksEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentExternalWorksEntryViewModel.assessmentExternalWorksIndexViewModel = assessmentService.GetAllAssessmentExternalWorks_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentExternalWorksEntryViewModel.assessmentTypeLocationMasterViews = assessmentService.GetAllAssessmentLocations().Where(x => x.AssessmentTypeID == 3).ToList();
                assessmentExternalWorksEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 3).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentExternalWorksEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentExternalWorksEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentExternalWorksEntryViewModel.assessmentExternalWorksTransMasterViewModels = assessmentService.GetAllAssessmentExternalWorks(ID).ToList();
                //if (assessmentExternalWorksEntryViewModel.assessmentExternalWorksTransMasterViewModels.Count > 0)
                //{
                //    List<int> ExternalWorksids = new List<int>();
                //    ExternalWorksids = assessmentExternalWorksEntryViewModel.assessmentExternalWorksTransMasterViewModels.Select(a => a.AssessmentEWKID).ToList();
                //    assessmentExternalWorksEntryViewModel.assessmentExternalWorksTransDetailViewModels = assessmentService.GetAllAssessmentExternalWorks_Detail(ExternalWorksids).ToList();
                //}
                assessmentExternalWorksEntryViewModel.MaxProcessCount = assessmentExternalWorksEntryViewModel.assessmentTypeModuleProcessMasterViewModels.GroupBy(s => s.AssessmentTypeLocationID).Select(x => x.Count()).Max();
                //assessmentExternalWorksEntryViewModel.MaxProcessCount = (assessmentExternalWorksEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Select(x => x.AssessmentTypeModuleProcessID).Count() / assessmentExternalWorksEntryViewModel.assessmentTypeLocationMasterViews.Select(x => x.AssessmentTypeLocationID).Count());
            }
            catch (Exception ex)
            {
                logger.Debug("AssessmentExternalWorksReportPDF :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentExternalWorksEntryViewModel);
        }

        public ActionResult AssessmentRoofConstructionReportPDF(int ID)
        {
            AssessmentRoofConstructionEntryViewModel assessmentRoofConstructionEntryViewModel = new AssessmentRoofConstructionEntryViewModel();
            ViewBag.Title = "ROOF CONSTRUCTION";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentRoofConstructionEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionIndexViewModel = assessmentService.GetAllAssessmentRoofConstruction_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentRoofConstructionEntryViewModel.assessmentTypeLocationMasterViews = new List<AssessmentTypeLocationMasterViewModel>();
                assessmentRoofConstructionEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 4).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentRoofConstructionEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentRoofConstructionEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentRoofConstructionEntryViewModel.moduleAndProcessesList = new List<ModuleAndProcessModel>();
                ModuleAndProcessModel moduleAndProcess = null;
                foreach (AssessmentTypeModuleMasterViewModel mod in assessmentRoofConstructionEntryViewModel.assessmentTypeModuleMasterViewModels)
                {
                    moduleAndProcess = new ModuleAndProcessModel();
                    moduleAndProcess.ModuleNames = mod.AssessmentTypeModuleShortName.Replace("&", "");
                    moduleAndProcess.ProcessIds = assessmentRoofConstructionEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).Select(a => a.AssessmentTypeModuleProcessID.ToString()).ToList();
                    assessmentRoofConstructionEntryViewModel.moduleAndProcessesList.Add(moduleAndProcess);
                    mod.ProcessCount = assessmentRoofConstructionEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID)?.Count();
                }
                assessmentRoofConstructionEntryViewModel.ModuleAndProcess = JsonConvert.SerializeObject(assessmentRoofConstructionEntryViewModel.moduleAndProcessesList);
                assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionTransMasterViewModels = assessmentService.GetAllAssessmentRoofConstruction(ID).ToList();
                if (assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionTransMasterViewModels.Count > 0)
                {
                    List<int> RoofConstructionids = new List<int>();
                    RoofConstructionids = assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionTransMasterViewModels.Select(a => a.AssessmentRFCID).ToList();
                    assessmentRoofConstructionEntryViewModel.assessmentRoofConstructionTransDetailViewModels = assessmentService.GetAllAssessmentRoofConstruction_Detail(RoofConstructionids).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("AssessmentRoofConstructionReportPDF :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentRoofConstructionEntryViewModel);
        }

        public ActionResult AssessmentFieldWindowWTTReportPDF(int ID)
        {
            AssessmentFieldWindowWaterTightnessTestEntryViewModel assessmentFieldWindowWaterTightnessTestEntryViewModel = new AssessmentFieldWindowWaterTightnessTestEntryViewModel();
            ViewBag.Title = "Field Window Water-Tightness Test (WTT) (Third Party Test)";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentFieldWindowWaterTightnessTestEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentWallMasterViewModels = new List<AssessmentWallMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentWindowMasterViewModels = new List<AssessmentWindowMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentJointMasterViewModels = new List<AssessmentJointMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentDirectionMasterViewModels = new List<AssessmentDirectionMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentLeakMasterViewModels = new List<AssessmentLeakMasterViewModel>();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentFieldWindowWaterTightnessTestIndexViewModel = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentFieldWindowWaterTightnessTestEntryViewModel.assessmentFieldWindowWaterTightnessTestTransViewModels = assessmentService.GetAllAssessmentFieldWindowWaterTightnessTest(ID).ToList();
            }
            catch (Exception ex)
            {
                logger.Debug("Field Window WTT Form :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentFieldWindowWaterTightnessTestEntryViewModel);
        }

        public ActionResult AssessmentWetAreaWTTReportPDF(int ID)
        {
            AssessmentWetAreaWaterTightnessTestEntryViewModel assessmentWetAreaWaterTightnessTestEntryViewModel = new AssessmentWetAreaWaterTightnessTestEntryViewModel();
            ViewBag.Title = "Wet Area Water-Tightness Test (WTT) (Third Party Test)";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                assessmentWetAreaWaterTightnessTestEntryViewModel.projectMasterView = assessmentService.GetProject(ID);
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestIndexViewModel = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_List(user.CompanyID).Where(x => x.ProjectID == ID).FirstOrDefault();
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestResultMasterViewModels = assessmentService.GetAllWetAreaWaterTightnessTestResults().Where(x => x.IsActive == 1).OrderBy(x => x.OrderBy).ToList();
                assessmentWetAreaWaterTightnessTestEntryViewModel.ResultCount = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestResultMasterViewModels.Count;
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleMasterViewModels = assessmentService.GetAllModules().Where(x => x.AssessmentTypeID == 6).ToList();
                List<int> Moduleids = new List<int>();
                Moduleids = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleMasterViewModels.Select(a => a.AssessmentTypeModuleID).ToList();
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleProcessMasterViewModels = assessmentService.GetAllModuleProcessByModuleIds(Moduleids).ToList();
                assessmentWetAreaWaterTightnessTestEntryViewModel.moduleAndProcessesList = new List<ModuleAndProcessModel>();
                ModuleAndProcessModel moduleAndProcess = null;
                foreach (AssessmentTypeModuleMasterViewModel mod in assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleMasterViewModels)
                {
                    moduleAndProcess = new ModuleAndProcessModel();
                    moduleAndProcess.ModuleNames = mod.AssessmentTypeModuleShortName.Replace("&", "");
                    moduleAndProcess.ProcessIds = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID).Select(a => a.AssessmentTypeModuleProcessID.ToString()).ToList();
                    assessmentWetAreaWaterTightnessTestEntryViewModel.moduleAndProcessesList.Add(moduleAndProcess);
                    mod.ProcessCount = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentTypeModuleProcessMasterViewModels.Where(x => x.AssessmentTypeModuleID == mod.AssessmentTypeModuleID)?.Count();
                }
                assessmentWetAreaWaterTightnessTestEntryViewModel.ModuleAndProcess = JsonConvert.SerializeObject(assessmentWetAreaWaterTightnessTestEntryViewModel.moduleAndProcessesList);
                assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransMasterViewModels = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest(ID).ToList();
                if (assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransMasterViewModels.Count > 0)
                {
                    List<int> WetAreaWaterTightnessTestids = new List<int>();
                    WetAreaWaterTightnessTestids = assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransMasterViewModels.Select(a => a.AssessmentWAWTTID).ToList();
                    assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransDetailViewModels = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_Detail(WetAreaWaterTightnessTestids).ToList();
                    assessmentWetAreaWaterTightnessTestEntryViewModel.assessmentWetAreaWaterTightnessTestTransDetailResultViewModels = assessmentService.GetAllAssessmentWetAreaWaterTightnessTest_DetailResult(WetAreaWaterTightnessTestids).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Wet Area Water Tightness Test Form :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(assessmentWetAreaWaterTightnessTestEntryViewModel);
        }

        public ActionResult PrintAssessmentReportToPdf(int Id, string filename, string filter)
        {
            ActionAsPdf resultPdf = new ActionAsPdf("AssessmentReportPDF", new { ID = Id, Filter = filter })
            {
                FileName = filename
            };
            return resultPdf;
        }
        #endregion Assessment Report

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}