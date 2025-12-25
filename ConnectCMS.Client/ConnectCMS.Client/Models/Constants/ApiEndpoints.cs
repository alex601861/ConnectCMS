namespace CMSTrain.Client.Models.Constants;

public abstract class ApiEndpoints
{
    public abstract class Analysis
    {
        public const string UploadUserResponseAnalysis = "analysis";
        public const string GetUserResponseAnalysisDetailsForFeedbacks = "analysis/feedbacks";
        public const string GetUserResponseAnalysisDetailsForAssessments = "analysis/assessments";
        public const string GetUserResponseAnalysisEvaluationDetailsForAssessments = "analysis/assessments/evaluation";
    }
    
    public abstract class Answer
    {
        public const string UploadCandidateQuestionnaireAnswers = "answer/candidate";
        public const string UploadSubordinateQuestionnaireAnswers = "answer/subordinate";
        public const string GetQuestionAnswerDetails = "answer";
        public const string GetResponseUserDetails = "answer/responses";
        public const string GetResponseUserDetailsForClient = "answer/responses/client";
        public const string GetUserResponseDetails = "answer/user-response";
    }
    
    public abstract class Attendance
    {
        public const string GetAttendanceRequestForCandidate = "attendance/candidate-details";
        public const string GetAttendanceRequestForClient = "attendance/client-details";
        public const string GetAttendanceRequestForClientList = "attendance/client-details/list";
        public const string GetAllAttendanceRequests = "attendance";
        public const string GetAllAttendanceRequestsList = "attendance/list";
        public const string UploadAttendance = "attendance";
        public const string CancelAttendance = "attendance";
        public const string ApproveRejectAttendance = "attendance/approve-reject";
        public const string DownloadAttendanceImage = "attendance/download";
        public const string ExportAttendanceDetails = "attendance/report";
    }
    
    public abstract class Authentication
    {
        public const string Login = "authentication/login";
        public const string UserRegister = "authentication/user-registration";
        public const string SelfRegister = "authentication/self-registration";
        public const string ClientCandidateRegister = "authentication/client-candidate-registration";
        public const string VerifyEmailConfirmation = "authentication/confirm-email";
        public const string ResetPassword = "authentication/reset-password";
        public const string ResetUserPassword = "authentication/reset-user-password";
        public const string Logout = "authentication/logout";
    }
    
    public abstract class Candidate
    {
        public const string GetCandidateDetailsById = "candidate";
    }
    
    public abstract class Certification
    {
        public const string GetCertificationDetailsById = "certification";
        public const string GetCertificationDetailsByTrainingId = "certification/training";
        public const string GetCertificationDetailsByTrainingCandidateId = "certification/training-candidate";
        public const string IssueTrainingCandidateCertification = "certification";
    }

    public abstract class Class
    {
        public const string GetAllClasses = "class"; 
        public const string GetAllClassesList = "class/list"; 
        public const string GetAllClassesForTrainers = "class/trainers"; 
        public const string GetAllClassesForTrainersList = "class/trainers/list"; 
        public const string GetAllClassesForCandidates = "class/candidates"; 
        public const string GetAllClassesForCandidatesList = "class/candidates/list"; 
        public const string GetAllCandidateClasses = "class/training-candidates"; 
        public const string GetAllCandidateClassesList = "class/training-candidates/list"; 
        public const string GetClassById = "class/details";
        public const string InsertClass = "class";
        public const string UpdateClass = "class";
        public const string ActivateDeactivateClass = "class";
        public const string GetClassDetailsCountForAdmin = "class/admin/count";
        public const string GetClassDetailsCountForCandidate = "class/candidate/count";
        public const string GetClassDetailsCountForClient = "class/client/count";
        public const string GetClassDetailsCountForTrainer = "class/trainer/count";
    }
    
    public abstract class ClassTrainers
    {
        public const string GetAllAvailableTrainingsForTrainers = "class-trainers/available-trainings"; 
        public const string GetAllAvailableTrainingsForTrainersList = "class-trainers/available-trainings/list"; 
        public const string GetAllTrainersForTraining = "class-trainers/training"; 
        public const string GetAllTrainersForTrainingList = "class-trainers/training/list"; 
        public const string GetAllTrainersForClass = "class-trainers/class";
        public const string GetAllTrainersForClassList = "class-trainers/class/list";
        public const string GetAllAssignedTrainingsForTrainers = "class-trainers/assigned-trainings";
        public const string GetAllAssignedTrainingsForTrainersList = "class-trainers/assigned-trainings/list";
        public const string AssignTrainersToClass = "class-trainers/assign";
        public const string UpdateTrainerDescription = "class-trainers/description";
        public const string GetTrainerDescriptionsOnTraining = "class-trainers/descriptions/training";
        public const string GetTrainerDescriptionsOnClass = "class-trainers/descriptions/class";
    }
    
