using System.Net.Http.Headers;
using System.Text.Json;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Identity;

namespace CMSTrain.Client.Service.Implementation;

public class ProfileService(IBaseService baseService) : IProfileService
{
    public async Task<ResponseDto<UserDetail?>?> GetUserProfile()
    {
        var response = await baseService.GetAsync<UserDetail?>(ApiEndpoints.Profile.GetUserProfile);

        return response;
    }

    public async Task<ResponseDto<RolesDto?>?> GetUserRole()
    {
        var response = await baseService.GetAsync<RolesDto?>(ApiEndpoints.Profile.GetUserRole);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateProfile(ProfileRequestDto profile)
    {
        var jsonRequest = JsonSerializer.Serialize(profile);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.Profile.UpdateProfile, Constants.UpdateType.Put, content);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateProfileImage(ProfileImageRequestDto profileImage)
    {
        var formData = new MultipartFormDataContent();

        var profileFileContent = new StreamContent(profileImage.ImageUrl.OpenReadStream(long.MaxValue));

        profileFileContent.Headers.ContentType = new MediaTypeHeaderValue(profileImage.ImageUrl.ContentType);

        formData.Add(profileFileContent, "ImageUrl", profileImage.ImageUrl.Name);

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Profile.UpdateProfileImage, Constants.UploadType.Patch, formData);

        return response;
    }

    public async Task<ResponseDto<bool?>?> ChangePassword(ChangePasswordRequestDto changePassword)
    {
        var jsonRequest = JsonSerializer.Serialize(changePassword);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        var response = await baseService.UpdateAsync<bool?>(ApiEndpoints.Profile.ChangePassword, Constants.UpdateType.Put, content);

        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteUserProfile()
    {
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Profile.DeleteUserProfile, Constants.DeleteType.Delete);

        return response;
    }
}