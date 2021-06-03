using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;
using BuildInspect.App_Start;
using BuildInspect.Models;
using BuildInspect.Models.Domain;
using BuildInspect.Models.ViewModel;
using BuildInspect.Models.Authentication;
using Ninject;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Repository.Interface;
using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.Service.Interface;
using BuildInspect.Models.Utility;

namespace BuildInspect
{
    public class MvcApplication : System.Web.HttpApplication
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            StartAutoMapper();
            GlobalConfiguration.Configuration.MessageHandlers.Add(new BasicAuthenticationMessageHandler());
        }

        private void StartAutoMapper()
        {
            Mapper.CreateMap<short, bool>().ConvertUsing<BooleanTypeConverter>();
            Mapper.CreateMap<bool, short>().ConvertUsing<ShortTypeConverter>();
            Mapper.CreateMap<int, bool>().ConvertUsing<BooleanIntTypeConverter>();
            Mapper.CreateMap<bool, int>().ConvertUsing<IntTypeConverter>();

           
            Mapper.CreateMap<UserViewModel, user>();
            Mapper.CreateMap<user, UserViewModel>();

            Mapper.CreateMap<EmployeeViewModel, employee>();
            Mapper.CreateMap<employee, EmployeeViewModel>();

            //Mapper.CreateMap<module, ModuleViewModel>();
            //Mapper.CreateMap<ModuleViewModel, module>();

            //Mapper.CreateMap<ScreenViewModel, screen>();
            //Mapper.CreateMap<screen, ScreenViewModel>();

            Mapper.CreateMap<usergroup, GroupViewModel>();
            Mapper.CreateMap<GroupViewModel, usergroup>();

            Mapper.CreateMap<company_master, CompanyMasterViewModel>();
            Mapper.CreateMap<CompanyMasterViewModel, company_master>();

            Mapper.CreateMap<SmartFMUserViewModel, AppUser>();
            Mapper.CreateMap<AppUser, SmartFMUserViewModel>();

            // Assessment - Module

            // Masters

            Mapper.CreateMap<assessment_type_master, AssessmentTypeMasterViewModel>();
            Mapper.CreateMap<AssessmentTypeMasterViewModel, assessment_type_master>();

            Mapper.CreateMap<assessors_master, AssessorsMasterViewModel>();
            Mapper.CreateMap<AssessorsMasterViewModel, assessors_master>();

            Mapper.CreateMap<assessment_development_type_master, AssessmentDevelopmentTypeMasterViewModel>();
            Mapper.CreateMap<AssessmentDevelopmentTypeMasterViewModel, assessment_development_type_master>();

            Mapper.CreateMap<assessment_project_master, AssessmentProjectMasterViewModel>();
            Mapper.CreateMap<AssessmentProjectMasterViewModel, assessment_project_master>();

            Mapper.CreateMap<assessment_project_assessors_detail, AssessmentProjectAssessorsDetailViewModel>();
            Mapper.CreateMap<AssessmentProjectAssessorsDetailViewModel, assessment_project_assessors_detail>();

            Mapper.CreateMap<qcinspection_project_rfwi_drawing_reference_files, QCInspectionProjectRFWIDrawingReferenceFilesViewModel>();
            Mapper.CreateMap<QCInspectionProjectRFWIDrawingReferenceFilesViewModel, qcinspection_project_rfwi_drawing_reference_files>();

            Mapper.CreateMap<assessment_direction_master, AssessmentDirectionMasterViewModel>();
            Mapper.CreateMap<AssessmentDirectionMasterViewModel, assessment_direction_master>();

            Mapper.CreateMap<assessment_type_location_master, AssessmentTypeLocationMasterViewModel>();
            Mapper.CreateMap<AssessmentTypeLocationMasterViewModel, assessment_type_location_master>();
            
            Mapper.CreateMap<assessment_type_module_master, AssessmentTypeModuleMasterViewModel>();
            Mapper.CreateMap<AssessmentTypeModuleMasterViewModel, assessment_type_module_master>();

            Mapper.CreateMap<assessment_type_module_Process_master, AssessmentTypeModuleProcessMasterViewModel>();
            Mapper.CreateMap<AssessmentTypeModuleProcessMasterViewModel, assessment_type_module_Process_master>();

            Mapper.CreateMap<assessment_joint_master, AssessmentJointMasterViewModel>();
            Mapper.CreateMap<AssessmentJointMasterViewModel, assessment_joint_master>();

            Mapper.CreateMap<assessment_leak_master, AssessmentLeakMasterViewModel>();
            Mapper.CreateMap<AssessmentLeakMasterViewModel, assessment_leak_master>();

            Mapper.CreateMap<assessment_wall_master, AssessmentWallMasterViewModel>();
            Mapper.CreateMap<AssessmentWallMasterViewModel, assessment_wall_master>();

            Mapper.CreateMap<assessment_window_master, AssessmentWindowMasterViewModel>();
            Mapper.CreateMap<AssessmentWindowMasterViewModel, assessment_window_master>();
           
            Mapper.CreateMap<assessment_wet_area_water_tightness_test_result_master, AssessmentWetAreaWaterTightnessTestResultMasterViewModel>();
            Mapper.CreateMap<AssessmentWetAreaWaterTightnessTestResultMasterViewModel, assessment_wet_area_water_tightness_test_result_master>();

            // Masters

            Mapper.CreateMap<assessment_internal_finishes_trn, AssessmentInternalFinishesTransMasterViewModel>();
            Mapper.CreateMap<AssessmentInternalFinishesTransMasterViewModel, assessment_internal_finishes_trn>();

            Mapper.CreateMap<assessment_internal_finishes_trn_detail, AssessmentInternalFinishesTransDetailViewModel>();
            Mapper.CreateMap<AssessmentInternalFinishesTransDetailViewModel, assessment_internal_finishes_trn_detail>();

            Mapper.CreateMap<assessment_external_wall_trn, AssessmentExternalWallTransMasterViewModel>();
            Mapper.CreateMap<AssessmentExternalWallTransMasterViewModel, assessment_external_wall_trn>();

            Mapper.CreateMap<assessment_external_wall_trn_detail, AssessmentExternalWallTransDetailViewModel>();
            Mapper.CreateMap<AssessmentExternalWallTransDetailViewModel, assessment_external_wall_trn_detail>();

            //Mapper.CreateMap<assessment_external_works_trn, AssessmentExternalWorksTransMasterViewModel>().ForSourceMember(x => x.assessment_project_master, y => y.Ignore()).ForSourceMember(x => x.assessment_type_location_master, y => y.Ignore()).ForMember(x => x.assessment_project_master, y => y.Ignore()).ForMember(x => x.assessment_type_location_master, y => y.Ignore());
            //Mapper.CreateMap<AssessmentExternalWorksTransMasterViewModel, assessment_external_works_trn>().ForSourceMember(x => x.assessment_project_master, y => y.Ignore()).ForSourceMember(x => x.assessment_type_location_master, y => y.Ignore()).ForMember(x => x.assessment_project_master, y => y.Ignore()).ForMember(x => x.assessment_type_location_master, y => y.Ignore());

            Mapper.CreateMap<assessment_external_works_trn, AssessmentExternalWorksTransMasterViewModel>();
            Mapper.CreateMap<AssessmentExternalWorksTransMasterViewModel, assessment_external_works_trn>();

            Mapper.CreateMap<assessment_external_works_trn_detail, AssessmentExternalWorksTransDetailViewModel>();
            Mapper.CreateMap<AssessmentExternalWorksTransDetailViewModel, assessment_external_works_trn_detail>();

            Mapper.CreateMap<assessment_roof_construction_trn, AssessmentRoofConstructionTransMasterViewModel>();
            Mapper.CreateMap<AssessmentRoofConstructionTransMasterViewModel, assessment_roof_construction_trn>();

            Mapper.CreateMap<assessment_roof_construction_trn_detail, AssessmentRoofConstructionTransDetailViewModel>();
            Mapper.CreateMap<AssessmentRoofConstructionTransDetailViewModel, assessment_roof_construction_trn_detail>();

            Mapper.CreateMap<assessment_field_window_water_tightness_test, AssessmentFieldWindowWaterTightnessTestTransViewModel>();
            Mapper.CreateMap<AssessmentFieldWindowWaterTightnessTestTransViewModel, assessment_field_window_water_tightness_test>();

            Mapper.CreateMap<assessment_wet_area_water_tightness_test_tran, AssessmentWetAreaWaterTightnessTestTransMasterViewModel>();
            Mapper.CreateMap<AssessmentWetAreaWaterTightnessTestTransMasterViewModel, assessment_wet_area_water_tightness_test_tran>();

            Mapper.CreateMap<assessment_wet_area_water_tightness_test_tran_detail, AssessmentWetAreaWaterTightnessTestTransDetailViewModel>();
            Mapper.CreateMap<AssessmentWetAreaWaterTightnessTestTransDetailViewModel, assessment_wet_area_water_tightness_test_tran_detail>();

            Mapper.CreateMap<assessment_wet_area_water_tightness_test_tran_detail_result, AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel>();
            Mapper.CreateMap<AssessmentWetAreaWaterTightnessTestTransDetailResultViewModel, assessment_wet_area_water_tightness_test_tran_detail_result>();

            // Assessment Module

            // QC Inspection - Module
            //Masters
            Mapper.CreateMap<qcinspection_project_master, QCInspectionProjectMasterViewModel>();
            Mapper.CreateMap<QCInspectionProjectMasterViewModel, qcinspection_project_master>();

            Mapper.CreateMap<qcinspection_project_files, QCInspectionProjectFilesViewModel>();
            Mapper.CreateMap<QCInspectionProjectFilesViewModel, qcinspection_project_files>();

            Mapper.CreateMap<qcinspection_project_PM_detail, QCInspectionProjectPMDetailViewModel>();
            Mapper.CreateMap<QCInspectionProjectPMDetailViewModel, qcinspection_project_PM_detail>();

            Mapper.CreateMap<qcinspection_project_Supervisor_detail, QCInspectionProjectSupervisorDetailViewModel>();
            Mapper.CreateMap<QCInspectionProjectSupervisorDetailViewModel, qcinspection_project_Supervisor_detail>();

            Mapper.CreateMap<qcinspection_project_MEInspector_detail, QCInspectionProjectMEInspectorDetailViewModel>();
            Mapper.CreateMap<QCInspectionProjectMEInspectorDetailViewModel, qcinspection_project_MEInspector_detail>();

            Mapper.CreateMap<qcinspection_project_StructureInspector_detail, QCInspectionProjectStructureInspectorDetailViewModel>();
            Mapper.CreateMap<QCInspectionProjectStructureInspectorDetailViewModel, qcinspection_project_StructureInspector_detail>();

            Mapper.CreateMap<qcinspection_project_OtherInspector_detail, QCInspectionProjectOtherInspectorDetailViewModel>();
            Mapper.CreateMap<QCInspectionProjectOtherInspectorDetailViewModel, qcinspection_project_OtherInspector_detail>();

            Mapper.CreateMap<qcinspection_subcontractor_master, QCInspectionSubcontractorMasterViewModel>();
            Mapper.CreateMap<QCInspectionSubcontractorMasterViewModel, qcinspection_subcontractor_master>();

            Mapper.CreateMap<qcinspection_subcontractor_trade_detail, QCInspectionSubcontractorTradeDetailViewModel>();
            Mapper.CreateMap<QCInspectionSubcontractorTradeDetailViewModel, qcinspection_subcontractor_trade_detail>();

            Mapper.CreateMap<qcinspection_block_master, QCInspectionBlockMasterViewModel>();
            Mapper.CreateMap<QCInspectionBlockMasterViewModel, qcinspection_block_master>();

            Mapper.CreateMap<qcinspection_level_master, QCInspectionLevelMasterViewModel>();
            Mapper.CreateMap<QCInspectionLevelMasterViewModel, qcinspection_level_master>();

            Mapper.CreateMap<qcinspection_unit_master, QCInspectionUnitMasterViewModel>();
            Mapper.CreateMap<QCInspectionUnitMasterViewModel, qcinspection_unit_master>();

            Mapper.CreateMap<qcinspection_defect_type_master, QCInspectionDefectTypeMasterViewModel> ();
            Mapper.CreateMap<QCInspectionDefectTypeMasterViewModel, qcinspection_defect_type_master>();

            Mapper.CreateMap<qcinspection_trade_master, QCInspectionTradeMasterViewModel>();
            Mapper.CreateMap<QCInspectionTradeMasterViewModel, qcinspection_trade_master>();

            Mapper.CreateMap<qcinspection_rfwi_general_checklist_master, QCInspectionRFWIGeneralCheckListMasterViewModel > ();
            Mapper.CreateMap<QCInspectionRFWIGeneralCheckListMasterViewModel, qcinspection_rfwi_general_checklist_master>();

            Mapper.CreateMap<qcinspection_rfwi_trade_master, QCInspectionRFWITradeMasterViewModel>();
            Mapper.CreateMap<QCInspectionRFWITradeMasterViewModel, qcinspection_rfwi_trade_master>();

            Mapper.CreateMap<qcinspection_rfwi_trade_item_detail, QCInspectionRFWITradeItemDetailViewModel>();
            Mapper.CreateMap<QCInspectionRFWITradeItemDetailViewModel, qcinspection_rfwi_trade_item_detail>();

            Mapper.CreateMap<qcinspection_rfwi_trade_detailed_checklist_detail, QCInspectionRFWITradeDetailedCheckListDetailViewModel>();
            Mapper.CreateMap<QCInspectionRFWITradeDetailedCheckListDetailViewModel, qcinspection_rfwi_trade_detailed_checklist_detail>();

            //Masters

            //Transaction
            Mapper.CreateMap<qcinspection_defect_form, QCInspectionDefectFormViewModel>();
            Mapper.CreateMap<QCInspectionDefectFormViewModel, qcinspection_defect_form>();

            Mapper.CreateMap<qcinspection_defect_files, QCInspectionDefectFormFilesViewModel>();
            Mapper.CreateMap<QCInspectionDefectFormFilesViewModel, qcinspection_defect_files>();

            Mapper.CreateMap<qcinspection_rfwi_form, QCInspectionRFWIFormViewModel>();
            Mapper.CreateMap<QCInspectionRFWIFormViewModel, qcinspection_rfwi_form>();

            Mapper.CreateMap<qcinspection_rfwi_form_general_checklist_detail, QCInspectionRFWIFormGeneralCheckListDetailViewModel>();
            Mapper.CreateMap<QCInspectionRFWIFormGeneralCheckListDetailViewModel, qcinspection_rfwi_form_general_checklist_detail>();

            Mapper.CreateMap<qcinspection_rfwi_form_location_detail, QCInspectionRFWIFormLocationDetailViewModel>();
            Mapper.CreateMap<QCInspectionRFWIFormLocationDetailViewModel, qcinspection_rfwi_form_location_detail>();

            Mapper.CreateMap<qcinspection_rfwi_form_trade_detailed_checklist_detail, QCInspectionRFWIFormTradeDetailedCheckListDetailViewModel>();
            Mapper.CreateMap<QCInspectionRFWIFormTradeDetailedCheckListDetailViewModel, qcinspection_rfwi_form_trade_detailed_checklist_detail>();

            Mapper.CreateMap<qcinspection_rfwi_form_trade_item_detail, QCInspectionRFWIFormTradeItemDetailViewModel>();
            Mapper.CreateMap<QCInspectionRFWIFormTradeItemDetailViewModel, qcinspection_rfwi_form_trade_item_detail>();

            //Transaction
            // QC Inspection - Module




        }

        private class BooleanTypeConverter : TypeConverter<short, bool>
        {
            protected override bool ConvertCore(short source)
            {
                if (source == 1)
                    return true;
                return false;
            }
        }

        private class BooleanIntTypeConverter : TypeConverter<int, bool>
        {
            protected override bool ConvertCore(int source)
            {
                if (source == 1)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Converts bool to short
        /// </summary>
        private class ShortTypeConverter : TypeConverter<bool, short>
        {
            protected override short ConvertCore(bool source)
            {
                if (source)
                    return 1;
                return 0;
            }
        }

        private class IntTypeConverter : TypeConverter<bool, int>
        {
            protected override int ConvertCore(bool source)
            {
                if (source)
                    return 1;
                return 0;
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                logger.Error("Error:");
                logger.Error(exception.Message);
                logger.Error(exception.StackTrace);

                int i = 0;
            }
        }

        protected void Session_Start()
        {
            AppSession.SetCurrentPage("");
        }
        protected void Session_End()
        {
            Session.Abandon();
        }
    }
}
