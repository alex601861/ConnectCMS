using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Organization;
using CMSTrain.Client.Models.Responses.Organization;

namespace CMSTrain.Client.Service.Implementation;

public class OrganizationService(IBaseService baseService) : IOrganizationService
{
    public async Task<ResponseDto<List<GetOrganizationDto>?>?> GetAllOrganizations()
    {
        var response = await baseService.GetAsync<List<GetOrganizationDto>?>(ApiEndpoints.Organization.GetAllOrganizationsList);

        return response;
    }

    public async Task<ResponseDto<GetOrganizationDto?>?> GetOrganizationById(Guid organizationId)
    {
        var pathParameter = new List<string>
        {
            organizationId.ToString()
        };
        
        var response = await baseService.GetAsync<GetOrganizationDto?>(ApiEndpoints.Organization.GetOrganizationById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> InsertOrganization(CreateOrganizationDto organization)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(organization.Name), "Name");
        formData.Add(new StringContent(organization.Address ?? ""), "Address");
        formData.Add(new StringContent(organization.Description ?? ""), "Description");

        if (organization.ImageUrl != null)
        {
            var organizationFileContent = new StreamContent(organization.ImageUrl!.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(organization.ImageUrl.ContentType);
            
            formData.Add(organizationFileContent, "ImageUrl", organization.ImageUrl.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Organization.InsertOrganization, Constants.UploadType.Post, formData);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> UpdateOrganization(UpdateOrganizationDto organization)
    {
        var formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(organization.Id.ToString()), "Id");
        formData.Add(new StringContent(organization.Name), "Name");
        formData.Add(new StringContent(organization.Address), "Address");
        formData.Add(new StringContent(organization.Description ?? ""), "Description");

        if (organization.ImageUrl != null)
        {
            var organizationFileContent = new StreamContent(organization.ImageUrl!.OpenReadStream(long.MaxValue));
            
            organizationFileContent.Headers.ContentType = new MediaTypeHeaderValue(organization.ImageUrl.ContentType);
            
            formData.Add(organizationFileContent, "ImageUrl", organization.ImageUrl.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Organization.UpdateOrganization, Constants.UploadType.Put, formData);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> ActivateDeactivateOrganization(Guid organizationId)
    {
        var pathParameter = new List<string>
        {
            organizationId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Organization.ActivateDeactivateOrganization, Constants.DeleteType.Patch, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteOrganization(Guid organizationId)
    {
        var pathParameter = new List<string>
        {
            organizationId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Organization.DeleteOrganization, Constants.DeleteType.Delete, pathParameter);

        return response;
    }
}