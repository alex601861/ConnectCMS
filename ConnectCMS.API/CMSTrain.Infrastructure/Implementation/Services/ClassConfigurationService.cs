using Newtonsoft.Json.Linq;
using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Property;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class ClassConfigurationService(IGenericRepository genericRepository) : IClassConfigurationService
{
    public T? GetProperty<T>(Guid moduleId, string key)
    {
        var classConfiguration = genericRepository.GetFirstOrDefault<ClassConfiguration>(x => x.ClassId == moduleId && x.PropertyPair.Key == key)
            ?? throw new NotFoundException("The following class's configuration could not be found.");
        
        if (classConfiguration.PropertyPair.Value is JObject jObject)
        {
            return jObject.ToObject<T>();
        }
        
        return (T?)Convert.ChangeType(classConfiguration.PropertyPair.Value, typeof(T));
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
        var properties = genericRepository.Get<ClassConfiguration>(x => x.ClassId == moduleId).ToList();

        return properties.ConvertAll(x => x.PropertyPair);
    }

    public void SaveProperty(Guid moduleId, string key, object value)
    {
        var @class = genericRepository.GetById<Class>(moduleId)
            ?? throw new NotFoundException("The following class could not be found.");
        
        var classConfiguration = genericRepository.GetFirstOrDefault<ClassConfiguration>(x => 
            x.ClassId == @class.Id && x.PropertyPair.Key == key);

        if (classConfiguration == null)
        {
            var newClassConfiguration = new ClassConfiguration
            {
                ClassId = @class.Id,
                PropertyPair = new KeyValueProperty
                {
                    Key = key,
                    Value = value
                }
            };
            
            genericRepository.Insert(newClassConfiguration);
        }
        else
        {
            classConfiguration.PropertyPair = new KeyValueProperty
            {
                Key = key,
                Value = value
            };
            
            genericRepository.Update(classConfiguration);
        }
    }

    public void DeleteProperty(Guid moduleId, string key)
    {
        var @class = genericRepository.GetById<Class>(moduleId)
                     ?? throw new NotFoundException("The following class could not be found.");
        
        var classConfiguration = genericRepository.GetFirstOrDefault<ClassConfiguration>(x => 
            x.ClassId == @class.Id && x.PropertyPair.Key == key)
            ?? throw new NotFoundException("The following class configuration could not be found.");
        
        genericRepository.Delete(classConfiguration);
    }
}