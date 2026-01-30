using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Interfaces.Services;

public interface IQrCodeService : ITransientService
{
    string GenerateQrCode(string data);
}