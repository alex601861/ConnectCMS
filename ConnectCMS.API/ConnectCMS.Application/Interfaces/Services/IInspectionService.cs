using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Inspection;
using CMSTrain.Application.DTOs.Questionnaires;
using CMSTrain.Domain.Common.Enum;

namespace CMSTrain.Application.Interfaces.Services;

public interface IInspectionService : ITransientService
{
    List<GetInspectionDto> GetAllInspections(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);

    List<GetInspectionDto> GetAllInspections(string? search = null, bool? isActive = null);
    
    GetInspectionDto GetInspectionById(Guid inspectionId);

    GetInspectionDto GetInspectionByType(InspectionType inspectionType);

    List<GetInspectionDto> GetAllAvailableTrainingInspections();

    List<GetInspectionDto> GetAllAvailableTrainingInspections(int pageNumber, int pageSize, out int rowCount);

    List<GetInspectionDto> GetAllAssignedTrainingInspections(Guid trainingId, string? search = null);

    List<GetInspectionDto> GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    void InsertInspection(CreateInspectionDto inspection);

    void UpdateInspection(UpdateInspectionDto inspection);

    void ActivateDeactivateInspection(Guid inspectionId);
    
    void UploadInspectionQuestionnaires(UploadInspectionQuestionnaireDto inspectionQuestionnaires);
}