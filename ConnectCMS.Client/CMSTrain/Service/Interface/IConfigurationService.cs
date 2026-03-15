using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Configuration;
using CMSTrain.Client.Models.Requests.Configuration.Class;
using CMSTrain.Client.Models.Requests.Configuration.Training;
using CMSTrain.Client.Models.Requests.Configuration.TrainingInspection;

namespace CMSTrain.Client.Service.Interface;

public interface IConfigurationService : ITransientService
{
    #region Training
    Task<ResponseDto<List<KeyValueProperty>?>?> GetAllTrainingConfigurations(Guid trainingId);
    
    Task<ResponseDto<TrainingResourceConfiguration?>?> GetTrainingResourceConfigurationByKey(Guid trainingId, string key);

    Task<ResponseDto<bool?>?> SaveTrainingResourceConfiguration(Guid trainingId, string key, TrainingResourceConfiguration trainingResourceConfiguration);

    Task<ResponseDto<TrainingCertificationConfigurationDownload?>?> GetTrainingCertificationConfigurationByKey(Guid trainingId, string key);

    Task<ResponseDto<bool?>?> SaveTrainingCertificationConfiguration(Guid trainingId, string key, TrainingCertificationConfigurationUpload trainingCertificationConfiguration);

    Task<ResponseDto<TrainingCertificationTriggerConfiguration?>?> GetTrainingCertificationTriggerConfigurationByKey(Guid trainingId, string key);

    Task<ResponseDto<bool?>?> SaveTrainingCertificationTriggerConfiguration(Guid trainingId, string key, TrainingCertificationTriggerConfiguration trainingCertificationConfiguration);
    
    Task<ResponseDto<bool?>?> DeleteTrainingConfiguration(Guid trainingId, string key);
    #endregion
    
    #region Class
    Task<ResponseDto<List<KeyValueProperty>?>?> GetAllClassConfigurations(Guid classId);
    
    Task<ResponseDto<ClassResourceConfiguration?>?> GetClassResourceConfigurationByKey(Guid classId, string key);
    
    Task<ResponseDto<bool?>?> SaveClassResourceConfiguration(Guid classId, string key, ClassResourceConfiguration classResourceConfiguration);
    
    Task<ResponseDto<ClassAttendanceConfiguration?>?> GetClassAttendanceConfigurationByKey(Guid classId, string key);
    
    Task<ResponseDto<bool?>?> SaveClassAttendanceConfiguration(Guid classId, string key, ClassAttendanceConfiguration classAttendanceConfiguration);
    
    Task<ResponseDto<bool?>?> DeleteClassConfiguration(Guid classId, string key);
    #endregion
    
    #region Training Inspection
    Task<ResponseDto<List<KeyValueProperty>?>?> GetAllTrainingInspectionConfigurations(Guid trainingInspectionId);
    
    Task<ResponseDto<TrainingInspectionConfiguration?>?> GetTrainingInspectionConfigurationByKey(Guid trainingInspectionId, string key);

    Task<ResponseDto<bool?>?> SaveTrainingInspectionConfiguration(Guid trainingInspectionId, string key, TrainingInspectionConfiguration trainingInspectionConfiguration);
    
    Task<ResponseDto<bool?>?> DeleteTrainingInspection(Guid trainingInspectionId, string key);
    #endregion
}