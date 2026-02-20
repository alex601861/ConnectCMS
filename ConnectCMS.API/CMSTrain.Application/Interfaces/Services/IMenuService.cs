using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Menu;

namespace CMSTrain.Application.Interfaces.Services;

public interface IMenuService : ITransientService
{
    void AssignMenus(RoleMenuRequestDto roleMenu);

    List<RoleMenuResponseDto> GetAllRoleMenus(Guid roleId);

    List<RoleMenuResponseDto> GetAllAssignedMenus();
}