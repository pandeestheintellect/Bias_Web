using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Interface
{
    public class AssessmentServices : IAssessmentServices
    {
        private readonly IAssessmentRepository assessmentRepository;
        public AssessmentServices(IAssessmentRepository _assessmentRepository)
        {
            assessmentRepository = _assessmentRepository;
        }

        // Project
        public List<AssessmentProjectMasterViewModel> GetAllProjects()
        {
            return assessmentRepository.GetAllProjects();
        }

        public List<AssessmentProjectMasterViewModel> GetAllProjectsBasedCompanyID(int? id)
        {
            return assessmentRepository.GetAllProjectsBasedCompanyID(id);
        }

        public AssessmentProjectMasterViewModel GetProject(int pid)
        {
            return assessmentRepository.GetProject(pid);
        }

        public int CreateProject(AssessmentProjectMasterViewModel project, List<AssessmentProjectAssessorsDetailViewModel> detailViewModels)
        {
            return assessmentRepository.CreateProject(project, detailViewModels);
        }

        public int SaveProject(AssessmentProjectMasterViewModel project, List<AssessmentProjectAssessorsDetailViewModel> detailViewModels)
        {
            return assessmentRepository.SaveProject(project, detailViewModels);
        }

        public int DeleteProject(int pID)
        {
            return assessmentRepository.DeleteProject(pID);
        }

        public bool CheckProject(string projectname)
        {
            return assessmentRepository.CheckProject(projectname);
        }
        public int CompletedProject(int pid)
        {
            return assessmentRepository.CompletedProject(pid);
        }
        // Project

        // Masters

        //Assessors
        public AssessorsMasterViewModel GetAssessor(int aid)
        {
            return assessmentRepository.GetAssessor(aid);
        }

        public int CreateAssessor(AssessorsMasterViewModel Assessor)
        {
            return assessmentRepository.CreateAssessor(Assessor);
        }

        public int SaveAssessor(AssessorsMasterViewModel Assessor)
        {
            return assessmentRepository.SaveAssessor(Assessor);
        }

        public int DeleteAssessor(int aID)
        {
            return assessmentRepository.DeleteAssessor(aID);
        }
        public List<AssessorsMasterViewModel> GetAllAssessors()
        {
            return assessmentRepository.GetAllAssessors();
        }

        public List<AssessorsMasterViewModel> GetAllAssessorsBasedCompanyID(int? id)
        {
            return assessmentRepository.GetAllAssessorsBasedCompanyID(id);
        }

        public bool CheckAssessor(string Assessorsname)
        {
            return assessmentRepository.CheckAssessor(Assessorsname);
        }
        //Assessors

        public List<AssessmentDevelopmentTypeMasterViewModel> GetAllDevelopmentTypes()
        {
            return assessmentRepository.GetAllDevelopmentTypes();
        }

        public List<AssessmentTypeMasterViewModel> GetAllAssessmentTypes()
        {
            return assessmentRepository.GetAllAssessmentTypes();
        }

        //Locations
        public AssessmentTypeLocationMasterViewModel GetLocation(int Id)
        {
            return assessmentRepository.GetLocation(Id);
        }

        public int CreateLocation(AssessmentTypeLocationMasterViewModel Location)
        {
            return assessmentRepository.CreateLocation(Location);
        }

        public int SaveLocation(AssessmentTypeLocationMasterViewModel Location)
        {
            return assessmentRepository.SaveLocation(Location);
        }

        public int DeleteLocation(int Id)
        {
            return assessmentRepository.DeleteLocation(Id);
        }
        public List<AssessmentTypeLocationMasterViewModel> GetAllAssessmentLocations()
        {
            return assessmentRepository.GetAllAssessmentLocations();
        }

        public bool CheckLocation(int AssessmentTypeID, string LocationName)
        {
            return assessmentRepository.CheckLocation(AssessmentTypeID, LocationName);
        }
        //Locations

        //Modules
        public AssessmentTypeModuleMasterViewModel GetModule(int Id)
        {
            return assessmentRepository.GetModule(Id);
        }

        public int CreateModule(AssessmentTypeModuleMasterViewModel Module)
        {
            return assessmentRepository.CreateModule(Module);
        }

        public int SaveModule(AssessmentTypeModuleMasterViewModel Module)
        {
            return assessmentRepository.SaveModule(Module);
        }

        public int DeleteModule(int Id)
        {
            return assessmentRepository.DeleteModule(Id);
        }

        public List<AssessmentTypeModuleMasterViewModel> GetAllModules()
        {
            return assessmentRepository.GetAllModules();
        }

        public bool CheckModule(int? AssessmentTypeID, string ModuleName)
        {
            return assessmentRepository.CheckModule(AssessmentTypeID, ModuleName);
        }
        //Modules

        //Module Process
        public AssessmentTypeModuleProcessMasterViewModel GetModuleProcess(int Id)
        {
            return assessmentRepository.GetModuleProcess(Id);
        }

        public int CreateModuleProcess(AssessmentTypeModuleProcessMasterViewModel ModuleProcess)
        {
            return assessmentRepository.CreateModuleProcess(ModuleProcess);
        }

        public int SaveModuleProcess(AssessmentTypeModuleProcessMasterViewModel ModuleProcess)
        {
            return assessmentRepository.SaveModuleProcess(ModuleProcess);
        }

        public int DeleteModuleProcess(int Id)
        {
            return assessmentRepository.DeleteModuleProcess(Id);
        }

        public List<AssessmentTypeModuleProcessMasterViewModel> GetAllModuleProcess()
        {
            return assessmentRepository.GetAllModuleProcess();
        }

        public List<AssessmentTypeModuleProcessMasterViewModel> GetAllModuleProcessByModuleIds(List<int> ids)
        {
            return assessmentRepository.GetAllModuleProcessByModuleIds(ids);
        }

        public bool CheckModuleProcess(int ModuleID, string ModuleProcessName)
        {
            return assessmentRepository.CheckModuleProcess(ModuleID, ModuleProcessName);
        }
        //Module Process


        // Directions
        public List<AssessmentDirectionMasterViewModel> GetAllDirections()
        {
            return assessmentRepository.GetAllDirections();
        }
        // Directions


        //Joints
        public AssessmentJointMasterViewModel GetJoint(int Id)
        {
            return assessmentRepository.GetJoint(Id);
        }

        public int CreateJoint(AssessmentJointMasterViewModel Joint)
        {
            return assessmentRepository.CreateJoint(Joint);
        }

        public int SaveJoint(AssessmentJointMasterViewModel Joint)
        {
            return assessmentRepository.SaveJoint(Joint);
        }

        public int DeleteJoint(int Id)
        {
            return assessmentRepository.DeleteJoint(Id);
        }

        public List<AssessmentJointMasterViewModel> GetAllJoints()
        {
            return assessmentRepository.GetAllJoints();
        }

        public bool CheckJoint(string JointName)
        {
            return assessmentRepository.CheckJoint(JointName);
        }
        //Joints

        //Leaks
        public AssessmentLeakMasterViewModel GetLeak(int Id)
        {
            return assessmentRepository.GetLeak(Id);
        }

        public int CreateLeak(AssessmentLeakMasterViewModel Leak)
        {
            return assessmentRepository.CreateLeak(Leak);
        }

        public int SaveLeak(AssessmentLeakMasterViewModel Leak)
        {
            return assessmentRepository.SaveLeak(Leak);
        }

        public int DeleteLeak(int Id)
        {
            return assessmentRepository.DeleteLeak(Id);
        }

        public List<AssessmentLeakMasterViewModel> GetAllLeaks()
        {
            return assessmentRepository.GetAllLeaks();
        }

        public bool CheckLeak(string LeakName)
        {
            return assessmentRepository.CheckLeak(LeakName);
        }
        //Leaks

        //Walls
        public AssessmentWallMasterViewModel GetWall(int Id)
        {
            return assessmentRepository.GetWall(Id);
        }

        public int CreateWall(AssessmentWallMasterViewModel Wall)
        {
            return assessmentRepository.CreateWall(Wall);
        }

        public int SaveWall(AssessmentWallMasterViewModel Wall)
        {
            return assessmentRepository.SaveWall(Wall);
        }

        public int DeleteWall(int Id)
        {
            return assessmentRepository.DeleteWall(Id);
        }

        public List<AssessmentWallMasterViewModel> GetAllWalls()
        {
            return assessmentRepository.GetAllWalls();
        }

        public bool CheckWall(string WallName)
        {
            return assessmentRepository.CheckWall(WallName);
        }
        //Walls

        //Windows
        public AssessmentWindowMasterViewModel GetWindow(int Id)
        {
            return assessmentRepository.GetWindow(Id);
        }

        public int CreateWindow(AssessmentWindowMasterViewModel Window)
        {
            return assessmentRepository.CreateWindow(Window);
        }

        public int SaveWindow(AssessmentWindowMasterViewModel Window)
        {
            return assessmentRepository.SaveWindow(Window);
        }

        public int DeleteWindow(int Id)
        {
            return assessmentRepository.DeleteWindow(Id);
        }

        public List<AssessmentWindowMasterViewModel> GetAllWindows()
        {
            return assessmentRepository.GetAllWindows();
        }

        public bool CheckWindow(string WindowName)
        {
            return assessmentRepository.CheckWindow(WindowName);
        }
        //Windows

        //Wet Area Water Tightness Test Result
        public AssessmentWetAreaWaterTightnessTestResultMasterViewModel GetWetAreaWaterTightnessTestResult(int Id)
        {
            return assessmentRepository.GetWetAreaWaterTightnessTestResult(Id);
        }

        public int CreateWetAreaWaterTightnessTestResult(AssessmentWetAreaWaterTightnessTestResultMasterViewModel WAWTTResult)
        {
            return assessmentRepository.CreateWetAreaWaterTightnessTestResult(WAWTTResult);
        }

        public int SaveWetAreaWaterTightnessTestResult(AssessmentWetAreaWaterTightnessTestResultMasterViewModel WAWTTResult)
        {
            return assessmentRepository.SaveWetAreaWaterTightnessTestResult(WAWTTResult);
        }

        public int DeleteWetAreaWaterTightnessTestResult(int Id)
        {
            return assessmentRepository.DeleteWetAreaWaterTightnessTestResult(Id);
        }

        public List<AssessmentWetAreaWaterTightnessTestResultMasterViewModel> GetAllWetAreaWaterTightnessTestResults()
        {
            return assessmentRepository.GetAllWetAreaWaterTightnessTestResults();
        }

        public bool CheckWetAreaWaterTightnessTestResult(string WAWTTResultName)
        {
            return assessmentRepository.CheckWetAreaWaterTightnessTestResult(WAWTTResultName);
        }
        //Wet Area Water Tightness Test Result

        public List<MasterSyncViewModel> GetAssessmentMasterSync()
        {
            return assessmentRepository.GetAssessmentMasterSync();
        }
        // Masters

        // Transactions
        public List<AssessmentSummaryDetailModel> GetAssessmentSummaryByProjectID(int? id)
        {
            return assessmentRepository.GetAssessmentSummaryByProjectID(id);
        }

        // Internal Finishes
        public List<AssessmentInternalFinishesIndexViewModel> GetAllAssessmentInternalFinishes_List(int? id)
        {
            return assessmentRepository.GetAllAssessmentInternalFinishes_List(id);
        }

        public List<AssessmentInternalFinishesTransMasterViewModel> GetAllAssessmentInternalFinishes(int id, string BatchID = "")
        {
            return assessmentRepository.GetAllAssessmentInternalFinishes(id, BatchID);
        }

        public AssessmentInternalFinishesTransMasterViewModel GetAllAssessmentInternalFinishes_ByID(int id)
        {
            return assessmentRepository.GetAllAssessmentInternalFinishes_ByID(id);
        }

        public List<AssessmentInternalFinishesTransDetailViewModel> GetAllAssessmentInternalFinishes_Detail(List<int> ids)
        {
            return assessmentRepository.GetAllAssessmentInternalFinishes_Detail(ids);
        }

        public AssessmentInternalFinishesTransDetailViewModel GetAllAssessmentInternalFinishes_DetailByID(int id)
        {
            return assessmentRepository.GetAllAssessmentInternalFinishes_DetailByID(id);
        }

        public List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> GetAllAssessmentWetAreaWaterTightnessTest_DetailResult(List<int> ids)
        {
            return assessmentRepository.GetAllAssessmentWetAreaWaterTightnessTest_DetailResult(ids);
        }

        public AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel GetAllAssessmentWetAreaWaterTightnessTest_DetailResultByID(int id)
        {
            return assessmentRepository.GetAllAssessmentWetAreaWaterTightnessTest_DetailResultByID(id);
        }

        public int CreateAssessmentInternalFinishesMaster(AssessmentInternalFinishesTransMasterViewModel masterViewModel, List<AssessmentInternalFinishesTransDetailViewModel> detailViewModels)
        {
            return assessmentRepository.CreateAssessmentInternalFinishesMaster(masterViewModel, detailViewModels);
        }

        public int SaveAssessmentInternalFinishes(AssessmentInternalFinishesTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentInternalFinishes(masterViewModel);
        }

        public int SaveAssessmentInternalFinishesDetail(AssessmentInternalFinishesTransDetailViewModel detailViewModel)
        {
            return assessmentRepository.SaveAssessmentInternalFinishesDetail(detailViewModel);
        }

        public int DeleteAssessmentInternalFinishes(string Ids)
        {
            return assessmentRepository.DeleteAssessmentInternalFinishes(Ids);
        }

        public List<AssessmentReportDetailModel> GetAssessmentInternalFinishesByProjectAndModuleID(int? pid, int? mid)
        {
            return assessmentRepository.GetAssessmentInternalFinishesByProjectAndModuleID(pid, mid);
        }

        public bool CheckAssessmentInternalFinishes(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate)
        {
            return assessmentRepository.CheckAssessmentInternalFinishes(pid, Block_Unit, LocationID, AssessmentDate);
        }
        // Internal Finishes

        // External Wall
        public List<AssessmentExternalWallIndexViewModel> GetAllAssessmentExternalWall_List(int? id)
        {
            return assessmentRepository.GetAllAssessmentExternalWall_List(id);
        }

        public List<AssessmentExternalWallTransMasterViewModel> GetAllAssessmentExternalWall(int id, string BatchID = "")
        {
            return assessmentRepository.GetAllAssessmentExternalWall(id, BatchID);
        }

        public AssessmentExternalWallTransMasterViewModel GetAllAssessmentExternalWall_ByID(int id)
        {
            return assessmentRepository.GetAllAssessmentExternalWall_ByID(id);
        }

        public List<AssessmentExternalWallTransDetailViewModel> GetAllAssessmentExternalWall_Detail(List<int> ids)
        {
            return assessmentRepository.GetAllAssessmentExternalWall_Detail(ids);
        }

        public AssessmentExternalWallTransDetailViewModel GetAllAssessmentExternalWall_DetailByID(int id)
        {
            return assessmentRepository.GetAllAssessmentExternalWall_DetailByID(id);
        }

        public int CreateAssessmentExternalWallMaster(AssessmentExternalWallTransMasterViewModel masterViewModel, List<AssessmentExternalWallTransDetailViewModel> detailViewModels)
        {
            return assessmentRepository.CreateAssessmentExternalWallMaster(masterViewModel, detailViewModels);
        }

        public int SaveAssessmentExternalWall(AssessmentExternalWallTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentExternalWall(masterViewModel);
        }

        public int SaveAssessmentExternalWallSignature(AssessmentExternalWallTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentExternalWallSignature(masterViewModel);
        }

        public int SaveAssessmentExternalWallDetail(AssessmentExternalWallTransDetailViewModel detailViewModel)
        {
            return assessmentRepository.SaveAssessmentExternalWallDetail(detailViewModel);
        }

        public int DeleteAssessmentExternalWall(string Ids)
        {
            return assessmentRepository.DeleteAssessmentExternalWall(Ids);
        }

        public bool CheckAssessmentExternalWall(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate)
        {
            return assessmentRepository.CheckAssessmentExternalWall(pid, Block_Unit, LocationID, AssessmentDate);
        }
        // External Wall

        // External Works
        public List<AssessmentExternalWorksIndexViewModel> GetAllAssessmentExternalWorks_List(int? id)
        {
            return assessmentRepository.GetAllAssessmentExternalWorks_List(id);
        }

        public List<AssessmentExternalWorksTransMasterViewModel> GetAllAssessmentExternalWorks(int id, string BatchID = "")
        {
            return assessmentRepository.GetAllAssessmentExternalWorks(id, BatchID);
        }

        public AssessmentExternalWorksTransMasterViewModel GetAllAssessmentExternalWorks_ByID(int id)
        {
            return assessmentRepository.GetAllAssessmentExternalWorks_ByID(id);
        }

        public List<AssessmentExternalWorksTransDetailViewModel> GetAllAssessmentExternalWorks_Detail(List<int> ids)
        {
            return assessmentRepository.GetAllAssessmentExternalWorks_Detail(ids);
        }

        public AssessmentExternalWorksTransDetailViewModel GetAllAssessmentExternalWorks_DetailByID(int id)
        {
            return assessmentRepository.GetAllAssessmentExternalWorks_DetailByID(id);
        }

        public int CreateAssessmentExternalWorksMaster(AssessmentExternalWorksTransMasterViewModel masterViewModel, List<AssessmentExternalWorksTransDetailViewModel> detailViewModels)
        {
            return assessmentRepository.CreateAssessmentExternalWorksMaster(masterViewModel, detailViewModels);
        }

        public int SaveAssessmentExternalWorks(AssessmentExternalWorksTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentExternalWorks(masterViewModel);
        }

        public int SaveAssessmentExternalWorksDetail(AssessmentExternalWorksTransDetailViewModel detailViewModel)
        {
            return assessmentRepository.SaveAssessmentExternalWorksDetail(detailViewModel);
        }

        public int SaveAssessmentExternalWorksSignature(AssessmentExternalWorksTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentExternalWorksSignature(masterViewModel);
        }

        public int DeleteAssessmentExternalWorks(string Ids)
        {
            return assessmentRepository.DeleteAssessmentExternalWorks(Ids);
        }

        public bool CheckAssessmentExternalWorks(int pid, int LocationID, string Remarks, DateTime AssessmentDate)
        {
            return assessmentRepository.CheckAssessmentExternalWorks(pid, LocationID, Remarks, AssessmentDate);
        }
        // External Works

        // Roof Construction
        public List<AssessmentRoofConstructionIndexViewModel> GetAllAssessmentRoofConstruction_List(int? id)
        {
            return assessmentRepository.GetAllAssessmentRoofConstruction_List(id);
        }

        public List<AssessmentRoofConstructionTransMasterViewModel> GetAllAssessmentRoofConstruction(int id, string BatchID = "")
        {
            return assessmentRepository.GetAllAssessmentRoofConstruction(id, BatchID);
        }

        public AssessmentRoofConstructionTransMasterViewModel GetAllAssessmentRoofConstruction_ByID(int id)
        {
            return assessmentRepository.GetAllAssessmentRoofConstruction_ByID(id);
        }

        public List<AssessmentRoofConstructionTransDetailViewModel> GetAllAssessmentRoofConstruction_Detail(List<int> ids)
        {
            return assessmentRepository.GetAllAssessmentRoofConstruction_Detail(ids);
        }

        public AssessmentRoofConstructionTransDetailViewModel GetAllAssessmentRoofConstruction_DetailByID(int id)
        {
            return assessmentRepository.GetAllAssessmentRoofConstruction_DetailByID(id);
        }

        public int CreateAssessmentRoofConstructionMaster(AssessmentRoofConstructionTransMasterViewModel masterViewModel, List<AssessmentRoofConstructionTransDetailViewModel> detailViewModels)
        {
            return assessmentRepository.CreateAssessmentRoofConstructionMaster(masterViewModel, detailViewModels);
        }

        public int SaveAssessmentRoofConstruction(AssessmentRoofConstructionTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentRoofConstruction(masterViewModel);
        }

        public int SaveAssessmentRoofConstructionDetail(AssessmentRoofConstructionTransDetailViewModel detailViewModel)
        {
            return assessmentRepository.SaveAssessmentRoofConstructionDetail(detailViewModel);
        }

        public int SaveAssessmentRoofConstructionSignature(AssessmentRoofConstructionTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentRoofConstructionSignature(masterViewModel);
        }

        public int DeleteAssessmentRoofConstruction(string Ids)
        {
            return assessmentRepository.DeleteAssessmentRoofConstruction(Ids);
        }

        public bool CheckAssessmentRoofConstruction(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate)
        {
            return assessmentRepository.CheckAssessmentRoofConstruction(pid, Block_Unit, LocationID, AssessmentDate);
        }
        // Roof Construction

        // Field Window Water Tightness Test
        public List<AssessmentFieldWindowWaterTightnessTestIndexViewModel> GetAllAssessmentFieldWindowWaterTightnessTest_List(int? id)
        {
            return assessmentRepository.GetAllAssessmentFieldWindowWaterTightnessTest_List(id);
        }

        public List<AssessmentFieldWindowWaterTightnessTestTransViewModel> GetAllAssessmentFieldWindowWaterTightnessTest(int id, string BatchID = "")
        {
            return assessmentRepository.GetAllAssessmentFieldWindowWaterTightnessTest(id, BatchID);
        }

        public AssessmentFieldWindowWaterTightnessTestTransViewModel GetAllAssessmentFieldWindowWaterTightnessTest_ByID(int id)
        {
            return assessmentRepository.GetAllAssessmentFieldWindowWaterTightnessTest_ByID(id);
        }

        public int CreateAssessmentFieldWindowWaterTightnessTest(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel)
        {
            return assessmentRepository.CreateAssessmentFieldWindowWaterTightnessTest(masterViewModel);
        }

        public int SaveAssessmentFieldWindowWaterTightnessTest(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentFieldWindowWaterTightnessTest(masterViewModel);
        }

        public int SaveAssessmentFieldWindowWaterTightnessTestSignature(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentFieldWindowWaterTightnessTestSignature(masterViewModel);
        }

        public int DeleteAssessmentFieldWindowWaterTightnessTest(string Ids)
        {
            return assessmentRepository.DeleteAssessmentFieldWindowWaterTightnessTest(Ids);
        }

        public bool CheckAssessmentFieldWindowWaterTightnessTest(int pid, string Block_Unit, DateTime AssessmentDate, int WallId, int WindowId, int JointId, int DirectionId, int LeakId)
        {
            return assessmentRepository.CheckAssessmentFieldWindowWaterTightnessTest(pid, Block_Unit, AssessmentDate, WallId, WindowId, JointId, DirectionId, LeakId);
        }
        // Field Window Water Tightness Test

        // Wet Area Water Tightness Test
        public List<AssessmentWetAreaWaterTightnessTestIndexViewModel> GetAllAssessmentWetAreaWaterTightnessTest_List(int? id)
        {
            return assessmentRepository.GetAllAssessmentWetAreaWaterTightnessTest_List(id);
        }

        public List<AssessmentWetAreaWaterTightnessTestTransMasterViewModel> GetAllAssessmentWetAreaWaterTightnessTest(int id, string BatchID = "")
        {
            return assessmentRepository.GetAllAssessmentWetAreaWaterTightnessTest(id, BatchID);
        }

        public AssessmentWetAreaWaterTightnessTestTransMasterViewModel GetAllAssessmentWetAreaWaterTightnessTest_ByID(int id)
        {
            return assessmentRepository.GetAllAssessmentWetAreaWaterTightnessTest_ByID(id);
        }

        public List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> GetAllAssessmentWetAreaWaterTightnessTest_Detail(List<int> ids)
        {
            return assessmentRepository.GetAllAssessmentWetAreaWaterTightnessTest_Detail(ids);
        }

        public AssessmentWetAreaWaterTightnessTestTransDetailViewModel GetAllAssessmentWetAreaWaterTightnessTest_DetailByID(int id)
        {
            return assessmentRepository.GetAllAssessmentWetAreaWaterTightnessTest_DetailByID(id);
        }

        public int CreateAssessmentWetAreaWaterTightnessTestMaster(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel, List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> detailViewModels, List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> detailResultViewModels)
        {
            return assessmentRepository.CreateAssessmentWetAreaWaterTightnessTestMaster(masterViewModel, detailViewModels, detailResultViewModels);
        }

        public int SaveAssessmentWetAreaWaterTightnessTest(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentWetAreaWaterTightnessTest(masterViewModel);
        }

        public int SaveAssessmentWetAreaWaterTightnessTestOtherResult(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentWetAreaWaterTightnessTestOtherResult(masterViewModel);
        }

        public int SaveAssessmentWetAreaWaterTightnessTestSignature(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel)
        {
            return assessmentRepository.SaveAssessmentWetAreaWaterTightnessTestSignature(masterViewModel);
        }

        public int SaveAssessmentWetAreaWaterTightnessTestDetail(AssessmentWetAreaWaterTightnessTestTransDetailViewModel detailViewModel)
        {
            return assessmentRepository.SaveAssessmentWetAreaWaterTightnessTestDetail(detailViewModel);
        }

        public int SaveAssessmentWetAreaWaterTightnessTestDetailResult(AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel detailResultViewModel)
        {
            return assessmentRepository.SaveAssessmentWetAreaWaterTightnessTestDetailResult(detailResultViewModel);
        }

        public int DeleteAssessmentWetAreaWaterTightnessTest(string Ids)
        {
            return assessmentRepository.DeleteAssessmentWetAreaWaterTightnessTest(Ids);
        }

        public bool CheckAssessmentWetAreaWaterTightnessTest(int pid, string Block_Unit, DateTime AssessmentDate)
        {
            return assessmentRepository.CheckAssessmentWetAreaWaterTightnessTest(pid, Block_Unit, AssessmentDate);
        }
        // Wet Area Water Tightness Test
    }
}