using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Domain.Common.Property;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using SchedulerModel = CMSTrain.Domain.Common.Enum.Scheduler;
using TrainingInspectionConfigurationModel = CMSTrain.Application.DTOs.Configuration.TrainingInspection.TrainingInspectionConfiguration;
using AbstractTrainingInspectionConfigurationDto = CMSTrain.Application.DTOs.Configuration.TrainingInspection.AbstractTrainingInspectionConfigurationDto;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class TrainingInspectionConfigurationService(IGenericRepository genericRepository, IHangfireService hangfireService) : ITrainingInspectionConfigurationService
{
    public T? GetProperty<T>(Guid trainingInspectionId, string key)
    {
        var trainingInspectionConfiguration = genericRepository.GetFirstOrDefault<TrainingInspectionConfiguration>(x => x.TrainingInspectionId == trainingInspectionId && x.PropertyPair.Key == key)
                                    ?? throw new NotFoundException("The following training inspection's configuration could not be found.");
        
        if (trainingInspectionConfiguration.PropertyPair.Value is JObject jObject)
        {
            return jObject.ToObject<T>();
        }

        return (T?)Convert.ChangeType(trainingInspectionConfiguration.PropertyPair.Value, typeof(T));
    }

    public T GetPropertyOrDefault<T>(Guid trainingInspectionId, string key, T defaultProperty)
    {
        var result = GetProperty<T>(trainingInspectionId, key);
            
        if (result != null) return result;

        if (defaultProperty != null) SaveProperty(trainingInspectionId, key, defaultProperty);
            
        return defaultProperty;
    }

    public List<KeyValueProperty> GetAllProperties(Guid trainingInspectionId, CancellationToken cancellationToken = default)
    {
        var properties = genericRepository.Get<TrainingInspectionConfiguration>(x => x.TrainingInspectionId == trainingInspectionId).ToList();

        return properties.ConvertAll(x => x.PropertyPair);
    }

    public void SaveProperty(Guid trainingInspectionId, string key, object value)
    {
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionId)
            ?? throw new NotFoundException("The following assigned training inspection could not be found.");

        var inspection = genericRepository.GetById<Inspection>(trainingInspection.InspectionId)
                         ?? throw new NotFoundException("The following inspection could not be found.");
        
        var trainingInspectionConfigurationModel = ParseToTrainingInspectionConfiguration(value);
        
        var trainingInspectionConfiguration = genericRepository.GetFirstOrDefault<TrainingInspectionConfiguration>(x => 
            x.TrainingInspectionId == trainingInspection.Id && x.PropertyPair.Key == key);

        Guid configurationId;
        var cronJobs = GenerateCronFromFirstAccessPeriod(trainingInspectionConfigurationModel.Accessibility);
        
        if (trainingInspectionConfiguration == null)
        {
            var newTrainingConfiguration = new TrainingInspectionConfiguration
            {
                TrainingInspectionId = trainingInspection.Id,
                PropertyPair = new KeyValueProperty
                {
                    Key = key,
                    Value = value
                }
            };
            
            configurationId = genericRepository.Insert(newTrainingConfiguration);
        }
        else
        {
            trainingInspectionConfiguration.PropertyPair = new KeyValueProperty
            {
                Key = key,
                Value = value
            };
            
            genericRepository.Update(trainingInspectionConfiguration);
            
            configurationId = trainingInspectionConfiguration.Id;
        }

        if (inspection.InspectionType != InspectionType.PersonalAssessment) return;
        
        hangfireService.RemoveRecurringJobs(configurationId.ToString());
            
        foreach (var cronJob in cronJobs)
        {
            var index = cronJobs.IndexOf(cronJob);

            var recurringJob = $"{configurationId.ToString()} % {index + 1}";
                
            hangfireService.HandleRecurringJob(recurringJob, cronJob, SchedulerModel.PersonalAssessment);
        }
    }

    public void DeleteProperty(Guid trainingInspectionId, string key)
    {
        var trainingInspection = genericRepository.GetById<TrainingInspection>(trainingInspectionId)
                                 ?? throw new NotFoundException("The following assigned training inspection could not be found.");
        
        var trainingInspectionConfiguration = genericRepository.GetFirstOrDefault<TrainingInspectionConfiguration>(x => 
                x.TrainingInspectionId == trainingInspection.Id && x.PropertyPair.Key == key)
                                    ?? throw new NotFoundException("The following training inspection configuration could not be found.");
        
        genericRepository.Delete(trainingInspectionConfiguration);
    }
    
    private static TrainingInspectionConfigurationModel ParseToTrainingInspectionConfiguration(object value)
    {
        if (value is TrainingInspectionConfigurationModel config)
        {
            return config;
        }

        try
        {
            return JsonConvert.DeserializeObject<TrainingInspectionConfigurationModel>(value.ToString()!) 
                   ?? throw new InvalidOperationException("The provided value cannot be parsed into Training Inspection Configuration.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("The provided value cannot be parsed into Training Inspection Configuration.", ex);
        }
    }

    private static List<string> GenerateCronFromFirstAccessPeriod(List<AbstractTrainingInspectionConfigurationDto> accessibilityPeriods)
    {
        return accessibilityPeriods.Select(accessibilityPeriod => 
            $"15 6 " +
            $"{accessibilityPeriod.AccessPeriod.Day - 1} " +
            $"{accessibilityPeriod.AccessPeriod.Month} * " +
            $"{accessibilityPeriod.AccessPeriod.Year}").ToList();
    }
}