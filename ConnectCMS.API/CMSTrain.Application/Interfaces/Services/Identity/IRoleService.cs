using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Identity;

namespace CMSTrain.Application.Interfaces.Services.Identity;

public interface IRoleService : ITransientService
{
    List<RolesDto> GetAllRoles();

    List<RolesDto> GetAllRoles(int pageNumber, int pageSize, out int rowCount, string? search = null);
    
    List<RolesDto> GetPrecedingRoles();

    Task InsertRole(RolesDto role);

    Task UpdateRole(RolesDto role);

    Task DeleteRole(Guid roleId);
}