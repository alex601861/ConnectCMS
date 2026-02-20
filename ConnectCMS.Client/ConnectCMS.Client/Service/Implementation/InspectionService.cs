using System.Text.Json;
using System.Net.Http.Headers;
using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Service.Interface;
using CMSTrain.Client.Models.Requests.Inspection;
using CMSTrain.Client.Models.Responses.Inspection;

namespace CMSTrain.Client.Service.Implementation;

public class InspectionService(IBaseService baseService) : IInspectionService
{
    public async Task<ResponseDto<GetInspectionDto?>?> GetInspectionById(Guid inspectionId)
    {
        var pathParameter = new List<string>
        {
            inspectionId.ToString()
        };
        
        var response = await baseService.GetAsync<GetInspectionDto?>(ApiEndpoints.Inspection.GetInspectionById, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetInspectionDto?>?> GetInspectionByType(InspectionType inspectionType)
    {
        var pathParameter = new List<string>
        {
            inspectionType.ToString()
        };
        
        var response = await baseService.GetAsync<GetInspectionDto?>(ApiEndpoints.Inspection.GetInspectionById, pathParameter);

        return response;
    }
    
    public async Task<CollectionDto<GetInspectionDto>?> GetAllInspections(int pageNumber, int pageSize, string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "isActive", isActive.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetInspectionDto>(endpoint: ApiEndpoints.Inspection.GetAllInspections, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetInspectionDto>?>?> GetAllInspections(string? search = null, bool? isActive = null)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "search", search },
            { "isActive", isActive.ToString() }
        };
        
        var response = await baseService.GetAsync<List<GetInspectionDto>?>(ApiEndpoints.Inspection.GetAllInspectionsList, parameters: queryParameter);

        return response;
    }

    public async Task<CollectionDto<GetInspectionDto>?> GetAllAvailableTrainingInspections(int pageNumber, int pageSize)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetInspectionDto>(ApiEndpoints.Inspection.GetAllAvailableTrainingInspections, parameters: queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetInspectionDto>?>?> GetAllAvailableTrainingInspections()
    {
        var response = await baseService.GetAsync<List<GetInspectionDto>?>(ApiEndpoints.Inspection.GetAllAvailableTrainingInspectionsList);

        return response;
    }
    
    public async Task<CollectionDto<GetInspectionDto>?> GetAllAssignedTrainingInspections(Guid trainingId, int pageNumber, int pageSize, string? search)
    {
        var queryParameter = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search }
        };
        
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetPagedAsync<GetInspectionDto>(ApiEndpoints.Inspection.GetAllAssignedTrainingInspections, pathParameter, queryParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetInspectionDto>?>?> GetAllAssignedTrainingInspections(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetInspectionDto>?>(ApiEndpoints.Inspection.GetAllAssignedTrainingInspectionsList, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> InsertInspection(CreateInspectionDto inspection)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(inspection.Name), "inspection.Name");
        formData.Add(new StringContent(inspection.Description), "inspection.Description");
        formData.Add(new StringContent(inspection.InspectionType.ToString()), "inspection.InspectionType");

        var fileContent = new StreamContent(inspection.Image.OpenReadStream(long.MaxValue));

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(inspection.Image.ContentType);

        formData.Add(content: fileContent, name: "inspection.Image", fileName: inspection.Image.Name);

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Inspection.InsertInspection, Constants.UploadType.Post, formData);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateInspection(UpdateInspectionDto inspection)
    {
        var formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(inspection.Id.ToString()), "inspection.Id");
        formData.Add(new StringContent(inspection.Name), "inspection.Name");
        formData.Add(new StringContent(inspection.Description), "inspection.Description");
        formData.Add(new StringContent(inspection.InspectionType.ToString()), "inspection.InspectionType");
        
        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Inspection.UpdateInspection, Constants.UpdateType.Put, formData);
        
        return response;
    }

    public async Task<ResponseDto<bool?>?> ActivateDeactivateInspection(Guid inspectionId)
    {
        var pathParameter = new List<string>
        {
            inspectionId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Inspection.ActivateDeactivateInspection, Constants.DeleteType.Patch, pathParameter);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UploadInspectionQuestionnaires(UploadInspectionQuestionnaireDto inspectionQuestionnaires)
    {
        var jsonRequest = JsonSerializer.Serialize(inspectionQuestionnaires);

        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        
        var response = await baseService.PostAsync<bool?>(ApiEndpoints.Inspection.UploadInspectionQuestionnaires, content);

        return response;
    }
}