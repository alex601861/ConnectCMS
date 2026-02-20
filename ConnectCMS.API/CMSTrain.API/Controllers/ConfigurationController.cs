using CMSTrain.Domain.Common.Property;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.Configuration.Class;
using CMSTrain.Application.DTOs.Configuration.Training;
using CMSTrain.Application.DTOs.Configuration.TrainingInspection;

namespace CMSTrain.Controllers;

[Route("api/configuration")]
public class ConfigurationController (
    IClassConfigurationService classConfigurationService, 
    ITrainingConfigurationService trainingConfigurationService, 
    ITrainingInspectionConfigurationService trainingInspectionConfigurationService) : BaseController<ConfigurationController>
{
    #region Training
    [HttpGet("training/{trainingId:guid}")]
    public IActionResult GetAllTrainingConfigurations(Guid trainingId)
    {
        var result = trainingConfigurationService.GetAllProperties(trainingId);
     
        return Ok(new ResponseDto<List<KeyValueProperty>>
        {
            Result = result,
            Message = "Successfully retrieved all training configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpGet("training/resource/{trainingId:guid}/{key}")]
    public IActionResult GetTrainingResourceConfigurationByKey(Guid trainingId, string key)
    {
        var result = trainingConfigurationService.GetProperty<TrainingResourceConfiguration>(trainingId, key);
     
        return Ok(new ResponseDto<TrainingResourceConfiguration>
        {
            Result = result,
            Message = "Successfully retrieved training resources configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpPost("training/resource/{trainingId:guid}/{key}")]
    public IActionResult SaveTrainingResourceConfiguration(Guid trainingId, string key, TrainingResourceConfiguration trainingResourceConfiguration)
    {
        trainingConfigurationService.SaveProperty(trainingId, key, trainingResourceConfiguration);
     
        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully saved training resource configurations.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }
    
    [HttpGet("training/certification/{trainingId:guid}/{key}")]
    public IActionResult GetTrainingCertificationConfigurationByKey(Guid trainingId, string key)
    {
        var result = trainingConfigurationService.GetProperty<TrainingCertificationConfigurationDownload>(trainingId, key);
     
        return Ok(new ResponseDto<TrainingCertificationConfigurationDownload>
        {
            Result = result,
            Message = "Successfully retrieved training certification configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpPost("training/certification/{trainingId:guid}/{key}")]
    public IActionResult SaveTrainingCertificationConfiguration(Guid trainingId, string key, [FromForm] TrainingCertificationConfigurationUpload trainingCertificationConfigurationUpload)
    {
        trainingConfigurationService.SavePropertyDetails(trainingId, key, trainingCertificationConfigurationUpload);
     
        return Ok(new ResponseDto<bool>
        {
            Message = "Successfully saved training certification configuration.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }
    
    [HttpGet("training/certification-trigger/{trainingId:guid}/{key}")]
    public IActionResult GetTrainingCertificationTriggerConfigurationByKey(Guid trainingId, string key)
    {
        var result = trainingConfigurationService.GetProperty<TrainingCertificationTriggerConfiguration>(trainingId, key);
     
        return Ok(new ResponseDto<TrainingCertificationTriggerConfiguration>
        {
            Result = result,
            Message = "Successfully retrieved training certification trigger point configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpPost("training/certification-trigger/{trainingId:guid}/{key}")]
    public IActionResult SaveTrainingCertificationTriggerConfiguration(Guid trainingId, string key, TrainingCertificationTriggerConfiguration trainingCertificationTriggerConfiguration)
    {
        trainingConfigurationService.SavePropertyDetails(trainingId, key, trainingCertificationTriggerConfiguration);
     
        return Ok(new ResponseDto<bool>
        {
            Message = "Successfully saved training certification trigger point configuration.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }

    [HttpDelete("training/{trainingId:guid}/{key}")]
    public IActionResult DeleteTrainingConfiguration(Guid trainingId, string key)
    {
        trainingConfigurationService.DeleteProperty(trainingId, key);
     
        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully deleted training configuration.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }
    #endregion

    #region Class
    [HttpGet("class/{classId:guid}")]
    public IActionResult GetAllClassConfigurations(Guid classId)
    {
        var result = classConfigurationService.GetAllProperties(classId);
     
        return Ok(new ResponseDto<List<KeyValueProperty>>
        {
            Result = result,
            Message = "Successfully retrieved all class configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpGet("class/resource/{classId:guid}/{key}")]
    public IActionResult GetClassResourceConfigurationByKey(Guid classId, string key)
    {
        var result = classConfigurationService.GetProperty<ClassResourceConfiguration>(classId, key);
     
        return Ok(new ResponseDto<ClassResourceConfiguration>
        {
            Result = result,
            Message = "Successfully retrieved class resource configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpPost("class/resource/{classId:guid}/{key}")]
    public IActionResult SaveClassResourceConfiguration(Guid classId, string key, ClassResourceConfiguration classResourceConfiguration)
    {
        classConfigurationService.SaveProperty(classId, key, classResourceConfiguration);
     
        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully saved class resource configuration.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }
    
    [HttpGet("class/attendance/{classId:guid}/{key}")]
    public IActionResult GetClassAttendanceConfigurationByKey(Guid classId, string key)
    {
        var result = classConfigurationService.GetProperty<ClassAttendanceConfiguration>(classId, key);
     
        return Ok(new ResponseDto<ClassAttendanceConfiguration>
        {
            Result = result,
            Message = "Successfully retrieved class attendance configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpPost("class/attendance/{classId:guid}/{key}")]
    public IActionResult SaveClassAttendanceConfiguration(Guid classId, string key, ClassAttendanceConfiguration classAttendanceConfiguration)
    {
        classConfigurationService.SaveProperty(classId, key, classAttendanceConfiguration);
     
        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully saved class attendance configuration.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }
    
    [HttpDelete("class/{classId:guid}/{key}")]
    public IActionResult DeleteClassConfiguration(Guid classId, string key)
    {
        classConfigurationService.DeleteProperty(classId, key);
     
        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully deleted class configuration.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }
    #endregion

    #region Training Inspection
    [HttpGet("training-inspection/{trainingInspectionId:guid}")]
    public IActionResult GetAllTrainingInspectionConfigurations(Guid trainingInspectionId)
    {
        var result = trainingInspectionConfigurationService.GetAllProperties(trainingInspectionId);
     
        return Ok(new ResponseDto<List<KeyValueProperty>>
        {
            Result = result,
            Message = "Successfully retrieved all training inspection configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpGet("training-inspection/{trainingInspectionId:guid}/{key}")]
    public IActionResult GetTrainingInspectionConfigurationByKey(Guid trainingInspectionId, string key)
    {
        var result = trainingInspectionConfigurationService.GetProperty<TrainingInspectionConfiguration>(trainingInspectionId, key);
     
        return Ok(new ResponseDto<TrainingInspectionConfiguration>
        {
            Result = result,
            Message = "Successfully retrieved training inspection configurations.",
            StatusCode = (int)HttpStatusCode.OK
        });
    }
    
    [HttpPost("training-inspection/{trainingInspectionId:guid}/{key}")]
    public IActionResult SaveTrainingInspectionConfiguration(Guid trainingInspectionId, string key, TrainingInspectionConfiguration trainingInspectionConfiguration)
    {
        trainingInspectionConfigurationService.SaveProperty(trainingInspectionId, key, trainingInspectionConfiguration);
     
        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully saved training inspection configuration.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }
    
    [HttpDelete("training-inspection/{trainingInspectionId:guid}/{key}")]
    public IActionResult DeleteTrainingInspectionConfiguration(Guid trainingInspectionId, string key)
    {
        trainingInspectionConfigurationService.DeleteProperty(trainingInspectionId, key);
     
        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully deleted training inspection configuration.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }
    #endregion
}