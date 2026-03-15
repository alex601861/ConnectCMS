using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Models.Requests.Class;
using CMSTrain.Client.Models.Responses.Class;
using CMSTrain.Client.Models.Responses.Count;
using CMSTrain.Client.Models.Responses.Country;
using CMSTrain.Client.Service.Base;
using CMSTrain.Client.Service.Interface;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace CMSTrain.Client.Service.Implementation;

public class ClassService(IBaseService baseService) : IClassService
{
    public async Task<CollectionDto<GetClassDto>?> GetAllClasses(Guid trainingId, int pageNumber, int pageSize, string? search = null, int? status = null)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };

        var parameters = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "status", status.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetClassDto>(ApiEndpoints.Class.GetAllClasses, pathParameter, parameters);

        return response;
    }
    
    public async Task<ResponseDto<List<GetClassDto>?>?> GetAllClasses(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetClassDto>?>(ApiEndpoints.Class.GetAllClassesList, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<List<GetClassForTrainersDto>?>?> GetAllClassesForTrainers(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
            
        };
        
        var response = await baseService.GetAsync<List<GetClassForTrainersDto>?>(ApiEndpoints.Class.GetAllClassesForTrainersList, pathParameter);

        return response;
    }

    public async Task<CollectionDto<GetClassForTrainersDto>?> GetAllClassesForTrainers(Guid trainingId, int pageNumber, int pageSize, string? search = null, int? status = null)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
            
        };
        
        var parameters = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "status", status.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetClassForTrainersDto>(endpoint: ApiEndpoints.Class.GetAllClassesForTrainers, pathParameter, parameters);

        return response;
    }

    public async Task<CollectionDto<GetClassForCandidatesDto>?> GetAllClassesForCandidates(Guid trainingId, int pageNumber, int pageSize, string? search = null, int? status = null)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var parameters = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() },
            { "search", search },
            { "status", status.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetClassForCandidatesDto>(ApiEndpoints.Class.GetAllClassesForCandidates, pathParameter, parameters);

        return response;
    }
    
    public async Task<ResponseDto<List<GetClassForCandidatesDto>?>?> GetAllClassesForCandidates(Guid trainingId)
    {
        var pathParameter = new List<string>
        {
            trainingId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetClassForCandidatesDto>?>(ApiEndpoints.Class.GetAllClassesForCandidatesList, pathParameter);

        return response;
    }

    public async Task<ResponseDto<GetClassDto?>?> GetClassById(Guid classId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var response = await baseService.GetAsync<GetClassDto?>(ApiEndpoints.Class.GetClassById, pathParameter);

        return response;
    }

    public async Task<CollectionDto<GetClassForCandidatesDto>?> GetAllCandidateClasses(Guid trainingCandidateId, int pageNumber, int pageSize)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var parameters = new Dictionary<string, string?>()
        {
            { "pageNumber", pageNumber.ToString() },
            { "pageSize", pageSize.ToString() }
        };
        
        var response = await baseService.GetPagedAsync<GetClassForCandidatesDto>(ApiEndpoints.Class.GetAllCandidateClasses, pathParameter, parameters);

        return response;
    }
    
    public async Task<ResponseDto<List<GetClassForCandidatesDto>?>?> GetAllCandidateClasses(Guid trainingCandidateId)
    {
        var pathParameter = new List<string>
        {
            trainingCandidateId.ToString()
        };
        
        var response = await baseService.GetAsync<List<GetClassForCandidatesDto>?>(ApiEndpoints.Class.GetAllCandidateClassesList, pathParameter);

        return response;
    }
    
    public async Task<ResponseDto<bool?>?> InsertClass(CreateClassDto @class)
    {
        // var jsonRequest = JsonSerializer.Serialize(@class);
        //
        // var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        //
        // var response = await baseService.PostAsync<bool?>(ApiEndpoints.Class.InsertClass, content);
        //
        // return response;
        
        var formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(@class.Title), "Title");
        formData.Add(new StringContent(@class.TrainingId.ToString()), "TrainingId");
        formData.Add(new StringContent(@class.Date.ToString()!), "Date");
        formData.Add(new StringContent(@class.StartTime.ToString()!), "StartTime");
        formData.Add(new StringContent(@class.EndTime.ToString()!), "EndTime");

        if (@class.Image != null)
        {
            var classFileContent = new StreamContent(@class.Image!.OpenReadStream(long.MaxValue));
            
            classFileContent.Headers.ContentType = new MediaTypeHeaderValue(@class.Image.ContentType);
            
            formData.Add(classFileContent, "Image", @class.Image.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Class.InsertClass, Constants.UploadType.Post, formData);

        return response;
    }

    public async Task<ResponseDto<bool?>?> UpdateClass(UpdateClassDto @class)
    {
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(@class.Id.ToString()), "Id");
        formData.Add(new StringContent(@class.Title), "Title");
        formData.Add(new StringContent(@class.TrainingId.ToString()), "TrainingId");
        formData.Add(new StringContent(@class.Date.ToString()!), "Date");
        formData.Add(new StringContent(@class.StartTime.ToString()!), "StartTime");
        formData.Add(new StringContent(@class.EndTime.ToString()!), "EndTime");

        if (@class.Image != null)
        {
            var classFileContent = new StreamContent(@class.Image!.OpenReadStream(long.MaxValue));
            
            classFileContent.Headers.ContentType = new MediaTypeHeaderValue(@class.Image.ContentType);
            
            formData.Add(classFileContent, "Image", @class.Image.Name);
        }

        var response = await baseService.UploadAsync<bool?>(ApiEndpoints.Class.UpdateClass, Constants.UploadType.Put, formData);

        return response;
    }

    public async Task<ResponseDto<bool?>?> DeleteClass(Guid classId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };
        
        var response = await baseService.DeleteAsync<bool?>(ApiEndpoints.Class.ActivateDeactivateClass, Constants.DeleteType.Delete, pathParameter);

        return response;
    }

    public async Task<ResponseDto<ClassCountDto?>?> GetClassCount(Guid classId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };

        var response = await baseService.GetAsync<ClassCountDto>(ApiEndpoints.Class.GetClassDetailsCountForAdmin, pathParameter);

        return response;
    }

    public async Task<ResponseDto<ClassCountDto?>?> GetClassCountForCandidate(Guid classId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };

        var response = await baseService.GetAsync<ClassCountDto>(ApiEndpoints.Class.GetClassDetailsCountForCandidate, pathParameter);

        return response;

    }

    public async Task<ResponseDto<ClassCountDto?>?> GetClassDetailsCountForClient(Guid classId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };

        var response = await baseService.GetAsync<ClassCountDto>(ApiEndpoints.Class.GetClassDetailsCountForClient, pathParameter);

        return response;

    }

    public async Task<ResponseDto<ClassCountDto?>?> GetClassDetailsCountForTrainer(Guid classId)
    {
        var pathParameter = new List<string>
        {
            classId.ToString()
        };

        var response = await baseService.GetAsync<ClassCountDto>(ApiEndpoints.Class.GetClassDetailsCountForTrainer, pathParameter);

        return response;

    }
}