using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Responses.ClientOrganization;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;

namespace CMSTrain.Client.Service.Implementation;

public class ClientOrganizationService(IBaseService baseService) : IClientOrganizationService
{
    public async Task<CollectionDto<GetClientOrganizationDto>?> GetAllClientOrganizations(int pageNumber, int pageSize, string? search, bool? isActive)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive?.ToString() }
        };

        var response = await baseService.GetPagedAsync<GetClientOrganizationDto>(endpoint: ApiEndpoints.ClientOrganization.GetAllClientOrganizations, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetClientOrganizationDto>?>?> GetAllClientOrganizations(string? search, bool? isActive)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search },
            { "isActive", isActive?.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetClientOrganizationDto>?>(ApiEndpoints.ClientOrganization.GetAllClientOrganizationsList, parameters: queryParameter);

        return response;
    }

    public async Task<ResponseDto<List<GetClientOrganizationDto>?>?> GetAllClientOrganizationsWithoutAdmin()
    {
        var response = await baseService.GetAsync<List<GetClientOrganizationDto>?>(ApiEndpoints.ClientOrganization.GetAllClientOrganizationsWithoutAdmin);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> RegisterClientOrganizationAdmin(RegisterClientAdminDto clientAdmin)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(clientAdmin.Name ?? ""), "Name");
        formData.Add(new StringContent(clientAdmin.Email ?? ""), "Email");
        formData.Add(new StringContent(clientAdmin.Password ?? ""), "Password");
        formData.Add(new StringContent(clientAdmin.PhoneNumber ?? ""), "PhoneNumber");
        formData.Add(new StringContent(clientAdmin.ConfirmPassword ?? ""), "ConfirmPassword");
        formData.Add(new StringContent(clientAdmin.OrganizationId.ToString()), "OrganizationId");
        formData.Add(new StringContent(clientAdmin.Gender.ToString() ?? throw new InvalidOperationException()), "Gender");
        formData.Add(new StringContent(clientAdmin.Address ?? ""), "Address");
        
        if (clientAdmin.DesignationId != Guid.Empty || clientAdmin.DesignationId != null)
        {
            formData.Add(new StringContent(clientAdmin.DesignationId.ToString() ?? string.Empty), "DesignationId");
        }
        
        if (clientAdmin.CountryId != null || clientAdmin.CountryId != Guid.Empty)
        {
            formData.Add(new StringContent(clientAdmin.CountryId.ToString() ?? throw new InvalidOperationException()), "CountryId");
        }
        
        if (clientAdmin.ImageUrl != null)
        {
            var organizationFileContent = new StreamContent(clientAdmin.ImageUrl!.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(clientAdmin.ImageUrl.ContentType);
            
            formData.Add(organizationFileContent, "ImageUrl", clientAdmin.ImageUrl.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.ClientOrganization.RegisterClientOrganizationAdmin, Constants.UploadType.Post, formData);

        return response;
    }
}