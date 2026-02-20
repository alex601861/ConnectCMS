using CMSTrain.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Property;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.Configuration.Training;
using CMSTrain.Application.Interfaces.Repositories.Base;
using SchedulerModel = CMSTrain.Domain.Common.Enum.Scheduler;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class TrainingConfigurationService(IGenericRepository genericRepository, 
    IHangfireService hangfireService,
    IFileService fileService) : ITrainingConfigurationService
{
    private const string CertificationsImageFilePath = Constants.FilePath.CertificationsImagesFilePath;

    public T? GetProperty<T>(Guid moduleId, string key)
    {
        var trainingConfiguration = genericRepository.GetFirstOrDefault<TrainingConfiguration>(x => x.TrainingId == moduleId && x.PropertyPair.Key == key)
                                    ?? throw new NotFoundException("The following training's configuration could not be found.");
        
        if (trainingConfiguration.PropertyPair.Value is JObject jObject)
        {
            return jObject.ToObject<T>();
        }

        return (T?)Convert.ChangeType(trainingConfiguration.PropertyPair.Value, typeof(T));
    }

    public T GetPropertyOrDefault<T>(Guid moduleId, string key, T defaultProperty)
    {
        var result = GetProperty<T>(moduleId, key);
            
        if (result != null) return result;

        if (defaultProperty != null) SaveProperty(moduleId, key, defaultProperty);
            
        return defaultProperty;
    }

    public List<KeyValueProperty> GetAllProperties(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var properties = genericRepository.Get<TrainingConfiguration>(x => x.TrainingId == moduleId).ToList();

        return properties.ConvertAll(x => x.PropertyPair);
    }

    public void SaveProperty(Guid moduleId, string key, object value)
    {
        var training = genericRepository.GetById<Training>(moduleId)
            ?? throw new NotFoundException("The following training could not be found.");
        
        var trainingConfiguration = genericRepository.GetFirstOrDefault<TrainingConfiguration>(x => 
            x.TrainingId == training.Id && x.PropertyPair.Key == key);

        if (trainingConfiguration == null)
        {
            var newTrainingConfiguration = new TrainingConfiguration
            {
                TrainingId = training.Id,
                PropertyPair = new KeyValueProperty
                {
                    Key = key,
                    Value = value
                }
            };
            
            genericRepository.Insert(newTrainingConfiguration);
        }
        else
        {
            trainingConfiguration.PropertyPair = new KeyValueProperty
            {
                Key = key,
                Value = value
            };
            
            genericRepository.Update(trainingConfiguration);
        }
    }

    public void SavePropertyDetails(Guid moduleId, string key, object value)
    {
        var training = genericRepository.GetById<Training>(moduleId)
                       ?? throw new NotFoundException("The following training could not be found.");
        
        var trainingConfiguration = genericRepository.GetFirstOrDefault<TrainingConfiguration>(x => 
            x.TrainingId == training.Id && x.PropertyPair.Key == key);

        var trainingCertificationConfigurationModel = ParseToTrainingCertificationConfiguration(value)?.Certification;

        if (trainingCertificationConfigurationModel != null)
        {
            var filePath = Path.Combine(CertificationsImageFilePath, training.Id.ToString());

            fileService.DeleteFolder(filePath);

            var signature = trainingCertificationConfigurationModel.Signature != null
                ? fileService.UploadDocument(trainingCertificationConfigurationModel.Signature, filePath)
                : string.Empty;
            
            var primaryLogo = trainingCertificationConfigurationModel.PrimaryLogo != null 
                ? fileService.UploadDocument(trainingCertificationConfigurationModel.PrimaryLogo, filePath)
                : string.Empty;
            
            var secondaryLogo = trainingCertificationConfigurationModel.SecondaryLogo != null 
                ? fileService.UploadDocument(trainingCertificationConfigurationModel.SecondaryLogo, filePath)
                : string.Empty;
            
            var trainingCertificationConfiguration = new TrainingCertificationConfigurationDownload()
            {
                Certification = new AbstractTrainingCertificationConfigurationDownloadDto()
                {
                    PrimaryColor = trainingCertificationConfigurationModel.PrimaryColor,
                    SecondaryColor = trainingCertificationConfigurationModel.SecondaryColor,
                    TertiaryColor = trainingCertificationConfigurationModel.TertiaryColor,
                    Signature = signature,
                    PrimaryLogo = primaryLogo,
                    SecondaryLogo = secondaryLogo
                }
            };

            if (trainingConfiguration == null)
            {
                var newTrainingConfiguration = new TrainingConfiguration
                {
                    TrainingId = training.Id,
                    PropertyPair = new KeyValueProperty
                    {
                        Key = key,
                        Value = trainingCertificationConfiguration
                    }
                };
                
                genericRepository.Insert(newTrainingConfiguration);
            }
            else
            {
                trainingConfiguration.PropertyPair = new KeyValueProperty
                {
                    Key = key,
                    Value = trainingCertificationConfiguration
                };
                
                genericRepository.Update(trainingConfiguration);
            }
            
            return;
        }
        
        var trainingCertificationTriggerConfigurationModel = ParseToTrainingCertificationTriggerConfiguration(value)?.Trigger;

        if (trainingCertificationTriggerConfigurationModel != null)
        {
            var trainingCertificationTriggerConfiguration = new TrainingCertificationTriggerConfiguration()
            {
                Trigger = new AbstractTrainingCertificationTriggerConfigurationDto()
                {
                    IsManual = trainingCertificationTriggerConfigurationModel.IsManual
                }
            };
            
            if (training.EndDate < DateOnly.FromDateTime(ExtensionMethod.GetDateTimeInLocalTimeZone()) || 
               (training.EndDate == DateOnly.FromDateTime(ExtensionMethod.GetDateTimeInLocalTimeZone()) && ExtensionMethod.GetDateTimeInLocalTimeZone().Hour > 16))
            {
                return;
            }
            
            if (trainingConfiguration == null)
            {
                var newTrainingConfiguration = new TrainingConfiguration
                {
                    TrainingId = training.Id,
                    PropertyPair = new KeyValueProperty
                    {
                        Key = key,
                        Value = trainingCertificationTriggerConfiguration
                    }
                };
                
                genericRepository.Insert(newTrainingConfiguration);
            }
            else
            {
                trainingConfiguration.PropertyPair = new KeyValueProperty
                {
                    Key = key,
                    Value = trainingCertificationTriggerConfiguration
                };
                
                genericRepository.Update(trainingConfiguration);
            }
            
            // Certifications are generated only after manually approving all the attendances.
            if (trainingCertificationTriggerConfigurationModel.IsManual)
            {
                hangfireService.RemoveRecurringJobs(training.Id.ToString());
            }
            else
            {
                hangfireService.HandleRecurringJob(training.Id.ToString(), $"15 11 {training.EndDate.Day} {training.EndDate.Month} ? {training.EndDate.Year}", SchedulerModel.CertificationTrigger);
            }
            
            return;
        }
        
        throw new BadRequestException("The following training certification configuration could not be parsed.", []);
    }

    public void DeleteProperty(Guid moduleId, string key)
    {
        var training = genericRepository.GetById<Training>(moduleId)
                     ?? throw new NotFoundException("The following training could not be found.");
        
        var trainingConfiguration = genericRepository.GetFirstOrDefault<TrainingConfiguration>(x => 
                                        x.TrainingId == training.Id && x.PropertyPair.Key == key)
                                    ?? throw new NotFoundException("The following training configuration could not be found.");
        
        genericRepository.Delete(trainingConfiguration);
    }
    
    private static TrainingCertificationConfigurationUpload? ParseToTrainingCertificationConfiguration(object value)
    {
        if (value is TrainingCertificationConfigurationUpload config)
        {
            return config;
        }

        try
        {
            return JsonConvert.DeserializeObject<TrainingCertificationConfigurationUpload>(value.ToString() ?? string.Empty);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    private static TrainingCertificationTriggerConfiguration? ParseToTrainingCertificationTriggerConfiguration(object value)
    {
        if (value is TrainingCertificationTriggerConfiguration config)
        {
            return config;
        }

        try
        {
            return JsonConvert.DeserializeObject<TrainingCertificationTriggerConfiguration>(value.ToString() ?? string.Empty);
        }
        catch (Exception)
        {
            return null;
        }
    }
}