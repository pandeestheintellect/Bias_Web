using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class QCInspectionProjectMasterViewModel
    {

        public int ProjectID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        [Required]
        public string Project_Name { get; set; }
        [Required]
        public string Project_ID { get; set; }
        
        public System.DateTime StartOn { get; set; }
        public System.DateTime EndOn { get; set; }
        public Nullable<int> Is_Completed { get; set; }
        public Nullable<int> ProjectManagerID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public CompanyMasterViewModel company_master { get; set; }
        public List<QCInspectionBlockMasterViewModel> qcinspection_block_master { get; set; }
        public List<QCInspectionProjectPMDetailViewModel> qcinspection_project_PM_detail { get; set; }
        public List<QCInspectionProjectSupervisorDetailViewModel> qcinspection_project_Supervisor_detail { get; set; }
        public List<QCInspectionProjectMEInspectorDetailViewModel> qcinspection_project_MEInspector_detail { get; set; }
        public List<QCInspectionProjectStructureInspectorDetailViewModel> qcinspection_project_StructureInspector_detail { get; set; }
        public List<QCInspectionProjectOtherInspectorDetailViewModel> qcinspection_project_OtherInspector_detail { get; set; }
        public List<QCInspectionProjectRFWIDrawingReferenceFilesViewModel> qcinspection_project_rfwi_drawing_reference_files { get; set; }
        public List<QCInspectionProjectFilesViewModel> qcinspection_project_files { get; set; }

        [NotMapped]
        public List<QCInspectionLevelMasterViewModel> qcinspection_level_master { get; set; }
        [NotMapped]
        public List<QCInspectionUnitMasterViewModel> qcinspection_unit_master { get; set; }

        [NotMapped]
        public QCInspectionBlockMasterViewModel Block { get; set; }

        [NotMapped]
        public QCInspectionLevelMasterViewModel Level { get; set; }

        [NotMapped]
        public QCInspectionUnitMasterViewModel Unit { get; set; }

        [NotMapped]
        public string StartDate { get; set; }

        [NotMapped]
        public string EndDate { get; set; }

        [NotMapped]
        public string ProjectManagers { get; set; }

        [NotMapped]
        public string Supervisors { get; set; }

        [NotMapped]
        public string MEInspectors { get; set; }

        [NotMapped]
        public string StructureInspectors { get; set; }

        [NotMapped]
        public string OtherInspectors { get; set; }
    }

    public class QCInspectionProjectMasterMobileViewModel
    {
        public int ProjectID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string Project_Name { get; set; }
        public string Project_ID { get; set; }
        public System.DateTime StartOn { get; set; }
        public System.DateTime EndOn { get; set; }
        public Nullable<int> Is_Completed { get; set; }
        public Nullable<int> ProjectManagerID { get; set; }
        public string ProjectManager_Name { get; set; }
        public List<QCInspectionLocationMobileViewModel> Locations { get; set; }
        public List<QCInspectionProjectPMDetailMobileViewModel> PMDetails { get; set; }
        public List<QCInspectionProjectSupervisorDetailMobileViewModel> SupervisorDetails { get; set; }
        public List<QCInspectionProjectMEInspectorDetailMobileViewModel> MEInspectorDetails { get; set; }
        public List<QCInspectionProjectStructureInspectorDetailMobileViewModel> StructureInspectorDetails { get; set; }
        public List<QCInspectionProjectOtherInspectorDetailMobileViewModel> OtherInspectorDetails { get; set; }
        public List<QCInspectionProjectRFWIDrawingReferenceFilesMobileViewModel> RFWIDrawingReferenceFiles { get; set; }
        public List<QCInspectionProjectFilesMobileViewModel> ProjectFiles { get; set; }
    }
}