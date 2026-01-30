using CMSTrain.Application.DTOs.Strategy;
using CMSTrain.Application.Common.Service;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.Interfaces.Services;

public interface IStrategyTraitService : ITransientService
{
    List<GetStrategyDto> GetAllStrategies(StrategicType traitType, int pageNumber, int pageSize, out int rowCount, string? search = null);

    List<GetStrategyDto> GetAllStrategies();

    List<GetStrategyModuleDto> GetAllStrategyModules(StrategicType type);

    GetAllStrategyTraitResultsDto GetAllStrategyTraitResults(string? strengthIds, string? weaknessIds);
    
    GetStrategyDto GetStrategyById(Guid strategyId);
    
    GetStrategyDetailsDto GetStrategyDetails();

    void InsertStrategy(InsertStrategyDto strategy);

    void UpdateStrategy(UpdateStrategyDto strategy);
    
    void DeleteStrategy(Guid strategyId);

    void UploadStrategyDetails(UploadStrategyDetailsDto strategyDetails);

    void UploadStrategyTraitQuestionnaire(UploadStrategyTraitQuestionnaireDto strategyDetails);
    
    GetStrategicTraitCountDto GetStrategicTraitCount();
    
    List<GetStrategyTraitQuestionnaireDto> GetStrategyTraitQuestionnaireResponses(int pageNumber, int pageSize, out int rowCount, DateTime? startDate = null, DateTime? endDate = null);

    List<GetStrategyTraitQuestionnaireDto> GetStrategyTraitQuestionnaireResponses(DateTime? startDate = null, DateTime? endDate = null);

    List<GetStrategyTraitQuestionnaireDto> GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId, int pageNumber, int pageSize, out int rowCount);
    
    List<GetStrategyTraitQuestionnaireDto> GetStrategyTraitQuestionnaireResponsesByUserId(Guid userId);
    
    GetStrategyTraitQuestionnaireDetailsDto GetStrategyTraitQuestionnaireDetails(Guid responseId);
}