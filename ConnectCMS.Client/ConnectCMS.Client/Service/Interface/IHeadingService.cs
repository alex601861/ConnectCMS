using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Requests.Heading;
using CMSTrain.Client.Models.Responses.Heading;
using CMSTrain.Client.Models.Constants;

namespace CMSTrain.Client.Service.Interface;

public interface IHeadingService : ITransientService
{
    Task<CollectionDto<GetHeadingDto>?> GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, int pageNumber, int pageSize, bool? isActive = null, string? search = null);

    Task<ResponseDto<List<GetHeadingDto>?>?> GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, bool? isActive = null,string? search = null);

    Task<ResponseDto<List<GetHeadingModuleDto>?>?> GetAllParentHeadings(FacetType facetType, InspectionType inspectionType);

    Task<ResponseDto<List<GetHeadingModuleDto>?>?> GetAllSubHeadings();

    Task<ResponseDto<GetHeadingDto?>?> GetHeadingById(Guid headingId);

    Task<ResponseDto<GetHeadingCountDto?>?> GetAllHeadingCount(FacetType facetType, InspectionType inspectionType);

    Task<ResponseDto<bool?>?> InsertHeading(CreateHeadingDto heading);

    Task<ResponseDto<bool?>?> UpdateHeading(UpdateHeadingDto heading);

    Task<ResponseDto<bool?>?> ActivateDeactivateHeading(Guid headingId);

    Task<ResponseDto<bool?>?> DeleteHeading(Guid headingId);
}