    public abstract class ClientOrganization
    {
        public const string GetAllClientOrganizations = "client-organization"; 
        public const string GetAllClientOrganizationsList = "client-organization/list"; 
        public const string GetAllClientOrganizationsWithoutAdmin = "client-organization/admin";
        public const string RegisterClientOrganizationAdmin = "client-organization"; 
    }
    
    public abstract class Configuration
    {
        public const string GetAllTrainingConfigurations = "configuration/training"; 
        public const string GetTrainingResourceConfigurationByKey = "configuration/training/resource"; 
        public const string SaveTrainingResourceConfiguration = "configuration/training/resource"; 
        public const string GetTrainingCertificationConfigurationByKey = "configuration/training/certification"; 
        public const string SaveTrainingCertificationConfiguration = "configuration/training/certification"; 
        public const string GetTrainingCertificationTriggerConfigurationByKey = "configuration/training/certification-trigger";
        public const string SaveTrainingCertificationTriggerConfiguration = "configuration/training/certification-trigger";
        public const string DeleteTrainingConfiguration = "configuration/training"; 
        
        public const string GetAllClassConfigurations = "configuration/class"; 
        public const string GetClassResourceConfigurationByKey = "configuration/class/resource"; 
        public const string SaveClassResourceConfiguration = "configuration/class/resource"; 
        public const string GetClassAttendanceConfigurationByKey = "configuration/class/attendance"; 
        public const string SaveClassAttendanceConfiguration = "configuration/class/attendance"; 
        public const string DeleteClassConfiguration = "configuration/class"; 
        
        public const string GetAllTrainingInspectionConfigurations = "configuration/training-inspection"; 
        public const string GetTrainingInspectionConfigurationByKey = "configuration/training-inspection"; 
        public const string SaveTrainingInspectionConfiguration = "configuration/training-inspection"; 
        public const string DeleteTrainingInspectionConfiguration = "configuration/training-inspection";
    }

    public abstract class Country
    {
        public const string GetAllCountries = "country";
        public const string GetAllCountriesList = "country/list";
        public const string GetCountryById = "country";
        public const string GetDefaultCountry = "country/default";
        public const string GetGlobalCountries = "country/global";
        public const string InsertCountry = "country";
        public const string UpdateCountry = "country";
        public const string ActivateDeactivateCountry = "country";
    }
    
    public abstract class Designation
    {
        public const string GetAllDesignation = "designation";
        public const string GetAllDesignationsList = "designation/list";
        public const string GetDesignationById = "designation";
        public const string InsertDesignation = "designation";
        public const string UpdateDesignation = "designation";
        public const string ActivateDeactivateDesignation = "designation";
    }
    
    public abstract class Dashboard
    {
        #region Trainer
        public const string GetTrainerDashboardCount = "dashboard/trainer/dashboard-count";
        public const string GetTrainerTotalClasses = "dashboard/trainer/total-classes";
        public const string GetTrainerActiveTrainings = "dashboard/trainer/active-trainings";
        public const string GetTrainerUpcomingClasses = "dashboard/trainer/upcoming-classes";
        public const string GetTrainerClassesByDate = "dashboard/trainer/class";
        public const string GetTrainerCompletedClasses = "dashboard/trainer/completed-classes";
        #endregion

        #region Admin
        public const string GetAdminDashboardCount = "dashboard/admin/dashboard-count";
        public const string GetUpcomingTrainings = "dashboard/admin/upcoming-trainings";
        public const string GetPopularTrainings = "dashboard/admin/popular-trainings";
        public const string GetTotalTrainingFormats = "dashboard/admin/training-format-count";
        public const string GetTrainingRequestSummary = "dashboard/admin/training-request-summary";
        #endregion

        #region Candidates
        public const string GetTrainingProgress = "dashboard/candidate/training-progress";
        public const string GetAssignedTrainings = "dashboard/candidate/assigned-trainings";
        public const string GetNewTrainings = "dashboard/candidate/new-trainings";
        public const string GetClassesForDatesForCandidates = "dashboard/candidate/classes-for-date";
        public const string GetUnansweredQuestionnaireDetailsForCandidate = "dashboard/candidate/unanswered-questionnaires";
        #endregion

