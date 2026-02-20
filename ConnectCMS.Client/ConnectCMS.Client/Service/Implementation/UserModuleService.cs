using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.User;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Responses.User;
using CMSTrain.Client.Models.Responses.Identity;

namespace CMSTrain.Client.Service.Implementation;

public class UserModuleService(IBaseService baseService) : IUserModuleService
{
    public async Task<ResponseDto<UserDetail?>?> GetUserProfileById(Guid userId)
    {
        var pathParameter = new List<string>()
        {
            userId.ToString()
        };
        
        var response = await baseService.GetAsync<UserDetail?>(ApiEndpoints.User.GetProfileByUserId, pathParameter);

        return response;
    }
    
    public async Task<CollectionDto<UserResponseDto>?> GetUsersByRole(int pageNumber, int pageSize, bool? isActive = null, string? search = null, Guid? roleId = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "isActive", isActive.ToString() },
            { "search", search },
            { "roleId", roleId?.ToString() }
        };

        var response = await baseService.GetPagedAsync<UserResponseDto>(endpoint: ApiEndpoints.User.GetUsersByRole, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<UserResponseDto>?>?> GetUsersByRole(bool? isActive = null, string? search = null, Guid? roleId = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "isActive", isActive.ToString() },
            { "search", search },
            { "roleId", roleId?.ToString() }
        };
        
        var response = await baseService.GetAsync<List<UserResponseDto>?>(endpoint: ApiEndpoints.User.GetUsersByRoleList, parameters: queryParameter);

        return response;
    }

    public async Task<CollectionDto<UserResponseDto>?> GetUsersForClientOrganization(int pageNumber, int pageSize, string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive?.ToString() }
        };

        var response = await baseService.GetPagedAsync<UserResponseDto>(endpoint: ApiEndpoints.User.GetUsersForClientOrganization, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<UserResponseDto>?>?> GetUsersForClientOrganization(string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search },
            { "isActive", isActive?.ToString() }
        };
        
        var users = await baseService.GetAsync<List<UserResponseDto>?>(ApiEndpoints.User.GetUsersForClientOrganizationList, parameters: queryParameter);

        return users;
    }

    public async Task<ResponseDto<bool?>?> UpdateUserDetails(UpdateUserRequestDto user)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(user.Id.ToString()), "Id");
        formData.Add(new StringContent(user.Name), "Name");
        formData.Add(new StringContent(user.EmailAddress), "EmailAddress");
        formData.Add(new StringContent(user.PhoneNumber), "PhoneNumber");
        formData.Add(new StringContent(user.CountryId.ToString()), "CountryId");
        formData.Add(new StringContent(user.Gender.ToString() ?? ""), "Gender");
        formData.Add(new StringContent(user.Address ?? ""), "Address");

        if (user.DesignationId != null)
        {
            formData.Add(new StringContent(user.DesignationId.ToString() ?? string.Empty), "DesignationId");
        }
        
        if (user.OrganizationId != null)
        {
            formData.Add(new StringContent(user.OrganizationId.ToString() ?? string.Empty), "OrganizationId");
        }

        if (user.Image != null)
        {
            var organizationFileContent = new StreamContent(user.Image.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(user.Image.ContentType);
            
            formData.Add(organizationFileContent, "Image", user.Image.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.User.UpdateUserDetails, Constants.UploadType.Put, formData);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> ActivateDeactivateUsers(Guid userId)
    {
        var pathParameter = new List<string>()
        {
            userId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.User.ActivateDeactivateUser, Constants.UpdateType.Patch, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteUser(Guid userId)
    {
        var pathParameter = new List<string>()
        {
            userId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.User.DeleteUser, Constants.DeleteType.Delete, pathParameter);

        return response;
    }
}