using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.DTOs.Heading;
using CMSTrain.Application.Common.Service;

namespace CMSTrain.Application.Interfaces.Services;

public interface IHeadingService : ITransientService
{
    List<GetHeadingDto> GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, int pageNumber, int pageSize, out int rowCount, bool? isActive = null, string? search = null);

    List<GetHeadingDto> GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, bool? isActive = null, string? search = null);

    List<GetHeadingModuleDto> GetAllParentHeadings(FacetType facetType, InspectionType inspectionType);

    List<GetHeadingModuleDto> GetAllSubHeadings();

    GetHeadingDto GetHeadingById(Guid headingId);
    
    GetHeadingCountDto GetHeadingCount(FacetType facetType, InspectionType inspectionType);

    void InsertHeading(CreateHeadingDto heading);
    
    void UpdateHeading(UpdateHeadingDto heading);
    
    void ActivateDeactivateHeading(Guid headingId);
    
    void DeleteHeading(Guid headingId);
}