        #region Client
        public const string GetTrainingProgressesForClient = "dashboard/client/training-progress";
        public const string GetAssignedTrainingsForClient = "dashboard/client/assigned-trainings";
        public const string GetNewTrainingsForClient = "dashboard/client/new-trainings";
        public const string GetClassesForDatesForClient = "dashboard/client/classes-for-date";
        public const string GetUnansweredQuestionnaireDetailsForClient = "dashboard/client/unanswered-questionnaires";
        #endregion

        #region Generic
        public const string GetAllClasses = "dashboard/training/classes/status";
        #endregion
    }

    public abstract class EmailConfirmation
    {
        public const string SelfRegistration = "email-confirmation/self-registration";
        public const string UserRegistration = "email-confirmation/user-registration";
        public const string ClientCandidateRegistration = "email-confirmation/client-candidate-registration";
        public const string ForgotPassword = "email-confirmation/forgot-password";
        public const string ResetUserPassword = "email-confirmation/reset-password";
        
        public const string TrainingRequest = "email-confirmation/training-request";
        public const string TrainingRequestAction = "email-confirmation/training-request-action";
    }
    
    public abstract class Heading
    {
        public const string GetAllHeadings = "heading";
        public const string GetAllHeadingsList = "heading";
        public const string GetAllParentHeadings = "heading/parent/heading";
        public const string GetAllSubHeadings = "heading/sub/heading";
        public const string GetHeadingById = "heading";
        public const string InsertHeading = "heading";
        public const string UpdateHeading = "heading";
        public const string ActivateDeactivateHeading = "heading";
        public const string DeleteHeading = "heading";
        public const string GetHeadingCount = "heading/count";
    }
    
    public abstract class Inspection
    {
        public const string GetAllInspections = "inspection";
        public const string GetAllInspectionsList = "inspection/list";
        public const string GetInspectionById = "inspection";
        public const string GetAllAvailableTrainingInspections = "inspection/available";
        public const string GetAllAvailableTrainingInspectionsList = "inspection/available/list";
        public const string GetAllAssignedTrainingInspections = "inspection/assigned";
        public const string GetAllAssignedTrainingInspectionsList = "inspection/assigned/list";
        public const string InsertInspection = "inspection";
        public const string UpdateInspection = "inspection";
        public const string ActivateDeactivateInspection = "inspection";
        public const string UploadInspectionQuestionnaires = "inspection/questionnaires";
    }

    public abstract class Organization
    {
        public const string GetAllOrganizations = "organization";
        public const string GetAllOrganizationsList = "organization/list";
        public const string GetOrganizationById = "organization";
        public const string InsertOrganization = "organization";
        public const string UpdateOrganization = "organization";
        public const string ActivateDeactivateOrganization = "organization";
        public const string DeleteOrganization = "organization";
    }

    public abstract class Profile
    {
        public const string GetUserProfile = "profile";
        public const string GetUserRole = "profile/role";
        public const string UpdateProfile = "profile";
        public const string UpdateProfileImage = "profile/image";
        public const string ChangePassword = "profile/password";
        public const string DeleteUserProfile = "profile";
    }

    public abstract class Resource
    {
        public const string GetResourceById = "resource/details";
        public const string GetTrainingResourceById = "resource/training/details";
        public const string GetClassResourceById = "resource/class/details";
        public const string GetAllResources = "resource";
        public const string GetAllResourcesList = "resource/list";
        public const string GetAllResourcesForTraining = "resource/training";
        public const string GetAllResourcesForTrainingList = "resource/training/list";
        public const string GetAllResourcesForClass = "resource/class";
        public const string GetAllResourcesForClassList = "resource/class/list";
        public const string ActivateDeactivateResourceForTraining = "resource/training";
        public const string ActivateDeactivateResourceForClass = "resource/class";
        public const string UploadResources = "resource";
        public const string UploadResourcesPost = "resource/post";
        public const string UploadResourceModule = "resource/module";
        public const string UploadResourcesForTraining = "resource/training";
        public const string UploadResourcesForClass = "resource/class";
        public const string UpdateResource = "resource";
        public const string UpdateResourcePost = "resource/post";
        public const string ActivateDeactivateResourceMaterial = "resource";
        public const string DeleteResourceMaterial = "resource";
        public const string RemoveResourceMaterialForTraining = "resource/training";
        public const string RemoveResourceMaterialForClass = "resource/class";
        public const string DownloadResourceMaterial = "resource/download";
        public const string NavigateToResourceMaterialLink = "resource/navigate";
        public const string GenerateModuleResourceMaterialQrCode = "resource/qr-code";
        public const string DownloadModuleResourceMaterialQrCode = "resource/qr-code";
    }

