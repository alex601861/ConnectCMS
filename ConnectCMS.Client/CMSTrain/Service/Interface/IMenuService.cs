using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Requests.Menu;
using CMSTrain.Client.Models.Responses.Menu;
using CMSTrain.Client.Service.Dependency;

namespace CMSTrain.Client.Service.Interface;

public interface IMenuService : ITransientService
{
    Task<ResponseDto<List<RoleMenuResponseDto>?>?> GetAllRoleMenus(Guid roleId);

    Task<ResponseDto<List<RoleMenuResponseDto>?>?> GetAllAssignedMenus();

    Task<ResponseDto<bool?>?> AssignRoleMenus(RoleMenuRequestDto menuRights);
}