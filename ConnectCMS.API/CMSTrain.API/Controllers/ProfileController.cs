using CMSTrain.Application.DTOs.Identity;
using CMSTrain.Application.Common.Response;
using CMSTrain.Application.Interfaces.Services.Identity;

namespace CMSTrain.Controllers;

[Route("api/profile")]
public class ProfileController(IProfileService profileService) : BaseController<ProfileController>
{
    [HttpGet]
    public IActionResult GetUserProfile()
    {
        var result = profileService.GetUserProfile();

        return Ok(new ResponseDto<UserDetail>()
        {
            Message = "Profile successfully retrieved.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = result
        });
    }
    
    [HttpGet("role")]
    public IActionResult GetUserRole()
    {
        var result = profileService.GetUserRole();

        return Ok(new ResponseDto<RolesDto>()
        {
            Message = "Role successfully retrieved.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = result
        });
    }

    [HttpPut]
    public IActionResult UpdateProfile(ProfileRequestDto profile)
    {
        profileService.UpdateUserProfile(profile);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User Profile successfully updated.",
            Result = true
        });
    }
    
    [HttpPatch("image")]
    public IActionResult UpdateProfileImage([FromForm] ProfileImageRequestDto profileImage)
    {
        profileService.UpdateProfileImage(profileImage);

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User Profile Image successfully updated.",
            Result = true
        });
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto changePassword)
    {
        await profileService.ChangePassword(changePassword);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Password changed successfully.",
            StatusCode = (int)HttpStatusCode.OK,
            Result = true
        });
    }

    [HttpDelete]
    public IActionResult DeleteUserProfile()
    {
        profileService.DeleteUserProfile();

        return Ok(new ResponseDto<bool>()
        {
            StatusCode = (int)HttpStatusCode.OK,
            Message = "User successfully deleted.",
            Result = true
        });
    }
}
