using System.Text.Json;
using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Configuration;
using CMSTrain.Client.Models.Requests.Configuration.Class;
using CMSTrain.Client.Models.Requests.Configuration.Training;
using TrainingInspectionConfiguration = CMSTrain.Client.Models.Requests.Configuration.TrainingInspection.TrainingInspectionConfiguration;

namespace CMSTrain.Client.Service.Implementation;

public class ConfigurationService(IBaseService baseService) : IConfigurationService
{
    public async Task<ResponseDto<List<KeyValueProperty>?>?> GetAllTrainingConfigurations(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<KeyValueProperty>?>(ApiEndpoints.Configuration.GetAllTrainingConfigurations, pathParameter);

        return response;
    }

    public async Task<ResponseDto<TrainingResourceConfiguration?>?> GetTrainingResourceConfigurationByKey(Guid trainingId, string key)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            key
        };
        
        var response = await baseService.GetAsync<TrainingResourceConfiguration?>(ApiEndpoints.Configuration.GetTrainingResourceConfigurationByKey, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> SaveTrainingResourceConfiguration(Guid trainingId, string key, TrainingResourceConfiguration trainingResourceConfiguration)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            key
        };
        
        var jsonRequest = JsonSerializer.Serialize(trainingResourceConfiguration);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Configuration.SaveTrainingResourceConfiguration, content, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<TrainingCertificationConfigurationDownload?>?> GetTrainingCertificationConfigurationByKey(Guid trainingId, string key)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            key
        };
        
        var response = await baseService.GetAsync<TrainingCertificationConfigurationDownload?>(ApiEndpoints.Configuration.GetTrainingCertificationConfigurationByKey, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> SaveTrainingCertificationConfiguration(Guid trainingId, string key, TrainingCertificationConfigurationUpload trainingCertificationConfiguration)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            key
        };
        
        var formData = new MultipartFormDataContent();
        var certification = trainingCertificationConfiguration.Certification;
        
        formData.Add(new StringContent(certification.PrimaryColor), "trainingCertificationConfigurationUpload.Certification.PrimaryColor");
        formData.Add(new StringContent(certification.SecondaryColor), "trainingCertificationConfigurationUpload.Certification.SecondaryColor");
        formData.Add(new StringContent(certification.TertiaryColor), "trainingCertificationConfigurationUpload.Certification.TertiaryColor");

        if (certification.PrimaryLogo != null)
        {
            var primaryLogoFileContent = new StreamContent(certification.PrimaryLogo!.OpenReadStream(long.MaxValue));
            
            primaryLogoFileContent.Headers.ContentType = new MediaTypeHeaderValue(certification.PrimaryLogo.ContentType);
            
            formData.Add(primaryLogoFileContent, "trainingCertificationConfigurationUpload.Certification.PrimaryLogo", certification.PrimaryLogo.Name);
        }
        
        if (certification.SecondaryLogo != null)
        {
            var secondaryLogoFileContent = new StreamContent(certification.SecondaryLogo!.OpenReadStream(long.MaxValue));
            
            secondaryLogoFileContent.Headers.ContentType = new MediaTypeHeaderValue(certification.SecondaryLogo.ContentType);
            
            formData.Add(secondaryLogoFileContent, "trainingCertificationConfigurationUpload.Certification.SecondaryLogo", certification.SecondaryLogo.Name);
        }
        
        if (certification.Signature != null)
        {
            var signatureFileContent = new StreamContent(certification.Signature!.OpenReadStream(long.MaxValue));
            
            signatureFileContent.Headers.ContentType = new MediaTypeHeaderValue(certification.Signature.ContentType);
            
            formData.Add(signatureFileContent, "trainingCertificationConfigurationUpload.Certification.Signature", certification.Signature.Name);
        }
        
        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Configuration.SaveTrainingCertificationConfiguration, Constants.UploadType.Post, formData, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<TrainingCertificationTriggerConfiguration?>?> GetTrainingCertificationTriggerConfigurationByKey(Guid trainingId, string key)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            key
        };
        
        var response = await baseService.GetAsync<TrainingCertificationTriggerConfiguration?>(ApiEndpoints.Configuration.GetTrainingCertificationTriggerConfigurationByKey, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> SaveTrainingCertificationTriggerConfiguration(Guid trainingId, string key, TrainingCertificationTriggerConfiguration trainingCertificationConfiguration)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            key
        };
        
        var jsonRequest = JsonSerializer.Serialize(trainingCertificationConfiguration);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Configuration.SaveTrainingCertificationTriggerConfiguration, content, pathParameter);
        
        return response;
    }
    
    public async Task<ResponseDto<bool?>?> DeleteTrainingConfiguration(Guid trainingId, string key)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString(),
            key
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Configuration.DeleteTrainingConfiguration, Constants.DeleteType.Delete, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<List<KeyValueProperty>?>?> GetAllClassConfigurations(Guid classId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var response = await baseService.GetAsync<List<KeyValueProperty>?>(ApiEndpoints.Configuration.GetAllClassConfigurations, pathParameter);

        return response;
    }

    public async Task<ResponseDto<ClassResourceConfiguration?>?> GetClassResourceConfigurationByKey(Guid classId, string key)
    {
        var pathParameter = new List<string>
        {
            classId.ToString(),
            key
        };
        
        var response = await baseService.GetAsync<ClassResourceConfiguration?>(ApiEndpoints.Configuration.GetClassResourceConfigurationByKey, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> SaveClassResourceConfiguration(Guid classId, string key, ClassResourceConfiguration classResourceConfiguration)
    {
        var pathParameter = new List<string>
        {
            classId.ToString(),
            key
        };
        
        var jsonRequest = JsonSerializer.Serialize(classResourceConfiguration);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Configuration.SaveClassResourceConfiguration, content, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<ClassAttendanceConfiguration?>?> GetClassAttendanceConfigurationByKey(Guid classId, string key)
    {
        var pathParameter = new List<string>
        {
            classId.ToString(),
            key
        };
        
        var response = await baseService.GetAsync<ClassAttendanceConfiguration?>(ApiEndpoints.Configuration.GetClassAttendanceConfigurationByKey, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> SaveClassAttendanceConfiguration(Guid classId, string key, ClassAttendanceConfiguration classAttendanceConfiguration)
    {
        var pathParameter = new List<string>
        {
            classId.ToString(),
            key
        };
        
        var jsonRequest = JsonSerializer.Serialize(classAttendanceConfiguration);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Configuration.SaveClassAttendanceConfiguration, content, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteClassConfiguration(Guid classId, string key)
    {
        var pathParameter = new List<string>
        {
            classId.ToString(),
            key
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Configuration.DeleteClassConfiguration, Constants.DeleteType.Delete, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<List<KeyValueProperty>?>?> GetAllTrainingInspectionConfigurations(Guid trainingInspectionId)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<List<KeyValueProperty>?>(ApiEndpoints.Configuration.GetAllTrainingInspectionConfigurations, pathParameter);

        return response;
    }

    public async Task<ResponseDto<TrainingInspectionConfiguration?>?> GetTrainingInspectionConfigurationByKey(Guid trainingInspectionId, string key)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString(),
            key
        };
        
        var response = await baseService.GetAsync<TrainingInspectionConfiguration?>(ApiEndpoints.Configuration.GetTrainingInspectionConfigurationByKey, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> SaveTrainingInspectionConfiguration(Guid trainingInspectionId, string key, TrainingInspectionConfiguration trainingInspectionConfiguration)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString(),
            key
        };
        
        var jsonRequest = JsonSerializer.Serialize(trainingInspectionConfiguration);
        
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Configuration.SaveTrainingInspectionConfiguration, content, pathParameter);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteTrainingInspection(Guid trainingInspectionId, string key)
    {
        var pathParameter = new List<string>
        {
            trainingInspectionId.ToString(),
            key
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Configuration.DeleteClassConfiguration, Constants.DeleteType.Delete, pathParameter);
        
        return response;
    }
}