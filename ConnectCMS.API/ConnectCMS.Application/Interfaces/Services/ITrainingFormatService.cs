using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.TrainingFormat;

namespace CMSTrain.Application.Interfaces.Services;

public interface ITrainingFormatService : ITransientService
{
    List<GetTrainingFormatDto> GetAllTrainingFormats(int pageNumber, int pageSize, out int rowCount, string? search, bool? isActive);

    List<GetTrainingFormatDto> GetAllTrainingFormats(string? search, bool? isActive);
    
    GetTrainingFormatDto GetTrainingFormatById(Guid id);

    void InsertTrainingFormat(CreateTrainingFormatDto trainingFormat);

    void UpdateTrainingFormat(UpdateTrainingFormatDto trainingFormat);

    void ActivateDeactivateTrainingFormat(Guid id);
}
