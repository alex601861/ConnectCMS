using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Menu;
using CMSTrain.Client.Models.Responses.Menu;

namespace CMSTrain.Client.Service.Implementation;

public class MenuService(IBaseService baseService) : IMenuService
{
    public async Task<ResponseDto<List<RoleMenuResponseDto>?>?> GetAllRoleMenus(Guid roleId)
    {
        var pathParameter = new List<string>
        {
            roleId.ToString()
        };
        
        var response = await baseService.GetAsync<List<RoleMenuResponseDto>?>(ApiEndpoints.RoleRights.GetAllRoleMenus, pathParameter);

        return response;
    }

    public async Task<ResponseDto<List<RoleMenuResponseDto>?>?> GetAllAssignedMenus()
    {
        var response = await baseService.GetAsync<List<RoleMenuResponseDto>?>(ApiEndpoints.RoleRights.GetAllAssignedMenus);

        return response;
    }

    public async Task<ResponseDto<bool?>?> AssignRoleMenus(RoleMenuRequestDto menuRights)
    {
        var jsonRequest = JsonSerializer.Serialize(menuRights);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.RoleRights.AssignRoleMenus, content);

        return response;
    }
}