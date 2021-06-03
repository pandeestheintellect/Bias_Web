using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Repository.Imp
{
    public interface IQCInspectionRepository
    {
        // Project
        QCInspectionProjectMasterViewModel GetProject(int pid);
        int CreateProject(QCInspectionProjectMasterViewModel project, List<QCInspectionProjectPMDetailViewModel> PMViewModels, List<QCInspectionProjectSupervisorDetailViewModel> SupervisorViewModels, List<QCInspectionProjectMEInspectorDetailViewModel> MEInspectorViewModels, List<QCInspectionProjectStructureInspectorDetailViewModel> StructureInspectorViewModels, List<QCInspectionProjectOtherInspectorDetailViewModel> OtherInspectorViewModels);
        int SaveProject(QCInspectionProjectMasterViewModel project, List<QCInspectionProjectPMDetailViewModel> PMViewModels, List<QCInspectionProjectSupervisorDetailViewModel> SupervisorViewModels, List<QCInspectionProjectMEInspectorDetailViewModel> MEInspectorViewModels, List<QCInspectionProjectStructureInspectorDetailViewModel> StructureInspectorViewModels, List<QCInspectionProjectOtherInspectorDetailViewModel> OtherInspectorViewModels);
        int DeleteProject(int pID);
        List<QCInspectionProjectMasterViewModel> GetAllProjects();
        List<QCInspectionProjectMasterViewModel> GetAllProjectsBasedCompanyID(int? id);
        List<QCInspectionProjectPMDetailViewModel> GetAllPMDetailByProjectID(int Id);
        List<QCInspectionProjectSupervisorDetailViewModel> GetAllSupervisorDetailByProjectID(int Id);
        List<QCInspectionProjectMEInspectorDetailViewModel> GetAllMEInspectorDetailByProjectID(int Id);
        List<QCInspectionProjectStructureInspectorDetailViewModel> GetAllStructureInspectorDetailByProjectID(int Id);
        List<QCInspectionProjectOtherInspectorDetailViewModel> GetAllOtherInspectorDetailByProjectID(int Id);
        bool CheckProject(int ProjectID, string projectname);
        int CompletedProject(int pid);
        // Project

        // Masters

        //Subcontractor
        QCInspectionSubcontractorMasterViewModel GetSubcontractor(int Id);
        int CreateSubcontractor(QCInspectionSubcontractorMasterViewModel subcontractor, List<QCInspectionSubcontractorTradeDetailViewModel> detailViewModels);
        int SaveSubcontractor(QCInspectionSubcontractorMasterViewModel subcontractor, List<QCInspectionSubcontractorTradeDetailViewModel> detailViewModels);
        int DeleteSubcontractor(int Id);
        List<QCInspectionSubcontractorMasterViewModel> GetAllSubcontractors();
        List<QCInspectionSubcontractorMasterViewModel> GetAllSubcontractorsBasedCompanyID(int Id);
        bool CheckSubcontractor(int CompanyID, int SubcontractorID, string SubcontractorName);
        //Subcontractor

        //Block
        QCInspectionBlockMasterViewModel GetBlock(int Id);
        int CreateBlock(QCInspectionBlockMasterViewModel Block);
        int SaveBlock(QCInspectionBlockMasterViewModel Block);
        int DeleteBlock(int Id);
        List<QCInspectionBlockMasterViewModel> GetAllBlocks();
        bool CheckBlock(int ProjectID, int BlockID, string BlockName);
        //Block

        //Level
        QCInspectionLevelMasterViewModel GetLevel(int Id);
        int CreateLevel(QCInspectionLevelMasterViewModel Level);
        int SaveLevel(QCInspectionLevelMasterViewModel Level);
        int DeleteLevel(int Id);
        List<QCInspectionLevelMasterViewModel> GetAllLevels();
        bool CheckLevel(int BlockID, int LevelID, string LevelName);
        //Level

        //Unit
        QCInspectionUnitMasterViewModel GetUnit(int Id);
        int CreateUnit(QCInspectionUnitMasterViewModel Unit);
        int SaveUnit(QCInspectionUnitMasterViewModel Unit);
        int DeleteUnit(int Id);
        List<QCInspectionUnitMasterViewModel> GetAllUnits();
        bool CheckUnit(int LevelID, int UnitID, string UnitName);
        //Unit

        //RFWI Drawings Reference File
        QCInspectionProjectRFWIDrawingReferenceFilesViewModel GetRFWIDrawingsReferenceFile(int Id);
        int CreateRFWIDrawingsReferenceFile(QCInspectionProjectRFWIDrawingReferenceFilesViewModel RFWIDrawingsReferenceFile);
        int DeleteRFWIDrawingsReferenceFile(int Id);
        List<QCInspectionProjectRFWIDrawingReferenceFilesViewModel> GetAllRFWIDrawingsReferenceFiles(int Id);
        bool CheckRFWIDrawingsReferenceFile(int ProjectID, string FileCaption);
        //RFWI Drawings Reference File

        //RFWI Project File
        QCInspectionProjectFilesViewModel GetProjectFile(int Id);
        int CreateProjectFile(QCInspectionProjectFilesViewModel RFWIFile);
        int DeleteProjectFile(int Id);
        List<QCInspectionProjectFilesViewModel> GetAllProjectFiles(int Id);
        bool CheckProjectFile(int ProjectID, string FileCaption);
        //RFWI  File

        //Defect Type
        QCInspectionDefectTypeMasterViewModel GetDefectType(int Id);
        int CreateDefectType(QCInspectionDefectTypeMasterViewModel DefectType);
        int SaveDefectType(QCInspectionDefectTypeMasterViewModel DefectType);
        int DeleteDefectType(int Id);
        List<QCInspectionDefectTypeMasterViewModel> GetAllDefectTypes();
        bool CheckDefectType(int DefectTypeID, string DefectTypeName);
        //Defect Type

        //Trade
        QCInspectionTradeMasterViewModel GetTrade(int Id);
        int CreateTrade(QCInspectionTradeMasterViewModel Trade);
        int SaveTrade(QCInspectionTradeMasterViewModel Trade);
        int DeleteTrade(int Id);
        List<QCInspectionTradeMasterViewModel> GetAllTrades();
        bool CheckTrade(int TradeID, string TradeName);
        //Trade

        //RFWI General CheckList
        QCInspectionRFWIGeneralCheckListMasterViewModel GetRFWIGeneralCheckList(int Id);
        int CreateRFWIGeneralCheckList(QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckList);
        int SaveRFWIGeneralCheckList(QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckList);
        int DeleteRFWIGeneralCheckList(int Id);
        List<QCInspectionRFWIGeneralCheckListMasterViewModel> GetAllRFWIGeneralCheckLists();
        bool CheckRFWIGeneralCheckList(int GeneralCheckListID, string GeneralCheckListName);
        //RFWI General CheckList

        //RFWITrade
        QCInspectionRFWITradeMasterViewModel GetRFWITrade(int Id);
        int CreateRFWITrade(QCInspectionRFWITradeMasterViewModel RFWITrade);
        int SaveRFWITrade(QCInspectionRFWITradeMasterViewModel RFWITrade);
        int DeleteRFWITrade(int Id);
        List<QCInspectionRFWITradeMasterViewModel> GetAllRFWITrades();
        bool CheckRFWITrade(int TradeID, string RFWITradeName);
        //RFWITrade

        //RFWI Trade Item
        QCInspectionRFWITradeItemDetailViewModel GetRFWITradeItem(int Id);
        int CreateRFWITradeItem(QCInspectionRFWITradeItemDetailViewModel RFWITradeItem);
        int SaveRFWITradeItem(QCInspectionRFWITradeItemDetailViewModel RFWITradeItem);
        int DeleteRFWITradeItem(int Id);
        List<QCInspectionRFWITradeItemDetailViewModel> GetAllRFWITradeItems();
        bool CheckRFWITradeItem(int TradeID, int TradeItemID, string ItemName);
        //RFWI Trade Item

        //RFWI Trade Detailed CheckList
        QCInspectionRFWITradeDetailedCheckListDetailViewModel GetRFWITradeDetailedCheckList(int Id);
        int CreateRFWITradeDetailedCheckList(QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeItem);
        int SaveRFWITradeDetailedCheckList(QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeItem);
        int DeleteRFWITradeDetailedCheckList(int Id);
        List<QCInspectionRFWITradeDetailedCheckListDetailViewModel> GetAllRFWITradeDetailedCheckLists();
        bool CheckRFWITradeDetailedCheckList(int TradeID, int TradeDetailedCheckListID, string DetailedCheckListName);
        //RFWI Trade Item

        List<MasterSyncViewModel> GetQCInspectionAndRFWIMasterSync();
        // Masters

        // Transactions
        // QC Inspection Defect Form
        QCInspectionDefectFormViewModel GetQCInspectionDefectForm(int Id);
        int CreateQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int SaveQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int DeleteQCInspectionDefectForm(int Id);
        int ApprovedQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int ReDoQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int ReDoDoneQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int RectificationQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int ReworkQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int ReworkDoneQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int CompletedQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int MobileSaveQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm);
        int MobileDeleteQCInspectionDefectForm(string Ids, string UserID);
        List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectForms();
        List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedProjectID(int Id, string BatchID = "");
        List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedUserID(int Id);
        List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedSubcontractorID(int Id);
        // QC Inspection Defect Form

        // RFWI Form
        QCInspectionRFWIFormViewModel GetRFWIForm(int Id);
        int CreateRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int SaveRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int DeleteRFWIForm(int Id);
        int MandESignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int StructureSignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int OtherSignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int RequestedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int CompletedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int RejectedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int ReInspectionRFWIForm(int Id);
        int MobileSaveRFWIForm(QCInspectionRFWIFormViewModel RFWIForm);
        int MobileDeleteRFWIForm(string Ids, string UserID);
        bool ValidateInspectionDateTimeSlot(DateTime InspectionOn, TimeSpan InspectionStartOn, TimeSpan InspectionEndOn);
        List<QCInspectionRFWIFormViewModel> GetAllRFWIForms();
        List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedProjectID(int Id, string BatchID = "");
        List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedUserID(int Id);
        List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedRTOInspectorID(int Id);
        // RFWI Form

        //RFWI Drawings Reference File
        QCInspectionRFWIFormLocationDetailViewModel GetRFWIFormLocationDetail(int Id);
        int CreateRFWIFormLocationDetail(QCInspectionRFWIFormLocationDetailViewModel RFWIFormLocation);
        int DeleteRFWIFormLocationDetail(int Id);
        List<QCInspectionRFWIFormLocationDetailViewModel> GetAllRFWIFormsLocationDetails(int Id);
        bool CheckRFWIFormLocationDetail(int QCInspectionRFWIFormID, int RFWIFormLocationDetailID, int UnitID);
        //RFWI Drawings Reference File

        // Transactions
    }
}