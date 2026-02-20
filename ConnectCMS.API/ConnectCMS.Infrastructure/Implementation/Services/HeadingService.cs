using CMSTrain.Domain.Entities;
using CMSTrain.Domain.Common.Enum;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Heading;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class HeadingService(IGenericRepository genericRepository) : IHeadingService
{
    public List<GetHeadingDto> GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, int pageNumber, int pageSize, out int rowCount, bool? isActive = null, string? search = null)
    {
        var headings = genericRepository
            .GetPagedResult<Heading>(pageNumber, pageSize, out rowCount, x => 
                x.Type == headingType && 
                x.Facet == facetType && 
                x.Inspection == inspectionType && 
                (isActive == null || x.IsActive == isActive) &&
                (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower()))).ToList();

        return (from heading in headings 
            let subHeadings = genericRepository.Get<Heading>(x => x.ParentHeadingId == heading.Id).ToList() 
            select new GetHeadingDto()
            {
                Id = heading.Id,
                Title = heading.Title,
                Description = heading.Description,
                IsActive = heading.IsActive,
                Type = heading.Type.ToString(),
                ParentHeadingId = null,
                SubHeadings = subHeadings.Select(x => 
                    new GetHeadingModuleDto
                    {
                        Id = x.Id, 
                        Title = x.Title, 
                        Description = x.Description, 
                        IsActive = x.IsActive,
                        Type = x.Type.ToString(),
                        ParentHeadingId = x.ParentHeadingId
                    }).ToList()
            }).ToList();
    }

    public List<GetHeadingDto> GetAllHeadings(HeadingType headingType, FacetType facetType, InspectionType inspectionType, bool? isActive = null, string? search = null)
    {
        var headings = genericRepository
            .Get<Heading>(x => 
                x.Type == headingType && 
                x.Facet == facetType && 
                x.Inspection == inspectionType && 
                (isActive == null || x.IsActive == isActive) &&
                (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower()))).ToList();

        return (from heading in headings 
            let subHeadings = genericRepository.Get<Heading>(x => x.ParentHeadingId == heading.Id).ToList() 
            select new GetHeadingDto()
            {
                Id = heading.Id,
                Title = heading.Title,
                Description = heading.Description,
                IsActive = heading.IsActive,
                Type = heading.Type.ToString(),
                ParentHeadingId = null,
                SubHeadings = subHeadings.Select(x => 
                    new GetHeadingModuleDto
                    {
                        Id = x.Id, 
                        Title = x.Title, 
                        Description = x.Description, 
                        IsActive = x.IsActive,
                        Type = x.Type.ToString(),
                        ParentHeadingId = x.ParentHeadingId
                    }).ToList()
            }).ToList();
    }

    public List<GetHeadingModuleDto> GetAllParentHeadings(FacetType facetType, InspectionType inspectionType)
    {
        var headings = genericRepository
            .Get<Heading>(x => x.ParentHeadingId == null && x.Facet == facetType && x.Inspection == inspectionType).ToList();

        return headings.Select(x => new GetHeadingModuleDto()
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            IsActive = x.IsActive,
            Type = x.Type.ToString(),
            ParentHeadingId = null
        }).ToList();
    }

    public List<GetHeadingModuleDto> GetAllSubHeadings()
    {
        var subHeadings = genericRepository
            .Get<Heading>(x => x.ParentHeadingId != null).ToList();

        return subHeadings.Select(x => new GetHeadingModuleDto()
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            IsActive = x.IsActive,
            Type = x.Type.ToString(),
            ParentHeadingId = x.ParentHeadingId
        }).ToList();
    }
    
    public GetHeadingDto GetHeadingById(Guid headingId)
    {
        var heading = genericRepository.GetById<Heading>(headingId)
            ?? throw new NotFoundException("The following heading could not be found.");
        
        var subHeadings = genericRepository.Get<Heading>(x => x.ParentHeadingId == heading.Id).ToList();
        
        return new GetHeadingDto()
        {
            Id = heading.Id,
            Title = heading.Title,
            Description = heading.Description,
            IsActive = heading.IsActive,
            Type = heading.Type.ToString(),
            ParentHeadingId = null,
            SubHeadings = subHeadings.Select(x => 
                new GetHeadingModuleDto
                {
                    Id = x.Id, 
                    Title = x.Title, 
                    Description = x.Description, 
                    IsActive = x.IsActive,
                    Type = x.Type.ToString(),
                    ParentHeadingId = x.ParentHeadingId
                }).ToList()
        };
    }

    public GetHeadingCountDto GetHeadingCount(FacetType facetType, InspectionType inspectionType)
    {
        var heading = genericRepository.GetCount<Heading>(x => x.Type == HeadingType.Heading && x.Facet == facetType && x.Inspection == inspectionType);

        var subHeading = genericRepository.GetCount<Heading>(x => x.Type == HeadingType.Subheading && x.Facet == facetType && x.Inspection == inspectionType);
        
        return new GetHeadingCountDto
        {
            HeadingCount = heading,
            SubHeadingCount = subHeading,
        };
    }
    
    public void InsertHeading(CreateHeadingDto heading)
    {
        var headingModel = new Heading()
        {
            Title = heading.Title,
            Description = heading.Description,
            Type = heading.Type,
            Facet = heading.Facet,
            ParentHeadingId = heading.ParentHeadingId == Guid.Empty ? null : heading.ParentHeadingId,
            Inspection = heading.Inspection
        };

        genericRepository.Insert(headingModel);
    }

    public void UpdateHeading(UpdateHeadingDto heading)
    {
        var headingModel = genericRepository.GetById<Heading>(heading.Id)
            ?? throw new NotFoundException("The following heading could not be found.");

        headingModel.Title = heading.Title;
        headingModel.Description = heading.Description;

        genericRepository.Update(headingModel);
    }

    public void ActivateDeactivateHeading(Guid headingId)
    {
        var heading = genericRepository.GetById<Heading>(headingId)
            ?? throw new NotFoundException("The following heading could not be found.");

        heading.IsActive = !heading.IsActive;
        
        genericRepository.Update(heading);
    }

    public void DeleteHeading(Guid headingId)
    {
        var heading = genericRepository.GetById<Heading>(headingId)
            ?? throw new NotFoundException("The following heading could not be found.");

        genericRepository.Delete(heading);
    }
}