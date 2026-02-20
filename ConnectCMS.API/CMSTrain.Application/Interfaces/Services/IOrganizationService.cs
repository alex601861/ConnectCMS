using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Organization;

namespace CMSTrain.Application.Interfaces.Services;

public interface IOrganizationService : ITransientService
{
    List<GetOrganizationDto> GetAllOrganizations(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);

    List<GetOrganizationDto> GetAllOrganizations(string? search = null, bool? isActive = null);
    
    GetOrganizationDto GetOrganizationById(Guid id);

    void InsertOrganization(CreateOrganizationDto organization);

    void UpdateOrganization(UpdateOrganizationDto organization);

    void ActivateDeactivateOrganization(Guid id);
    
    void DeleteOrganization(Guid organizationId);
}
