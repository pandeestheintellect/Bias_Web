using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Imp
{
    public interface IAssessmentServices
    {
        // Project
        AssessmentProjectMasterViewModel GetProject(int pid);
        int CreateProject(AssessmentProjectMasterViewModel project, List<AssessmentProjectAssessorsDetailViewModel> detailViewModels);
        int SaveProject(AssessmentProjectMasterViewModel project, List<AssessmentProjectAssessorsDetailViewModel> detailViewModels);
        int DeleteProject(int pID);
        List<AssessmentProjectMasterViewModel> GetAllProjects();
        List<AssessmentProjectMasterViewModel> GetAllProjectsBasedCompanyID(int? id);
        bool CheckProject(string projectname);
        int CompletedProject(int pid);
        // Project

        // Masters
        //Assessors
        AssessorsMasterViewModel GetAssessor(int aid);
        int CreateAssessor(AssessorsMasterViewModel Assessor);
        int SaveAssessor(AssessorsMasterViewModel Assessor);
        int DeleteAssessor(int aID);
        List<AssessorsMasterViewModel> GetAllAssessors();
        List<AssessorsMasterViewModel> GetAllAssessorsBasedCompanyID(int? id);
        bool CheckAssessor(string Assessorsname);
        //Assessors

        List<AssessmentDevelopmentTypeMasterViewModel> GetAllDevelopmentTypes();
        List<AssessmentTypeMasterViewModel> GetAllAssessmentTypes();
        //Location
        AssessmentTypeLocationMasterViewModel GetLocation(int Id);
        int CreateLocation(AssessmentTypeLocationMasterViewModel Location);
        int SaveLocation(AssessmentTypeLocationMasterViewModel Location);
        int DeleteLocation(int Id);
        List<AssessmentTypeLocationMasterViewModel> GetAllAssessmentLocations();
        bool CheckLocation(int AssessmentTypeID, string LocationName);
        //Location

        //Module
        AssessmentTypeModuleMasterViewModel GetModule(int Id);
        int CreateModule(AssessmentTypeModuleMasterViewModel Module);
        int SaveModule(AssessmentTypeModuleMasterViewModel Module);
        int DeleteModule(int Id);
        List<AssessmentTypeModuleMasterViewModel> GetAllModules();
        bool CheckModule(int? AssessmentTypeID, string ModuleName);
        //Module

        //Process
        AssessmentTypeModuleProcessMasterViewModel GetModuleProcess(int Id);
        int CreateModuleProcess(AssessmentTypeModuleProcessMasterViewModel ModuleProcess);
        int SaveModuleProcess(AssessmentTypeModuleProcessMasterViewModel ModuleProcess);
        int DeleteModuleProcess(int Id);
        List<AssessmentTypeModuleProcessMasterViewModel> GetAllModuleProcess();
        List<AssessmentTypeModuleProcessMasterViewModel> GetAllModuleProcessByModuleIds(List<int> ids);
        bool CheckModuleProcess(int ModuleID, string ProcessName);
        //Process

        //Directions
        List<AssessmentDirectionMasterViewModel> GetAllDirections();
        //Directions

        //Joint
        AssessmentJointMasterViewModel GetJoint(int Id);
        int CreateJoint(AssessmentJointMasterViewModel Joint);
        int SaveJoint(AssessmentJointMasterViewModel Joint);
        int DeleteJoint(int Id);
        List<AssessmentJointMasterViewModel> GetAllJoints();
        bool CheckJoint(string JointName);
        //Joint

        //Leak
        AssessmentLeakMasterViewModel GetLeak(int Id);
        int CreateLeak(AssessmentLeakMasterViewModel Leak);
        int SaveLeak(AssessmentLeakMasterViewModel Leak);
        int DeleteLeak(int Id);
        List<AssessmentLeakMasterViewModel> GetAllLeaks();
        bool CheckLeak(string JointName);
        //Leak

        //Wall
        AssessmentWallMasterViewModel GetWall(int Id);
        int CreateWall(AssessmentWallMasterViewModel Wall);
        int SaveWall(AssessmentWallMasterViewModel Wall);
        int DeleteWall(int Id);
        List<AssessmentWallMasterViewModel> GetAllWalls();
        bool CheckWall(string WallName);
        //Wall

        //Window
        AssessmentWindowMasterViewModel GetWindow(int Id);
        int CreateWindow(AssessmentWindowMasterViewModel Window);
        int SaveWindow(AssessmentWindowMasterViewModel Window);
        int DeleteWindow(int Id);
        List<AssessmentWindowMasterViewModel> GetAllWindows();
        bool CheckWindow(string WindowName);
        //Window

        //Wet Area Water Tightness Test Result
        AssessmentWetAreaWaterTightnessTestResultMasterViewModel GetWetAreaWaterTightnessTestResult(int Id);
        int CreateWetAreaWaterTightnessTestResult(AssessmentWetAreaWaterTightnessTestResultMasterViewModel WAWTTResult);
        int SaveWetAreaWaterTightnessTestResult(AssessmentWetAreaWaterTightnessTestResultMasterViewModel WAWTTResult);
        int DeleteWetAreaWaterTightnessTestResult(int Id);
        List<AssessmentWetAreaWaterTightnessTestResultMasterViewModel> GetAllWetAreaWaterTightnessTestResults();
        bool CheckWetAreaWaterTightnessTestResult(string WAWTTResultName);
        //Wet Area Water Tightness Test Result

        List<MasterSyncViewModel> GetAssessmentMasterSync();
        // Masters

        // Transactions
        List<AssessmentSummaryDetailModel> GetAssessmentSummaryByProjectID(int? id);
        // Internal Finishes
        List<AssessmentInternalFinishesIndexViewModel> GetAllAssessmentInternalFinishes_List(int? id);
        List<AssessmentInternalFinishesTransMasterViewModel> GetAllAssessmentInternalFinishes(int id, string BatchID = "");
        AssessmentInternalFinishesTransMasterViewModel GetAllAssessmentInternalFinishes_ByID(int id);
        List<AssessmentInternalFinishesTransDetailViewModel> GetAllAssessmentInternalFinishes_Detail(List<int> ids);
        AssessmentInternalFinishesTransDetailViewModel GetAllAssessmentInternalFinishes_DetailByID(int id);
        int CreateAssessmentInternalFinishesMaster(AssessmentInternalFinishesTransMasterViewModel masterViewModel, List<AssessmentInternalFinishesTransDetailViewModel> detailViewModels);
        int SaveAssessmentInternalFinishes(AssessmentInternalFinishesTransMasterViewModel masterViewModel);
        int SaveAssessmentInternalFinishesDetail(AssessmentInternalFinishesTransDetailViewModel detailViewModel);
        int DeleteAssessmentInternalFinishes(string Ids);
        List<AssessmentReportDetailModel> GetAssessmentInternalFinishesByProjectAndModuleID(int? pid, int? mid);
        bool CheckAssessmentInternalFinishes(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate);
        // Internal Finishes

        // External Wall
        List<AssessmentExternalWallIndexViewModel> GetAllAssessmentExternalWall_List(int? id);
        List<AssessmentExternalWallTransMasterViewModel> GetAllAssessmentExternalWall(int id, string BatchID = "");
        AssessmentExternalWallTransMasterViewModel GetAllAssessmentExternalWall_ByID(int id);
        List<AssessmentExternalWallTransDetailViewModel> GetAllAssessmentExternalWall_Detail(List<int> ids);
        AssessmentExternalWallTransDetailViewModel GetAllAssessmentExternalWall_DetailByID(int id);
        int CreateAssessmentExternalWallMaster(AssessmentExternalWallTransMasterViewModel masterViewModel, List<AssessmentExternalWallTransDetailViewModel> detailViewModels);
        int SaveAssessmentExternalWall(AssessmentExternalWallTransMasterViewModel masterViewModel);
        int SaveAssessmentExternalWallSignature(AssessmentExternalWallTransMasterViewModel masterViewModel);
        int SaveAssessmentExternalWallDetail(AssessmentExternalWallTransDetailViewModel detailViewModel);
        int DeleteAssessmentExternalWall(string Ids);
        bool CheckAssessmentExternalWall(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate);
        // External Wall

        // External Works
        List<AssessmentExternalWorksIndexViewModel> GetAllAssessmentExternalWorks_List(int? id);
        List<AssessmentExternalWorksTransMasterViewModel> GetAllAssessmentExternalWorks(int id, string BatchID = "");
        AssessmentExternalWorksTransMasterViewModel GetAllAssessmentExternalWorks_ByID(int id);
        List<AssessmentExternalWorksTransDetailViewModel> GetAllAssessmentExternalWorks_Detail(List<int> ids);
        AssessmentExternalWorksTransDetailViewModel GetAllAssessmentExternalWorks_DetailByID(int id);
        int CreateAssessmentExternalWorksMaster(AssessmentExternalWorksTransMasterViewModel masterViewModel, List<AssessmentExternalWorksTransDetailViewModel> detailViewModels);
        int SaveAssessmentExternalWorks(AssessmentExternalWorksTransMasterViewModel masterViewModel);
        int SaveAssessmentExternalWorksSignature(AssessmentExternalWorksTransMasterViewModel masterViewModel);
        int SaveAssessmentExternalWorksDetail(AssessmentExternalWorksTransDetailViewModel detailViewModel);
        int DeleteAssessmentExternalWorks(string Ids);
        bool CheckAssessmentExternalWorks(int pid, int LocationID, string Remarks, DateTime AssessmentDate);
        // External Works

        // Roof Construction
        List<AssessmentRoofConstructionIndexViewModel> GetAllAssessmentRoofConstruction_List(int? id);
        List<AssessmentRoofConstructionTransMasterViewModel> GetAllAssessmentRoofConstruction(int id, string BatchID = "");
        AssessmentRoofConstructionTransMasterViewModel GetAllAssessmentRoofConstruction_ByID(int id);
        List<AssessmentRoofConstructionTransDetailViewModel> GetAllAssessmentRoofConstruction_Detail(List<int> ids);
        AssessmentRoofConstructionTransDetailViewModel GetAllAssessmentRoofConstruction_DetailByID(int id);
        int CreateAssessmentRoofConstructionMaster(AssessmentRoofConstructionTransMasterViewModel masterViewModel, List<AssessmentRoofConstructionTransDetailViewModel> detailViewModels);
        int SaveAssessmentRoofConstruction(AssessmentRoofConstructionTransMasterViewModel masterViewModel);
        int SaveAssessmentRoofConstructionSignature(AssessmentRoofConstructionTransMasterViewModel masterViewModel);
        int SaveAssessmentRoofConstructionDetail(AssessmentRoofConstructionTransDetailViewModel detailViewModel);
        int DeleteAssessmentRoofConstruction(string Ids);
        bool CheckAssessmentRoofConstruction(int pid, string Block_Unit, int LocationID, DateTime AssessmentDate);
        // Roof Construction


        // Field Window Water Tightness Test
        List<AssessmentFieldWindowWaterTightnessTestIndexViewModel> GetAllAssessmentFieldWindowWaterTightnessTest_List(int? id);
        List<AssessmentFieldWindowWaterTightnessTestTransViewModel> GetAllAssessmentFieldWindowWaterTightnessTest(int id, string BatchID = "");
        AssessmentFieldWindowWaterTightnessTestTransViewModel GetAllAssessmentFieldWindowWaterTightnessTest_ByID(int id);
        int CreateAssessmentFieldWindowWaterTightnessTest(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel);
        int SaveAssessmentFieldWindowWaterTightnessTest(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel);
        int SaveAssessmentFieldWindowWaterTightnessTestSignature(AssessmentFieldWindowWaterTightnessTestTransViewModel masterViewModel);
        int DeleteAssessmentFieldWindowWaterTightnessTest(string Ids);
        bool CheckAssessmentFieldWindowWaterTightnessTest(int pid, string Block_Unit, DateTime AssessmentDate, int WallId, int WindowId, int JointId, int DirectionId, int LeakId);
        // Field Window Water Tightness Test

        // Wet Area Water Tightness Test
        List<AssessmentWetAreaWaterTightnessTestIndexViewModel> GetAllAssessmentWetAreaWaterTightnessTest_List(int? id);
        List<AssessmentWetAreaWaterTightnessTestTransMasterViewModel> GetAllAssessmentWetAreaWaterTightnessTest(int id, string BatchID = "");
        AssessmentWetAreaWaterTightnessTestTransMasterViewModel GetAllAssessmentWetAreaWaterTightnessTest_ByID(int id);
        List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> GetAllAssessmentWetAreaWaterTightnessTest_Detail(List<int> ids);
        AssessmentWetAreaWaterTightnessTestTransDetailViewModel GetAllAssessmentWetAreaWaterTightnessTest_DetailByID(int id);
        List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> GetAllAssessmentWetAreaWaterTightnessTest_DetailResult(List<int> ids);
        AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel GetAllAssessmentWetAreaWaterTightnessTest_DetailResultByID(int id);
        int CreateAssessmentWetAreaWaterTightnessTestMaster(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel, List<AssessmentWetAreaWaterTightnessTestTransDetailViewModel> detailViewModels, List<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel> detailResultViewModels);
        int SaveAssessmentWetAreaWaterTightnessTest(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel);
        int SaveAssessmentWetAreaWaterTightnessTestOtherResult(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel);
        int SaveAssessmentWetAreaWaterTightnessTestSignature(AssessmentWetAreaWaterTightnessTestTransMasterViewModel masterViewModel);
        int SaveAssessmentWetAreaWaterTightnessTestDetail(AssessmentWetAreaWaterTightnessTestTransDetailViewModel detailViewModel);
        int SaveAssessmentWetAreaWaterTightnessTestDetailResult(AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel detailResultViewModel);
        int DeleteAssessmentWetAreaWaterTightnessTest(string Ids);
        bool CheckAssessmentWetAreaWaterTightnessTest(int pid, string Block_Unit, DateTime AssessmentDate);
        // Wet Area Water Tightness Test
    }
}