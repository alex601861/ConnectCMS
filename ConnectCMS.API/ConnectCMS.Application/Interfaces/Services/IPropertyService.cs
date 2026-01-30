using CMSTrain.Application.Common.Service;
using CMSTrain.Domain.Common.Property;

namespace CMSTrain.Application.Interfaces.Services;

public interface IPropertyService : ITransientService
{
    T? GetProperty<T>(Guid moduleId, string key);

    T GetPropertyOrDefault<T>(Guid moduleId, string key, T defaultProperty);

    List<KeyValueProperty> GetAllProperties(Guid moduleId, CancellationToken cancellationToken = default);

    void SaveProperty(Guid moduleId, string key, object value);

    void DeleteProperty(Guid moduleId, string key);
}