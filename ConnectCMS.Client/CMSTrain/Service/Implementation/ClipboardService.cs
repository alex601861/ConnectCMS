using Microsoft.JSInterop;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class ClipboardService(IJSRuntime jsRuntime) : IClipboardService
{
    public async Task CopyTextToClipboard(string text)
    {
        await jsRuntime.InvokeVoidAsync("copyToClipboard", text);
    }
}