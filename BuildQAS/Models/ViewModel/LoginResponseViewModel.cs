using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class LoginResponseViewModel : ResponseViewModel
    {
        public BInLiteUserViewModel User { get; set; }        
        public String DownTimeMessage { get; set; }
    }

    public class SFMLoginResponseViewModel : SFMResponseViewModel
    {
        public SmartFMRespUserViewModel AppUser { get; set; }
        public String DownTimeMessage { get; set; }
    }

    public class BInLiteUserViewModel
    {
        public string LoginUserType;
        public string UserId { get; set; }
        public string GroupID { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string CompanyID { get; set; }
        public string ServiceType { get; set; }
        public string Zone { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class SmartFMRespUserViewModel
    {

        public string SmartFMUserID { get; set; }
        public string AppGroupID { get; set; }
        public string AppClientID { get; set; }
        public string userName { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
              
        public string Password { get; set; }
    }


    public class ResponseViewModel
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public String ToString()
        {
            return string.Format("Success:{0}  } ErrorMessage:{2}", Success, ErrorMessage);
        }

        // New variables required for content service

        public string Credentials { get; set; }


        public int UserId { get; set; }
    }

    public class SFMResponseViewModel
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public String ToString()
        {
            return string.Format("Success:{0}  } ErrorMessage:{2}", Success, ErrorMessage);
        }

        // New variables required for content service

        public string Credentials { get; set; }


        public int Userid { get; set; }
    }

    public class InternalFinishesResponseViewModel : ResponseViewModel
    {
        public List<AssessmentInternalFinishesTransMasterMobileViewModel> AssessmentInternalFinishesTransMasterMobileViewModels { get; set; }
    }

    public class ExternalWallResponseViewModel : ResponseViewModel
    {
        public List<AssessmentExternalWallTransMasterMobileViewModel> AssessmentExternalWallTransMasterViewModels { get; set; }
    }

    public class ExternalWorksResponseViewModel : ResponseViewModel
    {
        public List<AssessmentExternalWorksTransMasterMobileViewModel> AssessmentExternalWorksTransMasterMobileViewModels { get; set; }
    }

    public class RoofConstructionResponseViewModel : ResponseViewModel
    {
        public List<AssessmentRoofConstructionTransMasterMobileViewModel> AssessmentRoofConstructionTransMasterMobileViewModels { get; set; }
    }

    public class FieldWindowWaterTightnessTestResponseViewModel : ResponseViewModel
    {
        public List<AssessmentFieldWindowWaterTightnessTestTransMobileViewModel> AssessmentFieldWindowWaterTightnessTestTransMobileViewModels { get; set; }
    }

    public class WetAreaWaterTightnessTestResponseViewModel : ResponseViewModel
    {
        public List<AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModel> AssessmentWetAreaWaterTightnessTestTransMasterMobileViewModels { get; set; }
    }

    public class QCInspectionDefectFormResponseViewModel : ResponseViewModel
    {
        public List<QCInspectionDefectFormMobileViewModel> QCInspectionDefectFormMobileViewModels { get; set; }
    }

    public class QCInspectionRFWIFormResponseViewModel : ResponseViewModel
    {
        public List<QCInspectionRFWIFormMobileViewModel> QCInspectionRFWIFormMobileViewModels { get; set; }
    }

}