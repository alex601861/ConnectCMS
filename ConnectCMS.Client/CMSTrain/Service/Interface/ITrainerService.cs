using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.ClassTrainers;

namespace CMSTrain.Client.Service.Interface;

public interface ITrainerService : ITransientService
{
    Task<ResponseDto<List<GetTrainersDto>?>?> GetAllActiveTrainers();
}