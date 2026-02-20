using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Requests.Designation;
using CMSTrain.Client.Models.Responses.Designation;
using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Interface;

public interface IDesignationService : ITransientService
{
    Task<CollectionDto<GetDesignationDto>?> GetAllDesignations(int pageNumber, int pageSize, string? search = null, bool? isActive = null);
    
    Task<ResponseDto<List<GetDesignationDto>?>?> GetAllDesignations(string? search = null, bool? isActive = null);

    Task<ResponseDto<GetDesignationDto?>?> GetDesignationById(Guid countryId);
    
    Task<ResponseDto<bool?>?> InsertDesignation(CreateDesignationDto designation);

    Task<ResponseDto<bool?>?> UpdateDesignation(UpdateDesignationDto designation);

    Task<ResponseDto<bool?>?> ActivateDeactivateDesignation(Guid designationId);
}