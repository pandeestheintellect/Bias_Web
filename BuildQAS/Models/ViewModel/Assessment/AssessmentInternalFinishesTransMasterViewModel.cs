using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class AssessmentInternalFinishesTransMasterViewModel
    {
        public int AssessmentIFID { get; set; }
        public int? ProjectID { get; set; }
        public DateTime? AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public int? LocationID { get; set; }
        public int MobileAssessmentIFID { get; set; } = 0;
        public string BatchID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public AssessmentProjectMasterViewModel assessment_project_master { get; set; }
        public AssessmentTypeLocationMasterViewModel assessment_type_location_master { get; set; }
        public List<AssessmentInternalFinishesTransDetailViewModel> assessment_internal_finishes_trn_detail { get; set; }
    }

    public class AssessmentInternalFinishesTransMasterMobileViewModel
    {
        public int AssessmentIFID { get; set; }
        public int? ProjectID { get; set; }
        public string AssessmentDate { get; set; }
        public string Block_Unit { get; set; }
        public int? LocationID { get; set; }
        public string LocationName { get; set; }
        public int MobileAssessmentIFID { get; set; }
        public string BatchID { get; set; }
        public int Status { get; set; }
        public int CreatedOrUpdatedByUserId { get; set; }

        public List<AssessmentInternalFinishesTransDetailMobileViewModel> AssessmentInternalFinishesTransDetailMobileViewModels { get; set; }
    }

    public class AssessmentInternalFinishesTransMasterMobileDeleteViewModel
    {
        public int AssessmentIFID { get; set; }
    }
}