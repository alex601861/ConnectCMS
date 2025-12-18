using CMSTrain.Helper;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using CMSTrain.Application.DTOs.User;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Identity;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Application.Interfaces.Services.Identity;
using ResetUserPasswordDto = CMSTrain.Application.DTOs.User.ResetUserPasswordDto;

namespace CMSTrain.Identity.Implementation.Services;

public class UserService(IGenericRepository genericRepository, 
    IFileService fileService, 
    ICurrentUserService userService) : IUserService
{
    private const string UsersImagesFilePath = Constants.FilePath.UsersImagesFilePath;

    public UserDetail GetUserProfileById(Guid userId)
    {
        var user = genericRepository.GetById<User>(userId)
                   ?? throw new NotFoundException("The user was not found.");

        var userRole = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)
            ?? throw new NotFoundException("The following user has not been assigned to any role.");

        var role = genericRepository.GetById<Role>(userRole.RoleId)
             ?? throw new NotFoundException("The following role could not be found.");

        var organization = user.OrganizationId != null
            ? genericRepository.GetById<Organization>(user.OrganizationId) ?? 
              throw new NotFoundException("The following organization has not been registered to our system.")
            : null;
        
        var result = new UserDetail()
        {
            Id = user.Id,
            Name = user.Name,
            RoleId = role.Id,
            RoleName = role.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ImageUrl = user.ImageURL,
            Gender = user.Gender.ToString(),
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CountryId = user.CountryId,
            Country = genericRepository.GetById<Country>(user.CountryId)?.Name 
                      ?? throw new NotFoundException("The following country has not been registered to our system."),
            OrganizationId = organization?.Id ?? null,
            Organization = organization?.Name ?? null,
            Address = user.Address,
            DesignationId = user.DesignationId,
            Designation = user.DesignationId != null ? genericRepository.GetById<Designation>(user.DesignationId)?.Title ?? null : null,
            IsActive = user.IsActive
        };

        return result;
    }

    public List<UserResponseDto> GetUsersByRole(int pageNumber, int pageSize, out int rowCount, bool? isActive = null, string? search = null, Guid? roleId = default)
    {
        var userRoles = genericRepository.Get<UserRoles>(x => roleId == Guid.Empty || roleId == null || x.RoleId == roleId);

        var userIds = userRoles.Select(ur => ur.UserId).ToList();

        var users = genericRepository.GetPagedResult<User>(pageNumber, pageSize, out rowCount, x => 
            userIds.Contains(x.Id) && (isActive == null || x.IsActive == isActive) && (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList();

        var userResponse = users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Gender = user.Gender.ToString(),
            ImageUrl = user.ImageURL,
            PhoneNumber = user.PhoneNumber ?? "",
            RoleId = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)!.RoleId,
            Role = genericRepository.GetById<Role>(genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)!.RoleId)!.Name!,
            CountryId = user.CountryId,
            Country = genericRepository.GetById<Country>(user.CountryId)!.Name,
            OrganizationId = user.OrganizationId,
            Organization = user.OrganizationId != null ? genericRepository.GetById<Organization>(user.OrganizationId)!.Name : null,
            DesignationId = user.DesignationId,
            Designation = user.DesignationId != null ? genericRepository.GetById<Designation>(user.DesignationId)!.Title : null,
            IsActive = user.IsActive,
        }).ToList();

        return userResponse;
    }

    public List<UserResponseDto> GetUsersByRole(bool? isActive = null, string? search = null, Guid? roleId = default)
    {
        var userRoles = genericRepository.Get<UserRoles>(x => roleId == Guid.Empty || roleId == null || x.RoleId == roleId).ToList();

        var userIds = userRoles.Select(ur => ur.UserId).ToList();

        var users = genericRepository.Get<User>(x => userIds.Contains(x.Id) && 
                                                     (isActive == null || x.IsActive == isActive) && 
                                                     (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))).ToList();

        var userResponse = users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            ImageUrl = user.ImageURL,
            Gender = user.Gender.ToString(),
            PhoneNumber = user.PhoneNumber ?? "",
            CountryId = user.CountryId,
            Country = genericRepository.GetById<Country>(user.CountryId)?.Name ?? throw new NotFoundException("The respective country was not found."),
            OrganizationId = user.OrganizationId,
            Organization = user.OrganizationId != null ? genericRepository.GetById<Organization>(user.OrganizationId)?.Name ?? throw new NotFoundException("The respective organization was not found.") : null,
            IsActive = user.IsActive,
            RoleId = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)?.RoleId ?? throw new NotFoundException("The respective role was not found."),
            Role = genericRepository.GetById<Role>(genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)?.RoleId ?? throw new NotFoundException("The respective role was not found."))?.Name ?? throw new NotFoundException("The respective role was not found.")
        }).ToList();

        return userResponse;
    }

    public List<UserResponseDto> GetUsersForClientOrganization(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null)
    {
        var clientUserId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientUserId);

        rowCount = 0;

        if (clientUser!.OrganizationId == null) 
            throw new NotFoundException("The following user is not associated to any client organization.");

        var users = genericRepository.GetPagedResult<User>(pageNumber, pageSize, out rowCount, 
            x => x.OrganizationId == clientUser.OrganizationId
            && (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))
            && (isActive == null || x.IsActive == isActive)).ToList();

        var userResponse = users.OrderBy(x => x.RegisteredDate).Select(user => new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? "",
            UserName = user.UserName ?? "",
            ImageUrl = user.ImageURL,
            PhoneNumber = user.PhoneNumber ?? "",
            Gender = user.Gender.ToString(),
            RoleId = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)!.RoleId,
            Role = genericRepository.GetById<Role>(genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)!.RoleId)!.Name!,
            CountryId = user.CountryId,
            Country = genericRepository.GetById<Country>(user.CountryId)!.Name,
            OrganizationId = user.OrganizationId,
            Organization = user.OrganizationId != null ? genericRepository.GetById<Organization>(user.OrganizationId)!.Name : null,
            IsActive = user.IsActive,
        }).ToList();

        return userResponse;
    }

    public List<UserResponseDto> GetUsersForClientOrganization(string? search = null, bool? isActive = null)
    {
        var clientUserId = userService.GetUserId;

        var clientUser = genericRepository.GetById<User>(clientUserId)
                         ?? throw new NotFoundException(
                             "The following user has not been registered to our system.");

        if (clientUser.OrganizationId == null) 
            throw new NotFoundException("The following user is not associated to any client organization.");

        var users = genericRepository.Get<User>(x => x.OrganizationId == clientUser.OrganizationId
                                                     && (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search.ToLower()))
                                                     && (isActive == null || x.IsActive == isActive)).ToList();

        var userResponse = users.Select(user => new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            ImageUrl = user.ImageURL,
            Gender = user.Gender.ToString(),
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            CountryId = user.CountryId,
            Country = genericRepository.GetById<Country>(user.CountryId)!.Name,
            OrganizationId = user.OrganizationId,
            Organization = user.OrganizationId != null ? genericRepository.GetById<Organization>(user.OrganizationId)!.Name : null,
            IsActive = user.IsActive,
            RoleId = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)!.RoleId,
            Role = genericRepository.GetById<Role>(genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id)!.RoleId)!.Name!
        }).ToList();

        return userResponse;
    }

    public string ResetUserPassword(ResetUserPasswordDto resetPassword)
    {
        var user = genericRepository.GetById<User>(resetPassword.UserId)
                        ?? throw new NotFoundException("The respective user has not been registered to our system.");
        
        var password = ExtensionMethod.GeneratePassword();
        
        user.PasswordHash = password.HashPassword();

        return password;
    }
    
    public void UpdateUserDetails(UpdateUserRequestDto user)
    {
        var userModel = genericRepository.GetById<User>(user.Id)
                        ?? throw new NotFoundException("The user was not found.");
        
        userModel.Name = user.Name;
        userModel.PhoneNumber = user.PhoneNumber;
        userModel.CountryId = user.CountryId;
        userModel.Gender = user.Gender;
        userModel.Address = user.Address;
        userModel.DesignationId = user.DesignationId;
        userModel.OrganizationId = user.OrganizationId;
        
        if (user.Image != null)
        {
            if (!string.IsNullOrEmpty(userModel.ImageURL))
            {
                var userImagePath = Path.Combine(UsersImagesFilePath, userModel.ImageURL);

                fileService.DeleteFile(userImagePath);
            }
            
            userModel.ImageURL = fileService.UploadDocument(user.Image, UsersImagesFilePath);
        }
        
        if (userModel.Email != user.EmailAddress)
        {
            var existingEmailUser = genericRepository.GetFirstOrDefault<User>(x => x.Email == user.EmailAddress);
            
            if (existingEmailUser != null) 
                throw new BadRequestException("The following email already exists.", 
                    ["The email is already registered to another user set up in our system."]);
            
            userModel.Email = user.EmailAddress;
            userModel.UserName = user.EmailAddress;
            userModel.NormalizedEmail = user.EmailAddress.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
            userModel.NormalizedUserName = user.EmailAddress.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
        }
        
        genericRepository.Update(userModel);
    }

    public void ActivateDeactivateUsers(Guid userId)
    {
        var userModel = genericRepository.GetById<User>(userId)
            ?? throw new NotFoundException("The user was not found.");

        userModel.IsActive = !userModel.IsActive;
        userModel.EmailConfirmed = !userModel.EmailConfirmed;

        genericRepository.Update(userModel);
    }

    public void DeleteUser(Guid userId)
    {
        var userModel = genericRepository.GetById<User>(userId)
                        ?? throw new NotFoundException("The user was not found.");
        
        genericRepository.Delete(userModel);
    }
}
