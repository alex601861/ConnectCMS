namespace CMSTrain.Client.Models.Constants;

public abstract class Constants
{
    public abstract class Roles
    {
        public const string SuperAdmin = "Super Admin";
        public const string Admin = "Admin";
        public const string Trainer = "Trainer";
        public const string Client = "Client";
        public const string Candidate = "Candidate";
    }
    
    public abstract class UserRoles
    {
        public const string ClientAdmin = "Client Admin";
        public const string ClientCandidate = "Client Registered Candidate";
    }
    
    public abstract class UploadType
    {
        public const string Post = "Post";
        public const string Put = "Put";
        public const string Patch = "Patch";
    }

    public abstract class Pagination
    {
        public const int Page = 1;
        public const int Size = 12;
    }
    
    public abstract class UpdateType
    {
        public static readonly string Put = "Put";
        public static readonly string Patch = "Patch";
    }
    
    public abstract class DeleteType
    {
        public static readonly string Patch = "Patch";
        public static readonly string Delete = "Delete";
    }
    
    public abstract class Encryption
    {
        public const string Key = "@ff!N1ty";
    }

    public abstract class Provider
    {
        public const string Wasm = "WASM";
    }
    
    public abstract class Message
    {
        public const string FileSizeUploadMessage = "Please upload a valid file of less than 5 MB.";
        public const string ExceptionMessage = "An exception occured while handling your request, please try again.";
        public const string ImageUploadMessage = "Please upload a valid image file with the following extensions (.jpg, .jpeg, .png) only.";
        public const string ResourceUploadMessage = "Please upload a resource material file with the following extensions (.jpg, .jpeg, .png, .pdf) only.";
        public const string UnauthorizedMessage = "You are not authorized to access the module, please log in with your credentials for authentication purposes.";
    }
    
    public abstract class Uploads
    {
        public const string Image = "Image";
        public const string Resource = "Resource";

        public const string ImageExtensions = "image/x-jpg,image/x-png,image/jpeg";
        public const string ResourceExtensions = "image/x-jpg,image/x-png,image/jpeg,application/pdf,image/gif";
    }
    
    private abstract class FolderPath
    {
        public const string Images = "images";
        public const string Resources = "resources";
    }

    public abstract class FilePath
    {
        public const string UsersImagesFilePath = $"{FolderPath.Images}/user-images";
        public const string TrainingsImagesFilePath = $"{FolderPath.Images}/training-images";
        public const string OrganizationsImagesFilePath = $"{FolderPath.Images}/organization-images";
        public const string InspectionImagesFilePath = $"{FolderPath.Images}/inspection-images";
        public const string ClassesImagesFilePath = $"{FolderPath.Images}/class-images";
        public const string CertificationsImagesFilePath = $"{FolderPath.Images}/certification-images/";

        public const string ResourcesFilePath = $"{FolderPath.Resources}";
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
    
    public abstract class CandidateType
    {
        public const int All = 1;
        public const int Organizational = 2;
       
        public const string AllAction = "All";
        public const string OrganizationalAction = "Organizational";
    }
    
    public abstract class ResourceMaterial
    {
        public const string Training = "training";
        public const string Class = "class";
    }
    
    public abstract class SubordinateType
    {
        public static int? All = null;
        public const int Junior = 1;
        public const int Peer = 2;
        public const int Supervisor = 3;
       
        public const string AllAction = "All";
        public const string JuniorAction = "Junior";
        public const string PeerAction = "Peer";
        public const string SupervisorAction = "Supervisor";
    }
    
    public abstract class ActivationStatus
    {
        public static bool? All = null;
        
        public const bool Active = true;
        public const bool Inactive = false;
        
        public const string AllAction = "All";
        public const string ActiveAction = "Active";
        public const string InactiveAction = "Inactive";
    }
    
    public abstract class TimePeriod
    {
        public const int All = 0;
        public const int Weekly = 1;
        public const int Monthly = 2;
        public const int Yearly = 3;
        
