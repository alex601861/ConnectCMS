using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Email;
using CMSTrain.Client.Service.Extensions;
using CMSTrain.Client.Models.Requests.File;
using CMSTrain.Client.Models.Requests.User;
using CMSTrain.Client.Models.Responses.User;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Country;
using CMSTrain.Client.Models.Responses.Identity;
using CMSTrain.Client.Models.Responses.ClientOrganization;
using CMSTrain.Client.Models.Responses.Designation;

namespace CMSTrain.Client.Pages.Administrator.User;

public partial class Users : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllUsers();
        await GetAllRoles();
        await GetAllCountries();
        await GetCandidateRole();
        await GetDefaultCountry();
        await GetPrecedingRoles();
        await GetAllDesignations();
        await GetOrganizationRole();
        await GetAllClientOrganizations();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Users;
    }
    #endregion

    #region Module Entity Data
    private GetCountryDto DefaultCountry { get; set; } = new();

    private List<RolesDto> Roles { get; set; } = [];
    
    private List<RolesDto> PrecedingRoles { get; set; } = [];

    private List<GetCountryDto> Countries { get; set; } = [];

    private List<GetDesignationDto> Designations { get; set; } = [];

    private List<GetClientOrganizationDto> AdminOrganizations { get; set; } = new();
    
    private List<GetClientOrganizationDto> CandidateOrganizations { get; set; } = new();
    
    private async Task GetAllRoles()
    {
        var response = await RoleService.GetAllRoles();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        Roles = response.Result;
    }
    
    private async Task GetPrecedingRoles()
    {
        var response = await RoleService.GetPrecedingRoles();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        PrecedingRoles = response.Result;
    }

    private async Task GetAllCountries()
    {
        var response = await CountryService.GetAllCountries(isActive: true);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        Countries = response.Result;
    }
    
    private async Task GetDefaultCountry()
    {
        var response = await CountryService.GetDefaultCountry();

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DefaultCountry = response.Result;
    }
    
    private async Task GetOrganizationsWithoutAdmin()
    {
        var result = await ClientOrganizationService.GetAllClientOrganizationsWithoutAdmin();

        if (result?.Result is null)
        {
            return;
        }

        AdminOrganizations = result.Result;
    }
    
    private async Task GetAllClientOrganizations()
    {
        var result = await ClientOrganizationService.GetAllClientOrganizations(isActive: true);

        if (result?.Result is null)
        {
            return;
        }

        CandidateOrganizations = result.Result;
    }
    
    private async Task GetAllDesignations()
    {
        var result = await DesignationService.GetAllDesignations(isActive: true);

        if (result?.Result is null)
        {
            return;
        }

        Designations = result.Result;
    }
    #endregion

    #region Search and Filter
    private string _search = string.Empty;
    
    private string Search
    {
        get => _search;
        set
        {
            if (_search == value) return;
            _search = value;
            _ = OnSearchInputAsync(_search);
        }
    }
    
    private async Task OnSearchInputAsync(string search)
    {
        Search = search;

        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        await GetAllUsers();
        
        StateHasChanged();
    }
    
    private Guid RoleId { get; set; }

    private async Task OnUserFilter()
    {
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        UserList = null;

        await GetAllUsers();
    }
    
    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive; 
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        UserList = null;

        await GetAllUsers();
    }
    #endregion

    #region User Data
    private CollectionDto<UserResponseDto>? UserList { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        UserList = null;

        await GetAllUsers(); 
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;

        UserList = null;
        
        await GetAllUsers();
    }

    private async Task GetAllUsers()
    {
        try
        {
            var response = await UserModuleService.GetUsersByRole(PageNumber, PageSize, IsActive, Search, ExtensionMethods.ToNullOrValue(RoleId));

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            UserList = response;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Register User
    private bool IsSelfRegisteredCandidate { get; set; } = true;
    
    private bool IsCreateModalOpen { get; set; }
    
    private bool OpenUserImageDrawerToggle { get; set; }

    private UserRegisterDto RegisterDto { get; set; } = new();

    private bool ShowAdminOrganization => RegisterDto.RoleId == OrganizationRole.Id;

    private bool ShowCandidateOrganization => RegisterDto.RoleId == CandidateRole.Id;

    private RolesDto OrganizationRole { get; set; } = new();

    private RolesDto CandidateRole { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateUserButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(RegisterDto.Name) ||
               string.IsNullOrEmpty(RegisterDto.Email) ||
               string.IsNullOrEmpty(RegisterDto.PhoneNumber) || 
               string.IsNullOrEmpty(RegisterDto.Password) ||
               string.IsNullOrEmpty(RegisterDto.ConfirmPassword) ||
               string.IsNullOrEmpty(RegisterDto.Address) ||
               RegisterDto.CountryId == Guid.Empty ||
               RegisterDto.Gender == null ||
               RegisterDto.RoleId == Guid.Empty;
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleUserBusySubmit(bool isBusySubmitting)
    {
        IsCreateUserButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private bool IsCreateButtonDisabled
    {
        get => _isCreateButtonDisabled || 
               string.IsNullOrEmpty(RegisterDto.Name) ||
               string.IsNullOrEmpty(RegisterDto.Email) ||
               string.IsNullOrEmpty(RegisterDto.PhoneNumber) ||
               string.IsNullOrEmpty(RegisterDto.Password) ||
               string.IsNullOrEmpty(RegisterDto.ConfirmPassword) ||
               string.IsNullOrEmpty(RegisterDto.Address) ||
               RegisterDto.CountryId == Guid.Empty ||
               RegisterDto.Gender == null ||
               RegisterDto.RoleId == Guid.Empty;
        set => _isCreateButtonDisabled = value;
    }

    private void OpenCloseCreateUserModal()
    {
        IsCreateModalOpen = !IsCreateModalOpen;
        
        StateHasChanged();
    }
    
    private void HandeCreateBusySubmit(bool isBusySubmitting)
    {
        IsCreateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private async Task GetOrganizationRole()
    {
        var result = await RoleService.GetAllRoles();

        if (result?.Result is null)
        {
            return;
        }

        OrganizationRole = result.Result.First(x => x.Name == Constants.Roles.Client);
    }
    
    private async Task GetCandidateRole()
    {
        var result = await RoleService.GetAllRoles();

        if (result?.Result is null)
        {
            return;
        }

        CandidateRole = result.Result.First(x => x.Name == Constants.Roles.Candidate);
    }
    
    private async Task OpenRegisterUsersModal()
    {
        RegisterDto = new UserRegisterDto()
        {
            CountryId = DefaultCountry.Id
        };

        OpenCloseCreateUserModal();

        IsSelfRegisteredCandidate = true;
        
        await GetAllClientOrganizations();
        
        await GetOrganizationsWithoutAdmin();
        
        StateHasChanged();
    }
    
    private void ToggleUserRegistrationImage()
    {
        OpenUserImageDrawerToggle = !OpenUserImageDrawerToggle;
    }

    private async Task RegisterNewUser(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseCreateUserModal();
            return;
        }

        try
        {
            HandleUserBusySubmit(true);

            var result = await AuthenticationService.UserRegister(RegisterDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllRoles();
                    await GetAllUsers();
                    await GetOrganizationsWithoutAdmin();
                    await SendRegisterEmailConfirmation(RegisterDto);
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandeCreateBusySubmit(false);
        }
    }

    private async Task SendRegisterEmailConfirmation(UserRegisterDto userRegistration)
    {
        try
        {
            HandeCreateBusySubmit(true);

            var emailConfirmation = new UserRegistrationRequestDto()
            {
                EmailAddress = userRegistration.Email,
                Password = userRegistration.Password ?? ""
            };

            var result = await EmailConfirmationService.UserRegistration(emailConfirmation);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandleUserBusySubmit(false);
            OpenCloseCreateUserModal();
            HandeCreateBusySubmit(false);
        }
    }
    private void HandleRegisterUserImageUpload(FileUploadResultDto fileUpload)
    {
        RegisterDto.ImageUrl = fileUpload.File;
        RegisterDto.Image = fileUpload.Base64File ?? "";
        ToggleUserRegistrationImage();
        StateHasChanged();
    }
    #endregion

    #region Details
    private UpdateUserRequestDto UserRequest { get; set; } = new();
    
    private bool IsUpdateUserDetailsModalOpen { get; set; }
    
    private bool CandidateType
    {
        get => IsSelfRegisteredCandidate;
        set
        {
            IsSelfRegisteredCandidate = value;
            
            if (IsSelfRegisteredCandidate)
            {
                UserRequest.OrganizationId = null;
            }
        }
    }
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateUserButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UserRequest.Name) ||
               string.IsNullOrEmpty(UserRequest.EmailAddress) ||
               string.IsNullOrEmpty(UserRequest.PhoneNumber) ||
               string.IsNullOrEmpty(UserRequest.Address) ||
               UserRequest.CountryId == Guid.Empty ||
               UserRequest.RoleId == Guid.Empty ||
               UserRequest.Gender == null;
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleUserUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateUserButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private async Task OpenUpdateUserDetailsModal(Guid userId)
    {
        var response = await UserModuleService.GetUserProfileById(userId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);

            return;
        }

        var userDetails = response.Result;
        
        UserRequest = new UpdateUserRequestDto
        {
            Id = userDetails.Id,
            Name = userDetails.Name,
            EmailAddress = userDetails.Email,
            PhoneNumber = userDetails.PhoneNumber,
            CountryId = userDetails.CountryId,
            RoleId = userDetails.RoleId,
            Role = userDetails.RoleName,
            Gender = Enum.TryParse(userDetails.Gender, out GenderType gender) ? gender : GenderType.Other,
            ImageUrl = string.IsNullOrEmpty(userDetails.ImageUrl) 
                ? ""
                : FileManager.FetchFileUrl(userDetails.ImageUrl, Constants.FilePath.UsersImagesFilePath),
            OrganizationId = userDetails.OrganizationId,
            Organization = userDetails.Organization,
            Address = userDetails.Address,
            DesignationId = userDetails.DesignationId
        };

        IsSelfRegisteredCandidate = UserRequest.OrganizationId != Guid.Empty;

        await GetAllClientOrganizations();
        
        OpenUpdateCloseUserDetailsModal();
    }

    private void OpenUpdateCloseUserDetailsModal()
    {
        IsUpdateUserDetailsModalOpen = !IsUpdateUserDetailsModalOpen;

        StateHasChanged();
    }

    private async Task UpdateUserDetails(bool isClosed)
    {
        if (isClosed)
        {
            OpenUpdateCloseUserDetailsModal();
            
            return;
        }

        try
        {
            HandleUserUpdateBusySubmit(true);

            var result = await UserModuleService.UpdateUserDetails(UserRequest);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    OpenUpdateCloseUserDetailsModal();
                    await GetAllUsers();
                    break;
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandleUserUpdateBusySubmit(false);
        }
    }
    
    private bool OpenUpdateUserImageDrawerToggle { get; set; }
    
    private void ToggleUpdateUserImage()
    {
        OpenUpdateUserImageDrawerToggle = !OpenUpdateUserImageDrawerToggle;
    }

    private void HandleUpdateUserImageUpload(FileUploadResultDto fileUpload)
    {
        UserRequest.ImageUrl = fileUpload.Base64File;
        UserRequest.Image = fileUpload.File;
        ToggleUpdateUserImage();
        StateHasChanged();
    }
    #endregion

    #region Delete
    private bool IsDeleteModalOpen { get; set; }
    
    private bool IsActivateDeactivateUser { get; set; }

    private UserDetail DeleteUserDetailDto { get; set; } = new();

    private async Task OpenUserActivateDeactivateModal(Guid userId)
    {
        var response = await UserModuleService.GetUserProfileById(userId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteUserDetailDto = response.Result;

        IsActivateDeactivateUser = true;

        StateHasChanged(); 
    }

    private async Task ActivateDeactivateUsers(bool isClosed)
    {
        if (isClosed)
        {
            IsActivateDeactivateUser = false;
            return;
        }
        
        try
        {
            var result = await UserModuleService.ActivateDeactivateUsers(DeleteUserDetailDto.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllUsers();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    IsActivateDeactivateUser = false;
                    break;
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    
    private async Task OpenUserDeleteModal(Guid userId)
    {
        var response = await UserModuleService.GetUserProfileById(userId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteUserDetailDto = response.Result;

        IsDeleteModalOpen = true;

        StateHasChanged();
    }
    
    private async Task DeleteUser(bool isClosed)
    {
        if (isClosed)
        {
            IsDeleteModalOpen = false;
            return;
        }

        try
        {
            var result = await UserModuleService.DeleteUser(DeleteUserDetailDto.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllUsers();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    IsDeleteModalOpen = false;
                    break;
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
    }
    #endregion

    #region Reset User Password
    private bool IsResetPasswordModalOpen { get; set; }
    
    private void OpenCloseResetPasswordModal()
    {
        IsResetPasswordModalOpen = !IsResetPasswordModalOpen;
        
        StateHasChanged();
    }

    private bool IsResetPasswordButtonDisabled { get; set; }

    private void HandleBusyResetButton(bool isBusyHandling)
    {
        IsResetPasswordButtonDisabled = isBusyHandling;
        
        StateHasChanged();
    }
    
    private ResetUserPasswordDto ResetPassword { get; set; } = new();
    
    private async Task OpenResetUserPasswordModal(Guid userId)
    {
        var response = await UserModuleService.GetUserProfileById(userId);
        
        ResetPassword.UserId = userId;

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }
        
        OpenCloseResetPasswordModal();
    }
    
    private async Task ResetUserPassword(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseResetPasswordModal();
            return;
        }

        try
        {
            HandleBusyResetButton(true);

            var result = await AuthenticationService.ResetUserPassword(ResetPassword);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                HandleBusyResetButton(false);
                return;
            }

            var resetUserPassword = new ResetPasswordRequestDto()
            {
                UserId = ResetPassword.UserId,
                Password = result.Result.Password
            };

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllUsers();
                    await HandleEmailConfirmation(resetUserPassword);
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandleBusyResetButton(false);
        }
    }
    
    private async Task HandleEmailConfirmation(ResetPasswordRequestDto resetUserPassword)
    {
        try
        {
            HandleBusyResetButton(true);

            var emailConfirmation = new ResetPasswordRequestDto()
            {
                UserId = resetUserPassword.UserId,
                Password = resetUserPassword.Password,
            };

            var result = await EmailConfirmationService.ResetPassword(emailConfirmation);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    break;
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status404NotFound:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandleBusyResetButton(false);
            OpenCloseResetPasswordModal();
        }
    }
    #endregion
    
    #region Password Visibility
    private bool PasswordVisibility { get; set; }

    private InputType PasswordInput { get; set; } = InputType.Password;

    private string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordVisibility()
    {
        if (PasswordVisibility)
        {
            PasswordVisibility = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInput = InputType.Password;
        }
        else
        {
            PasswordVisibility = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInput = InputType.Text;
        }
    }
    #endregion
}