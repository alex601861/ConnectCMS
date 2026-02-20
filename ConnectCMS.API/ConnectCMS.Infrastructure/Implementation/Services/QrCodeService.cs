using QRCoder;
using SixLabors.ImageSharp;
using CMSTrain.Application.Exceptions;
using SixLabors.ImageSharp.Formats.Png;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class QrCodeService : IQrCodeService
{
    public string GenerateQrCode(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new BadRequestException("QR Code could not be generated.",
                ["The data to generate the QR code is empty."]);
        
        using var qrGenerator = new QRCodeGenerator();
        
        var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        
        var qrCode = new PngByteQRCode(qrCodeData);
        
        var qrCodeAsPngByteArr = qrCode.GetGraphic(20);

        using var image = Image.Load(qrCodeAsPngByteArr);
        
        using var ms = new MemoryStream();
        
        image.Save(ms, new PngEncoder());
        
        return Convert.ToBase64String(ms.ToArray());
    }
}