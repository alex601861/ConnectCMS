using Microsoft.AspNetCore.Components.Forms;

namespace CMSTrain.Client.Models.Constants;

public class BrowserFile : IBrowserFile
{
    private readonly MemoryStream _stream;

    public BrowserFile(byte[] fileData, string fileName, string contentType)
    {
        _stream = new MemoryStream(fileData);
        Name = fileName;
        ContentType = contentType;
        Size = fileData.Length;
        LastModified = DateTimeOffset.Now;
    }

    public string Name { get; }
    public string ContentType { get; }
    public long Size { get; }
    public DateTimeOffset LastModified { get; }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        if (Size > maxAllowedSize)
            throw new IOException($"File size ({Size} bytes) exceeds the maximum allowed size ({maxAllowedSize} bytes).");

        return _stream;
    }
}