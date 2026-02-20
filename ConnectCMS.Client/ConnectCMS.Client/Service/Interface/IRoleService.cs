using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Identity;

namespace CMSTrain.Client.Service.Interface;

public interface IRoleService : ITransientService
{
    Task<CollectionDto<RolesDto>?> GetAllRoles(int pageNumber, int pageSize, string? search = null);
    
    Task<ResponseDto<List<RolesDto>?>?> GetAllRoles();
    
    Task<ResponseDto<List<RolesDto>?>?> GetPrecedingRoles();

    Task<ResponseDto<RolesDto?>?> GetRoleById(Guid roleId);
    
    Task<ResponseDto<bool?>?> InsertRole(RolesDto role);

    Task<ResponseDto<bool?>?> UpdateRole(RolesDto role);

    Task<ResponseDto<bool?>?> DeleteRole(Guid roleId);
}