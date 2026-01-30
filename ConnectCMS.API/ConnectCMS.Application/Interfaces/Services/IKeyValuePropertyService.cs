using CMSTrain.Application.Common.Service;
using CMSTrain.Domain.Common.Property;

namespace CMSTrain.Application.Interfaces.Services;

public interface IKeyValuePropertyService : ITransientService
{
    T? GetProperty<T>(KeyValueProperty keyValuePair);
    
    KeyValueProperty SaveProperty(string key, object value);
}