    public abstract class PersonalityTest
    {
        public const string GetPersonalityTestQuestionnaires = "personality-test/questionnaire";
        public const string GetPersonalityTestResponses = "personality-test/response";
        public const string UploadPersonalityTestAnswers = "personality-test";
        public const string GetPersonalityTestAnalysis = "personality-test/analysis";
    }
    
    public abstract class PersonalityTrait
    {
        public const string GetAllPersonalityTraits = "personality-trait";
        public const string GetAllPersonalityTraitsList = "personality-trait/list";
        public const string GetPersonalityTraitById = "personality-trait";
        public const string GetPersonalityTrait = "personality-trait";
        public const string UpdatePersonalityTrait = "personality-trait";
    }
    
    public abstract class Questionnaire
    {
        public const string UploadQuestionnairesViaExcel = "questionnaire/excel";
        public const string UploadQuestionnaires = "questionnaire";
        public const string DownloadExcelFormat = "questionnaire/download-questionnaire-format";
        public const string DownloadQuestionnaireSheet = "questionnaire/download-questionnaire";
        public const string GetQuestionnaireModuleDetails = "questionnaire/details";
        public const string GetAllQuestionnairesForCandidates = "questionnaire/candidate";
        public const string GetQuestionnaireDetails = "questionnaire";
        public const string GetAllQuestionnairesForTrainingInspection = "questionnaire/inspection/details";
        public const string GetAllQuestionnairesFromInspectionUpload = "questionnaire/inspection";
        public const string GetQuestionnaireValidity = "questionnaire/validity";
        public const string GetGeneralQuestionnaireAnswerResponses = "questionnaire/stats";
        public const string ExportQuestionnaireDetails = "questionnaire/report";
        public const string GetTrainingQuestionnaireDetails = "questionnaire/training";
        public const string GenerateQuestionnaireAnswerUploadFormQrCode = "questionnaire/qr-code";
        public const string DownloadQuestionnaireAnswerUploadFormQrCode = "questionnaire/qr-code";
    }
    
    public abstract class Role
    {
        public const string GetAllRoles = "roles";
        public const string GetAllRolesList = "roles/list";
        public const string GetPrecedingRoles = "roles/preceding";
        public const string GetRoleById = "roles";
        public const string InsertRole = "roles";
        public const string UpdateRole = "roles";
        public const string DeleteRole = "roles";
    }

    public abstract class RoleRights
    {
        public const string GetAllRoleMenus = "role-rights";
        public const string AssignRoleMenus = "role-rights";
        public const string GetAllAssignedMenus = "role-rights";
    }

    public abstract class Subordinate
    {
        public const string GetSubordinateById = "subordinate/details";
        public const string GetSubordinateDetails = "subordinate";
        public const string GetSubordinateDetailsList = "subordinate/list";
        public const string GetSubordinateDetailsForTrainingCandidate = "subordinate/training-candidate";
        public const string GetSubordinateDetailsForTrainingCandidateList = "subordinate/training-candidate/list";
        public const string GetSubordinateViewDetails = "subordinate";
        public const string InsertSubordinateForCandidates = "subordinate/candidate";
        public const string InsertSubordinateForTrainingCandidates = "subordinate/training-candidate";
    }
    
