using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Identity;

namespace CMSTrain.Application.Interfaces.Services.Identity;

public interface IProfileService : ITransientService
{
    UserDetail GetUserProfile();

    RolesDto GetUserRole();

    void UpdateUserProfile(ProfileRequestDto profileDetails);

    void UpdateProfileImage(ProfileImageRequestDto profileImage);

    Task ChangePassword(ChangePasswordRequestDto changePasswordDto);

    void DeleteUserProfile();
}