using CMSTrain.Application.DTOs.User;
using CMSTrain.Application.DTOs.Identity;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services.Identity;

namespace CMSTrain.Controllers;

[Route("api/user")]
public class UserController(IUserService userService) : BaseController<UserController>
{
    [HttpGet("{userId:guid}")]
    public IActionResult GetProfileByUserId(Guid userId)
    {
        var result = userService.GetUserProfileById(userId);

        return Ok(new ResponseDto<UserDetail>()
        {
            Message = "Profile successfully retrieved.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = result
        });
    }

    [HttpGet]
    public IActionResult GetUsersByRole(int pageNumber, int pageSize, bool? isActive, string? search, Guid? roleId)
    {
        var users = userService.GetUsersByRole(pageNumber, pageSize, out var rowCount, isActive, search, roleId);

        return Ok(new CollectionDto<UserResponseDto>(users, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Users successfully retrieved."
        });
    }

    [HttpGet("list")]
    public IActionResult GetUsersByRole(bool? isActive, string? search, Guid? roleId)
    {
        var users = userService.GetUsersByRole(isActive, search, roleId);

        return Ok(new ResponseDto<List<UserResponseDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Users successfully retrieved.",
            Result = users
        });
    }

    [HttpGet("organization")]
    public IActionResult GetUsersForClientOrganization(int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var users = userService.GetUsersForClientOrganization(pageNumber, pageSize, out var rowCount, search, isActive);

        return Ok(new CollectionDto<UserResponseDto>(users, rowCount, pageNumber, pageSize)
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Users successfully retrieved."
        });
    }

    [HttpGet("organization/list")]
    public IActionResult GetClientOrganizationUsers(string? search, bool? isActive)
    {
        var users = userService.GetUsersForClientOrganization(search, isActive);

        return Ok(new ResponseDto<List<UserResponseDto>>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "Users successfully retrieved.",
            Result = users
        });
    }

    [HttpPut]
    public IActionResult UpdateUserDetails(UpdateUserRequestDto user)
    {
        userService.UpdateUserDetails(user);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User details successfully updated.",
            Result = true
        });
    }
    
    [HttpPatch("{userId:guid}")]
    public IActionResult ActivateDeactivateUser(Guid userId)
    {
        userService.ActivateDeactivateUsers(userId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The status of user successfully updated.",
            Result = true
        });
    }
    
    [HttpDelete("{userId:guid}")]
    public IActionResult DeleteUser(Guid userId)
    {
        userService.DeleteUser(userId);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "The user has been successfully deleted.",
            Result = true
        });
    }
}
