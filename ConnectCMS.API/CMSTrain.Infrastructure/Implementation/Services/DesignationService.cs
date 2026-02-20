using CMSTrain.Application.DTOs.Designation;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Domain.Entities;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class DesignationService(IGenericRepository genericRepository) : IDesignationService
{
    public List<GetDesignationDto> GetAllDesignations(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var designations = genericRepository.GetPagedResult<Designation>(pageNumber, pageSize, out rowCount, 
            x => (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower()))
                 && (isActive == null || x.IsActive == isActive)).ToList();
        
        return designations.Select(x => new GetDesignationDto()
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            IsActive = x.IsActive
        }).ToList();
    }

    public List<GetDesignationDto> GetAllDesignations(string? search = null, bool? isActive = null)
    {
        var designations = genericRepository.Get<Designation>(x => (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search.ToLower()))
                                                            && (isActive == null || x.IsActive == isActive)).ToList();

        return designations.Select(designation => new GetDesignationDto()
        {
            Id = designation.Id,
            Title = designation.Title,
            Description = designation.Description,
            IsActive = designation.IsActive,
        }).ToList();
    }

    public GetDesignationDto GetDesignationById(Guid id)
    {
        var designation = genericRepository.GetById<Designation>(id)
                      ?? throw new NotFoundException("The following designation was not found.");

        var result = new GetDesignationDto()
        {
            Id = designation.Id,
            Title = designation.Title,
            Description = designation.Description,
            IsActive = designation.IsActive
        };

        return result;
    }

    public void InsertDesignation(CreateDesignationDto designation)
    {
        var existingDesignation = genericRepository.GetFirstOrDefault<Designation>(x => x.Title == designation.Title);

        if (existingDesignation != null) 
        {
            throw new NotFoundException("The following designation already exists.");
        }

        var designationModel = new Designation()
        {
            Title = designation.Title,
            Description = designation.Description,
            IsActive = true
        };
        
        genericRepository.Insert(designationModel);
    }

    public void UpdateDesignation(UpdateDesignationDto designation)
    {
        var designationModel = genericRepository.GetById<Designation>(designation.Id)
                          ?? throw new NotFoundException("The following designation with specified Id not found.");

        designationModel.Title = designation.Title;
        designationModel.Description = designation.Description;
        
        genericRepository.Update(designationModel);
    }

    public void ActivateDeactivateDesignation(Guid id)
    {
        var designationModel = genericRepository.GetById<Designation>(id)
                           ?? throw new NotFoundException("The following designation with specified Id not found.");

        designationModel.IsActive = !designationModel.IsActive; 
        
        genericRepository.Update(designationModel);
    }
}