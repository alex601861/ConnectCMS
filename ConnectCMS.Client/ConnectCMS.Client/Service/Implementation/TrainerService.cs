using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.ClassTrainers;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class TrainerService(IBaseService baseService) : ITrainerService
{
    public async Task<ResponseDto<List<GetTrainersDto>?>?> GetAllActiveTrainers()
    {
        var response = await baseService.GetAsync<List<GetTrainersDto>?>(ApiEndpoints.Trainer.GetAllActiveTrainersList);

        return response;
    }
}