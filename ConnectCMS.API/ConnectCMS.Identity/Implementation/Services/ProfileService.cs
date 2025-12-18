using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Common.User;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Application.DTOs.Identity;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Application.Interfaces.Services.Identity;

namespace CMSTrain.Identity.Implementation.Services;

public class ProfileService(
    UserManager<User> userManager,
    IGenericRepository genericRepository,
    ICurrentUserService userService,
    IFileService fileService) : IProfileService
{
    private const string UsersImageFilePath = Constants.FilePath.UsersImagesFilePath;

    public UserDetail GetUserProfile()
    {
        var userId = userService.GetUserId;

        var user = genericRepository.GetById<User>(userId) ??
                   throw new NotFoundException("The following user has not been registered to our system.");

        var userRole = genericRepository.GetFirstOrDefault<UserRoles>(x => x.UserId == user.Id) ??
                       throw new NotFoundException(
                           "The following user has not been registered to a valid role to our system.");

        var role = genericRepository.GetById<Role>(userRole.RoleId) ??
                   throw new NotFoundException("The following role has not been registered to our system.");

        var organization = user.OrganizationId != null
            ? genericRepository.GetById<Organization>(user.OrganizationId) ?? 
                throw new NotFoundException("The following organization has not been registered to our system.")
            : null;
        
        var result = new UserDetail()
        {
            Id = user.Id,
            Name = user.Name,
            RoleId = role.Id,
            RoleName = role.Name ?? "",
            Email = user.Email ?? "",
            ImageUrl = user.ImageURL,
            Gender = user.Gender.ToString(),
            PhoneNumber = user.PhoneNumber ?? "",
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

    public RolesDto GetUserRole()
    {
        var userInRole = userService.GetUserRole;

        var role = genericRepository.GetFirstOrDefault<Role>(r => r.Name == userInRole) ??
                   throw new NotFoundException("The following role has not been registered to our system.");

        return new RolesDto
        {
            Id = role.Id,
            Name = role.Name ?? Constants.Roles.Candidate
        };
    }

    public void UpdateUserProfile(ProfileRequestDto profileDetails)
    {
        var userId = userService.GetUserId;

        var userModel = genericRepository.GetById<User>(userId)
                        ?? throw new NotFoundException("The user was not found.");

        if (userModel.Email != profileDetails.Email)
        {
            var existingEmailUser = genericRepository.GetFirstOrDefault<User>(x => x.Email == profileDetails.Email);
            
            if (existingEmailUser != null) 
                throw new BadRequestException("The following email already exists.", 
                    ["The email is already registered to another user set up in our system."]);
            
            userModel.Email = profileDetails.Email;
            userModel.UserName = profileDetails.Email;
            userModel.NormalizedEmail = profileDetails.Email.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
            userModel.NormalizedUserName = profileDetails.Email.ToUpper(System.Globalization.CultureInfo.CurrentCulture);
        }
        
        userModel.Name = profileDetails.Name;
        userModel.PhoneNumber = profileDetails.PhoneNumber;
        userModel.CountryId = profileDetails.CountryId;
        userModel.Gender = profileDetails.Gender;
        userModel.DesignationId = profileDetails.DesignationId;
        userModel.Address = profileDetails.Address;

        genericRepository.Update(userModel);
    }

    public void UpdateProfileImage(ProfileImageRequestDto profileImage)
    {
        var userId = userService.GetUserId;

        var userModel = genericRepository.GetById<User>(userId)
                        ?? throw new NotFoundException("The user was not found.");
        
        if (!string.IsNullOrEmpty(userModel.ImageURL))
        {
            var userImagePath = Path.Combine(UsersImageFilePath, userModel.ImageURL);

            fileService.DeleteFile(userImagePath);
        }
        
        var imageUrl = fileService.UploadDocument(profileImage.ImageUrl, UsersImageFilePath);

        userModel.ImageURL = imageUrl;

        genericRepository.Update(userModel);
    }

    public async Task ChangePassword(ChangePasswordRequestDto changePassword)
    {
        var userId = userService.GetUserId;

        var user = genericRepository.GetById<User>(userId) ?? throw new NotFoundException("The user was not found.");

        if (changePassword.NewPassword != changePassword.ConfirmPassword)
        {
            throw new BadRequestException("Your password could not be changed",
                ["The new password doesn't match with your confirmed password"]);
        }

        var result = await userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);

        if (!result.Succeeded)
        {
            throw new BadRequestException("Your password could not be changed.",
                result.Errors.Select(x => x.Description).ToArray());
        }
    }

    public void DeleteUserProfile()
    {
        var userId = userService.GetUserId;

        var userModel = genericRepository.GetById<User>(userId)
                        ?? throw new NotFoundException("The following training format already exists.");

        if (!userModel.IsActive)
        {
            throw new BadRequestException("The following user could not be deleted",
                ["The user is not at an active state."]);
        }

        userModel.IsActive = !userModel.IsActive;

        genericRepository.Update(userModel);
    }
}