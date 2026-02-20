using Newtonsoft.Json.Linq;
using CMSTrain.Domain.Common.Property;
using CMSTrain.Application.Interfaces.Services;
using Newtonsoft.Json;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class KeyValuePropertyService : IKeyValuePropertyService
{
    public T? GetProperty<T>(KeyValueProperty keyValuePair)
    {
        switch (keyValuePair.Value)
        {
            case JArray jArray:
                return jArray.ToObject<T>();
            case JObject jObject:
                return jObject.ToObject<T>();
            case string jsonString:
                try
                {
                    return JsonConvert.DeserializeObject<T>(jsonString);
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException("Failed to deserialize the JSON string into the specified type.", ex);
                }

                break;
            default:
                return (T?)Convert.ChangeType(keyValuePair.Value, typeof(T));
        }
    }

    public KeyValueProperty SaveProperty(string key, object value)
    {
        return new KeyValueProperty()
        {
            Key = key,
            Value = value
        };
    }
}