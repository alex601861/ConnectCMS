using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CMSTrain.Application.Common.User;
using CMSTrain.Application.DTOs.EmailConfirmation;
using CMSTrain.Application.DTOs.Identity;
using CMSTrain.Application.Interfaces.Services.Identity;
using CMSTrain.Application.Settings;
using CMSTrain.Domain.Common;
using CMSTrain.Domain.Entities.Identity;
using CMSTrain.Helper;
using CMSTrain.Identity.Implementation.Manager;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CMSTrain.Application.Interfaces.Services;
using CMSTrain.Application.Exceptions;
using CMSTrain.Application.Interfaces.Repositories.Base;
using CMSTrain.Domain.Entities;

namespace CMSTrain.Identity.Implementation.Services;

public class AuthenticationService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<JwtSettings> jwtSettings,
    ICurrentUserService userService,
    TokenManager tokenManager,
    IGenericRepository genericRepository,
    IFileService fileService) : IAuthenticationService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    private const string UsersImagesFilePath = Constants.FilePath.UsersImagesFilePath;

    private static bool IsValidProvider(string provider) => provider is Constants.Provider.Api or Constants.Provider.Wasm;

    public async Task<UserLoginResponseDto> Login(LoginDto login)
    {
        if (!IsValidProvider(login.Provider))
            throw new NotFoundException("Please provide a valid identifier before logging in to our system.");

        var user = await userManager.FindByEmailAsync(login.Email) ??
                         throw new NotFoundException("The following user has not been registered to our system.");

        if (!user.IsActive)
            throw new BadRequestException("You can not log in to the system.",
                ["The following user is not active, please contact the administrator"]);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, login.Password);

        if (!isPasswordValid)
            throw new NotFoundException("Invalid password, please try again.");

        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var issuer = _jwtSettings.Issuer;

        var audience = _jwtSettings.Audience;

        var accessTokenExpirationInMinutes = Convert.ToInt32(_jwtSettings.AccessTokenExpirationInMinutes);

        // var refreshTokenExpirationInDays = Convert.ToInt32(_jwtSettings.RefreshTokenExpirationInDays);

        var userRoles = await userManager.GetRolesAsync(user);

        var roleName = userRoles.FirstOrDefault();

        var role = await roleManager.FindByNameAsync(roleName!);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Role, role!.Name!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var symmetricSigningKey = new SymmetricSecurityKey(key);

        var signingCredentials = new SigningCredentials(symmetricSigningKey, SecurityAlgorithms.HmacSha256);

        var expirationTime = ExtensionMethod.GetUtcDate().AddMinutes(accessTokenExpirationInMinutes);

        var accessToken = new JwtSecurityToken(
            issuer,
            audience,
            claims: authClaims,
            signingCredentials: signingCredentials,
            expires: expirationTime
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(accessToken);

        var token = login.Provider == Constants.Provider.Api
            ? jwt
            : StringCipher.Encrypt(jwt, Constants.Encryption.Key);

        var organization = user.OrganizationId != null
            ? genericRepository.GetById<Organization>(user.OrganizationId) ?? 
              throw new NotFoundException("The following organization has not been registered to our system.")
            : null;
        
        var userDetails = new UserDetail()
        {
            Id = user.Id,
            Email = user.Email ?? "",
            Name = user.Name,
            RoleId = role.Id,
            RoleName = role.Name ?? "",
            ImageUrl = user.ImageURL,
            PhoneNumber = user.PhoneNumber ?? "",
            Gender = user.Gender.ToString(),
            Address = user.Address,
            DesignationId = user.DesignationId,
            Designation = user.DesignationId != null ? genericRepository.GetById<Designation>(user.DesignationId)?.Title ?? null : null,
            IsActive = user.IsActive,
            CountryId = user.CountryId,
            Country = genericRepository.GetById<Country>(user.CountryId)?.Name 
                      ?? throw new NotFoundException("The following country has not been registered to our system."),
            OrganizationId = organization?.Id ?? null,
            Organization = organization?.Name ?? null,
        };

        var result = new UserLoginResponseDto()
        {
            Token = token,
            UserDetails = userDetails
        };

        return result;
    }

    public async Task<RegistrationResponseDto> SelfCandidateRegister(CandidateRegisterDto candidate)
    {
        var existingUser = await userManager.FindByEmailAsync(candidate.Email);

        if (existingUser != null)
            throw new NotFoundException(
                "An existing user with the following email address already exists in our system, please try again with a new email address.");
        
        var userImageUrl = candidate.ImageUrl != null
            ? fileService.UploadDocument(candidate.ImageUrl, UsersImagesFilePath)
            : null;

        var country = candidate.CountryId == null || candidate.CountryId == Guid.Empty
            ? genericRepository.GetFirstOrDefault<Country>(x => x.Name == "Nepal")
              ?? throw new NotFoundException("The respective country was not found.")
            : genericRepository.GetById<Country>(candidate.CountryId)
              ?? throw new NotFoundException("The respective country was not found.");

        if (candidate.Password != candidate.ConfirmPassword)
        {
            var exception = new[]
            {
                "The password do not match with confirm password.",
            };

            throw new BadRequestException("The following client admin could not be added", exception);
        }
        
        var user = new User
        {
            Name = candidate.Name,
            UserName = candidate.Email,
            NormalizedUserName = candidate.Email.ToUpper(),
            Email = candidate.Email,
            EmailConfirmed = false,
            NormalizedEmail = candidate.Email.ToUpper(),
            PhoneNumber = candidate.PhoneNumber,
            Gender = candidate.Gender,
            CountryId = country.Id,
            OrganizationId = null,
            ImageURL = userImageUrl,
            IsActive = false,
            PhoneNumberConfirmed = true,
            RegisteredDate = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, candidate.Password);

        if (result.Succeeded)
        {
            var candidateRole = await roleManager.FindByNameAsync(Constants.Roles.Candidate);

            if (candidateRole != null)
            {
                await userManager.AddToRoleAsync(user, candidateRole.Name!);
            }
        }
        else
        {
            var errors = result.Errors.Select(x => $"{x.Description}");

            throw new BadRequestException("The following user couldn't be created.", errors.ToArray());
        }

        var candidateUser = await userManager.FindByEmailAsync(user.Email);

        var response = new RegistrationResponseDto()
        {
            UserId = candidateUser?.Id.ToString(),
        };

        return response;
    }

    public async Task<RegistrationResponseDto> ClientCandidateRegister(CandidateRegisterDto candidate)
    {
        var userId = userService.GetUserId;

        var clientUser = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("The following client organization admin could not be found.");

        var existingUser = await userManager.FindByEmailAsync(candidate.Email);

        if (existingUser != null)
            throw new NotFoundException(
                "An existing user with the following email address already exists in our system, please try again with a new email address.");
        
        var userImageUrl = candidate.ImageUrl != null
            ? fileService.UploadDocument(candidate.ImageUrl, UsersImagesFilePath)
            : null;

        var country = candidate.CountryId == null || candidate.CountryId == Guid.Empty
                        ? genericRepository.GetFirstOrDefault<Country>(x => x.Name == "Nepal")
                          ?? throw new NotFoundException("The respective country was not found.")
                        : genericRepository.GetById<Country>(candidate.CountryId)
                          ?? throw new NotFoundException("The respective country was not found.");

        var designation = candidate.DesignationId == null || candidate.DesignationId == Guid.Empty
            ? null
            : genericRepository.GetById<Designation>(candidate.DesignationId)
              ?? throw new NotFoundException("The respective designation was not found.");
        
        if (candidate.Password != candidate.ConfirmPassword)
        {
            var exception = new[]
            {
                "The password do not match with confirm password.",
            };

            throw new BadRequestException("The following client admin could not be added", exception);
        }
        
        var user = new User
        {
            Name = candidate.Name,
            UserName = candidate.Email,
            NormalizedUserName = candidate.Email.ToUpper(),
            Email = candidate.Email,
            Gender = candidate.Gender,
            EmailConfirmed = true,
            IsActive = true,
            NormalizedEmail = candidate.Email.ToUpper(),
            PhoneNumber = candidate.PhoneNumber,
            CountryId = country.Id,
            OrganizationId = clientUser.OrganizationId,
            ImageURL = userImageUrl,
            PhoneNumberConfirmed = true,
            RegisteredDate = DateTime.UtcNow,
            Address = candidate.Address,
            DesignationId = designation?.Id
        };

        var result = await userManager.CreateAsync(user, candidate.Password);

        if (result.Succeeded)
        {
            var candidateRole = await roleManager.FindByNameAsync(Constants.Roles.Candidate);

            if (candidateRole != null)
            {
                await userManager.AddToRoleAsync(user, candidateRole.Name!);
            }
        }
        else
        {
            var errors = result.Errors.Select(x => $"{x.Description}");

            throw new BadRequestException("The following user couldn't be created.", errors.ToArray());
        }

        var candidateUser = await userManager.FindByEmailAsync(user.Email);

        var response = new RegistrationResponseDto()
        {
            UserId = candidateUser?.Id.ToString(),
        };

        return response;
    }

    public async Task<RegistrationResponseDto> UserRegister(UserRegisterDto user)
    {
        var existingUser = await userManager.FindByEmailAsync(user.Email);

        if (existingUser != null)
            throw new NotFoundException(
                "An existing user with the following email address already exists in our system, please try again with a new email address.");

        var userImageUrl = user.ImageUrl != null
            ? fileService.UploadDocument(user.ImageUrl, UsersImagesFilePath)
            : null;

        var country = user.CountryId == null || user.CountryId == Guid.Empty
            ? genericRepository.GetFirstOrDefault<Country>(x => x.Name == "Nepal")
              ?? throw new NotFoundException("The respective country was not found.")
            : genericRepository.GetById<Country>(user.CountryId)
              ?? throw new NotFoundException("The respective country was not found.");

        var designation = user.DesignationId == null || user.DesignationId == Guid.Empty
            ? null
            : genericRepository.GetById<Designation>(user.DesignationId)
              ?? throw new NotFoundException("The respective designation was not found.");

        if (user.Password != user.ConfirmPassword)
        {
            var exception = new[]
            {
                "The password do not match with confirm password.",
            };

            throw new BadRequestException("The following client admin could not be added", exception);
        }
        
        var userModel = new User
        {
            Name = user.Name,
            UserName = user.Email,
            NormalizedUserName = user.Email.ToUpper(),
            Email = user.Email,
            Gender = user.Gender,
            EmailConfirmed = true,
            NormalizedEmail = user.Email.ToUpper(),
            PhoneNumber = user.PhoneNumber,
            CountryId = country.Id,
            ImageURL = userImageUrl,
            IsActive = true,
            PhoneNumberConfirmed = true,
            RegisteredDate = DateTime.UtcNow,
            OrganizationId = user.OrganizationId == Guid.Empty ? null : user.OrganizationId,
            Address = user.Address,
            DesignationId = designation?.Id
        };

        var result = await userManager.CreateAsync(userModel, user.Password);

        if (result.Succeeded)
        {
            var role = await roleManager.FindByIdAsync(user.RoleId.ToString());

            if (role != null)
            {
                await userManager.AddToRoleAsync(userModel, role.Name!);
            }
        }
        else
        {
            var errors = result.Errors.Select(x => $"{x.Description}");

            throw new BadRequestException("The following user couldn't be created.", errors.ToArray());
        }

        var candidateUser = await userManager.FindByEmailAsync(user.Email);

        var response = new RegistrationResponseDto
        {
            UserId = candidateUser?.Id.ToString(),
        };

        return response;
    }

    public async Task VerifyEmailConfirmation(EmailVerificationRequestDto emailVerification)
    {
        var user = await userManager.FindByIdAsync(emailVerification.UserId) ??
                   throw new NotFoundException("The user was not found.");

        var result = await userManager.ConfirmEmailAsync(user, emailVerification.VerificationCode);

        if (!result.Succeeded)
        {
            throw new BadRequestException("The following user could not be verified.",
                ["Invalid user identifier or verification code."]);
        }

        user.IsActive = true;
        user.EmailConfirmed = true;

        await userManager.UpdateAsync(user);
    }

    public async Task ResetPassword(ForgotPasswordEmailRequestDto verifyPasswordDto)
    {
        var user = await userManager.FindByIdAsync(verifyPasswordDto.UserId)
                   ?? throw new NotFoundException("The following user has not been registered to our system.");

        var result = await userManager.ResetPasswordAsync(user, verifyPasswordDto.Token, verifyPasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => $"{x.Description}");

            throw new BadRequestException("Your password couldn't be changed.", errors.ToArray());
        }
    }

    public async Task<ResetPasswordRequestDto> ResetUserPassword(ResetUserPasswordDto resetUserPassword)
    {
        var user = genericRepository.GetById<User>(resetUserPassword.UserId)
                   ?? throw new NotFoundException("The following user has not been registered to our system.");

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        var newPassword = GenerateRandomPassword();

        var resetResult = await userManager.ResetPasswordAsync(user, resetToken, newPassword);

        if (!resetResult.Succeeded)
        {
            var errors = resetResult.Errors.Select(x => $"{x.Description}");

            throw new BadRequestException("Your password couldn't be changed.", errors.ToArray());
        }

        var result = new ResetPasswordRequestDto()
        {
            UserId = user.Id,
            Password = newPassword,
        };

        return result;
    }

    private static string GenerateRandomPassword(int length = 12)
    {
        if (length < 2)
            throw new ArgumentException(
                "Password length must be at least 2 to include both alphanumeric and non-alphanumeric characters.");

        const string numericChars = "0123456789";
        const string nonAlphanumericChars = "!@#$%&";
        const string alphanumericChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        var random = new Random();
        var remainingChars = new char[length - 2];
        var numericChar = numericChars[random.Next(nonAlphanumericChars.Length)];
        var nonAlphaChar = nonAlphanumericChars[random.Next(nonAlphanumericChars.Length)];

        for (var i = 0; i < remainingChars.Length; i++)
        {
            remainingChars[i] = alphanumericChars[random.Next(alphanumericChars.Length)];
        }

        var combinedPassword = nonAlphaChar.ToString() + numericChar + new string(remainingChars);

        return new string(combinedPassword.OrderBy(_ => random.Next()).ToArray());
    }

    public void ExpireToken(string token)
    {
        tokenManager.BlackList.Add(token);
    }

    public bool IsTokenExpired(string token)
    {
        return tokenManager.BlackList.Contains(token);
    }
}