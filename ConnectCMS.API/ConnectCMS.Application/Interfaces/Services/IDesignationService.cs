using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Designation;

namespace CMSTrain.Application.Interfaces.Services;

public interface IDesignationService : ITransientService
{
    List<GetDesignationDto> GetAllDesignations(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);
    
    List<GetDesignationDto> GetAllDesignations(string? search = null, bool? isActive = null);
    
    GetDesignationDto GetDesignationById(Guid id);
    
    void InsertDesignation(CreateDesignationDto designation);
    
    void UpdateDesignation(UpdateDesignationDto designation);

    void ActivateDeactivateDesignation(Guid id);
}