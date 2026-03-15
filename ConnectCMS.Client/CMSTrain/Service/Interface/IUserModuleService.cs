using CMSTrain.Client.Models.Base;
using CMSTrain.Client.Models.Requests.User;
using CMSTrain.Client.Service.Dependency;
using CMSTrain.Client.Models.Responses.User;
using CMSTrain.Client.Models.Responses.Identity;

namespace CMSTrain.Client.Service.Interface;

public interface IUserModuleService : ITransientService
{
    Task<ResponseDto<UserDetail?>?> GetUserProfileById(Guid userId);

    Task<CollectionDto<UserResponseDto>?> GetUsersByRole(int pageNumber, int pageSize, bool? isActive = null, string? search = null, Guid? roleId = null);

    Task<ResponseDto<List<UserResponseDto>?>?> GetUsersByRole(bool? isActive = null, string? search = null, Guid? roleId = null);

    Task<CollectionDto<UserResponseDto>?> GetUsersForClientOrganization(int pageNumber, int pageSize, string? search = null, bool? isActive = null);

    Task<ResponseDto<List<UserResponseDto>?>?> GetUsersForClientOrganization(string? search = null, bool? isActive = null);
    
    Task<ResponseDto<bool?>?> UpdateUserDetails(UpdateUserRequestDto user);

    Task<ResponseDto<bool?>?> ActivateDeactivateUsers(Guid userId);
    
    Task<ResponseDto<bool?>?> DeleteUser(Guid userId);
}