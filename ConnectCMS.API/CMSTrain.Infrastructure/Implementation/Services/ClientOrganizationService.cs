using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.Exceptions;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.DTOs.ClientOrganization;
using CMSTrain.Application.Interfaces.Repositories.Base;

namespace CMSTrain.Infrastructure.Implementation.Services;

public class ClientOrganizationService(IGenericRepository genericRepository, IFileService fileService) : IClientOrganizationService
{
    private const string UsersImagesFilePath = Constants.FilePath.UsersImagesFilePath;

    public List<GetClientOrganizationDto> GetAllClientOrganizations(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var result = new List<GetClientOrganizationDto>();

        var organizations = genericRepository.GetPagedResult<Organization>(pageNumber, pageSize, out rowCount, x => 
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower())) &&
            (isActive == null || x.IsActive == isActive)).ToList();

        var administration = genericRepository.GetFirstOrDefault<Role>(x => 
                                 x.Name == Constants.Roles.Client) ?? 
                             throw new NotFoundException("The following client organization administrator role has not been registered to our system.");

        var adminUserRoles = genericRepository.Get<UserRoles>(x => x.RoleId == administration.Id).ToList();

        var adminUserIds = adminUserRoles.Select(x => x.UserId).ToList();
        
        foreach (var organization in organizations)
        {
            var adminUser = genericRepository.GetFirstOrDefault<User>(x =>
                x.OrganizationId == organization.Id && adminUserIds.Contains(x.Id));

            var clientUserCount = genericRepository.GetCount<User>(x => x.OrganizationId == organization.Id);
            
            var organizationDto = new GetClientOrganizationDto()
            {
                Id = organization.Id,
                Name = organization.Name,
                Address = organization.Address,
                Description = organization.Description,
                ImageUrl = organization.ImageUrl,
                IsActive = organization.IsActive,
                UserCount = clientUserCount,
                Admin = adminUser == null ? null : new GetClientAdminDto
                {
                    Id = adminUser.Id,
                    Name = adminUser.Name,
                    EmailAddress = adminUser.Email ?? "",
                    PhoneNumber = adminUser.PhoneNumber ?? "",
                    ImageUrl = adminUser.ImageURL
                }
            };

            result.Add(organizationDto);
        }

        return result;
    }

    public List<GetClientOrganizationDto> GetAllClientOrganizations(string? search = null, bool? isActive = null)
    {
        var result = new List<GetClientOrganizationDto>();

        var organizations = genericRepository.Get<Organization>(x => 
            (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower())) &&
            (isActive == null || x.IsActive == isActive)).ToList();

        var administration = genericRepository.GetFirstOrDefault<Role>(x => 
                                 x.Name == Constants.Roles.Client) ?? 
                             throw new NotFoundException("The following client organization administrator role has not been registered to our system.");

        var adminUserRoles = genericRepository.Get<UserRoles>(x => x.RoleId == administration.Id).ToList();

        var adminUserIds = adminUserRoles.Select(x => x.UserId).ToList();
        
        foreach (var organization in organizations)
        {
            var adminUser = genericRepository.GetFirstOrDefault<User>(x =>
                x.OrganizationId == organization.Id && adminUserIds.Contains(x.Id));
            
            var organizationDto = new GetClientOrganizationDto()
            {
                Id = organization.Id,
                Name = organization.Name,
                Address = organization.Address,
                Description = organization.Description,
                ImageUrl = organization.ImageUrl,
                IsActive = organization.IsActive,
                Admin = adminUser == null ? null : new GetClientAdminDto
                {
                    Id = adminUser.Id,
                    Name = adminUser.Name,
                    EmailAddress = adminUser.Email ?? "",
                    PhoneNumber = adminUser.PhoneNumber ?? "",
                    ImageUrl = adminUser.ImageURL
                }
            };

            result.Add(organizationDto);
        }

        return result;
    }

    public List<GetClientOrganizationDto> GetAllClientOrganizationsWithoutAdmin()
    {
        var clientOrganizations = genericRepository.Get<Organization>(x => x.IsActive).ToList();

        var userOrganizations = genericRepository.Get<User>(x => x.OrganizationId != null && x.IsActive).ToList();

        var organizations = clientOrganizations
            .Where(org => userOrganizations.All(user => user.OrganizationId != org.Id))
            .ToList();

        return organizations.Select(organization => new GetClientOrganizationDto()
        {
            Id = organization.Id,
            Name = organization.Name,
            Address = organization.Address,
            Description = organization.Description,
            ImageUrl = organization.ImageUrl,
            IsActive = organization.IsActive,
            Admin = null
        }).ToList();
    }
    
    public void RegisterClientOrganizationAdmin(RegisterClientAdminDto clientAdmin)
    {
        var organization = genericRepository.GetById<Organization>(clientAdmin.OrganizationId) 
                           ?? throw new NotFoundException("The following organization could not be found.");
        
        var administration = genericRepository.GetFirstOrDefault<Role>(x => 
                                 x.Name == Constants.Roles.Client) ?? 
                             throw new NotFoundException("The following client organization administrator role has not been registered to our system.");
        
        var existingUser = genericRepository.GetFirstOrDefault<User>(x => x.Email == clientAdmin.Email);

        if (existingUser != null)
            throw new NotFoundException("An existing user with the following email address already exists in our system, please try again with a new email address.");

        if (clientAdmin.Password != clientAdmin.ConfirmPassword)
        {
            var exception = new[]
            {
                "The password do not match with confirm password.",
            };

            throw new BadRequestException("The following client admin could not be added", exception);
        }

        var userImageUrl = clientAdmin.ImageUrl != null
            ? fileService.UploadDocument(clientAdmin.ImageUrl, UsersImagesFilePath)
            : null;

        var country = clientAdmin.CountryId == null || clientAdmin.CountryId == Guid.Empty
            ? genericRepository.GetFirstOrDefault<Country>(x => x.Name == "Nepal")
              ?? throw new NotFoundException("The respective country was not found.")
            : genericRepository.GetById<Country>(clientAdmin.CountryId)
              ?? throw new NotFoundException("The respective country was not found.");

        var designation = clientAdmin.DesignationId == null || clientAdmin.DesignationId == Guid.Empty
            ? null
            : genericRepository.GetById<Designation>(clientAdmin.DesignationId)
              ?? throw new NotFoundException("The respective designation was not found.");
        
        var clientOrganizationAdmin = new User
        {
            Name = clientAdmin.Name,
            UserName = clientAdmin.Email,
            NormalizedUserName = clientAdmin.Email.ToUpper(),
            Email = clientAdmin.Email,
            EmailConfirmed = true,
            NormalizedEmail = clientAdmin.Email.ToUpper(),
            PhoneNumber = clientAdmin.PhoneNumber,
            Gender = clientAdmin.Gender,
            CountryId = country.Id,
            OrganizationId = organization.Id,
            ImageURL = userImageUrl,
            IsActive = true,
            DesignationId = designation?.Id,
            Address = clientAdmin.Address,
            PasswordHash = clientAdmin.Password.HashPassword()
        };
        
        var clientOrganizationAdminId = genericRepository.Insert(clientOrganizationAdmin);

        var clientOrganizationAdministrator = new UserRoles()
        {
            UserId = clientOrganizationAdminId,
            RoleId = administration.Id
        };

        genericRepository.Insert(clientOrganizationAdministrator);
    }
}