        public const string AllAction = "All";
        public const string WeeklyAction = "Weekly";
        public const string MonthlyAction = "Monthly";
        public const string YearlyAction = "Yearly";
    }
    
    public abstract class Schedule
    {
        public static int? All = null;
        public const int Scheduled = -1;
        public const int Completed = 1;
        public const int InProgress = 0;
        
        public const string AllAction = "All";
        public const string ScheduledAction = "Scheduled";
        public const string CompletedAction = "Completed";
        public const string InProgressAction = "In-Progress";
    }
    
    public abstract class Css
    {
        public const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
    }
    
    public abstract class FontFamily
    {
        public const string Satoshi = "Satoshi";
        public const string Poppins = "Poppins";
        public const string Roboto = "Roboto";
        public const string Lato = "Lato";
        public const string Inter = "Inter";
    }
    
    public abstract class PrimaryColor
    {
        public const string Yellow = "Yellow";
        public const string Orange = "Orange";
    }
    
    public abstract class LocalStorage
    {
        public const string Jwt = "jwt";
        public const string Token = "token";
        public const string Preference = "preference";
        public const string Navigation = "return-url";
        public const string LogOutHandler = "log-out-handler";
    }
    
    public abstract class Menu
    {
        public const string Path = "images/menu/";
        public const string DefaultIcon = "<svg width=\"18\" height=\"18\" viewBox=\"0 0 18 18\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\">\r\n<path d=\"M13.3425 17.0566H4.6575C2.6025 17.0566 0.9375 15.3841 0.9375 13.3291V7.77162C0.9375 6.75162 1.5675 5.46912 2.3775 4.83912L6.42 1.68912C7.635 0.744121 9.5775 0.699121 10.8375 1.58412L15.4725 4.83162C16.365 5.45412 17.0625 6.78912 17.0625 7.87662V13.3366C17.0625 15.3841 15.3975 17.0566 13.3425 17.0566ZM7.11 2.57412L3.0675 5.72412C2.535 6.14412 2.0625 7.09662 2.0625 7.77162V13.3291C2.0625 14.7616 3.225 15.9316 4.6575 15.9316H13.3425C14.775 15.9316 15.9375 14.7691 15.9375 13.3366V7.87662C15.9375 7.15662 15.42 6.15912 14.8275 5.75412L10.1925 2.50662C9.3375 1.90662 7.9275 1.93662 7.11 2.57412Z\" fill=\"#141414\"/>\r\n<path d=\"M5.62508 12.9356C5.48258 12.9356 5.34008 12.8831 5.22758 12.7706C5.01008 12.5531 5.01008 12.1931 5.22758 11.9756L7.62758 9.57563C7.74758 9.45563 7.90508 9.39562 8.07758 9.41062C8.24258 9.42562 8.39258 9.51562 8.49008 9.65812L9.30758 10.8881L11.9701 8.22563C12.1876 8.00813 12.5476 8.00813 12.7651 8.22563C12.9826 8.44313 12.9826 8.80312 12.7651 9.02062L9.61508 12.1706C9.49508 12.2906 9.33758 12.3506 9.16508 12.3356C9.00008 12.3206 8.85008 12.2306 8.75258 12.0881L7.93508 10.8581L6.02258 12.7706C5.91008 12.8831 5.76758 12.9356 5.62508 12.9356Z\" fill=\"#141414\"/>\r\n<path d=\"M12.375 10.6875C12.0675 10.6875 11.8125 10.4325 11.8125 10.125V9.1875H10.875C10.5675 9.1875 10.3125 8.9325 10.3125 8.625C10.3125 8.3175 10.5675 8.0625 10.875 8.0625H12.375C12.6825 8.0625 12.9375 8.3175 12.9375 8.625V10.125C12.9375 10.4325 12.6825 10.6875 12.375 10.6875Z\" fill=\"#141414\"/>\r\n</svg>\r\n";
    }
    
    public abstract class Resource
    {
        public const string Training = "training";
        public const string Class = "class";
    }
}