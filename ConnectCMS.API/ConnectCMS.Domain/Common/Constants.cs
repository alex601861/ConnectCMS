namespace CMSTrain.Domain.Common;

public abstract class Constants
{
    public abstract class Roles
    {
        public const string Superadmin = "Super Admin";
        public const string Admin = "Admin";
        public const string Trainer = "Trainer";
        public const string Client = "Client";
        public const string Candidate = "Candidate";
    }

    public abstract class SuperAdmin
    {
        public const string Identifier = "66cd3c59-b2e9-4bd9-8e39-65f550c59c1c";

        public abstract class Development
        {
            public const string Name = "Affinity";
            public const string EmailAddress = "affinity@affinity.io";
            public const string DecryptedPassword = "radi0V!oleta";
        }
        
        public abstract class Production
        {
            public const string Name = "Connect Admin";
            public const string EmailAddress = "it@cmsnpl.com.np";
            public const string DecryptedPassword = "(W5n3P@!@2025";
        }
        
        public abstract class Hangfire
        {
            public const string Username = "affinity@affinity.io";
            public const string DecryptedPassword = "@ff!N1ty";
        }
        
        public abstract class MailSettings
        {
            public const string Username = "Siddhartha";
            public const string EmailAddress = "siddhartha.affinity@gmail.com";
        }
    }

    public abstract class DbProviderKeys
    {
        public const string SqlServer = "mssql";
        public const string Npgsql = "postgresql";
    }

    private abstract class FolderPath
    {
        public const string Images = "images";
        public const string Resources = "resources";
        public const string EmailTemplates = "email-templates";
    }

    public abstract class FilePath
    {
        public const string UsersImagesFilePath = $"{FolderPath.Images}/user-images/";
        public const string TrainingsImagesFilePath = $"{FolderPath.Images}/training-images/";
        public const string ClassesImagesFilePath = $"{FolderPath.Images}/class-images/";
        public const string AttendanceImageFilePath = $"{FolderPath.Images}/attendance-images/";
        public const string InspectionsImagesFilePath = $"{FolderPath.Images}/inspection-images/";
        public const string OrganizationsImagesFilePath = $"{FolderPath.Images}/organization-images/";
        public const string CertificationsImagesFilePath = $"{FolderPath.Images}/certification-images/";

        public const string ResourcesFilePath = $"{FolderPath.Resources}/";
        public const string EmailTemplateFilePath = $"{FolderPath.EmailTemplates}/";
    }

    public abstract class RequestAction
    {
        public const int Pending = 0;
        public const int Accepted = 1;
        public const int Rejected = -1;
        public const int Available = 2;

        public const string PendingAction = "Pending";
        public const string AcceptedAction = "Accepted";
        public const string RejectedAction = "Rejected";
        
        public const string MarkedAction = "Marked";
        public const string AvailableAction = "Available";
        public const string NotMarkedAction = "Not Marked";
    }

    public abstract class StatusAction
    {
        public const int Available = 1;
        public const int All = 0;
        public const int Expired = -1;
       
        public const string AvailableAction = "Available";
        public const string ExpiredAction = "Expired";
        public const string AllAction = "All";
    }
    
    public abstract class Schedule
    {
        public const int Scheduled = -1;
        public const int Completed = 1;
        public const int InProgress = 0;
        
        public const string ScheduledAction = "Scheduled";
        public const string CompletedAction = "Completed";
        public const string InProgressAction = "In-Progress";
    }
    
    public abstract class TimePeriod
    {
        public const int All = 0;
        public const int Weekly = 1;
        public const int Monthly = 2;
        public const int Yearly = 3;
    }
    
    public abstract class SubordinateType
    {
        public const int Junior = 1;
        public const int Peer = 2;
        public const int Supervisor = 3;
    }
    
    public abstract class Encryption
    {
        public const string Key = "@ff!N1ty";
    }

    public abstract class Provider
    {
        public const string Api = "API";
        public const string Wasm = "WASM";
    }
        
    public abstract class Cors
    {
        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    }
    
    public abstract class Navigation
    {
        public const string SelfRegistration = "email-verification-confirmation";
        public const string ResetPassword = "reset-password";
        public const string ResourceMaterialDownload = "resource/view";
        public const string SubordinateAnswerUploadForm = "subordinate-answer-upload-form";
        
        public const string StrategicTraitQuestionnaireAnswerUploadForm = "strategic-trait-questionnaire";
        public const string PersonalityTestQuestionnaireAnswerUploadForm = "personality-test";
        public const string FeedbackAssessmentQuestionnaireAnswerUploadForm = "candidate-answer-upload-form";
    }
    
    public abstract class Resource
    {
        public const string Training = "training";
        public const string Class = "class";
    }
    
    public abstract class Authentication
    {
        public const string PasswordCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}|;:,.<>?/";
    }
    
    public abstract class CountryFlag
    {
        public const string Url = "https://cdn.jsdelivr.net/npm/country-flag-emoji-json@2.0.0/dist/images";
    }
    
    public abstract class Period
    {
        public const int Weekly = 1;
        public const int Monthly = 2;
        public const int Yearly = 3;
    }
}
