using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Identity;

namespace CMSTrain.Client.Service.Interface;

public interface IProfileService : ITransientService
{
    Task<ResponseDto<UserDetail?>?> GetUserProfile();

    Task<ResponseDto<RolesDto?>?> GetUserRole();
    
    Task<ResponseDto<bool?>?> UpdateProfile(ProfileRequestDto profile);
    
    Task<ResponseDto<bool?>?> UpdateProfileImage(ProfileImageRequestDto profileImage);

    Task<ResponseDto<bool?>?> ChangePassword(ChangePasswordRequestDto changePassword);

    Task<ResponseDto<bool?>?> DeleteUserProfile();
}