using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class RoleService(IBaseService baseService) : IRoleService
{
    public async Task<CollectionDto<RolesDto>?> GetAllRoles(int pageNumber, int pageSize, string? search = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
        };

        var response = await baseService.GetPagedAsync<RolesDto>(endpoint: ApiEndpoints.Role.GetAllRoles, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<RolesDto>?>?> GetAllRoles()
    {
        var response = await baseService.GetAsync<List<RolesDto>?>(ApiEndpoints.Role.GetAllRolesList);

        return response;
    }
    
    public async Task<ResponseDto<List<RolesDto>?>?> GetPrecedingRoles()
    {
        var response = await baseService.GetAsync<List<RolesDto>?>(ApiEndpoints.Role.GetPrecedingRoles);

        return response;
    }

    public async Task<ResponseDto<RolesDto?>?> GetRoleById(Guid roleId)
    {
        var pathParameter = new List<string>
        {
            roleId.ToString()
        };
        
        var response = await baseService.GetAsync<RolesDto?>(ApiEndpoints.Role.GetRoleById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> InsertRole(RolesDto role)
    {
        var jsonRequest = JsonSerializer.Serialize(role);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Role.InsertRole, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateRole(RolesDto role)
    {
        var jsonRequest = JsonSerializer.Serialize(role);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.Role.UpdateRole, Constants.UpdateType.Put, content);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteRole(Guid roleId)
    {
        var pathParameter = new List<string>
        {
            roleId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Role.DeleteRole, Constants.DeleteType.Delete, pathParameter);

        return response;
    }
}