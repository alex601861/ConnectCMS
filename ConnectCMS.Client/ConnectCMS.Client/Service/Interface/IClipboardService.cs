using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Interface;

public interface IClipboardService : ITransientService
{
    Task CopyTextToClipboard(string text);
}