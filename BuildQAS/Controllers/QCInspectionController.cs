using BuildInspect.Models.Domain;
using BuildInspect.Models.Security;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Utility;
using BuildInspect.Models.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Globalization;

namespace BuildInspect.Controllers
{
    [AccessDeniedAuthorize]
    public class QCInspectionController : SuperBaseController
    {
        private BuildInspectEntities db = new BuildInspectEntities();
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IQCInspectionServices qcInspectionServices;
        private readonly IUserServices userService;

        public QCInspectionController(IQCInspectionServices _qcInspectionServices, IUserServices _userService)
        {
            qcInspectionServices = _qcInspectionServices;
            userService = _userService;
        }

        public ActionResult Index()
        {
            AppSession.SetCurrentMenu("QCInspection", "", "Index");
            AppSession.SetCurrentPage("/QCInspection/Index");
            return View();
        }

        public ActionResult PMIndex()
        {
            AppSession.SetCurrentMenu("PM", "", "Index");
            AppSession.SetCurrentPage("/QCInspection/PMIndex");
            return View();
        }

        #region Project

        // GET: Project
        public ActionResult ProjectIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/ProjectIndex");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Project");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(qcInspectionServices.GetAllProjects().ToList());
            }
            else
            {
                return View(qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).ToList());
            }
        }

        public ActionResult ProjectCreate()
        {
            AppSession.SetCurrentPage("/QCInspection/ProjectCreate");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Project");
            var cpyID = AppSession.GetCompanyId();
            QCInspectionProjectMasterViewModel projectMasterViewModel = new QCInspectionProjectMasterViewModel();
            projectMasterViewModel.CompanyID = cpyID;
            projectMasterViewModel.StartOn = DateTime.Now;
            projectMasterViewModel.EndOn = DateTime.Now.AddMonths(1);
            projectMasterViewModel.Is_Completed = 0;
            ViewBag.ProjectManagersList = userService.getAllUsers().Where(a => a.GroupID == 4 && a.IsActive == 1).ToList();
            ViewBag.SupervisorList = userService.getAllUsers().Where(a => a.GroupID == 5 && a.IsActive == 1).ToList();
            ViewBag.MEInspectorList = userService.getAllUsers().Where(a => a.GroupID == 9 && a.IsActive == 1).ToList();
            ViewBag.StructureInspectorList = userService.getAllUsers().Where(a => a.GroupID == 10 && a.IsActive == 1).ToList();
            ViewBag.OtherInspectorList = userService.getAllUsers().Where(a => a.GroupID == 11 && a.IsActive == 1).ToList();
            projectMasterViewModel.ProjectManagerID = ViewBag.ProjectManagersList[ViewBag.ProjectManagersList.Count - 1].UserID;
            return View(projectMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectCreate(QCInspectionProjectMasterViewModel Project)
        {

            if (qcInspectionServices.CheckProject(Project.ProjectID, Project.Project_Name) == false)
            {
                Project.CompanyID = AppSession.GetCompanyId();
                Project.CreatedBy = AppSession.GetCurrentUserId();
                Project.StartOn = DateTime.ParseExact(Project.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Project.EndOn = DateTime.ParseExact(Project.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Project.Is_Completed = 0;
                Project.CreatedDate = DateTime.Now;

                List<QCInspectionProjectPMDetailViewModel> PMDetails = new List<QCInspectionProjectPMDetailViewModel>();
                foreach (var Id in Project.ProjectManagers.Split(','))
                {
                    PMDetails.Add(new QCInspectionProjectPMDetailViewModel()
                    {
                        UserID = int.Parse(Id.ToString()),
                        CreatedBy = AppSession.GetCurrentUserId(),
                        CreatedDate = DateTime.Now
                    });
                }

                List<QCInspectionProjectSupervisorDetailViewModel> SupervisorDetails = new List<QCInspectionProjectSupervisorDetailViewModel>();
                foreach (var Id in Project.Supervisors.Split(','))
                {
                    SupervisorDetails.Add(new QCInspectionProjectSupervisorDetailViewModel()
                    {
                        UserID = int.Parse(Id.ToString()),
                        CreatedBy = AppSession.GetCurrentUserId(),
                        CreatedDate = DateTime.Now
                    });
                }

                List<QCInspectionProjectMEInspectorDetailViewModel> MEDetails = new List<QCInspectionProjectMEInspectorDetailViewModel>();
                foreach (var Id in Project.MEInspectors.Split(','))
                {
                    MEDetails.Add(new QCInspectionProjectMEInspectorDetailViewModel()
                    {
                        UserID = int.Parse(Id.ToString()),
                        CreatedBy = AppSession.GetCurrentUserId(),
                        CreatedDate = DateTime.Now
                    });
                }

                List<QCInspectionProjectStructureInspectorDetailViewModel> StructureDetails = new List<QCInspectionProjectStructureInspectorDetailViewModel>();
                foreach (var Id in Project.StructureInspectors.Split(','))
                {
                    StructureDetails.Add(new QCInspectionProjectStructureInspectorDetailViewModel()
                    {
                        UserID = int.Parse(Id.ToString()),
                        CreatedBy = AppSession.GetCurrentUserId(),
                        CreatedDate = DateTime.Now
                    });
                }

                List<QCInspectionProjectOtherInspectorDetailViewModel> OtherDetails = new List<QCInspectionProjectOtherInspectorDetailViewModel>();
                foreach (var Id in Project.OtherInspectors.Split(','))
                {
                    OtherDetails.Add(new QCInspectionProjectOtherInspectorDetailViewModel()
                    {
                        UserID = int.Parse(Id.ToString()),
                        CreatedBy = AppSession.GetCurrentUserId(),
                        CreatedDate = DateTime.Now
                    });
                }

                var result = qcInspectionServices.CreateProject(Project, PMDetails, SupervisorDetails, MEDetails, StructureDetails, OtherDetails);
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
            AppSession.SetCurrentPage("/QCInspection/ProjectCreate/" + id.ToString());
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Project");
            List<int> Blockids = new List<int>();
            List<int> Levelids = new List<int>();
            var cpy = qcInspectionServices.GetProject(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            cpy.StartDate = string.Format("{0:dd/MM/yyyy}", cpy.StartOn);
            cpy.EndDate = string.Format("{0:dd/MM/yyyy}", cpy.EndOn);
            cpy.qcinspection_block_master = qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == id).ToList();
            Blockids = cpy.qcinspection_block_master.Select(a => a.BlockID).ToList();
            cpy.qcinspection_level_master = qcInspectionServices.GetAllLevels().Where(x => Blockids.Contains(x.BlockID)).ToList();
            Levelids = cpy.qcinspection_level_master.Select(a => a.LevelID).ToList();
            cpy.qcinspection_unit_master = qcInspectionServices.GetAllUnits().Where(x => Levelids.Contains(x.LevelID)).ToList();
            cpy.Block = new QCInspectionBlockMasterViewModel();
            cpy.Level = new QCInspectionLevelMasterViewModel();
            cpy.Level.BlockList = new List<SelectListItem>();
            cpy.Unit = new QCInspectionUnitMasterViewModel();
            cpy.Unit.LevelList = new List<SelectListItem>();
            ViewBag.ProjectManagersList = userService.getAllUsers().Where(a => a.GroupID == 4 && a.IsActive == 1).ToList();
            ViewBag.SupervisorList = userService.getAllUsers().Where(a => a.GroupID == 5 && a.IsActive == 1).ToList();
            ViewBag.MEInspectorList = userService.getAllUsers().Where(a => a.GroupID == 9 && a.IsActive == 1).ToList();
            ViewBag.StructureInspectorList = userService.getAllUsers().Where(a => a.GroupID == 10 && a.IsActive == 1).ToList();
            ViewBag.OtherInspectorList = userService.getAllUsers().Where(a => a.GroupID == 11 && a.IsActive == 1).ToList();
            ViewBag.ProjectCompleted = cpy.Is_Completed;
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectEdit(QCInspectionProjectMasterViewModel Project)
        {
            Project.StartOn = DateTime.ParseExact(Project.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Project.EndOn = DateTime.ParseExact(Project.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Project.UpdatedBy = AppSession.GetCurrentUserId();
            Project.UpdatedDate = DateTime.Now;

            List<QCInspectionProjectPMDetailViewModel> PMDetails = new List<QCInspectionProjectPMDetailViewModel>();
            foreach (var Id in Project.ProjectManagers.Split(','))
            {
                PMDetails.Add(new QCInspectionProjectPMDetailViewModel()
                {
                    UserID = int.Parse(Id.ToString()),
                    CreatedBy = AppSession.GetCurrentUserId(),
                    CreatedDate = DateTime.Now
                });
            }

            List<QCInspectionProjectSupervisorDetailViewModel> SupervisorDetails = new List<QCInspectionProjectSupervisorDetailViewModel>();
            foreach (var Id in Project.Supervisors.Split(','))
            {
                SupervisorDetails.Add(new QCInspectionProjectSupervisorDetailViewModel()
                {
                    UserID = int.Parse(Id.ToString()),
                    CreatedBy = AppSession.GetCurrentUserId(),
                    CreatedDate = DateTime.Now
                });
            }

            List<QCInspectionProjectMEInspectorDetailViewModel> MEDetails = new List<QCInspectionProjectMEInspectorDetailViewModel>();
            foreach (var Id in Project.MEInspectors.Split(','))
            {
                MEDetails.Add(new QCInspectionProjectMEInspectorDetailViewModel()
                {
                    UserID = int.Parse(Id.ToString()),
                    CreatedBy = AppSession.GetCurrentUserId(),
                    CreatedDate = DateTime.Now
                });
            }

            List<QCInspectionProjectStructureInspectorDetailViewModel> StructureDetails = new List<QCInspectionProjectStructureInspectorDetailViewModel>();
            foreach (var Id in Project.StructureInspectors.Split(','))
            {
                StructureDetails.Add(new QCInspectionProjectStructureInspectorDetailViewModel()
                {
                    UserID = int.Parse(Id.ToString()),
                    CreatedBy = AppSession.GetCurrentUserId(),
                    CreatedDate = DateTime.Now
                });
            }

            List<QCInspectionProjectOtherInspectorDetailViewModel> OtherDetails = new List<QCInspectionProjectOtherInspectorDetailViewModel>();
            foreach (var Id in Project.OtherInspectors.Split(','))
            {
                OtherDetails.Add(new QCInspectionProjectOtherInspectorDetailViewModel()
                {
                    UserID = int.Parse(Id.ToString()),
                    CreatedBy = AppSession.GetCurrentUserId(),
                    CreatedDate = DateTime.Now
                });
            }

            var result = qcInspectionServices.SaveProject(Project, PMDetails, SupervisorDetails, MEDetails, StructureDetails, OtherDetails);
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
            var result = qcInspectionServices.DeleteProject(id);
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
        [HttpPost, ActionName("CompletedProject")]
        [ValidateAntiForgeryToken]
        public ActionResult CompletedProject(QCInspectionProjectMasterViewModel Project)
        {
            var result = qcInspectionServices.CompletedProject(Project.ProjectID);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public ActionResult PartialBlockList(int ProjectID, bool ProjectCompleted = false)
        {
            List<QCInspectionBlockMasterViewModel> blockList = qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == ProjectID).ToList();
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(blockList);
        }

        public ActionResult PartialBlock(int ProjectID, int BlockID = 0, bool ProjectCompleted = false)
        {
            QCInspectionBlockMasterViewModel block = new QCInspectionBlockMasterViewModel();
            if (BlockID > 0)
            {
                block = qcInspectionServices.GetBlock(BlockID);
            }
            else
            {
                block.ProjectID = ProjectID;
                block.OrderBy = 1;
            }
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(block);
        }

        [HttpPost]
        public ActionResult PartialBlock(QCInspectionBlockMasterViewModel BlockMasterViewModel)
        {
            var data = "Error";
            if (qcInspectionServices.CheckBlock(BlockMasterViewModel.ProjectID, BlockMasterViewModel.BlockID, BlockMasterViewModel.BlockName) == false)
            {
                if (BlockMasterViewModel.BlockID > 0)
                {
                    BlockMasterViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                    BlockMasterViewModel.UpdatedDate = DateTime.Now;
                    var result = qcInspectionServices.SaveBlock(BlockMasterViewModel);
                    if (result > 0)
                    {
                        data = "Success:Block Updation Successful!";
                    }
                    else
                    {
                        data = "Block Updation Failed!";
                    }
                }
                else
                {
                    BlockMasterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                    BlockMasterViewModel.CreatedDate = DateTime.Now;

                    var result = qcInspectionServices.CreateBlock(BlockMasterViewModel);
                    if (result > 0)
                    {
                        data = "Success:Block Creation Successful!";
                    }
                    else
                    {
                        data = "Block Creation failed!";
                    }
                }
            }
            else
            {
                data = "Block Name Already exits!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteBlock")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBlock(int id)
        {
            var result = qcInspectionServices.DeleteBlock(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public ActionResult PartialLevelList(int ProjectID, bool ProjectCompleted = false)
        {
            List<int> Blockids = new List<int>();
            Blockids = qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == ProjectID).Select(a => a.BlockID).ToList();
            List<QCInspectionLevelMasterViewModel> levelList = qcInspectionServices.GetAllLevels().Where(x => Blockids.Contains(x.BlockID)).ToList();
            levelList.ToList().ForEach(w => w.BlockName = qcInspectionServices.GetBlock(w.BlockID).BlockName);
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(levelList);
        }

        public ActionResult PartialLevel(int ProjectID, int LevelID = 0, bool ProjectCompleted = false)
        {
            QCInspectionLevelMasterViewModel level = new QCInspectionLevelMasterViewModel();
            if (LevelID > 0)
            {
                level = qcInspectionServices.GetLevel(LevelID);
            }
            else
            {
                level.OrderBy = 1;
            }
            level.BlockList = new List<SelectListItem>();
            foreach (var block in qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == ProjectID).ToList())
            {
                level.BlockList.Add(new SelectListItem() { Text = block.BlockName, Value = block.BlockID.ToString() });
            }
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(level);
        }

        [HttpPost]
        public ActionResult PartialLevel(QCInspectionLevelMasterViewModel LevelMasterViewModel)
        {
            var data = "Error";
            if (qcInspectionServices.CheckLevel(LevelMasterViewModel.BlockID, LevelMasterViewModel.LevelID, LevelMasterViewModel.LevelName) == false)
            {
                if (LevelMasterViewModel.LevelID > 0)
                {
                    LevelMasterViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                    LevelMasterViewModel.UpdatedDate = DateTime.Now;
                    var result = qcInspectionServices.SaveLevel(LevelMasterViewModel);
                    if (result > 0)
                    {
                        data = "Success:Level Updation Successful!";
                    }
                    else
                    {
                        data = "Level Updation Failed!";
                    }
                }
                else
                {
                    LevelMasterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                    LevelMasterViewModel.CreatedDate = DateTime.Now;

                    var result = qcInspectionServices.CreateLevel(LevelMasterViewModel);
                    if (result > 0)
                    {
                        data = "Success:Block Creation Successful!";
                    }
                    else
                    {
                        data = "Block Creation failed!";
                    }
                }
            }
            else
            {
                data = "Level Already exits!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteLevel")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLevel(int id)
        {
            var result = qcInspectionServices.DeleteLevel(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public ActionResult PartialUnitList(int ProjectID, bool ProjectCompleted = false)
        {
            List<int> Blockids = new List<int>();
            List<int> Levelids = new List<int>();
            Blockids = qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == ProjectID).Select(a => a.BlockID).ToList();
            Levelids = qcInspectionServices.GetAllLevels().Where(x => Blockids.Contains(x.BlockID)).Select(a => a.LevelID).ToList();
            List<QCInspectionUnitMasterViewModel> unitList = qcInspectionServices.GetAllUnits().Where(x => Levelids.Contains(x.LevelID)).ToList();
            unitList.ToList().ForEach(w => w.qcinspection_level_master.BlockName = qcInspectionServices.GetBlock(w.qcinspection_level_master.BlockID).BlockName);
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(unitList);
        }

        public ActionResult PartialUnit(int ProjectID, int UnitID = 0, bool ProjectCompleted = false)
        {
            QCInspectionUnitMasterViewModel unit = new QCInspectionUnitMasterViewModel();
            if (UnitID > 0)
            {
                unit = qcInspectionServices.GetUnit(UnitID);
            }
            else
            {
                unit.OrderBy = 1;
            }
            List<int> Blockids = new List<int>();
            List<int> Levelids = new List<int>();
            unit.LevelList = new List<SelectListItem>();
            Blockids = qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == ProjectID).Select(a => a.BlockID).ToList();
            foreach (var level in qcInspectionServices.GetAllLevels().Where(x => Blockids.Contains(x.BlockID)).ToList())
            {
                unit.LevelList.Add(new SelectListItem() { Text = (qcInspectionServices.GetBlock(level.BlockID).BlockName) + "  " + level.LevelName, Value = level.LevelID.ToString() });
            }
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(unit);
        }

        [HttpPost]
        public ActionResult PartialUnit(QCInspectionUnitMasterViewModel UnitMasterViewModel)
        {
            var data = "Error";
            if (qcInspectionServices.CheckUnit(UnitMasterViewModel.LevelID, UnitMasterViewModel.UnitID, UnitMasterViewModel.UnitName) == false)
            {
                if (UnitMasterViewModel.UnitID > 0)
                {
                    UnitMasterViewModel.UpdatedBy = AppSession.GetCurrentUserId();
                    UnitMasterViewModel.UpdatedDate = DateTime.Now;
                    var result = qcInspectionServices.SaveUnit(UnitMasterViewModel);
                    if (result > 0)
                    {
                        data = "Success:Unit Updation Successful!";
                    }
                    else
                    {
                        data = "Unit Updation Failed!";
                    }
                }
                else
                {
                    UnitMasterViewModel.CreatedBy = AppSession.GetCurrentUserId();
                    UnitMasterViewModel.CreatedDate = DateTime.Now;

                    var result = qcInspectionServices.CreateUnit(UnitMasterViewModel);
                    if (result > 0)
                    {
                        data = "Success:Unit Creation Successful!";
                    }
                    else
                    {
                        data = "Unit Creation failed!";
                    }
                }
            }
            else
            {
                data = "Unit Already exits!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteUnit")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUnit(int id)
        {
            var result = qcInspectionServices.DeleteUnit(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public ActionResult PartialRFWIDrawingsReferenceList(int ProjectID, bool ProjectCompleted = false)
        {
            List<QCInspectionProjectRFWIDrawingReferenceFilesViewModel> RFWIDrawingsReferenceList = qcInspectionServices.GetAllRFWIDrawingsReferenceFiles(ProjectID).ToList();
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(RFWIDrawingsReferenceList);
        }

        public ActionResult PartialRFWIDrawingsReference(int ProjectID, bool ProjectCompleted = false)
        {
            QCInspectionProjectRFWIDrawingReferenceFilesViewModel RFWIDrawing = new QCInspectionProjectRFWIDrawingReferenceFilesViewModel();
            RFWIDrawing.ProjectID = ProjectID;
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(RFWIDrawing);
        }

        [HttpPost]
        public ActionResult PartialRFWIDrawingsReference(QCInspectionProjectRFWIDrawingReferenceFilesViewModel RFWIDrawing)
        {
            var data = "Error";
            if (qcInspectionServices.CheckRFWIDrawingsReferenceFile(RFWIDrawing.ProjectID, RFWIDrawing.FileCaption) == false)
            {
                RFWIDrawing.FileName = RFWIDrawing.DrawingReferenceFile.FileName;
                RFWIDrawing.FilePath = Server.MapPath("~/images/RFWIDrawings/") + RFWIDrawing.ProjectID.ToString() + "\\" + RFWIDrawing.DrawingReferenceFile.FileName;
                RFWIDrawing.CreatedBy = AppSession.GetCurrentUserId();
                RFWIDrawing.CreatedDate = DateTime.Now;

                var result = qcInspectionServices.CreateRFWIDrawingsReferenceFile(RFWIDrawing);
                if (result > 0)
                {
                    if (!Directory.Exists(Server.MapPath("~/images/RFWIDrawings/") + RFWIDrawing.ProjectID.ToString()))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/images/RFWIDrawings/") + RFWIDrawing.ProjectID.ToString());
                    }

                    if (RFWIDrawing.DrawingReferenceFile != null)
                    {
                        RFWIDrawing.DrawingReferenceFile.SaveAs(Server.MapPath("~/images/RFWIDrawings/") + RFWIDrawing.ProjectID.ToString() + "/" + RFWIDrawing.DrawingReferenceFile.FileName);
                    }

                    data = "Success:RFWI Drawing File Creation Successful!";
                }
                else
                {
                    data = "RFWI Drawing File Creation failed!";
                }
            }
            else
            {
                data = "File Caption Already exits!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteRFWIDrawingsReference")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRFWIDrawingsReference(int Id)
        {
            var RFWIDrawing = qcInspectionServices.GetRFWIDrawingsReferenceFile(Id);
            var result = qcInspectionServices.DeleteRFWIDrawingsReferenceFile(Id);
            if (result > 0)
            {
                FileInfo file = new FileInfo(Server.MapPath("~/images/RFWIDrawings/") + RFWIDrawing.ProjectID.ToString() + "/" + RFWIDrawing.FileName);
                if (file.Exists)
                {
                    file.Delete();
                }
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }


        public ActionResult PartialProjectFileList(int ProjectID, bool ProjectCompleted = false)
        {
            List<QCInspectionProjectFilesViewModel> ProjectFileList = qcInspectionServices.GetAllProjectFiles(ProjectID).ToList();
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(ProjectFileList);
        }

        public ActionResult PartialProjectFile(int ProjectID, bool ProjectCompleted = false)
        {
            QCInspectionProjectFilesViewModel ProjectFile = new QCInspectionProjectFilesViewModel();
            ProjectFile.ProjectID = ProjectID;
            ViewBag.ProjectCompleted = (ProjectCompleted == true ? 1 : 0);
            return PartialView(ProjectFile);
        }

        [HttpPost]
        public ActionResult PartialProjectFile(QCInspectionProjectFilesViewModel ProjectFile)
        {
            var data = "Error";
            if (qcInspectionServices.CheckProjectFile(ProjectFile.ProjectID, ProjectFile.FileCaption) == false)
            {
                ProjectFile.FileName = ProjectFile.PlanOrDocumentFile.FileName;
                ProjectFile.FilePath = Server.MapPath("~/images/ProjectFiles/") + ProjectFile.ProjectID.ToString() + "\\" + ProjectFile.PlanOrDocumentFile.FileName;
                ProjectFile.CreatedBy = AppSession.GetCurrentUserId();
                ProjectFile.CreatedDate = DateTime.Now;

                var result = qcInspectionServices.CreateProjectFile(ProjectFile);
                if (result > 0)
                {
                    if (!Directory.Exists(Server.MapPath("~/images/ProjectFiles/") + ProjectFile.ProjectID.ToString()))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/images/ProjectFiles/") + ProjectFile.ProjectID.ToString());
                    }

                    if (ProjectFile.PlanOrDocumentFile != null)
                    {
                        ProjectFile.PlanOrDocumentFile.SaveAs(Server.MapPath("~/images/ProjectFiles/") + ProjectFile.ProjectID.ToString() + "/" + ProjectFile.PlanOrDocumentFile.FileName);
                    }

                    data = "Success:Project File Creation Successful!";
                }
                else
                {
                    data = "Proejct File Creation failed!";
                }
            }
            else
            {
                data = "File Caption Already exits!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteProjectFile")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProjectFile(int Id)
        {
            var ProjectFile = qcInspectionServices.GetProjectFile(Id);
            var result = qcInspectionServices.DeleteProjectFile(Id);
            if (result > 0)
            {
                FileInfo file = new FileInfo(Server.MapPath("~/images/ProjectFiles/") + ProjectFile.ProjectID.ToString() + "/" + ProjectFile.FileName);
                if (file.Exists)
                {
                    file.Delete();
                }
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region Trade

        // GET: Trade
        public ActionResult TradeIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/TradeIndex");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Trade");
            return View(qcInspectionServices.GetAllTrades().ToList());
        }

        public ActionResult TradeCreate()
        {
            AppSession.SetCurrentPage("/QCInspection/TradeCreate");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Trade");
            QCInspectionTradeMasterViewModel TradeMasterViewTrade = new QCInspectionTradeMasterViewModel();
            return View(TradeMasterViewTrade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TradeCreate(QCInspectionTradeMasterViewModel Trade)
        {
            if (ModelState.IsValid)
            {
                if (qcInspectionServices.CheckTrade(Trade.TradeID, Trade.TradeName) == false)
                {
                    Trade.CreatedBy = AppSession.GetCurrentUserId();
                    Trade.CreatedDate = DateTime.Now;
                    Trade.IsActive = 1;
                    var result = qcInspectionServices.CreateTrade(Trade);
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
                    return getFailedOperation("Trade Name Already exits!");
                }
            }
            return View(Trade);
        }

        public ActionResult TradeEdit(int? id)
        {
            AppSession.SetCurrentPage("/QCInspection/TradeEdit/" + id.ToString());
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Trade");
            var cpy = qcInspectionServices.GetTrade(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TradeEdit(QCInspectionTradeMasterViewModel Trade)
        {
            if (ModelState.IsValid)
            {
                if (qcInspectionServices.CheckTrade(Trade.TradeID, Trade.TradeName) == false)
                {
                    Trade.UpdatedBy = AppSession.GetCurrentUserId();
                    Trade.UpdatedDate = DateTime.Now;
                    var result = qcInspectionServices.SaveTrade(Trade);
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
                    return getFailedOperation("Trade Name Already exits!");
                }
            }
            return View(Trade);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteTrade")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTrade(int id)
        {
            var result = qcInspectionServices.DeleteTrade(id);
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

        #region Defect Type

        // GET: DefectType
        public ActionResult DefectTypeIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/DefectTypeIndex");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Defect Type");
            return View(qcInspectionServices.GetAllDefectTypes().ToList());
        }

        public ActionResult DefectTypeCreate()
        {
            AppSession.SetCurrentPage("/QCInspection/DefectTypeCreate");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Defect Type");
            QCInspectionDefectTypeMasterViewModel DefectTypeMasterViewDefectType = new QCInspectionDefectTypeMasterViewModel();
            return View(DefectTypeMasterViewDefectType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DefectTypeCreate(QCInspectionDefectTypeMasterViewModel DefectType)
        {
            if (ModelState.IsValid)
            {
                if (qcInspectionServices.CheckDefectType(DefectType.DefectTypeID, DefectType.DefectName) == false)
                {
                    DefectType.CreatedBy = AppSession.GetCurrentUserId();
                    DefectType.CreatedDate = DateTime.Now;
                    DefectType.IsActive = 1;
                    var result = qcInspectionServices.CreateDefectType(DefectType);
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
                    return getFailedOperation("Defect Type Name Already exits!");
                }
            }
            return View(DefectType);
        }

        public ActionResult DefectTypeEdit(int? id)
        {
            AppSession.SetCurrentPage("/QCInspection/DefectTypeEdit/" + id.ToString());
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Defect Type");
            var cpy = qcInspectionServices.GetDefectType(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DefectTypeEdit(QCInspectionDefectTypeMasterViewModel DefectType)
        {
            if (ModelState.IsValid)
            {
                DefectType.UpdatedBy = AppSession.GetCurrentUserId();
                DefectType.UpdatedDate = DateTime.Now;
                var result = qcInspectionServices.SaveDefectType(DefectType);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(DefectType);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteDefectType")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDefectType(int id)
        {
            var result = qcInspectionServices.DeleteDefectType(id);
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

        #region Subcontractor
        // GET: Subcontractor
        public ActionResult SubcontractorIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/SubcontractorIndex");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Subcontractor");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(qcInspectionServices.GetAllSubcontractors().ToList());
            }
            else
            {
                return View(qcInspectionServices.GetAllSubcontractors().Where(a => a.CompanyID == cid).ToList());
            }
        }

        public ActionResult SubcontractorCreate()
        {
            AppSession.SetCurrentPage("/QCInspection/SubcontractorCreate");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Subcontractor");
            ViewBag.TradeList = qcInspectionServices.GetAllTrades().ToList();
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            QCInspectionSubcontractorMasterViewModel SubcontractorMasterViewSubcontractor = new QCInspectionSubcontractorMasterViewModel();
            SubcontractorMasterViewSubcontractor.CompanyID = cid;
            var users = userService.getAllUsers().Where(a => a.GroupID == 6 && a.SubCon_ID == null).ToList();
            users.Insert(0, new UserViewModel() { UserID = 0, DisplayName = "-Select-" });
            ViewBag.SubCon_ID = new SelectList(users, "UserID", "DisplayName", SubcontractorMasterViewSubcontractor.SubCon_ID);
            return View(SubcontractorMasterViewSubcontractor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubcontractorCreate(QCInspectionSubcontractorMasterViewModel Subcontractor)
        {
            if (ModelState.IsValid)
            {
                if (qcInspectionServices.CheckSubcontractor(Subcontractor.CompanyID, Subcontractor.SubcontractorID, Subcontractor.Name) == false)
                {
                    Subcontractor.CreatedBy = AppSession.GetCurrentUserId();
                    Subcontractor.CreatedDate = DateTime.Now;
                    Subcontractor.IsActive = 1;

                    List<QCInspectionSubcontractorTradeDetailViewModel> detailViewModels = new List<QCInspectionSubcontractorTradeDetailViewModel>();
                    foreach (var Id in Subcontractor.Trades.Split(','))
                    {
                        detailViewModels.Add(new QCInspectionSubcontractorTradeDetailViewModel()
                        {
                            SubcontractorID = 0,
                            TradeID = int.Parse(Id.ToString()),
                            CreatedBy = AppSession.GetCurrentUserId(),
                            CreatedDate = DateTime.Now
                        });
                    }

                    var result = qcInspectionServices.CreateSubcontractor(Subcontractor, detailViewModels);
                    if (result > 0)
                    {
                        // Existing Subcontractor User
                        var user = userService.getAllUsers().Where(x => x.SubCon_ID == result).FirstOrDefault();
                        if (user != null)
                        {
                            user.SubCon_ID = null;
                            user.UpdatedBy = AppSession.GetCurrentUserId();
                            user.UpdatedDate = DateTime.Now;
                            result = userService.SaveUser(user);
                        }
                        // Existing Subcontractor User
                        if (Subcontractor.SubCon_ID > 0)
                        {
                            user = userService.GetUser(Subcontractor.SubCon_ID);
                            if (user != null)
                            {
                                user.SubCon_ID = result;
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
                    return getFailedOperation("Subcontractor Name Already exits!");
                }
            }
            return View(Subcontractor);
        }

        public ActionResult SubcontractorEdit(int? id)
        {
            AppSession.SetCurrentPage("/QCInspection/SubcontractorEdit/" + id.ToString());
            AppSession.SetCurrentMenu("QCInspection", "Masters", "Subcontractor");
            var cpy = qcInspectionServices.GetSubcontractor(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            // Existing Subcontractor User
            var user = userService.getAllUsers().Where(x => x.SubCon_ID == cpy.SubcontractorID).FirstOrDefault();
            if (user != null)
            {
                cpy.SubCon_ID = user.UserID;
                cpy.User_Name = user.DisplayName;
            }
            // Existing Subcontractor User
            //var users = userService.getAllUsers().Where(a => a.GroupID == 6).ToList();
            //users.Insert(0, new UserViewModel() { UserID = 0, DisplayName = "-Select-" });
            //ViewBag.SubCon_ID = new SelectList(users, "UserID", "DisplayName", cpy.SubCon_ID);
            ViewBag.TradeList = qcInspectionServices.GetAllTrades().ToList();
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubcontractorEdit(QCInspectionSubcontractorMasterViewModel Subcontractor)
        {
            if (ModelState.IsValid)
            {
                Subcontractor.UpdatedBy = AppSession.GetCurrentUserId();
                Subcontractor.UpdatedDate = DateTime.Now;

                List<QCInspectionSubcontractorTradeDetailViewModel> detailViewModels = new List<QCInspectionSubcontractorTradeDetailViewModel>();
                foreach (var Id in Subcontractor.Trades.Split(','))
                {
                    detailViewModels.Add(new QCInspectionSubcontractorTradeDetailViewModel()
                    {
                        TradeID = int.Parse(Id.ToString()),
                        CreatedBy = AppSession.GetCurrentUserId(),
                        CreatedDate = DateTime.Now
                    });
                }
                var result = qcInspectionServices.SaveSubcontractor(Subcontractor, detailViewModels);
                if (result > 0)
                {
                    //// Existing Subcontractor User
                    //var user = userService.getAllUsers().Where(x => x.SubCon_ID == Subcontractor.SubcontractorID).FirstOrDefault();
                    //if (user != null)
                    //{
                    //    user.SubCon_ID = null;
                    //    user.UpdatedBy = AppSession.GetCurrentUserId();
                    //    user.UpdatedDate = DateTime.Now;
                    //    result = userService.SaveUser(user);
                    //}
                    //// Existing Subcontractor User
                    //if (Subcontractor.SubCon_ID > 0)
                    //{
                    //    user = userService.GetUser(Subcontractor.SubCon_ID);
                    //    if (user != null)
                    //    {
                    //        user.SubCon_ID = Subcontractor.SubcontractorID;
                    //        user.UpdatedBy = AppSession.GetCurrentUserId();
                    //        user.UpdatedDate = DateTime.Now;
                    //        result = userService.SaveUser(user);
                    //    }
                    //}
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(Subcontractor);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteSubcontractor")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSubcontractor(int id)
        {
            var result = qcInspectionServices.DeleteSubcontractor(id);
            if (result > 0)
            {
                // Existing Subcontractor User
                var user = userService.getAllUsers().Where(x => x.SubCon_ID == id).FirstOrDefault();
                if (user != null)
                {
                    user.SubCon_ID = null;
                    user.UpdatedBy = AppSession.GetCurrentUserId();
                    user.UpdatedDate = DateTime.Now;
                    result = userService.SaveUser(user);
                }
                // Existing Subcontractor User
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion

        #region RFWI General Check List

        // GET: RFWIGeneralCheckList
        public ActionResult RFWIGeneralCheckListIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/RFWIGeneralCheckListIndex");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "RFWI General Check List");
            return View(qcInspectionServices.GetAllRFWIGeneralCheckLists().ToList());
        }

        public ActionResult RFWIGeneralCheckListCreate()
        {
            AppSession.SetCurrentPage("/QCInspection/RFWIGeneralCheckListCreate");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "RFWI General Check List");
            QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckListMasterViewRFWIGeneralCheckList = new QCInspectionRFWIGeneralCheckListMasterViewModel();
            return View(RFWIGeneralCheckListMasterViewRFWIGeneralCheckList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIGeneralCheckListCreate(QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckList)
        {
            if (ModelState.IsValid)
            {
                if (qcInspectionServices.CheckRFWIGeneralCheckList(RFWIGeneralCheckList.GeneralCheckListID, RFWIGeneralCheckList.GeneralCheckListName) == false)
                {
                    RFWIGeneralCheckList.CreatedBy = AppSession.GetCurrentUserId();
                    RFWIGeneralCheckList.CreatedDate = DateTime.Now;
                    RFWIGeneralCheckList.IsActive = 1;
                    var result = qcInspectionServices.CreateRFWIGeneralCheckList(RFWIGeneralCheckList);
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
                    return getFailedOperation("RFWI General Check List Name Already exits!");
                }
            }
            return View(RFWIGeneralCheckList);
        }

        public ActionResult RFWIGeneralCheckListEdit(int? id)
        {
            AppSession.SetCurrentPage("/QCInspection/RFWIGeneralCheckListEdit/" + id.ToString());
            AppSession.SetCurrentMenu("QCInspection", "Masters", "RFWI General Check List");
            var cpy = qcInspectionServices.GetRFWIGeneralCheckList(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIGeneralCheckListEdit(QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckList)
        {
            if (ModelState.IsValid)
            {
                RFWIGeneralCheckList.UpdatedBy = AppSession.GetCurrentUserId();
                RFWIGeneralCheckList.UpdatedDate = DateTime.Now;
                var result = qcInspectionServices.SaveRFWIGeneralCheckList(RFWIGeneralCheckList);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(RFWIGeneralCheckList);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteRFWIGeneralCheckList")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRFWIGeneralCheckList(int id)
        {
            var result = qcInspectionServices.DeleteRFWIGeneralCheckList(id);
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

        #region RFWI Trade
        // GET: Project
        public ActionResult RFWITradeIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/RFWITradeIndex");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "RFWI Trade");
            return View(qcInspectionServices.GetAllRFWITrades().ToList());
        }

        public ActionResult RFWITradeCreate()
        {
            AppSession.SetCurrentPage("/QCInspection/RFWITradeCreate");
            AppSession.SetCurrentMenu("QCInspection", "Masters", "RFWI Trade");
            var cpyID = AppSession.GetCompanyId();
            QCInspectionRFWITradeMasterViewModel RFWITradeMasterViewModel = new QCInspectionRFWITradeMasterViewModel();
            return View(RFWITradeMasterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWITradeCreate(QCInspectionRFWITradeMasterViewModel RFWITrade)
        {
            if (ModelState.IsValid)
            {
                if (qcInspectionServices.CheckRFWITrade(RFWITrade.TradeID, RFWITrade.TradeName) == false)
                {
                    RFWITrade.CreatedBy = AppSession.GetCurrentUserId();
                    RFWITrade.CreatedDate = DateTime.Now;

                    var result = qcInspectionServices.CreateRFWITrade(RFWITrade);
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
                    return getFailedOperation("RFWI Trade Name Already exits!");
                }
            }
            return View(RFWITrade);
        }

        public ActionResult RFWITradeEdit(int Id)
        {
            AppSession.SetCurrentPage("/QCInspection/RFWITradeEdit/" + Id.ToString());
            AppSession.SetCurrentMenu("QCInspection", "Masters", "RFWI Trade");
            var cpy = qcInspectionServices.GetRFWITrade(Id);
            if (cpy == null)
            {
                return HttpNotFound();
            }
            cpy.qcinspection_rfwi_trade_item_detail = qcInspectionServices.GetAllRFWITradeItems().Where(x => x.TradeID == Id).ToList();
            cpy.qcinspection_rfwi_trade_detailed_checklist_detail = qcInspectionServices.GetAllRFWITradeDetailedCheckLists().Where(x => x.TradeID == Id).ToList();
            cpy.RFWITradeItemDetail = new QCInspectionRFWITradeItemDetailViewModel();
            cpy.RFWITradeDetailedCheckListDetail = new QCInspectionRFWITradeDetailedCheckListDetailViewModel();
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWITradeEdit(QCInspectionRFWITradeMasterViewModel RFWITrade)
        {
            if (ModelState.IsValid)
            {
                RFWITrade.UpdatedBy = AppSession.GetCurrentUserId();
                RFWITrade.UpdatedDate = DateTime.Now;
                var result = qcInspectionServices.SaveRFWITrade(RFWITrade);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(RFWITrade);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteRFWITrade")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRFWITrade(int id)
        {
            var result = qcInspectionServices.DeleteRFWITrade(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public ActionResult PartialRFWITradeItemDetailList(int TradeID)
        {
            List<QCInspectionRFWITradeItemDetailViewModel> RFWITradeItemDetailViewModels = qcInspectionServices.GetAllRFWITradeItems().Where(x => x.TradeID == TradeID).ToList();
            return PartialView(RFWITradeItemDetailViewModels);
        }

        public ActionResult PartialRFWITradeItemDetail(int TradeID, int ItemID = 0)
        {
            QCInspectionRFWITradeItemDetailViewModel RFWITradeItemDetail = new QCInspectionRFWITradeItemDetailViewModel();
            if (ItemID > 0)
            {
                RFWITradeItemDetail = qcInspectionServices.GetRFWITradeItem(ItemID);
            }
            else
            {
                RFWITradeItemDetail.TradeID = TradeID;
                RFWITradeItemDetail.OrderBy = 1;
            }
            return PartialView(RFWITradeItemDetail);
        }

        [HttpPost]
        public ActionResult PartialRFWITradeItemDetail(QCInspectionRFWITradeItemDetailViewModel RFWITradeItemDetail)
        {
            var data = "Error";
            if (qcInspectionServices.CheckRFWITradeItem(RFWITradeItemDetail.TradeID, RFWITradeItemDetail.TradeItemID, RFWITradeItemDetail.ItemName) == false)
            {
                if (RFWITradeItemDetail.TradeItemID > 0)
                {
                    RFWITradeItemDetail.UpdatedBy = AppSession.GetCurrentUserId();
                    RFWITradeItemDetail.UpdatedDate = DateTime.Now;
                    var result = qcInspectionServices.SaveRFWITradeItem(RFWITradeItemDetail);
                    if (result > 0)
                    {
                        data = "Success:RFWI Trade Item Updation Successful!";
                    }
                    else
                    {
                        data = "RFWI Trade Item Updation Failed!";
                    }
                }
                else
                {
                    RFWITradeItemDetail.CreatedBy = AppSession.GetCurrentUserId();
                    RFWITradeItemDetail.CreatedDate = DateTime.Now;

                    var result = qcInspectionServices.CreateRFWITradeItem(RFWITradeItemDetail);
                    if (result > 0)
                    {
                        data = "Success:RFWI Trade Item Creation Successful!";
                    }
                    else
                    {
                        data = "RFWI Trade Item Creation failed!";
                    }
                }
            }
            else
            {
                data = "RFWI Trade Item Name Already exits!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteRFWITradeItemDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRFWITradeItemDetail(int id)
        {
            var result = qcInspectionServices.DeleteRFWITradeItem(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public ActionResult PartialRFWITradeDetailedCheckListDetailList(int TradeID)
        {
            List<QCInspectionRFWITradeDetailedCheckListDetailViewModel> RFWITradeDetailedCheckListDetailViewModels = qcInspectionServices.GetAllRFWITradeDetailedCheckLists().Where(x => x.TradeID == TradeID).ToList();
            return PartialView(RFWITradeDetailedCheckListDetailViewModels);
        }

        public ActionResult PartialRFWITradeDetailedCheckListDetail(int TradeID, int DetailedCheckListID = 0)
        {
            QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeDetailedCheckListDetail = new QCInspectionRFWITradeDetailedCheckListDetailViewModel();
            if (DetailedCheckListID > 0)
            {
                RFWITradeDetailedCheckListDetail = qcInspectionServices.GetRFWITradeDetailedCheckList(DetailedCheckListID);
            }
            else
            {
                RFWITradeDetailedCheckListDetail.TradeID = TradeID;
            }
            return PartialView(RFWITradeDetailedCheckListDetail);
        }

        [HttpPost]
        public ActionResult PartialRFWITradeDetailedCheckListDetail(QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeDetailedCheckListDetail)
        {
            var data = "Error";
            if (qcInspectionServices.CheckRFWITradeDetailedCheckList(RFWITradeDetailedCheckListDetail.TradeID, RFWITradeDetailedCheckListDetail.TradeDetailedCheckListID, RFWITradeDetailedCheckListDetail.DetailedCheckListName) == false)
            {
                if (RFWITradeDetailedCheckListDetail.TradeDetailedCheckListID > 0)
                {
                    RFWITradeDetailedCheckListDetail.UpdatedBy = AppSession.GetCurrentUserId();
                    RFWITradeDetailedCheckListDetail.UpdatedDate = DateTime.Now;
                    var result = qcInspectionServices.SaveRFWITradeDetailedCheckList(RFWITradeDetailedCheckListDetail);
                    if (result > 0)
                    {
                        data = "Success:RFWI Trade Detailed Check List Updation Successful!";
                    }
                    else
                    {
                        data = "RFWI Trade Detailed Check List Updation Failed!";
                    }
                }
                else
                {
                    RFWITradeDetailedCheckListDetail.CreatedBy = AppSession.GetCurrentUserId();
                    RFWITradeDetailedCheckListDetail.CreatedDate = DateTime.Now;

                    var result = qcInspectionServices.CreateRFWITradeDetailedCheckList(RFWITradeDetailedCheckListDetail);
                    if (result > 0)
                    {
                        data = "Success:RFWI Trade Detailed Check List Creation Successful!";
                    }
                    else
                    {
                        data = "RFWI Trade Detailed Check List Creation failed!";
                    }
                }
            }
            else
            {
                data = "RFWI Trade Detailed Check List Name Already exits!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteRFWITradeDetailedCheckListDetail")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRFWITradeDetailedCheckListDetail(int id)
        {
            var result = qcInspectionServices.DeleteRFWITradeDetailedCheckList(id);
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

        #region QC Inspection Form
        public ActionResult QCInspectionIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/QCInspectionIndex");
            AppSession.SetCurrentMenu("QCInspection", "", "QC Inspection");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            List<QCInspectionDefectFormViewModel> qCInspectionDefectFormViewModels = new List<QCInspectionDefectFormViewModel>();
            if (gid == 1)
            {
                qCInspectionDefectFormViewModels = qcInspectionServices.GetAllQCInspectionDefectForms().ToList();
            }
            else if (gid == 4)
            {
                qCInspectionDefectFormViewModels = qcInspectionServices.GetAllQCInspectionDefectForms().Where(x => x.ProjectManagerID == uid).ToList();
            }
            else if (gid == 5)
            {
                qCInspectionDefectFormViewModels = qcInspectionServices.GetAllQCInspectionDefectFormsBasedUserID(uid).ToList();
            }
            else if (gid == 6)
            {
                int subid = Convert.ToInt32(AppSession.GetUserDetail().SubCon_ID);
                qCInspectionDefectFormViewModels = qcInspectionServices.GetAllQCInspectionDefectFormsBasedSubcontractorID(subid).ToList();
            }
            else
            {
                qCInspectionDefectFormViewModels = qcInspectionServices.GetAllQCInspectionDefectForms().ToList();
            }

            List<int> Projectids = new List<int>();
            Projectids = qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).Select(a => a.ProjectID).ToList();
            qCInspectionDefectFormViewModels = qCInspectionDefectFormViewModels.Where(x => Projectids.Contains(x.ProjectID)).ToList();
            //qcinspection_unit_master.qcinspection_level_master.qcinspection_block_master.BlockName @item.qcinspection_unit_master.qcinspection_level_master.LevelName - @item.qcinspection_unit_master.UnitName
            qCInspectionDefectFormViewModels.ToList().ForEach(w => w.ProjectName = qcInspectionServices.GetProject(w.ProjectID).Project_Name);
            qCInspectionDefectFormViewModels.ToList().ForEach(w => w.LocationName = qcInspectionServices.GetBlock(w.qcinspection_unit_master.qcinspection_level_master.BlockID).BlockName + " " + w.qcinspection_unit_master.qcinspection_level_master.LevelName + "-" + w.qcinspection_unit_master.UnitName);
            return View(qCInspectionDefectFormViewModels);
        }

        public ActionResult PartialQCInspectionUnitList(int ProjectID)
        {
            QCInspectionDefectFormViewModel qCInspectionDefectFormViewModel = new QCInspectionDefectFormViewModel();
            qCInspectionDefectFormViewModel.UnitList = new List<SelectListItem>();
            foreach (var block in qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == ProjectID).OrderBy(x => x.OrderBy).ToList())
            {
                foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                {
                    foreach (var unit in qcInspectionServices.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                    {
                        qCInspectionDefectFormViewModel.UnitList.Add(new SelectListItem() { Text = block.BlockName + " " + level.LevelName + "-" + unit.UnitName, Value = unit.UnitID.ToString() });
                    }
                }
            }
            return PartialView(qCInspectionDefectFormViewModel);
        }

        public ActionResult PartialQCInspectionSubcontractorList(int TradeID)
        {
            var cid = AppSession.GetCompanyId();
            QCInspectionDefectFormViewModel qCInspectionDefectFormViewModel = new QCInspectionDefectFormViewModel();
            qCInspectionDefectFormViewModel.SubcontractorList = new List<SelectListItem>();
            foreach (var subcon in qcInspectionServices.GetAllSubcontractors().Where(x => x.CompanyID == cid && x.qcinspection_subcontractor_trade_detail.Where(t => t.TradeID == TradeID).Count() > 0).OrderBy(x => x.Name).ToList())
            {
                qCInspectionDefectFormViewModel.SubcontractorList.Add(new SelectListItem() { Text = subcon.Name, Value = subcon.SubcontractorID.ToString() });
            }
            return PartialView(qCInspectionDefectFormViewModel);
        }

        public ActionResult PartialQCInspectionProjectManagerList(int ProjectID)
        {
            QCInspectionDefectFormViewModel qCInspectionDefectFormViewModel = new QCInspectionDefectFormViewModel();
            qCInspectionDefectFormViewModel.ProjectManagerList = new List<SelectListItem>();
            foreach (var PMName in qcInspectionServices.GetAllPMDetailByProjectID(ProjectID).OrderBy(x => x.user.DisplayName).ToList())
            {
                qCInspectionDefectFormViewModel.ProjectManagerList.Add(new SelectListItem() { Text = PMName.user.DisplayName, Value = PMName.UserID.ToString() });
            }
            qCInspectionDefectFormViewModel.ProjectManagerID = (int)qcInspectionServices.GetProject(ProjectID)?.ProjectManagerID;
            return PartialView(qCInspectionDefectFormViewModel);
        }

        public ActionResult QCInspectionCreate()
        {
            AppSession.SetCurrentPage("/QCInspection/QCInspectionCreate");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            QCInspectionDefectFormViewModel qCInspectionDefectFormViewModel = new QCInspectionDefectFormViewModel();
            qCInspectionDefectFormViewModel.ProjectList = new List<SelectListItem>();
            foreach (var objPro in qcInspectionServices.GetAllProjectsBasedCompanyID(cid).Where(x => x.Is_Completed == 0).OrderBy(x => x.Project_Name).ToList())
            {
                if (gid == 5 && objPro.qcinspection_project_Supervisor_detail.Where(x => x.UserID == uid).Count() > 0)
                {
                    qCInspectionDefectFormViewModel.ProjectList.Add(new SelectListItem() { Text = objPro.Project_Name, Value = objPro.ProjectID.ToString() });
                }
            }
            var pid = int.Parse(qCInspectionDefectFormViewModel.ProjectList.FirstOrDefault().Value);
            qCInspectionDefectFormViewModel.UnitList = new List<SelectListItem>();
            foreach (var block in qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == pid).OrderBy(x => x.OrderBy).ToList())
            {
                foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                {
                    foreach (var unit in qcInspectionServices.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                    {
                        qCInspectionDefectFormViewModel.UnitList.Add(new SelectListItem() { Text = block.BlockName + " " + level.LevelName + "-" + unit.UnitName, Value = unit.UnitID.ToString() });
                    }
                }
            }
            qCInspectionDefectFormViewModel.DefectTypeList = new List<SelectListItem>();
            foreach (var DType in qcInspectionServices.GetAllDefectTypes().ToList())
            {
                qCInspectionDefectFormViewModel.DefectTypeList.Add(new SelectListItem() { Text = DType.DefectName, Value = DType.DefectTypeID.ToString() });
            }
            qCInspectionDefectFormViewModel.TradeList = new List<SelectListItem>();
            foreach (var trade in qcInspectionServices.GetAllTrades().ToList())
            {
                qCInspectionDefectFormViewModel.TradeList.Add(new SelectListItem() { Text = trade.TradeName, Value = trade.TradeID.ToString() });
            }
            var tid = int.Parse(qCInspectionDefectFormViewModel.TradeList.FirstOrDefault().Value);
            qCInspectionDefectFormViewModel.SubcontractorList = new List<SelectListItem>();
            foreach (var subcon in qcInspectionServices.GetAllSubcontractors().Where(x => x.CompanyID == cid && x.qcinspection_subcontractor_trade_detail.Where(t => t.TradeID == tid).Count() > 0).OrderBy(x => x.Name).ToList())
            {
                qCInspectionDefectFormViewModel.SubcontractorList.Add(new SelectListItem() { Text = subcon.Name, Value = subcon.SubcontractorID.ToString() });
            }
            qCInspectionDefectFormViewModel.ProjectManagerList = new List<SelectListItem>();
            foreach (var PMName in qcInspectionServices.GetAllPMDetailByProjectID(pid).OrderBy(x => x.user.DisplayName).ToList())
            {
                qCInspectionDefectFormViewModel.ProjectManagerList.Add(new SelectListItem() { Text = PMName.user.DisplayName, Value = PMName.UserID.ToString() });
            }
            qCInspectionDefectFormViewModel.ProjectManagerID = (int)qcInspectionServices.GetProject(pid)?.ProjectManagerID;
            qCInspectionDefectFormViewModel.CreatedDate = DateTime.Now;
            return View(qCInspectionDefectFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionCreate(QCInspectionDefectFormViewModel QCInspection)
        {
            if (ModelState.IsValid)
            {
                QCInspection.Remarks = "<==Created on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                QCInspection.CreatedBy = AppSession.GetCurrentUserId();
                QCInspection.CreatedDate = DateTime.Now;
                QCInspection.FilePath = Server.MapPath("~/images/QCInspection/");
                if (QCInspection.Status == "Approved")
                {
                    QCInspection.ApprovedBy = AppSession.GetCurrentUserId();
                    QCInspection.ApprovedDate = DateTime.Now;
                    QCInspection.Status = "Approved";
                    QCInspection.Remarks += "<==Approved on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " By " + AppSession.GetCurrentUserName() + "==>";
                }
                var result = qcInspectionServices.CreateQCInspectionDefectForm(QCInspection);
                if (result > 0)
                {
                    if (!Directory.Exists(Server.MapPath("~/images/QCInspection/") + result.ToString() + "/Defect"))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + result.ToString() + "/Defect");
                        Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + result.ToString() + "/Rectify");
                    }

                    foreach (var fileBase in QCInspection.DefectFiles)
                    {
                        if (fileBase != null)
                        {
                            fileBase.SaveAs(Server.MapPath("~/images/QCInspection/") + result.ToString() + "/Defect/" + fileBase.FileName);
                        }
                    }
                    return Json(new
                    {
                        value = string.Format("{0}", result)
                    });
                }
                else
                {
                    return getFailedOperation();
                }
            }

            return View(QCInspection);
        }

        public ActionResult QCInspectionEdit(int Id)
        {
            AppSession.SetCurrentPage("/QCInspection/QCInspectionEdit/" + Id.ToString());
            var QCInspection = qcInspectionServices.GetQCInspectionDefectForm(Id);
            if (QCInspection == null)
            {
                return HttpNotFound();
            }
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            var pid = QCInspection.ProjectID;
            QCInspection.ProjectList = new List<SelectListItem>();
            foreach (var objPro in qcInspectionServices.GetAllProjectsBasedCompanyID(cid).Where(x => x.Is_Completed == 0 || x.ProjectID == pid).OrderBy(x => x.Project_Name).ToList())
            {
                if (gid == 5 && objPro.qcinspection_project_Supervisor_detail.Where(x => x.UserID == uid).Count() > 0)
                {
                    QCInspection.ProjectList.Add(new SelectListItem() { Text = objPro.Project_Name, Value = objPro.ProjectID.ToString() });
                }
                else if (gid == 4 && objPro.qcinspection_project_PM_detail.Where(x => x.UserID == uid).Count() > 0)
                {
                    QCInspection.ProjectList.Add(new SelectListItem() { Text = objPro.Project_Name, Value = objPro.ProjectID.ToString() });
                }
                else if (QCInspection.ProjectID == objPro.ProjectID)
                {
                    QCInspection.ProjectList.Add(new SelectListItem() { Text = objPro.Project_Name, Value = objPro.ProjectID.ToString() });
                }
            }

            QCInspection.UnitList = new List<SelectListItem>();
            foreach (var block in qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == pid).OrderBy(x => x.OrderBy).ToList())
            {
                foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                {
                    foreach (var unit in qcInspectionServices.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                    {
                        QCInspection.UnitList.Add(new SelectListItem() { Text = block.BlockName + " " + level.LevelName + "-" + unit.UnitName, Value = unit.UnitID.ToString() });
                    }
                }
            }
            QCInspection.DefectTypeList = new List<SelectListItem>();
            foreach (var DType in qcInspectionServices.GetAllDefectTypes().ToList())
            {
                QCInspection.DefectTypeList.Add(new SelectListItem() { Text = DType.DefectName, Value = DType.DefectTypeID.ToString() });
            }
            QCInspection.TradeList = new List<SelectListItem>();
            foreach (var trade in qcInspectionServices.GetAllTrades().ToList())
            {
                QCInspection.TradeList.Add(new SelectListItem() { Text = trade.TradeName, Value = trade.TradeID.ToString() });
            }
            var tid = int.Parse(QCInspection.TradeList.FirstOrDefault().Value);
            QCInspection.SubcontractorList = new List<SelectListItem>();
            foreach (var subcon in qcInspectionServices.GetAllSubcontractors().Where(x => x.CompanyID == cid && x.qcinspection_subcontractor_trade_detail.Where(t => t.TradeID == tid).Count() > 0).OrderBy(x => x.Name).ToList())
            {
                QCInspection.SubcontractorList.Add(new SelectListItem() { Text = subcon.Name, Value = subcon.SubcontractorID.ToString() });
            }
            QCInspection.ProjectManagerList = new List<SelectListItem>();
            foreach (var PMName in qcInspectionServices.GetAllPMDetailByProjectID(pid).OrderBy(x => x.user.DisplayName).ToList())
            {
                QCInspection.ProjectManagerList.Add(new SelectListItem() { Text = PMName.user.DisplayName, Value = PMName.UserID.ToString() });
            }
            QCInspection.ProjectName = qcInspectionServices.GetProject(QCInspection.ProjectID).Project_Name;
            QCInspection.LocationName = qcInspectionServices.GetBlock(QCInspection.qcinspection_unit_master.qcinspection_level_master.BlockID).BlockName + " " + QCInspection.qcinspection_unit_master.qcinspection_level_master.LevelName + "-" + QCInspection.qcinspection_unit_master.UnitName;
            return View(QCInspection);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionEdit(QCInspectionDefectFormViewModel QCInspection)
        {
            //if (ModelState.IsValid)
            //{
            QCInspection.UpdatedBy = AppSession.GetCurrentUserId();
            QCInspection.UpdatedDate = DateTime.Now;
            QCInspection.FilePath = Server.MapPath("~/images/QCInspection/");
            var result = qcInspectionServices.SaveQCInspectionDefectForm(QCInspection);
            if (result > 0)
            {
                if (QCInspection.DefectFiles != null && QCInspection.DefectFiles.Count > 0)
                {
                    if (!Directory.Exists(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Defect"))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Defect");
                        Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Rectify");
                    }

                    foreach (var fileBase in QCInspection.DefectFiles)
                    {
                        if (fileBase != null)
                        {
                            fileBase.SaveAs(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Defect/" + fileBase.FileName);
                        }
                    }
                }
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
            //}
            //return View(QCInspection);
        }

        [HttpPost, ActionName("DeleteQCInspection")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteQCInspection(int Id)
        {
            var result = qcInspectionServices.DeleteQCInspectionDefectForm(Id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionApproved(QCInspectionDefectFormViewModel QCInspection)
        {
            QCInspection.UpdatedBy = AppSession.GetCurrentUserId();
            QCInspection.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.ApprovedQCInspectionDefectForm(QCInspection);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionReDo(QCInspectionDefectFormViewModel QCInspection)
        {
            QCInspection.UpdatedBy = AppSession.GetCurrentUserId();
            QCInspection.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.ReDoQCInspectionDefectForm(QCInspection);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionReDoDone(QCInspectionDefectFormViewModel QCInspection)
        {
            QCInspection.UpdatedBy = AppSession.GetCurrentUserId();
            QCInspection.UpdatedDate = DateTime.Now;
            QCInspection.FilePath = Server.MapPath("~/images/QCInspection/");
            var result = qcInspectionServices.ReDoDoneQCInspectionDefectForm(QCInspection);
            if (result > 0)
            {
                if (QCInspection.DefectFiles != null && QCInspection.DefectFiles.Count > 0)
                {
                    if (!Directory.Exists(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Defect"))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Defect");
                        Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Rectify");
                    }

                    foreach (var fileBase in QCInspection.DefectFiles)
                    {
                        if (fileBase != null)
                        {
                            fileBase.SaveAs(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Defect/" + fileBase.FileName);
                        }
                    }
                }
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionRectified(QCInspectionDefectFormViewModel QCInspection)
        {
            QCInspection.UpdatedBy = AppSession.GetCurrentUserId();
            QCInspection.UpdatedDate = DateTime.Now;
            QCInspection.FilePath = Server.MapPath("~/images/QCInspection/");
            var result = qcInspectionServices.RectificationQCInspectionDefectForm(QCInspection);
            if (result > 0)
            {
                if (!Directory.Exists(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Rectify"))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Defect");
                    Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Rectify");
                }

                foreach (var fileBase in QCInspection.RectifyFiles)
                {
                    if (fileBase != null)
                    {
                        fileBase.SaveAs(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Rectify/" + fileBase.FileName);
                    }
                }
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionReWork(QCInspectionDefectFormViewModel QCInspection)
        {
            QCInspection.UpdatedBy = AppSession.GetCurrentUserId();
            QCInspection.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.ReworkQCInspectionDefectForm(QCInspection);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionReWorkDone(QCInspectionDefectFormViewModel QCInspection)
        {
            QCInspection.UpdatedBy = AppSession.GetCurrentUserId();
            QCInspection.UpdatedDate = DateTime.Now;
            QCInspection.FilePath = Server.MapPath("~/images/QCInspection/");
            var result = qcInspectionServices.ReworkDoneQCInspectionDefectForm(QCInspection);
            if (result > 0)
            {
                if (!Directory.Exists(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Rectify"))
                {
                    Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Defect");
                    Directory.CreateDirectory(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Rectify");
                }

                foreach (var fileBase in QCInspection.RectifyFiles)
                {
                    if (fileBase != null)
                    {
                        fileBase.SaveAs(Server.MapPath("~/images/QCInspection/") + QCInspection.QCInspectionDefectID.ToString() + "/Rectify/" + fileBase.FileName);
                    }
                }
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QCInspectionCompleted(QCInspectionDefectFormViewModel QCInspection)
        {
            QCInspection.UpdatedBy = AppSession.GetCurrentUserId();
            QCInspection.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.CompletedQCInspectionDefectForm(QCInspection);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }
        #endregion QC Inspection

        #region RFWI Form
        public ActionResult RFWIFormIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/RFWIFormIndex");
            AppSession.SetCurrentMenu("QCInspection", "", "RFWI");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            List<QCInspectionRFWIFormViewModel> qCInspectionRFWIFormViewModels = new List<QCInspectionRFWIFormViewModel>();
            List<int> Projectids = new List<int>();
            if (gid < 5)
            {
                qCInspectionRFWIFormViewModels = qcInspectionServices.GetAllRFWIForms().ToList();
                Projectids = qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).Select(a => a.ProjectID).ToList();
            }
            else if (gid == 5)
            {
                qCInspectionRFWIFormViewModels = qcInspectionServices.GetAllRFWIFormsBasedUserID(uid).ToList();
                Projectids = qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).Select(a => a.ProjectID).ToList();
            }
            // RTO Inspector
            else if (gid == 8)
            {
                qCInspectionRFWIFormViewModels = qcInspectionServices.GetAllRFWIFormsBasedRTOInspectorID(uid).ToList();
                Projectids = qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).Select(a => a.ProjectID).ToList();
            }
            // M&E Inspector
            else if (gid == 9)
            {
                qCInspectionRFWIFormViewModels = qcInspectionServices.GetAllRFWIForms().Where(a => ((a.OtherTradeClearance_MandE == true && a.OtherTradeClearance_MandESignature == null && a.Status == "Pending") || a.OtherTradeClearance_MandEBy == uid) && a.qcinspection_rfwi_form_location_detail.Count() > 0).ToList();
                Projectids = qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid && a.qcinspection_project_MEInspector_detail.Where(w => w.UserID == uid).Count() > 0).Select(a => a.ProjectID).ToList();
            }
            // Structure Inspector
            else if (gid == 10)
            {
                qCInspectionRFWIFormViewModels = qcInspectionServices.GetAllRFWIForms().Where(a => ((a.OtherTradeClearance_Structure == true && a.OtherTradeClearance_StructureSignature == null && a.Status == "Pending") || a.OtherTradeClearance_StructureBy == uid) && a.qcinspection_rfwi_form_location_detail.Count() > 0).ToList();
                Projectids = qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid && a.qcinspection_project_StructureInspector_detail.Where(w => w.UserID == uid).Count() > 0).Select(a => a.ProjectID).ToList();
            }
            // Other Inspector
            else if (gid == 11)
            {
                qCInspectionRFWIFormViewModels = qcInspectionServices.GetAllRFWIForms().Where(a => ((a.OtherTradeClearance_Other == true && a.OtherTradeClearance_OtherSignature == null && a.Status == "Pending") || a.OtherTradeClearance_OtherBy == uid) && a.qcinspection_rfwi_form_location_detail.Count() > 0).ToList();
                Projectids = qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid && a.qcinspection_project_OtherInspector_detail.Where(w => w.UserID == uid).Count() > 0).Select(a => a.ProjectID).ToList();
            }
            else
            {
                qCInspectionRFWIFormViewModels = qcInspectionServices.GetAllRFWIForms().ToList();
                Projectids = qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).Select(a => a.ProjectID).ToList();
            }

            qCInspectionRFWIFormViewModels = qCInspectionRFWIFormViewModels.Where(x => Projectids.Contains(x.ProjectID)).ToList();

            int iCnt = 1;
            List<QCInspectionLocationMobileViewModel> QCInspectionLocationMobileViewModels = new List<QCInspectionLocationMobileViewModel>();
            foreach (var block in qcInspectionServices.GetAllBlocks().Where(x => Projectids.Contains(x.ProjectID)).OrderBy(x => x.OrderBy).ToList())
            {
                foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                {
                    foreach (var unit in qcInspectionServices.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
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
            ViewBag.LocationModels = QCInspectionLocationMobileViewModels;
            return View(qCInspectionRFWIFormViewModels);
        }

        public ActionResult PartialRFWIFormTradeDetailedCheckList(int TradeID)
        {
            QCInspectionRFWIFormViewModel qCInspectionRFWIFormViewModel = new QCInspectionRFWIFormViewModel();
            qCInspectionRFWIFormViewModel.TradeDetailedCheckList = new List<SelectListItem>();
            foreach (var TradeDet in qcInspectionServices.GetAllRFWITradeDetailedCheckLists().Where(x => x.TradeID == TradeID).OrderBy(x => x.OrderBy).ToList())
            {
                qCInspectionRFWIFormViewModel.TradeDetailedCheckList.Add(new SelectListItem() { Text = TradeDet.DetailedCheckListName, Value = TradeDet.TradeDetailedCheckListID.ToString() });
            }
            return PartialView(qCInspectionRFWIFormViewModel);
        }

        public ActionResult PartialRFWIFormTradeItemList(int TradeID)
        {
            QCInspectionRFWIFormViewModel qCInspectionRFWIFormViewModel = new QCInspectionRFWIFormViewModel();
            qCInspectionRFWIFormViewModel.TradeItemList = new List<SelectListItem>();
            foreach (var TradeDet in qcInspectionServices.GetAllRFWITradeItems().Where(x => x.TradeID == TradeID).OrderBy(x => x.OrderBy).ToList())
            {
                qCInspectionRFWIFormViewModel.TradeItemList.Add(new SelectListItem() { Text = TradeDet.ItemName, Value = TradeDet.TradeItemID.ToString() });
            }
            return PartialView(qCInspectionRFWIFormViewModel);
        }

        public ActionResult PartialRFWIFormLocationList(int Id)
        {
            var qCInspectionRFWIFormViewModel = qcInspectionServices.GetRFWIForm(Id);
            return PartialView(qCInspectionRFWIFormViewModel);
        }

        public ActionResult PartialRFWIFormLocation(int ProjectID, int QCInspectionRFWIFormID, int RFWIFormLocationDetailID = 0)
        {
            QCInspectionRFWIFormLocationDetailViewModel RFWIFormLocationDetailViewModel = new QCInspectionRFWIFormLocationDetailViewModel();
            if (RFWIFormLocationDetailID > 0)
            {
                RFWIFormLocationDetailViewModel = qcInspectionServices.GetRFWIFormLocationDetail(RFWIFormLocationDetailID);
            }
            else
            {
                RFWIFormLocationDetailViewModel.QCInspectionRFWIFormID = QCInspectionRFWIFormID;
            }

            RFWIFormLocationDetailViewModel.UnitList = new List<SelectListItem>();
            foreach (var block in qcInspectionServices.GetAllBlocks().Where(x => x.ProjectID == ProjectID).OrderBy(x => x.OrderBy).ToList())
            {
                foreach (var level in block.qcinspection_level_master.OrderBy(x => x.OrderBy))
                {
                    foreach (var unit in qcInspectionServices.GetAllUnits().Where(x => x.LevelID == level.LevelID).OrderBy(x => x.OrderBy).ToList())
                    {
                        RFWIFormLocationDetailViewModel.UnitList.Add(new SelectListItem() { Text = block.BlockName + " " + level.LevelName + "-" + unit.UnitName, Value = unit.UnitID.ToString() });
                    }
                }
            }
            RFWIFormLocationDetailViewModel.DrawingReferenceFilesList = new List<SelectListItem>();
            foreach (var RefFile in qcInspectionServices.GetAllRFWIDrawingsReferenceFiles(ProjectID).ToList())
            {
                RFWIFormLocationDetailViewModel.DrawingReferenceFilesList.Add(new SelectListItem() { Text = RefFile.FileCaption, Value = RefFile.QCInspectionDrawingReferenceFileID.ToString() });
            }
            return PartialView(RFWIFormLocationDetailViewModel);
        }

        [HttpPost]
        public ActionResult PartialRFWIFormLocation(QCInspectionRFWIFormLocationDetailViewModel RFWIFormLocationDetailViewModel)
        {
            var data = "Error";
            if (qcInspectionServices.CheckRFWIFormLocationDetail(RFWIFormLocationDetailViewModel.QCInspectionRFWIFormID, RFWIFormLocationDetailViewModel.RFWIFormLocationDetailID, RFWIFormLocationDetailViewModel.UnitID) == false)
            {
                RFWIFormLocationDetailViewModel.CreatedBy = AppSession.GetCurrentUserId();
                RFWIFormLocationDetailViewModel.CreatedDate = DateTime.Now;
                var result = qcInspectionServices.CreateRFWIFormLocationDetail(RFWIFormLocationDetailViewModel);
                if (result > 0)
                {
                    data = "Success:RFWI Form Location Creation Successful!";
                }
                else
                {
                    data = "RFWI Form Location Creation failed!";
                }
            }
            else
            {
                data = "RFWI Form Location Already exits!";
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("DeleteRFWIFormLocation")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRFWIFormLocation(int id)
        {
            var result = qcInspectionServices.DeleteRFWIFormLocationDetail(id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public ActionResult PartialRFWIFormAvailableSlots(int InspectorID, string InspectionDate)
        {
            List<AvailableSlotsModel> availableSlotsModels = new List<AvailableSlotsModel>();
            foreach (var objIns in qcInspectionServices.GetAllRFWIFormsBasedRTOInspectorID(InspectorID).Where(x => x.Status != "Rejected" && x.InspectionOn == DateTime.Parse(InspectionDate) && x.InspectionStartOn != null && x.InspectionEndOn != null).OrderBy(w => w.InspectionStartOn).ToList())
            {
                availableSlotsModels.Add(new AvailableSlotsModel()
                {
                    ProjectName = objIns.qcinspection_project_master.Project_Name,
                    QCInspectionRFWINo = objIns.QCInspectionRFWINo + "  " + DateTime.Today.Add(objIns.InspectionStartOn.Value).ToString("hh:mm tt") + " to " + DateTime.Today.Add(objIns.InspectionEndOn.Value).ToString("hh:mm tt"),
                    InspectionStartOn = objIns.InspectionStartOn,
                    InspectionEndOn = objIns.InspectionEndOn
                });
            }
            return PartialView(availableSlotsModels);
        }

        public ActionResult RFWIFormCreate()
        {
            AppSession.SetCurrentPage("/QCInspection/RFWIFormCreate");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            QCInspectionRFWIFormViewModel qCInspectionRFWIFormViewModel = new QCInspectionRFWIFormViewModel();
            qCInspectionRFWIFormViewModel.ProjectList = new List<SelectListItem>();
            foreach (var objPro in qcInspectionServices.GetAllProjectsBasedCompanyID(cid).Where(x => x.Is_Completed == 0).OrderBy(x => x.Project_Name).ToList())
            {
                qCInspectionRFWIFormViewModel.ProjectList.Add(new SelectListItem() { Text = objPro.Project_Name, Value = objPro.ProjectID.ToString() });
            }
            qCInspectionRFWIFormViewModel.TradeList = new List<SelectListItem>();
            foreach (var trade in qcInspectionServices.GetAllRFWITrades().ToList())
            {
                qCInspectionRFWIFormViewModel.TradeList.Add(new SelectListItem() { Text = trade.TradeName, Value = trade.TradeID.ToString() });
            }
            var tid = int.Parse(qCInspectionRFWIFormViewModel.TradeList.FirstOrDefault().Value);
            qCInspectionRFWIFormViewModel.GeneralCheckList = new List<SelectListItem>();
            foreach (var GChkList in qcInspectionServices.GetAllRFWIGeneralCheckLists().OrderBy(x => x.OrderBy).ToList())
            {
                qCInspectionRFWIFormViewModel.GeneralCheckList.Add(new SelectListItem() { Text = GChkList.GeneralCheckListName, Value = GChkList.GeneralCheckListID.ToString() });
            }

            qCInspectionRFWIFormViewModel.TradeDetailedCheckList = new List<SelectListItem>();
            foreach (var TradeDet in qcInspectionServices.GetAllRFWITradeDetailedCheckLists().Where(x => x.TradeID == tid).OrderBy(x => x.OrderBy).ToList())
            {
                qCInspectionRFWIFormViewModel.TradeDetailedCheckList.Add(new SelectListItem() { Text = TradeDet.DetailedCheckListName, Value = TradeDet.TradeDetailedCheckListID.ToString() });
            }

            qCInspectionRFWIFormViewModel.TradeItemList = new List<SelectListItem>();
            foreach (var TradeDet in qcInspectionServices.GetAllRFWITradeItems().Where(x => x.TradeID == tid).OrderBy(x => x.OrderBy).ToList())
            {
                qCInspectionRFWIFormViewModel.TradeItemList.Add(new SelectListItem() { Text = TradeDet.ItemName, Value = TradeDet.TradeItemID.ToString() });
            }

            qCInspectionRFWIFormViewModel.InspectorList = new List<SelectListItem>();
            foreach (var DType in userService.getAllUsers().Where(x => x.GroupID == 8).ToList())
            {
                qCInspectionRFWIFormViewModel.InspectorList.Add(new SelectListItem() { Text = DType.DisplayName, Value = DType.UserID.ToString() });
            }
            qCInspectionRFWIFormViewModel.CreatedDate = DateTime.Now;
            qCInspectionRFWIFormViewModel.RequestFor = "Work Inspection";
            return View(qCInspectionRFWIFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormCreate(QCInspectionRFWIFormViewModel RFWIForm)
        {
            //if (ModelState.IsValid)
            //{
            if (string.IsNullOrEmpty(RFWIForm.InspectionStartTime))
            {
                RFWIForm.InspectionStartOn = null;
            }
            else
            {
                RFWIForm.InspectionStartOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionStartTime));
            }
            if (string.IsNullOrEmpty(RFWIForm.InspectionEndTime))
            {
                RFWIForm.InspectionEndOn = null;
            }
            else
            {
                RFWIForm.InspectionEndOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionEndTime));
            }
            RFWIForm.QCInspectionRFWINo = null;
            RFWIForm.CreatedBy = AppSession.GetCurrentUserId();
            RFWIForm.CreatedDate = DateTime.Now;

            var result = qcInspectionServices.CreateRFWIForm(RFWIForm);
            if (result > 0)
            {
                return Json(new
                {
                    value = string.Format("{0}", result)
                });
            }
            else
            {
                return getFailedOperation();
            }

            //if (qcInspectionServices.ValidateInspectionDateTimeSlot(RFWIForm.InspectionOn, (TimeSpan) RFWIForm.InspectionStartOn, (TimeSpan) RFWIForm.InspectionEndOn))
            //{
            //    var result = qcInspectionServices.CreateRFWIForm(RFWIForm);
            //    if (result > 0)
            //    {
            //        return Json(new
            //        {
            //            value = string.Format("{0}", result)
            //        });
            //    }
            //    else
            //    {
            //        return getFailedOperation();
            //    }
            //}
            //else
            //{
            //    return getFailedOperation("Start/End Time Already exits!");
            //}

        }

        public ActionResult RFWIFormEdit(int Id)
        {
            AppSession.SetCurrentPage("/QCInspection/RFWIFormEdit/" + Id.ToString());
            var qCInspectionRFWIFormViewModel = qcInspectionServices.GetRFWIForm(Id);
            if (qCInspectionRFWIFormViewModel == null)
            {
                return HttpNotFound();
            }
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            var pid = qCInspectionRFWIFormViewModel.ProjectID;
            if (qCInspectionRFWIFormViewModel.InspectionStartOn != null)
            {
                qCInspectionRFWIFormViewModel.InspectionStartTime = DateTime.Today.Add(qCInspectionRFWIFormViewModel.InspectionStartOn.Value).ToString("hh:mm tt").ToLower();
            }
            if (qCInspectionRFWIFormViewModel.InspectionEndOn != null)
            {
                qCInspectionRFWIFormViewModel.InspectionEndTime = DateTime.Today.Add(qCInspectionRFWIFormViewModel.InspectionEndOn.Value).ToString("hh:mm tt").ToLower();
            }
            qCInspectionRFWIFormViewModel.ProjectList = new List<SelectListItem>();
            foreach (var objPro in qcInspectionServices.GetAllProjectsBasedCompanyID(cid).Where(x => x.Is_Completed == 0 || x.ProjectID == pid).OrderBy(x => x.Project_Name).ToList())
            {
                qCInspectionRFWIFormViewModel.ProjectList.Add(new SelectListItem() { Text = objPro.Project_Name, Value = objPro.ProjectID.ToString() });
            }
            qCInspectionRFWIFormViewModel.TradeList = new List<SelectListItem>();
            foreach (var trade in qcInspectionServices.GetAllRFWITrades().ToList())
            {
                qCInspectionRFWIFormViewModel.TradeList.Add(new SelectListItem() { Text = trade.TradeName, Value = trade.TradeID.ToString() });
            }
            var tid = qCInspectionRFWIFormViewModel.TradeID;
            qCInspectionRFWIFormViewModel.GeneralCheckList = new List<SelectListItem>();
            foreach (var GChkList in qcInspectionServices.GetAllRFWIGeneralCheckLists().OrderBy(x => x.OrderBy).ToList())
            {
                qCInspectionRFWIFormViewModel.GeneralCheckList.Add(new SelectListItem() { Text = GChkList.GeneralCheckListName, Value = GChkList.GeneralCheckListID.ToString() });
            }

            qCInspectionRFWIFormViewModel.TradeDetailedCheckList = new List<SelectListItem>();
            foreach (var TradeDet in qcInspectionServices.GetAllRFWITradeDetailedCheckLists().Where(x => x.TradeID == tid).OrderBy(x => x.OrderBy).ToList())
            {
                qCInspectionRFWIFormViewModel.TradeDetailedCheckList.Add(new SelectListItem() { Text = TradeDet.DetailedCheckListName, Value = TradeDet.TradeDetailedCheckListID.ToString() });
            }

            qCInspectionRFWIFormViewModel.TradeItemList = new List<SelectListItem>();
            foreach (var TradeDet in qcInspectionServices.GetAllRFWITradeItems().Where(x => x.TradeID == tid).OrderBy(x => x.OrderBy).ToList())
            {
                qCInspectionRFWIFormViewModel.TradeItemList.Add(new SelectListItem() { Text = TradeDet.ItemName, Value = TradeDet.TradeItemID.ToString() });
            }

            qCInspectionRFWIFormViewModel.InspectorList = new List<SelectListItem>();
            foreach (var DType in userService.getAllUsers().Where(x => x.GroupID == 8 && x.IsActive == 1).ToList())
            {
                qCInspectionRFWIFormViewModel.InspectorList.Add(new SelectListItem() { Text = DType.DisplayName, Value = DType.UserID.ToString() });
            }

            if (qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEBy != null)
            {
                qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEByName = userService.GetUser((int)qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEBy)?.DisplayName;
            }

            if (qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureBy != null)
            {
                qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureByName = userService.GetUser((int)qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureBy)?.DisplayName;
            }

            if (qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherBy != null)
            {
                qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherByName = userService.GetUser((int)qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherBy)?.DisplayName;
            }

            qCInspectionRFWIFormViewModel.CreatedDate = DateTime.Now;
            qCInspectionRFWIFormViewModel.ProceedRequest = false;
            if (!string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureSignature) || !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_MandESignature) || !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherSignature))
            {
                qCInspectionRFWIFormViewModel.OtherSigned = true;
            }
            else
            {
                qCInspectionRFWIFormViewModel.OtherSigned = false;
            }

            // SMO
            // 111
            if (qCInspectionRFWIFormViewModel.OtherTradeClearance_Structure == true && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandE == true && qCInspectionRFWIFormViewModel.OtherTradeClearance_Other == true)
            {
                if(qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureOn !=null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureSignature) && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_MandESignature) && qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherSignature))
                {
                    qCInspectionRFWIFormViewModel.ProceedRequest = true;
                }
            }
            // 000
            else if (qCInspectionRFWIFormViewModel.OtherTradeClearance_Structure == false && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandE == false && qCInspectionRFWIFormViewModel.OtherTradeClearance_Other == false)
            {
                qCInspectionRFWIFormViewModel.ProceedRequest = true;
            }
            // 101
            else if (qCInspectionRFWIFormViewModel.OtherTradeClearance_Structure == true && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandE == false && qCInspectionRFWIFormViewModel.OtherTradeClearance_Other == true)
            {
                if (qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureSignature) && qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherSignature))
                {
                    qCInspectionRFWIFormViewModel.ProceedRequest = true;
                }
            }
            // 110
            else if (qCInspectionRFWIFormViewModel.OtherTradeClearance_Structure == true && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandE == true && qCInspectionRFWIFormViewModel.OtherTradeClearance_Other == false)
            {
                if (qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureSignature) && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_MandESignature))
                {
                    qCInspectionRFWIFormViewModel.ProceedRequest = true;
                }
            }
            // 100
            else if (qCInspectionRFWIFormViewModel.OtherTradeClearance_Structure == true && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandE == false && qCInspectionRFWIFormViewModel.OtherTradeClearance_Other == false)
            {
                if (qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureSignature))
                {
                    qCInspectionRFWIFormViewModel.ProceedRequest = true;
                }
            }
            // 001
            else if (qCInspectionRFWIFormViewModel.OtherTradeClearance_Structure == false && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandE == false && qCInspectionRFWIFormViewModel.OtherTradeClearance_Other == true)
            {
                if (qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherSignature))
                {
                    qCInspectionRFWIFormViewModel.ProceedRequest = true;
                }
            }
            // 010
            else if (qCInspectionRFWIFormViewModel.OtherTradeClearance_Structure == false && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandE == true && qCInspectionRFWIFormViewModel.OtherTradeClearance_Other == false)
            {
                if (qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_MandESignature))
                {
                    qCInspectionRFWIFormViewModel.ProceedRequest = true;
                }
            }
            // 011
            else if (qCInspectionRFWIFormViewModel.OtherTradeClearance_Structure == false && qCInspectionRFWIFormViewModel.OtherTradeClearance_MandE == true && qCInspectionRFWIFormViewModel.OtherTradeClearance_Other == true)
            {
                if (qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_MandESignature) && qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherOn != null && !string.IsNullOrEmpty(qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherSignature))
                {
                    qCInspectionRFWIFormViewModel.ProceedRequest = true;
                }
            }
            if (qCInspectionRFWIFormViewModel.qcinspection_rfwi_form_trade_item_detail?.Count() > 0)
            {
                qCInspectionRFWIFormViewModel.TradeItemID = qCInspectionRFWIFormViewModel.qcinspection_rfwi_form_trade_item_detail[0].TradeItemID;
            }
            return View(qCInspectionRFWIFormViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormEdit(QCInspectionRFWIFormViewModel RFWIForm)
        {
            RFWIForm.UpdatedBy = AppSession.GetCurrentUserId();
            RFWIForm.UpdatedDate = DateTime.Now;

            if (string.IsNullOrEmpty(RFWIForm.InspectionStartTime))
            {
                RFWIForm.InspectionStartOn = null;
            }
            else
            {
                RFWIForm.InspectionStartOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionStartTime));
            }
            if (string.IsNullOrEmpty(RFWIForm.InspectionEndTime))
            {
                RFWIForm.InspectionEndOn = null;
            }
            else
            {
                RFWIForm.InspectionEndOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionEndTime));
            }

            if (RFWIForm.InspectionOn !=null)
            {
                if (qcInspectionServices.ValidateInspectionDateTimeSlot(RFWIForm.InspectionOn.Value, (TimeSpan)RFWIForm.InspectionStartOn, (TimeSpan)RFWIForm.InspectionEndOn))
                {
                    var result = qcInspectionServices.SaveRFWIForm(RFWIForm);
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
                    return getFailedOperation("Start/End Time Already exits!");
                }
            }
            else
            {
                var result = qcInspectionServices.SaveRFWIForm(RFWIForm);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormRequested(QCInspectionRFWIFormViewModel RFWIForm)
        {
            if (string.IsNullOrEmpty(RFWIForm.InspectionStartTime))
            {
                RFWIForm.InspectionStartOn = null;
            }
            else
            {
                RFWIForm.InspectionStartOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionStartTime));
            }
            if (string.IsNullOrEmpty(RFWIForm.InspectionEndTime))
            {
                RFWIForm.InspectionEndOn = null;
            }
            else
            {
                RFWIForm.InspectionEndOn = LocalizationHelper.DateTimeToTimeSpan(DateTime.Parse(RFWIForm.InspectionEndTime));
            }

            if (qcInspectionServices.ValidateInspectionDateTimeSlot(RFWIForm.InspectionOn.Value, (TimeSpan)RFWIForm.InspectionStartOn, (TimeSpan)RFWIForm.InspectionEndOn))
            {
                var result = qcInspectionServices.RequestedRFWIForm(RFWIForm);
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
                return getFailedOperation("Start/End Time Already exits!");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormMandESignOff(QCInspectionRFWIFormViewModel RFWIForm)
        {
            if (RFWIForm.OtherTradeClearance_MandESignature != null)
            {
                if (RFWIForm.OtherTradeClearance_MandEOn == null)
                {
                    RFWIForm.OtherTradeClearance_MandEOn = DateTime.Now;
                }
            }
            
            RFWIForm.UpdatedBy = AppSession.GetCurrentUserId();
            RFWIForm.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.MandESignOffRFWIForm(RFWIForm);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormStructureSignOff(QCInspectionRFWIFormViewModel RFWIForm)
        {
            if (RFWIForm.OtherTradeClearance_StructureSignature != null)
            {
                if (RFWIForm.OtherTradeClearance_StructureOn == null)
                {
                    RFWIForm.OtherTradeClearance_StructureOn = DateTime.Now;
                }
            }

            RFWIForm.UpdatedBy = AppSession.GetCurrentUserId();
            RFWIForm.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.StructureSignOffRFWIForm(RFWIForm);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormOtherSignOff(QCInspectionRFWIFormViewModel RFWIForm)
        {
            if (RFWIForm.OtherTradeClearance_OtherSignature != null)
            {
                if (RFWIForm.OtherTradeClearance_OtherOn == null)
                {
                    RFWIForm.OtherTradeClearance_OtherOn = DateTime.Now;
                }
            }

            RFWIForm.UpdatedBy = AppSession.GetCurrentUserId();
            RFWIForm.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.OtherSignOffRFWIForm(RFWIForm);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormRejected(QCInspectionRFWIFormViewModel RFWIForm)
        {
            RFWIForm.UpdatedBy = AppSession.GetCurrentUserId();
            RFWIForm.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.RejectedRFWIForm(RFWIForm);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormCompleted(QCInspectionRFWIFormViewModel RFWIForm)
        {
            if (RFWIForm.OtherTradeClearance_StructureSignature != null)
            {
                if (RFWIForm.OtherTradeClearance_StructureOn == null)
                {
                    RFWIForm.OtherTradeClearance_StructureOn = DateTime.Now;
                }
            }
            else
            {
                RFWIForm.OtherTradeClearance_StructureByName = null;
            }

            if (RFWIForm.OtherTradeClearance_MandESignature != null)
            {
                if (RFWIForm.OtherTradeClearance_MandEOn == null)
                {
                    RFWIForm.OtherTradeClearance_MandEOn = DateTime.Now;
                }
            }
            else
            {
                RFWIForm.OtherTradeClearance_MandEByName = null;
            }
            if (RFWIForm.OtherTradeClearance_OtherSignature != null)
            {
                if (RFWIForm.OtherTradeClearance_OtherOn == null)
                {
                    RFWIForm.OtherTradeClearance_OtherOn = DateTime.Now;
                }
            }
            else
            {
                RFWIForm.OtherTradeClearance_OtherByName = null;
            }
            RFWIForm.UpdatedBy = AppSession.GetCurrentUserId();
            RFWIForm.UpdatedDate = DateTime.Now;
            var result = qcInspectionServices.CompletedRFWIForm(RFWIForm);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        [HttpPost, ActionName("RFWIFormReInspection")]
        [ValidateAntiForgeryToken]
        public ActionResult RFWIFormReInspection(int Id)
        {
            var result = qcInspectionServices.ReInspectionRFWIForm(Id);
            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
        }

        public ActionResult RFWIFormReport(int Id)
        {
            var qCInspectionRFWIFormViewModel = qcInspectionServices.GetRFWIForm(Id);
            if (qCInspectionRFWIFormViewModel == null)
            {
                return HttpNotFound();
            }
            if (qCInspectionRFWIFormViewModel.InspectionStartOn != null)
            {
                qCInspectionRFWIFormViewModel.InspectionStartTime = DateTime.Today.Add(qCInspectionRFWIFormViewModel.InspectionStartOn.Value).ToString("hh:mm tt").ToLower();
            }
            if (qCInspectionRFWIFormViewModel.InspectionEndOn != null)
            {
                qCInspectionRFWIFormViewModel.InspectionEndTime = DateTime.Today.Add(qCInspectionRFWIFormViewModel.InspectionEndOn.Value).ToString("hh:mm tt").ToLower();
            }
            if(qCInspectionRFWIFormViewModel?.RequestBy > 0)
            {
                qCInspectionRFWIFormViewModel.RequestByName = userService.GetUser((int)qCInspectionRFWIFormViewModel.RequestBy).DisplayName;
            }

            if (qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEBy != null)
            {
                qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEByName = userService.GetUser((int)qCInspectionRFWIFormViewModel.OtherTradeClearance_MandEBy)?.DisplayName;
            }

            if (qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureBy != null)
            {
                qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureByName = userService.GetUser((int)qCInspectionRFWIFormViewModel.OtherTradeClearance_StructureBy)?.DisplayName;
            }

            if (qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherBy != null)
            {
                qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherByName = userService.GetUser((int)qCInspectionRFWIFormViewModel.OtherTradeClearance_OtherBy)?.DisplayName;
            }
            return View(qCInspectionRFWIFormViewModel);
        }


        [HttpPost, ActionName("DeleteRFWIForm")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRFWIForm(int Id)
        {
            var result = qcInspectionServices.DeleteRFWIForm(Id);
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

        #region Project Plan Documents
        public ActionResult ProjectDocumentsIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/ProjectDocumentsIndex");
            AppSession.SetCurrentMenu("PM", "Reports", "Project Documents");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(qcInspectionServices.GetAllProjects().ToList());
            }
            else
            {
                return View(qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).ToList());
            }
        }

        public ActionResult ProjectDocumentList(int ProjectID)
        {
            ViewBag.Module = "Report";
            List<QCInspectionProjectFilesViewModel> ProjectFileList = new List<QCInspectionProjectFilesViewModel>();
            try
            {
                ProjectFileList = qcInspectionServices.GetAllProjectFiles(ProjectID).ToList();
                QCInspectionProjectMasterViewModel qCInspectionProjectMasterViewModel = qcInspectionServices.GetProject(ProjectID);
                ViewBag.Title = "Project Document List for " + qCInspectionProjectMasterViewModel.Project_Name;
            }
            catch (Exception ex)
            {
                logger.Debug("RFWI Progress Report :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(ProjectFileList);
        }
        #endregion Project Plan Documents

        #region RFWI Progress Report
        public ActionResult RFWIProgressReportIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/RFWIProgressReportIndex");
            AppSession.SetCurrentMenu("PM", "Reports", "RWFI Progress Report");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(qcInspectionServices.GetAllProjects().ToList());
            }
            else
            {
                return View(qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).ToList());
            }
        }

        public ActionResult RFWIProgressReport(int ID)
        {
            RFWIProgressReportViewModel rFWIProgressReportViewModel = new RFWIProgressReportViewModel();
            ViewBag.Module = "Report";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                QCInspectionProjectMasterViewModel qCInspectionProjectMasterViewModel = qcInspectionServices.GetProject(ID);
                ViewBag.Title = "RWFI - Progress Status for " + qCInspectionProjectMasterViewModel.Project_Name;
                rFWIProgressReportViewModel.Project_Name = qCInspectionProjectMasterViewModel.Project_Name;
                rFWIProgressReportViewModel.ProjectID = qCInspectionProjectMasterViewModel.ProjectID;
                rFWIProgressReportViewModel.Project_ID = qCInspectionProjectMasterViewModel.Project_ID;
                rFWIProgressReportViewModel.StartOn = qCInspectionProjectMasterViewModel.StartOn;
                rFWIProgressReportViewModel.EndOn = qCInspectionProjectMasterViewModel.EndOn;
                rFWIProgressReportViewModel.Is_Completed = qCInspectionProjectMasterViewModel.Is_Completed;
                rFWIProgressReportViewModel.RFWIProgressReportDetailModels = new List<RFWIProgressReportDetailModel>();
                foreach (var RFWIDet in qcInspectionServices.GetAllRFWIFormsBasedProjectID(ID).OrderBy(x => x.QCInspectionRFWIFormID).ToList())
                {
                    rFWIProgressReportViewModel.RFWIProgressReportDetailModels.Add(new RFWIProgressReportDetailModel()
                    {
                        QCInspectionRFWIFormID = RFWIDet.QCInspectionRFWIFormID,
                        RFWINo = RFWIDet.QCInspectionRFWINo,
                        RequestedOn = RFWIDet.RequestOn,
                        RequestedBy = userService.GetUser(Convert.ToInt32(RFWIDet.CreatedBy)).DisplayName,
                        Trade = RFWIDet.qcinspection_rfwi_trade_master.TradeName,
                        InspectorName = RFWIDet.user.DisplayName,
                        InspectionOn = RFWIDet.InspectionOn,
                        Status = RFWIDet.Status
                    });

                    if (RFWIDet.Status == "Pending")
                    {
                        rFWIProgressReportViewModel.PendingCount++;
                    }
                    else if (RFWIDet.Status == "Rejected")
                    {
                        rFWIProgressReportViewModel.RejectedCount++;
                    }
                    else if (RFWIDet.Status == "Completed")
                    {
                        rFWIProgressReportViewModel.CompletedCount++;
                    }
                    rFWIProgressReportViewModel.RequestedCount++;
                }
                if (rFWIProgressReportViewModel.RequestedCount > 0)
                {
                    rFWIProgressReportViewModel.PendingPercentage = (rFWIProgressReportViewModel.PendingCount / rFWIProgressReportViewModel.RequestedCount) * 100;
                    rFWIProgressReportViewModel.RejectedPercentage = (rFWIProgressReportViewModel.RejectedCount / rFWIProgressReportViewModel.RequestedCount) * 100;
                    rFWIProgressReportViewModel.CompletedPercentage = (rFWIProgressReportViewModel.CompletedCount / rFWIProgressReportViewModel.RequestedCount) * 100;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("RFWI Progress Report :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(rFWIProgressReportViewModel);
        }
        #endregion

        #region QCInspection Progress Report
        public ActionResult QCInspectionProgressReportIndex()
        {
            AppSession.SetCurrentPage("/QCInspection/QCInspectionProgressReportIndex");
            AppSession.SetCurrentMenu("PM", "Reports", "QC Inspection Progress Report");
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(qcInspectionServices.GetAllProjects().ToList());
            }
            else
            {
                return View(qcInspectionServices.GetAllProjects().Where(a => a.CompanyID == cid).ToList());
            }
        }
        public ActionResult QCInspectionProgressReport(int ID)
        {
            QCInspectionProgressReportViewModel qCInspectionProgressReportViewModel = new QCInspectionProgressReportViewModel();
            ViewBag.Module = "Report";
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                QCInspectionProjectMasterViewModel qCInspectionProjectMasterViewModel = qcInspectionServices.GetProject(ID);
                ViewBag.Title = "QC Inspection - Progress Status for " + qCInspectionProjectMasterViewModel.Project_Name;
                qCInspectionProgressReportViewModel.Project_Name = qCInspectionProjectMasterViewModel.Project_Name;
                qCInspectionProgressReportViewModel.ProjectID = qCInspectionProjectMasterViewModel.ProjectID;
                qCInspectionProgressReportViewModel.Project_ID = qCInspectionProjectMasterViewModel.Project_ID;
                qCInspectionProgressReportViewModel.StartOn = qCInspectionProjectMasterViewModel.StartOn;
                qCInspectionProgressReportViewModel.EndOn = qCInspectionProjectMasterViewModel.EndOn;
                qCInspectionProgressReportViewModel.Is_Completed = qCInspectionProjectMasterViewModel.Is_Completed;
                qCInspectionProgressReportViewModel.QCInspectionProgressReportSummaryModels = new List<QCInspectionProgressReportSummaryModel>();
                qCInspectionProgressReportViewModel.QCInspectionProgressReportDetailModels = new List<QCInspectionProgressReportDetailModel>();
                foreach (var QCDet in qcInspectionServices.GetAllQCInspectionDefectFormsBasedProjectID(ID).OrderBy(x => x.QCInspectionDefectID).ToList())
                {
                    qCInspectionProgressReportViewModel.QCInspectionProgressReportDetailModels.Add(new QCInspectionProgressReportDetailModel()
                    {
                        QCInspectionDefectID = QCDet.QCInspectionDefectID,
                        QCInspectionDefectNo = QCDet.QCInspectionDefectNo,
                        SubcontractorName = QCDet.qcinspection_subcontractor_master.Name,
                        Trade = QCDet.qcinspection_trade_master.TradeName,
                        Status = QCDet.Status,
                        RequestedOn = QCDet.CreatedDate
                    });

                    if (QCDet.Status == "Rectified")
                    {
                        qCInspectionProgressReportViewModel.RectifiedCount++;
                    }
                    else if (QCDet.Status == "Completed")
                    {
                        qCInspectionProgressReportViewModel.CompletedCount++;
                    }
                    qCInspectionProgressReportViewModel.TotalCount++;
                    qCInspectionProgressReportViewModel.RequestedCount++;
                }

                if (qCInspectionProgressReportViewModel.RequestedCount > 0)
                {
                    qCInspectionProgressReportViewModel.RequestedPercentage = (qCInspectionProgressReportViewModel.RequestedCount / qCInspectionProgressReportViewModel.TotalCount) * 100;
                    qCInspectionProgressReportViewModel.RectifiedPercentage = (qCInspectionProgressReportViewModel.RectifiedCount / qCInspectionProgressReportViewModel.TotalCount) * 100;
                    qCInspectionProgressReportViewModel.CompletedPercentage = (qCInspectionProgressReportViewModel.CompletedCount / qCInspectionProgressReportViewModel.TotalCount) * 100;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("RFWI Progress Report :");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
            }
            return View(qCInspectionProgressReportViewModel);
        }
        #endregion

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

//CREATE TRIGGER[dbo].[TRG_AFR_qcinspection_rfwi_form]
//ON[dbo].[qcinspection_rfwi_form]
//AFTER INSERT AS
//DECLARE @QCInspectionRFWINo nvarchar(20);
//DECLARE @seqcnt nvarchar(15);
//DECLARE @pad integer;
//BEGIN
//  SELECT @QCInspectionRFWINo = ins.QCInspectionRFWINo FROM INSERTED ins;
//  IF @QCInspectionRFWINo is null
//	Begin
//      SET @seqcnt = 'RFWI-' + FORMAT(getdate(),'yy') + '-%';
//	    SELECT @pad = CONVERT(INT,ISNULL(MAX(RIGHT(QCInspectionRFWINo, 3)),0)) + 1 FROM qcinspection_rfwi_form WHERE InspectionNo = 1 AND QCInspectionRFWINo like @seqcnt;
//      SET @seqcnt = REPLICATE('0', 3 - LEN(@pad)) + CAST(@pad as nvarchar(5));
//      UPDATE[dbo].[qcinspection_rfwi_form] SET QCInspectionRFWINo = 'RFWI-' + FORMAT(getdate(), 'yy') + '-' + CAST(@seqcnt as nvarchar(10)) WHERE QCInspectionRFWIFormID = @@IDENTITY;
//	End
//END


//CREATE TRIGGER[dbo].[TRG_AFR_qcinspection_defect_form]
//ON[dbo].[qcinspection_defect_form]
//AFTER INSERT AS
//DECLARE @seqcnt nvarchar(15);
//DECLARE @pad integer;
//BEGIN
//    SET @seqcnt = 'QC-' + FORMAT(getdate(),'yy') + '-%';
//	  SELECT @pad = CONVERT(INT,ISNULL(MAX(RIGHT(QCInspectionDefectNo, 3)),0)) + 1 FROM qcinspection_defect_form WHERE QCInspectionDefectNo like @seqcnt;
//	  SET @seqcnt = REPLICATE('0', 3 - LEN(@pad)) + CAST(@pad as nvarchar(5));
//	  UPDATE[dbo].[qcinspection_defect_form] SET QCInspectionDefectNo = 'QC-' + FORMAT(getdate(), 'yy') + '-' + CAST(@seqcnt as nvarchar(10)) WHERE QCInspectionDefectNo IS NULL AND QCInspectionDefectID = @@IDENTITY;
//END
