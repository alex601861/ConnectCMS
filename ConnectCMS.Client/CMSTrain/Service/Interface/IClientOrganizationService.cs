using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.ClientOrganization;

namespace CMSTrain.Client.Service.Interface;

public interface IClientOrganizationService : ITransientService
{
    Task<CollectionDto<GetClientOrganizationDto>?> GetAllClientOrganizations(int pageNumber, int pageSize, string? search = null, bool? isActive = null);
    
    Task<ResponseDto<List<GetClientOrganizationDto>?>?> GetAllClientOrganizations(string? search = null, bool? isActive = null);

    Task<ResponseDto<List<GetClientOrganizationDto>?>?> GetAllClientOrganizationsWithoutAdmin();

    Task<ResponseDto<bool?>?> RegisterClientOrganizationAdmin(RegisterClientAdminDto clientAdmin);
}