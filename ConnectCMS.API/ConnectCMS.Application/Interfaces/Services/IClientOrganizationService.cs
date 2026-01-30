using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.ClientOrganization;

namespace CMSTrain.Application.Interfaces.Services;

public interface IClientOrganizationService : ITransientService
{
    List<GetClientOrganizationDto> GetAllClientOrganizations(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);

    List<GetClientOrganizationDto> GetAllClientOrganizations(string? search = null, bool? isActive = null);

    List<GetClientOrganizationDto> GetAllClientOrganizationsWithoutAdmin();
    
    void RegisterClientOrganizationAdmin(RegisterClientAdminDto clientAdmin);
}