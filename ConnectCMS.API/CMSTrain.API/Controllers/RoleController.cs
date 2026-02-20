using CMSTrain.Application.DTOs.Identity;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services.Identity;

namespace CMSTrain.Controllers;

[Route("api/roles")]
public class RoleController(IRoleService roleService) : BaseController<RoleController>
{
    [HttpGet]
    public IActionResult GetAllRoles(int pageNumber, int pageSize, string? search)
    {
        var roles = roleService.GetAllRoles(pageNumber, pageSize, out var rowCount, search);

        return Ok(new CollectionDto<RolesDto>(roles, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Roles successfully retrieved."
        });
    }

    [HttpGet("preceding")]
    public IActionResult GetPrecedingRoles()
    {
        var roles = roleService.GetPrecedingRoles();
    
        return Ok(new ResponseDto<List<RolesDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Roles successfully retrieved.",
            Result = roles
        });
    }
    
    [HttpGet("list")]
    public IActionResult GetAllRoles()
    {
        var roles = roleService.GetAllRoles();

        return Ok(new ResponseDto<List<RolesDto>>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Roles successfully retrieved.",
            Result = roles
        });
    }

    [HttpPost]
    public async Task<IActionResult> InsertRole(RolesDto role)
    {
        await roleService.InsertRole(role);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Role successfully created.",
            Result = true
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateRole(RolesDto role)
    {
        await roleService.UpdateRole(role);

        return Ok(new ResponseDto<bool>
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Role successfully updated.",
            Result = true
        });
    }

    [HttpDelete("{roleId:guid}")]
    public async Task<IActionResult> DeleteRole(Guid roleId)
    {
        await roleService.DeleteRole(roleId);
        
        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Role successfully deleted.",
            Result = true
        });
    }
}
