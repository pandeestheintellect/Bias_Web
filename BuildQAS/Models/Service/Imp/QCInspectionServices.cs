using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Interface
{
    public class QCInspectionServices : IQCInspectionServices
    {
        private readonly IQCInspectionRepository qcInspectionRepository;
        public QCInspectionServices(IQCInspectionRepository _qcInspectionRepository)
        {
            qcInspectionRepository = _qcInspectionRepository;
        }

        // Project
        public List<QCInspectionProjectMasterViewModel> GetAllProjects()
        {
            return qcInspectionRepository.GetAllProjects();
        }

        public List<QCInspectionProjectMasterViewModel> GetAllProjectsBasedCompanyID(int id)
        {
            return qcInspectionRepository.GetAllProjectsBasedCompanyID(id);
        }

        public List<QCInspectionProjectPMDetailViewModel> GetAllPMDetailByProjectID(int Id)
        {
            return qcInspectionRepository.GetAllPMDetailByProjectID(Id);
        }

        public List<QCInspectionProjectSupervisorDetailViewModel> GetAllSupervisorDetailByProjectID(int Id)
        {
            return qcInspectionRepository.GetAllSupervisorDetailByProjectID(Id);
        }

        public List<QCInspectionProjectMEInspectorDetailViewModel> GetAllMEInspectorDetailByProjectID(int Id)
        {
            return qcInspectionRepository.GetAllMEInspectorDetailByProjectID(Id);
        }

        public List<QCInspectionProjectStructureInspectorDetailViewModel> GetAllStructureInspectorDetailByProjectID(int Id)
        {
            return qcInspectionRepository.GetAllStructureInspectorDetailByProjectID(Id);
        }

        public List<QCInspectionProjectOtherInspectorDetailViewModel> GetAllOtherInspectorDetailByProjectID(int Id)
        {
            return qcInspectionRepository.GetAllOtherInspectorDetailByProjectID(Id);
        }

        public QCInspectionProjectMasterViewModel GetProject(int pid)
        {
            return qcInspectionRepository.GetProject(pid);
        }

        public int CreateProject(QCInspectionProjectMasterViewModel project, List<QCInspectionProjectPMDetailViewModel> PMViewModels, List<QCInspectionProjectSupervisorDetailViewModel> SupervisorViewModels, List<QCInspectionProjectMEInspectorDetailViewModel> MEInspectorViewModels, List<QCInspectionProjectStructureInspectorDetailViewModel> StructureInspectorViewModels, List<QCInspectionProjectOtherInspectorDetailViewModel> OtherInspectorViewModels)
        {
            return qcInspectionRepository.CreateProject(project, PMViewModels, SupervisorViewModels,MEInspectorViewModels, StructureInspectorViewModels,OtherInspectorViewModels);
        }

        public int SaveProject(QCInspectionProjectMasterViewModel project, List<QCInspectionProjectPMDetailViewModel> PMViewModels, List<QCInspectionProjectSupervisorDetailViewModel> SupervisorViewModels, List<QCInspectionProjectMEInspectorDetailViewModel> MEInspectorViewModels, List<QCInspectionProjectStructureInspectorDetailViewModel> StructureInspectorViewModels, List<QCInspectionProjectOtherInspectorDetailViewModel> OtherInspectorViewModels)
        {
            return qcInspectionRepository.SaveProject(project, PMViewModels, SupervisorViewModels, MEInspectorViewModels, StructureInspectorViewModels, OtherInspectorViewModels);
        }

        public int DeleteProject(int pID)
        {
            return qcInspectionRepository.DeleteProject(pID);
        }

        public bool CheckProject(int ProjectID, string projectname)
        {
            return qcInspectionRepository.CheckProject(ProjectID, projectname);
        }

        public int CompletedProject(int pid)
        {
            return qcInspectionRepository.CompletedProject(pid);
        }
        // Project

        // Masters

        //Subcontractor
        public List<QCInspectionSubcontractorMasterViewModel> GetAllSubcontractors()
        {
            return qcInspectionRepository.GetAllSubcontractors();
        }

        public List<QCInspectionSubcontractorMasterViewModel> GetAllSubcontractorsBasedCompanyID(int Id)
        {
            return qcInspectionRepository.GetAllSubcontractorsBasedCompanyID(Id);
        }

        public QCInspectionSubcontractorMasterViewModel GetSubcontractor(int Id)
        {
            return qcInspectionRepository.GetSubcontractor(Id);
        }

        public int CreateSubcontractor(QCInspectionSubcontractorMasterViewModel subcontractor, List<QCInspectionSubcontractorTradeDetailViewModel> detailViewModels)
        {
            return qcInspectionRepository.CreateSubcontractor(subcontractor, detailViewModels);
        }

        public int SaveSubcontractor(QCInspectionSubcontractorMasterViewModel subcontractor, List<QCInspectionSubcontractorTradeDetailViewModel> detailViewModels)
        {
            return qcInspectionRepository.SaveSubcontractor(subcontractor, detailViewModels);
        }

        public int DeleteSubcontractor(int Id)
        {
            return qcInspectionRepository.DeleteSubcontractor(Id);
        }

        public bool CheckSubcontractor(int CompanyID, int SubcontractorID, string SubcontractorName)
        {
            return qcInspectionRepository.CheckSubcontractor(CompanyID, SubcontractorID, SubcontractorName);
        }
        //Subcontractor

        //Block
        public QCInspectionBlockMasterViewModel GetBlock(int Id)
        {
            return qcInspectionRepository.GetBlock(Id);
        }

        public int CreateBlock(QCInspectionBlockMasterViewModel Block)
        {
            return qcInspectionRepository.CreateBlock(Block);
        }

        public int SaveBlock(QCInspectionBlockMasterViewModel Block)
        {
            return qcInspectionRepository.SaveBlock(Block);
        }

        public int DeleteBlock(int Id)
        {
            return qcInspectionRepository.DeleteBlock(Id);
        }

        public List<QCInspectionBlockMasterViewModel> GetAllBlocks()
        {
            return qcInspectionRepository.GetAllBlocks();
        }

        public bool CheckBlock(int ProjectID, int BlockID, string BlockName)
        {
            return qcInspectionRepository.CheckBlock(ProjectID, BlockID, BlockName);
        }
        //Block

        //Level
        public QCInspectionLevelMasterViewModel GetLevel(int Id)
        {
            return qcInspectionRepository.GetLevel(Id);
        }

        public int CreateLevel(QCInspectionLevelMasterViewModel Level)
        {
            return qcInspectionRepository.CreateLevel(Level);
        }

        public int SaveLevel(QCInspectionLevelMasterViewModel Level)
        {
            return qcInspectionRepository.SaveLevel(Level);
        }

        public int DeleteLevel(int Id)
        {
            return qcInspectionRepository.DeleteLevel(Id);
        }

        public List<QCInspectionLevelMasterViewModel> GetAllLevels()
        {
            return qcInspectionRepository.GetAllLevels();
        }

        public bool CheckLevel(int BlockID, int LevelID, string LevelName)
        {
            return qcInspectionRepository.CheckLevel(BlockID, LevelID, LevelName);
        }
        //Level

        //Unit
        public QCInspectionUnitMasterViewModel GetUnit(int Id)
        {
            return qcInspectionRepository.GetUnit(Id);
        }

        public int CreateUnit(QCInspectionUnitMasterViewModel Unit)
        {
            return qcInspectionRepository.CreateUnit(Unit);
        }

        public int SaveUnit(QCInspectionUnitMasterViewModel Unit)
        {
            return qcInspectionRepository.SaveUnit(Unit);
        }

        public int DeleteUnit(int Id)
        {
            return qcInspectionRepository.DeleteUnit(Id);
        }

        public List<QCInspectionUnitMasterViewModel> GetAllUnits()
        {
            return qcInspectionRepository.GetAllUnits();
        }

        public bool CheckUnit(int LevelID, int UnitID, string UnitName)
        {
            return qcInspectionRepository.CheckUnit(LevelID, UnitID, UnitName);
        }
        //Unit

        //RFWI Drawings Reference File
        public QCInspectionProjectRFWIDrawingReferenceFilesViewModel GetRFWIDrawingsReferenceFile(int Id)
        {
            return qcInspectionRepository.GetRFWIDrawingsReferenceFile(Id);
        }

        public int CreateRFWIDrawingsReferenceFile(QCInspectionProjectRFWIDrawingReferenceFilesViewModel RFWIDrawingsReferenceFile)
        {
            return qcInspectionRepository.CreateRFWIDrawingsReferenceFile(RFWIDrawingsReferenceFile);
        }

        public int DeleteRFWIDrawingsReferenceFile(int Id)
        {
            return qcInspectionRepository.DeleteRFWIDrawingsReferenceFile(Id);
        }

        public List<QCInspectionProjectRFWIDrawingReferenceFilesViewModel> GetAllRFWIDrawingsReferenceFiles(int Id)
        {
            return qcInspectionRepository.GetAllRFWIDrawingsReferenceFiles(Id);
        }

        public bool CheckRFWIDrawingsReferenceFile(int ProjectID, string FileCaption)
        {
            return qcInspectionRepository.CheckRFWIDrawingsReferenceFile(ProjectID, FileCaption);
        }
        //RFWI Drawings Reference File

        //RFWI Project File
        public QCInspectionProjectFilesViewModel GetProjectFile(int Id)
        {
            return qcInspectionRepository.GetProjectFile(Id);
        }

        public int CreateProjectFile(QCInspectionProjectFilesViewModel RFWIFile)
        {
            return qcInspectionRepository.CreateProjectFile(RFWIFile);
        }

        public int DeleteProjectFile(int Id)
        {
            return qcInspectionRepository.DeleteProjectFile(Id);
        }

        public List<QCInspectionProjectFilesViewModel> GetAllProjectFiles(int Id)
        {
            return qcInspectionRepository.GetAllProjectFiles(Id);
        }

        public bool CheckProjectFile(int ProjectID, string FileCaption)
        {
            return qcInspectionRepository.CheckProjectFile(ProjectID, FileCaption);
        }
        //RFWI  File

        //Defect Type
        public QCInspectionDefectTypeMasterViewModel GetDefectType(int Id)
        {
            return qcInspectionRepository.GetDefectType(Id);
        }

        public int CreateDefectType(QCInspectionDefectTypeMasterViewModel DefectType)
        {
            return qcInspectionRepository.CreateDefectType(DefectType);
        }

        public int SaveDefectType(QCInspectionDefectTypeMasterViewModel DefectType)
        {
            return qcInspectionRepository.SaveDefectType(DefectType);
        }

        public int DeleteDefectType(int Id)
        {
            return qcInspectionRepository.DeleteDefectType(Id);
        }

        public List<QCInspectionDefectTypeMasterViewModel> GetAllDefectTypes()
        {
            return qcInspectionRepository.GetAllDefectTypes();
        }

        public bool CheckDefectType(int DefectTypeID, string DefectTypeName)
        {
            return qcInspectionRepository.CheckDefectType(DefectTypeID, DefectTypeName);
        }
        //Defect Type

        //Trade
        public QCInspectionTradeMasterViewModel GetTrade(int Id)
        {
            return qcInspectionRepository.GetTrade(Id);
        }

        public int CreateTrade(QCInspectionTradeMasterViewModel Trade)
        {
            return qcInspectionRepository.CreateTrade(Trade);
        }

        public int SaveTrade(QCInspectionTradeMasterViewModel Trade)
        {
            return qcInspectionRepository.SaveTrade(Trade);
        }

        public int DeleteTrade(int Id)
        {
            return qcInspectionRepository.DeleteTrade(Id);
        }

        public List<QCInspectionTradeMasterViewModel> GetAllTrades()
        {
            return qcInspectionRepository.GetAllTrades();
        }

        public bool CheckTrade(int TradeID, string TradeName)
        {
            return qcInspectionRepository.CheckTrade(TradeID, TradeName);
        }
        //Trade

        //RFWI General CheckList
        public QCInspectionRFWIGeneralCheckListMasterViewModel GetRFWIGeneralCheckList(int Id)
        {
            return qcInspectionRepository.GetRFWIGeneralCheckList(Id);
        }

        public int CreateRFWIGeneralCheckList(QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckList)
        {
            return qcInspectionRepository.CreateRFWIGeneralCheckList(RFWIGeneralCheckList);
        }

        public int SaveRFWIGeneralCheckList(QCInspectionRFWIGeneralCheckListMasterViewModel RFWIGeneralCheckList)
        {
            return qcInspectionRepository.SaveRFWIGeneralCheckList(RFWIGeneralCheckList);
        }

        public int DeleteRFWIGeneralCheckList(int Id)
        {
            return qcInspectionRepository.DeleteRFWIGeneralCheckList(Id);
        }

        public List<QCInspectionRFWIGeneralCheckListMasterViewModel> GetAllRFWIGeneralCheckLists()
        {
            return qcInspectionRepository.GetAllRFWIGeneralCheckLists();
        }

        public bool CheckRFWIGeneralCheckList(int GeneralCheckListID, string GeneralCheckListName)
        {
            return qcInspectionRepository.CheckRFWIGeneralCheckList(GeneralCheckListID, GeneralCheckListName);
        }
        //RFWI General CheckList

        //RFWI Trade
        public QCInspectionRFWITradeMasterViewModel GetRFWITrade(int Id)
        {
            return qcInspectionRepository.GetRFWITrade(Id);
        }

        public int CreateRFWITrade(QCInspectionRFWITradeMasterViewModel RFWITrade)
        {
            return qcInspectionRepository.CreateRFWITrade(RFWITrade);
        }

        public int SaveRFWITrade(QCInspectionRFWITradeMasterViewModel RFWITrade)
        {
            return qcInspectionRepository.SaveRFWITrade(RFWITrade);
        }

        public int DeleteRFWITrade(int Id)
        {
            return qcInspectionRepository.DeleteRFWITrade(Id);
        }

        public List<QCInspectionRFWITradeMasterViewModel> GetAllRFWITrades()
        {
            return qcInspectionRepository.GetAllRFWITrades();
        }

        public bool CheckRFWITrade(int TradeID, string RFWITradeName)
        {
            return qcInspectionRepository.CheckRFWITrade(TradeID, RFWITradeName);
        }
        //RFWI Trade

        //RFWI Trade Item
        public QCInspectionRFWITradeItemDetailViewModel GetRFWITradeItem(int Id)
        {
            return qcInspectionRepository.GetRFWITradeItem(Id);
        }

        public int CreateRFWITradeItem(QCInspectionRFWITradeItemDetailViewModel RFWITradeItem)
        {
            return qcInspectionRepository.CreateRFWITradeItem(RFWITradeItem);
        }

        public int SaveRFWITradeItem(QCInspectionRFWITradeItemDetailViewModel RFWITradeItem)
        {
            return qcInspectionRepository.SaveRFWITradeItem(RFWITradeItem);
        }

        public int DeleteRFWITradeItem(int Id)
        {
            return qcInspectionRepository.DeleteRFWITradeItem(Id);
        }

        public List<QCInspectionRFWITradeItemDetailViewModel> GetAllRFWITradeItems()
        {
            return qcInspectionRepository.GetAllRFWITradeItems();
        }

        public bool CheckRFWITradeItem(int TradeID, int TradeItemID, string ItemName)
        {
            return qcInspectionRepository.CheckRFWITradeItem(TradeID, TradeItemID, ItemName);
        }
        //RFWI Trade Item

        //RFWI Trade Detailed CheckList
        public QCInspectionRFWITradeDetailedCheckListDetailViewModel GetRFWITradeDetailedCheckList(int Id)
        {
            return qcInspectionRepository.GetRFWITradeDetailedCheckList(Id);
        }

        public int CreateRFWITradeDetailedCheckList(QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeItem)
        {
            return qcInspectionRepository.CreateRFWITradeDetailedCheckList(RFWITradeItem);
        }

        public int SaveRFWITradeDetailedCheckList(QCInspectionRFWITradeDetailedCheckListDetailViewModel RFWITradeItem)
        {
            return qcInspectionRepository.SaveRFWITradeDetailedCheckList(RFWITradeItem);
        }

        public int DeleteRFWITradeDetailedCheckList(int Id)
        {
            return qcInspectionRepository.DeleteRFWITradeDetailedCheckList(Id);
        }

        public List<QCInspectionRFWITradeDetailedCheckListDetailViewModel> GetAllRFWITradeDetailedCheckLists()
        {
            return qcInspectionRepository.GetAllRFWITradeDetailedCheckLists();
        }

        public bool CheckRFWITradeDetailedCheckList(int TradeID, int TradeDetailedCheckListID, string DetailedCheckListName)
        {
            return qcInspectionRepository.CheckRFWITradeDetailedCheckList(TradeID, TradeDetailedCheckListID, DetailedCheckListName);
        }
        //RFWI Trade Detailed CheckList

        public List<MasterSyncViewModel> GetQCInspectionAndRFWIMasterSync()
        {
            return qcInspectionRepository.GetQCInspectionAndRFWIMasterSync();
        }

        // Masters

        // Transactions
        // QC Inspection Defect Form
        public QCInspectionDefectFormViewModel GetQCInspectionDefectForm(int Id)
        {
            return qcInspectionRepository.GetQCInspectionDefectForm(Id);
        }

        public int CreateQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.CreateQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int SaveQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.SaveQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int MobileSaveQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.MobileSaveQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int DeleteQCInspectionDefectForm(int Id)
        {
            return qcInspectionRepository.DeleteQCInspectionDefectForm(Id);
        }

        public int MobileDeleteQCInspectionDefectForm(string Ids, string UserID)
        {
            return qcInspectionRepository.MobileDeleteQCInspectionDefectForm(Ids, UserID);
        }

        public int ApprovedQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.ApprovedQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int ReDoQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.ReDoQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int ReDoDoneQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.ReDoDoneQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int RectificationQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.RectificationQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int ReworkQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.ReworkQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int ReworkDoneQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.ReworkDoneQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public int CompletedQCInspectionDefectForm(QCInspectionDefectFormViewModel QCInspectionDefectForm)
        {
            return qcInspectionRepository.CompletedQCInspectionDefectForm(QCInspectionDefectForm);
        }

        public List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectForms()
        {
            return qcInspectionRepository.GetAllQCInspectionDefectForms();
        }

        public List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedProjectID(int Id, string BatchID = "")
        {
            return qcInspectionRepository.GetAllQCInspectionDefectFormsBasedProjectID(Id, BatchID);
        }

        public List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedUserID(int Id)
        {
            return qcInspectionRepository.GetAllQCInspectionDefectFormsBasedUserID(Id);
        }

        public List<QCInspectionDefectFormViewModel> GetAllQCInspectionDefectFormsBasedSubcontractorID(int Id)
        {
            return qcInspectionRepository.GetAllQCInspectionDefectFormsBasedSubcontractorID(Id);
        }
        // QC Inspection Defect Form

        // RFWI Form
        public QCInspectionRFWIFormViewModel GetRFWIForm(int Id)
        {
            return qcInspectionRepository.GetRFWIForm(Id);
        }

        public int CreateRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.CreateRFWIForm(RFWIForm);
        }

        public int SaveRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.SaveRFWIForm(RFWIForm);
        }

        public int DeleteRFWIForm(int Id)
        {
            return qcInspectionRepository.DeleteRFWIForm(Id);
        }

        public int MandESignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.MandESignOffRFWIForm(RFWIForm);
        }

        public int StructureSignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.StructureSignOffRFWIForm(RFWIForm);
        }

        public int OtherSignOffRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.OtherSignOffRFWIForm(RFWIForm);
        }

        public int RequestedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.RequestedRFWIForm(RFWIForm);
        }

        public int CompletedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.CompletedRFWIForm(RFWIForm);
        }

        public int RejectedRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.RejectedRFWIForm(RFWIForm);
        }

        public int ReInspectionRFWIForm(int Id)
        {
            return qcInspectionRepository.ReInspectionRFWIForm(Id);
        }

        public int MobileSaveRFWIForm(QCInspectionRFWIFormViewModel RFWIForm)
        {
            return qcInspectionRepository.MobileSaveRFWIForm(RFWIForm);
        }
        public int MobileDeleteRFWIForm(string Ids, string UserID)
        {
            return qcInspectionRepository.MobileDeleteRFWIForm(Ids, UserID);
        }

        public bool ValidateInspectionDateTimeSlot(DateTime InspectionOn, TimeSpan InspectionStartOn, TimeSpan InspectionEndOn)
        {
            return qcInspectionRepository.ValidateInspectionDateTimeSlot(InspectionOn, InspectionStartOn, InspectionEndOn);
        }

        public List<QCInspectionRFWIFormViewModel> GetAllRFWIForms()
        {
            return qcInspectionRepository.GetAllRFWIForms();
        }

        public List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedProjectID(int Id, string BatchID = "")
        {
            return qcInspectionRepository.GetAllRFWIFormsBasedProjectID(Id, BatchID);
        }

        public List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedUserID(int Id)
        {
            return qcInspectionRepository.GetAllRFWIFormsBasedUserID(Id);
        }

        public List<QCInspectionRFWIFormViewModel> GetAllRFWIFormsBasedRTOInspectorID(int Id)
        {
            return qcInspectionRepository.GetAllRFWIFormsBasedRTOInspectorID(Id);
        }
        // RFWI Form

        //RFWI Drawings Reference File
        public QCInspectionRFWIFormLocationDetailViewModel GetRFWIFormLocationDetail(int Id)
        {
            return qcInspectionRepository.GetRFWIFormLocationDetail(Id);
        }

        public int CreateRFWIFormLocationDetail(QCInspectionRFWIFormLocationDetailViewModel RFWIFormLocation)
        {
            return qcInspectionRepository.CreateRFWIFormLocationDetail(RFWIFormLocation);
        }

        public int DeleteRFWIFormLocationDetail(int Id)
        {
            return qcInspectionRepository.DeleteRFWIFormLocationDetail(Id);
        }

        public List<QCInspectionRFWIFormLocationDetailViewModel> GetAllRFWIFormsLocationDetails(int Id)
        {
            return qcInspectionRepository.GetAllRFWIFormsLocationDetails(Id);
        }

        public bool CheckRFWIFormLocationDetail(int QCInspectionRFWIFormID, int RFWIFormLocationDetailID, int UnitID)
        {
            return qcInspectionRepository.CheckRFWIFormLocationDetail(QCInspectionRFWIFormID, RFWIFormLocationDetailID, UnitID);
        }
        //RFWI Drawings Reference File

        // Transactions
    }
}