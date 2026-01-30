namespace CMSTrain.Application.Interfaces.Services;

public interface ITrainingConfigurationService : IPropertyService
{
    void SavePropertyDetails(Guid moduleId, string key, object value);
}