    public  abstract class StrategicTrait
    {
        public const string GetAllStrategies = "strategy-trait";
        public const string GetAllStrategiesList = "strategy-trait/list";
        public const string GetAllStrategyModules = "strategy-trait/modules";
        public const string GetAllStrategyTraitResults = "strategy-trait/modules/traits";
        public const string GetStrategyById = "strategy-trait";
        public const string GetStrategyDetails = "strategy-trait/details";
        public const string InsertStrategy = "strategy-trait";
        public const string UpdateStrategy = "strategy-trait";
        public const string DeleteStrategy = "strategy-trait";
        public const string UploadStrategyDetails = "strategy-trait/upload-strategy-details";
        public const string UploadStrategyTraitQuestionnaire = "strategy-trait/upload-strategy-traits";
        public const string GetStrategicTraitCount = "strategy-trait/count";
        public const string GetStrategyTraitQuestionnaireResponses = "strategy-trait/responses";
        public const string GetStrategyTraitQuestionnaireResponsesList = "strategy-trait/responses";
        public const string GetStrategyTraitQuestionnaireResponsesByUserId = "strategy-trait/responses";
        public const string GetStrategyTraitQuestionnaireResponsesByUserIdList = "strategy-trait/responses";
        public const string GetStrategyTraitQuestionnaireDetails = "strategy-trait/responses/details";
    }
    
    public  abstract class SubordinateQuestionnaire
    {
        public const string GetTrainingById = "subordinate-questionnaire/training/details";
        public const string GetInspectionById = "subordinate-questionnaire/inspection/details";
        public const string GetSubordinateById = "subordinate-questionnaire/subordinate/details";
        public const string GetCandidateBySubordinateId = "subordinate-questionnaire/candidate/details";
        public const string GetTrainingInspectionById = "subordinate-questionnaire/training/inspection/details";
        public const string GetAllQuestionnairesForSubordinates = "subordinate-questionnaire/details";
    }
    
    public abstract class Trainer
    {
        public const string GetAllActiveTrainers = "trainers";
        public const string GetAllActiveTrainersList = "trainers/list";
    }
    
    public abstract class TrainingFormat
    {
        public const string GetAllTrainingFormats = "training-format";
        public const string GetAllTrainingFormatsList = "training-format/list";
        public const string GetTrainingFormatById = "training-format";
        public const string InsertTrainingFormat = "training-format";
        public const string UpdateTrainingFormat = "training-format";
        public const string ActivateDeactivateTrainingFormat = "training-format";
    }

    public abstract class Training
    {
        public const string GetAllTrainings = "training"; 
        public const string GetAllTrainingsList = "training/list"; 
        public const string GetTrainingById = "training/details";
        public const string InsertTraining = "training";
        public const string UpdateTraining = "training";
        public const string ActivateDeactivateTraining = "training";
        
        public const string GetAllTrainingsForCandidate = "training/available/candidate";
        public const string GetAllTrainingsForCandidateList = "training/available/candidate/list";
        public const string GetAssignedTrainingsForCandidate = "training/assigned/candidate";
        public const string GetAssignedTrainingsForCandidateList = "training/assigned/candidate/list";
        
        public const string GetAllTrainingsForClient = "training/available/client";
        public const string GetAllTrainingsForClientList = "training/available/client/list";
        public const string GetAssignedTrainingsForClient = "training/assigned/client";
        public const string GetAssignedTrainingsForClientList = "training/assigned/client/list";
        
        public const string GetAllTrainingsForTrainer = "training/available/trainer"; 
        public const string GetAllTrainingsForTrainerList = "training/available/trainer/list"; 
        public const string GetAssignedTrainingsForTrainer = "training/assigned/trainer"; 
        public const string GetAssignedTrainingsForTrainerList = "training/assigned/trainer/list"; 
        
        public const string GetTrainingModuleCount = "training/module/count";
        public const string GetTrainingDetailsCount = "training/details-count";
        public const string GetAvailableTrainingsCount = "training/count";
        public const string GetAllAssignedClientOrganizations = "training/client-organizations";
        
        public const string GetAvailableTrainingCountsForCandidate = "training/available/candidate/count";
        public const string GetAllAssignedTrainingCountsForCandidate = "training/assigned/candidate/count";
        public const string GetTrainingDetailsCountForCandidate = "training/assigned/candidate/training/count";

        public const string GetAvailableTrainingCountForTrainers = "training/available/trainer/count";
        public const string GetAllAssignedTrainingCountForTrainers = "training/assigned/trainer/count";
        
        public const string GetAvailableTrainingCountForClient = "training/available/client/count";
        public const string GetAllAssignedTrainingCountsForClient = "training/assigned/client/count";
        public const string GetTrainingDetailsCountForClient = "training/assigned/client/training/count";

        public const string GetTrainingDetailsByInspection = "training/details/inspection";
        public const string GetTrainingDetailsByQuestionnaire = "training/details/questionnaire";
    }

