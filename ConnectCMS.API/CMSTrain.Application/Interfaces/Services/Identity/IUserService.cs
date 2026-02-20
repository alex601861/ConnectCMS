using CMSTrain.Application.Common.Service;
using CMSTrain.Application.DTOs.Identity;
using CMSTrain.Application.DTOs.User;
using ResetUserPasswordDto = CMSTrain.Application.DTOs.User.ResetUserPasswordDto;

namespace CMSTrain.Application.Interfaces.Services.Identity;

public interface IUserService : ITransientService
{
    UserDetail GetUserProfileById(Guid userId);

    List<UserResponseDto> GetUsersByRole(int pageNumber, int pageSize, out int rowCount, bool? isActive = null, string? search = null, Guid? roleId = default);

    List<UserResponseDto> GetUsersByRole(bool? isActive = null, string? search = null, Guid? roleId = default);

    List<UserResponseDto> GetUsersForClientOrganization(int pageNumber, int pageSize, out int rowCount, string? search = null, bool? isActive = null);

    List<UserResponseDto> GetUsersForClientOrganization(string? search = null, bool? isActive = null);

    string ResetUserPassword(ResetUserPasswordDto resetPassword);
    
    void UpdateUserDetails(UpdateUserRequestDto user);
    
    void ActivateDeactivateUsers(Guid userId);
    
    void DeleteUser(Guid userId);
}
