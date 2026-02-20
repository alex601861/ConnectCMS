using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Organization;
using CMSTrain.Client.Models.Responses.Organization;

namespace CMSTrain.Client.Service.Interface;

public interface IOrganizationService : ITransientService
{
    Task<ResponseDto<List<GetOrganizationDto>?>?> GetAllOrganizations();

    Task<ResponseDto<GetOrganizationDto?>?> GetOrganizationById(Guid organizationId);
    
    Task<ResponseDto<bool?>?> InsertOrganization(CreateOrganizationDto organization);

    Task<ResponseDto<bool?>?> UpdateOrganization(UpdateOrganizationDto organization);

    Task<ResponseDto<bool?>?> ActivateDeactivateOrganization(Guid organizationId);
    
    Task<ResponseDto<bool?>?> DeleteOrganization(Guid organizationId);
}