    public abstract class TrainingCandidate
    {
        public const string GetTrainingCandidateAssignmentDetails = "training-candidate/assignment-details";
        public const string GetApprovedTrainingCandidateAssignmentDetails = "training-candidate/approved-assignment-details";
        public const string GetTrainingCandidateAssignmentDetailsForTraining = "training-candidate/training-assignment-details";
        public const string GetTrainingRequestsCount = "training-candidate/summary";
        public const string GetApprovalMatrixCount = "training-candidate/approval-matrix-count";
        public const string SelfCandidateAssignment = "training-candidate/self-request";
        public const string ClientCandidateAssignment = "training-candidate/client-request";
        public const string AdminCandidateAssignment = "training-candidate/admin-request";
        public const string ApprovalRejectTrainingCandidateRequest = "training-candidate/approve-reject";
        public const string RemoveCandidateFromTraining = "training-candidate";
        public const string CancelTrainingRequest = "training-candidate/cancel";
        public const string GetAllTrainingRequestsForAdmin = "training-candidate/requests/admin";
        public const string GetAllTrainingRequestsForAdminList = "training-candidate/requests/admin/list";
        public const string GetAllTrainingRequestsForCandidate = "training-candidate/requests/candidate";
        public const string GetAllTrainingRequestsForCandidateList = "training-candidate/requests/candidate/list";
        public const string GetTrainingRequestCountsForCandidate = "training-candidate/requests/candidate/count";
        public const string GetAllApprovedCandidatesForTraining = "training-candidate/approved-candidates"; 
        public const string GetAllApprovedCandidatesForTrainingList = "training-candidate/approved-candidates/list";
        public const string GetUnassignedCandidateForTraining = "training-candidate/unassigned-candidates";
        public const string GetUnassignedClientCandidateForTraining = "training-candidate/unassigned/client/candidates";
        public const string GetAllColleagueCandidatesForTraining = "training-candidate/colleague-candidates";
        public const string GetAllColleagueCandidatesForTrainingList = "training-candidate/colleague-candidates/list";
        public const string GetAllOrganizationalCandidatesForTraining = "training-candidate/organizational-candidates";
        public const string GetAllOrganizationalCandidatesForTrainingList = "training-candidate/organizational-candidates/list";
        public const string GetAllAssignedCandidatesForClient = "training-candidate/client-candidates";
        public const string GetAllAssignedCandidatesForClientList = "training-candidate/client-candidates/list";
        public const string GetAllClientCandidatesForTraining = "training-candidate/client-candidates";
        public const string GetClientOrganizationForCandidateCount = "training-candidate/client-candidates-count";
    }
    
    public abstract class TrainingInspection
    {
        public const string GetTrainingInspectionById = "training-inspection/details";
        public const string GetTrainingInspectionByQuestionnaire = "training-inspection/questionnaire/details";
        public const string GetAllTrainingInspections = "training-inspection";
        public const string GetAllTrainingInspectionsList = "training-inspection/list";
        public const string GetAllTrainingInspectionsForCandidate = "training-inspection/candidate";
        public const string GetAllTrainingInspectionsForCandidateList = "training-inspection/candidate/list";
        public const string GetAllTrainingInspectionsForClient = "training-inspection/client";
        public const string GetAllTrainingInspectionsForClientList = "training-inspection/client/list";
        public const string GetAllTrainingInspectionsForTrainingCandidate = "training-inspection/training-candidate";
        public const string GetAllTrainingInspectionsForTrainingCandidateList = "training-inspection/training-candidate/list";
        public const string GetCandidateTrainingInspectionDetails = "training-inspection/candidate/details";
        public const string GetSubordinateTrainingInspectionDetails = "training-inspection/subordinate/details";
        public const string GetCandidateTrainingInspectionDetailsForTrainingCandidate = "training-inspection/training-candidate/details";
        public const string GetTrainingInspectionQuestionnairesCount = "training-inspection/questionnaires/count";
        public const string GetTrainingInspectionPhaseCounts = "training-inspection/count";
        public const string AssignTrainingInspections = "training-inspection";
    }

    public abstract class User
    {
        public const string GetProfileByUserId = "user";
        public const string GetUsersByRole = "user";
        public const string GetUsersByRoleList = "user/list";
        public const string GetUsersForClientOrganization = "user/organization";
        public const string GetUsersForClientOrganizationList = "user/organization/list";
        public const string UpdateUserDetails = "user";
        public const string ActivateDeactivateUser = "user";
        public const string DeleteUser = "user";
    }
}