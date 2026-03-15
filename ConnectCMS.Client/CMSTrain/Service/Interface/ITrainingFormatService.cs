using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.TrainingFormat;
using CMSTrain.Client.Models.Responses.TrainingFormat;

namespace CMSTrain.Client.Service.Interface;

public interface ITrainingFormatService : ITransientService
{
    Task<CollectionDto<GetTrainingFormatDto>?> GetTrainingFormats(int pageNumber, int pageSize, string? search = null, bool? isActive = null);
    
    Task<ResponseDto<List<GetTrainingFormatDto>?>?> GetTrainingFormats(string? search = null, bool? isActive = null);

    Task<ResponseDto<GetTrainingFormatDto?>?> GetTrainingFormatById(Guid trainingFormatId);
    
    Task<ResponseDto<bool?>?> InsertTrainingFormat(CreateTrainingFormatDto trainingFormat);
    
    Task<ResponseDto<bool?>?> UpdateTrainingFormat(UpdateTrainingFormatDto trainingFormat);

    Task<ResponseDto<bool?>?> ActivateDeactivateTrainingFormat(Guid trainingFormatId);
}