using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Strategy;
using CMSTrain.Client.Models.Responses.Strategy;

namespace CMSTrain.Client.Service.Interface;

public interface IStrategicTraitService : ITransientService
{
    Task<CollectionDto<GetStrategyDto>?> GetAllStrategies(StrategicType traitType, int pageNumber, int pageSize, string? search = null);
    
    Task<ResponseDto<List<GetStrategyDto>?>?> GetAllStrategies();

    Task<ResponseDto<List<GetStrategyModuleDto>?>?> GetAllStrategyModules(StrategicType type);
    
    Task<ResponseDto<GetAllStrategyTraitResultsDto?>?> GetAllStrategyTraitResults(string strengthIds, string weaknessIds);
    
    Task<ResponseDto<GetStrategyDto?>?> GetStrategyById(Guid strategyId);

    Task<ResponseDto<GetStrategyDetailsDto?>?> GetStrategyDetails();

    Task<ResponseDto<bool?>?> InsertStrategy(InsertStrategyDto strategy);

    Task<ResponseDto<bool?>?> UpdateStrategy(UpdateStrategyDto strategy);

    Task<ResponseDto<bool?>?> DeleteStrategy(Guid strategyId);

    Task<ResponseDto<bool?>?> UploadStrategyDetails(UploadStrategyDetailsDto strategyDetails);

    Task<ResponseDto<bool?>?> UploadStrategyTraitQuestionnaire(UploadStrategyTraitQuestionnaireDto strategyDetails);
    
    Task<ResponseDto<GetStrategicTraitCountDto?>?> GetStrategicTraitCount();

    Task<CollectionDto<GetStrategyTraitQuestionnaireDto>?> GetStrategyTraitQuestionnaireResponses(int pageNumber, int pageSize, DateTime? startDate = null, DateTime? endDate = null);

    Task<ResponseDto<List<GetStrategyTraitQuestionnaireDto>?>?> GetStrategyTraitQuestionnaireResponses(DateTime? startDate = null, DateTime? endDate = null);
    
    Task<CollectionDto<GetStrategyTraitQuestionnaireDto>?> GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId, int pageNumber, int pageSize);

    Task<ResponseDto<List<GetStrategyTraitQuestionnaireDto>?>?> GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId);

    Task<ResponseDto<GetStrategyTraitQuestionnaireDetailsDto?>?> GetStrategyTraitQuestionnaireDetails(Guid responseId);
}