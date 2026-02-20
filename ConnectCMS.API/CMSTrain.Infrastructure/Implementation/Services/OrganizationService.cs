using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.DTOs.Organization;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Domain.Entities.Identity;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class OrganizationService(IGenericRepository genericRepository, IFileService fileService) : IOrganizationService
{
    private const string OrganizationsImagesFilePath = Constants.FilePath.OrganizationsImagesFilePath;

    public List<GetOrganizationDto> GetAllOrganizations(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var organizations = genericRepository.GetPagedResult<Organization>(pageNumber, pageSize, out rowCount,
            x => 
                (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower())) &&
                (isActive == null || x.IsActive == isActive)).ToList();

        var result = organizations.Select(x => new GetOrganizationDto()
        {
            Id = x.Id,
            Name = x.Name,
            Address = x.Address,
            Description = x.Description,
            ImageUrl = x.ImageUrl,
            IsActive = x.IsActive
        }).ToList();

        return result;
    }

    public List<GetOrganizationDto> GetAllOrganizations(string? search = null, bool? isActive = null)
    {
        var organizations = genericRepository.Get<Organization>(x => 
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower())) &&
            (isActive == null || x.IsActive == isActive)).ToList();

        return organizations.Select(organization => new GetOrganizationDto()
            {
                Id = organization.Id,
                Name = organization.Name,
                Address = organization.Address,
                Description = organization.Description,
                ImageUrl = organization.ImageUrl,
                IsActive = organization.IsActive,
            })
            .ToList();
    }

    public GetOrganizationDto GetOrganizationById(Guid id)
    {
        var organization = genericRepository.GetById<Organization>(id)
            ?? throw new NotFoundException("The following organization with specified Id was not found.");

        var result = new GetOrganizationDto()
        {
            Id= organization.Id,
            Name = organization.Name,
            Address = organization.Address,
            Description = organization.Description,
            ImageUrl = organization.ImageUrl,
            IsActive = organization.IsActive
        };

        return result;
    }

    public void InsertOrganization(CreateOrganizationDto organization)
    {
        var existingOrganization = genericRepository.GetFirstOrDefault<Organization>(x => x.Name == organization.Name);

        if (existingOrganization != null) 
        {
            throw new NotFoundException("The following organization with the specified name already exist.");
        }
        
        var organizationImageUrl = organization.ImageUrl != null
            ? fileService.UploadDocument(organization.ImageUrl, OrganizationsImagesFilePath)
            : null;

        var organizationModel = new Organization
        {
            Name = organization.Name,
            Address = organization.Address ?? "",
            Description = organization.Description ?? "",
            ImageUrl = organizationImageUrl ?? "",
        };

        genericRepository.Insert(organizationModel);
    }

    public void UpdateOrganization(UpdateOrganizationDto organization)
    {
        var organizationModel = genericRepository.GetById<Organization>(organization.Id)
            ?? throw new NotFoundException("The following organization with the specified Id was not found.");

        organizationModel.Name = organization.Name;
        organizationModel.Address = organization.Address ?? "";
        organizationModel.Description = organization.Description ?? "";

        if (organization.ImageUrl != null)
        {
            var organizationPath = Path.Combine(OrganizationsImagesFilePath, organizationModel.ImageUrl);

            fileService.DeleteFile(organizationPath);
            
            var organizationImageUrl = fileService.UploadDocument(organization.ImageUrl, OrganizationsImagesFilePath);

            organizationModel.ImageUrl = organizationImageUrl;
        }

        genericRepository.Update(organizationModel);
    }

    public void ActivateDeactivateOrganization(Guid id)
    {
        var organizationModel = genericRepository.GetById<Organization>(id)
            ?? throw new NotFoundException("The following organization with the specified Id was not found.");

        organizationModel.IsActive = !organizationModel.IsActive;

        genericRepository.Update(organizationModel);

        var users = genericRepository.Get<User>(x => x.OrganizationId == organizationModel.Id).ToList();

        foreach (var user in users)
        {
            user.IsActive = organizationModel.IsActive;
            
            genericRepository.Update(user);
        }
    }

    public void DeleteOrganization(Guid organizationId)
    {
        var organization = genericRepository.GetById<Organization>(organizationId)
                             ?? throw new NotFoundException("The organization was not found.");

        var organizationUsers = genericRepository.Get<User>(x => 
            x.OrganizationId == organization.Id).ToList();

        if (organizationUsers.Count != 0)
        {
            throw new BadRequestException("The following organization could not be deleted.", ["The respective organization have users assigned to them."]);
        }
        
        genericRepository.Delete(organization);
    }
}
