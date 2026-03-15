using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.File;
using CMSTrain.Client.Models.Responses.User;
using CMSTrain.Client.Models.Requests.Identity;
using CMSTrain.Client.Models.Responses.Country;
using CMSTrain.Client.Models.Responses.Designation;
using CMSTrain.Client.Models.Responses.Identity;

namespace CMSTrain.Client.Pages.Client.User;

public partial class Users : ComponentBase
{
    private UserDetail? UserDetails {  get; set; }
    
    private bool IsModalOpen { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllCountries();
        await GetDefaultCountry();
        await GetAllClientUsers();
        await GetAllDesignations();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Users;
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
        
        await GetAllClientUsers();
        
        StateHasChanged();
    }
    
    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive; 
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;
        
        UsersList = null;

        await GetAllClientUsers();
    }
    #endregion
    
    #region Client Users
    private CollectionDto<UserResponseDto>? UsersList { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        UsersList = null;
        
        await GetAllClientUsers();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        UsersList = null;
        
        await GetAllClientUsers();
    }
    
    private async Task GetAllClientUsers()
    {
        try
        {
            var response = await UserModuleService.GetUsersForClientOrganization(PageNumber, PageSize, Search, IsActive);
        
            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
        
            UsersList = response;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
    }
    #endregion

    #region Module Data
    private GetCountryDto DefaultCountry { get; set; } = new();

    private List<GetCountryDto> Countries { get; set; } = [];

    private List<GetDesignationDto> Designations { get; set; } = [];

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
    
    #region Register User
    private bool IsCreateModalOpen { get; set; }
    
    private CandidateRegisterDto RegisterDto { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateClientUserButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(RegisterDto.Name) ||
               string.IsNullOrEmpty(RegisterDto.PhoneNumber) ||
               string.IsNullOrEmpty(RegisterDto.Email) ||
               string.IsNullOrEmpty(RegisterDto.Password) ||
               string.IsNullOrEmpty(RegisterDto.ConfirmPassword) ||
               string.IsNullOrEmpty(RegisterDto.Address) ||
               RegisterDto.DesignationId == Guid.Empty ||
               RegisterDto.Gender == null ||
               RegisterDto.CountryId == Guid.Empty ;
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleClientUserBusySubmit(bool isBusySubmitting)
    {
        IsCreateClientUserButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private string ImageUrl { get; set; } = "";
    
    private bool OpenClientRegisterImageDrawerToggle { get; set; }

    private void HandleClientRegisterImageUpload(FileUploadResultDto fileUpload)
    {
        RegisterDto.ImageUrl = fileUpload.File;
        ImageUrl = fileUpload.Base64File ?? "";
        ToggleClientRegisterImage();
        StateHasChanged();
    }

    private void OpenCloseRegisterClientUserModal()
    {
        IsCreateModalOpen = !IsCreateModalOpen;

        StateHasChanged();
    }
    
    private void OpenRegisterClientUsersModal()
    {
        RegisterDto = new CandidateRegisterDto()
        {
            CountryId = DefaultCountry.Id
        };

        OpenCloseRegisterClientUserModal();
    }

    private async Task RegisterClientUser(bool isClosed)
    {
        if (isClosed)
        {
            IsCreateModalOpen = false;
            return;
        }

        try
        {
            HandleClientUserBusySubmit(true);

            var result = await AuthenticationService.ClientCandidateRegister(RegisterDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllClientUsers();
                    OpenCloseRegisterClientUserModal();
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
            HandleClientUserBusySubmit(false);
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

    #region File Upload
    private void ToggleClientRegisterImage()
    {
        OpenClientRegisterImageDrawerToggle = !OpenClientRegisterImageDrawerToggle;
    }
    #endregion
    
    #region Delete
    private bool IsDeleteModalOpen { get; set; }

    private UserDetail DeleteUserDetailDto { get; set; } = new();

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
            var result = await UserModuleService.ActivateDeactivateUsers(DeleteUserDetailDto.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllClientUsers();
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
}