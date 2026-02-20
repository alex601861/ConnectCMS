using CMSTrain.Application.DTOs.Menu;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services;

namespace CMSTrain.Controllers;

[Route("api/role-rights")]
public class RoleRightsController(IMenuService menuService) : BaseController<RoleRightsController>
{
    [HttpGet]
    public IActionResult GetAllAssignedMenus()
    {
        var result = menuService.GetAllAssignedMenus();

        return Ok(new ResponseDto<List<RoleMenuResponseDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Assigned role rights successfully retrieved.",
            Result = result
        });
    }
    
    [HttpGet("{roleId:guid}")]
    public IActionResult GetAllRoleMenus(Guid roleId)
    {
        var result = menuService.GetAllRoleMenus(roleId);

        return Ok(new ResponseDto<List<RoleMenuResponseDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Role rights successfully retrieved.",
            Result = result
        });
    }

    [HttpPost]
    public IActionResult AssignRoleMenus(RoleMenuRequestDto roleMenus)
    {
        menuService.AssignMenus(roleMenus);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Role rights successfully assigned.",
            Result = true
        });
    }
}
