using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.File;
using CMSTrain.Client.Models.Responses.Country;
using CMSTrain.Client.Models.Requests.Organization;
using CMSTrain.Client.Models.Requests.User;
using CMSTrain.Client.Models.Responses.Organization;
using CMSTrain.Client.Models.Responses.ClientOrganization;
using CMSTrain.Client.Models.Responses.Designation;

namespace CMSTrain.Client.Pages.Administrator.Client;

public partial class Client : ComponentBase
{
    private bool IsClientOrganizationAdminModalOpen { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();

        await GetAllCountries();
        
        await GetDefaultCountry();

        await GetAllDesignations();
        
        await GetAllClientOrganizations();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Client;
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
        
        await GetAllClientOrganizations();
        
        StateHasChanged();
    }

    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive; 
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;

        ClientOrganizationList = null;

        await GetAllClientOrganizations();
    }
    #endregion

    #region Module Entity Data
    private GetCountryDto DefaultCountry { get; set; } = new();

    private List<GetCountryDto> Countries { get; set; } = new();

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

    #region Organizations
    private CollectionDto<GetClientOrganizationDto>? ClientOrganizationList { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;
        
        ClientOrganizationList = null;
        
        await GetAllClientOrganizations();
    }
    
    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;
        
        ClientOrganizationList = null;
        
        await GetAllClientOrganizations();
    }
    
    private async Task GetAllClientOrganizations()
    {
        try
        {
            var response = await ClientOrganizationService.GetAllClientOrganizations(PageNumber, PageSize, Search, IsActive);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            ClientOrganizationList = response;
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
    }
    #endregion
    
    #region Register Admin
    private RegisterClientAdminDto RegisterClientAdminDto { get; set; } = new();
    
    private bool _isAdminCreateButtonDisabled;

    private bool IsAdminCreateButtonDisabled
    {
        get => _isAdminCreateButtonDisabled || 
               string.IsNullOrEmpty(RegisterClientAdminDto.Name) ||
               string.IsNullOrEmpty(RegisterClientAdminDto.Email) ||
               string.IsNullOrEmpty(RegisterClientAdminDto.PhoneNumber) ||
               string.IsNullOrEmpty(RegisterClientAdminDto.Password) ||
               string.IsNullOrEmpty(RegisterClientAdminDto.ConfirmPassword) ||
               string.IsNullOrEmpty(RegisterClientAdminDto.Address) ||
               RegisterClientAdminDto.CountryId == Guid.Empty || 
               RegisterClientAdminDto.DesignationId == Guid.Empty || 
               RegisterClientAdminDto.Gender == null;
        set => _isAdminCreateButtonDisabled = value;
    }
    
    private void HandleCreateAdminBusySubmit(bool isBusySubmitting)
    {
        IsAdminCreateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenClientAdminRegisterModal(Guid organizationId)
    {
        OpenCloseUserDetailsModal();
        
        RegisterClientAdminDto = new RegisterClientAdminDto()
        {
            OrganizationId = organizationId,
            CountryId = DefaultCountry.Id
        };
    }
    
    private void OpenCloseUserDetailsModal()
    {
        IsClientOrganizationAdminModalOpen = !IsClientOrganizationAdminModalOpen;
        StateHasChanged();
    }
    
    private async Task RegisterClientOrganizationAdmin(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseUserDetailsModal();
            return;
        }

        try
        {
            HandleCreateAdminBusySubmit(true);

            var response = await ClientOrganizationService.RegisterClientOrganizationAdmin(RegisterClientAdminDto);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            switch (response.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseUserDetailsModal();
                    await GetAllClientOrganizations();
                    SnackbarService.ShowSnackbar(response.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status404NotFound:
                case StatusCode.Status400BadRequest:
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status500InternalServerError:
                    SnackbarService.ShowSnackbar(response.Message, Severity.Error, Variant.Outlined);
                    break;
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        finally
        {
            HandleCreateAdminBusySubmit(false);
        }
    }
    
    private void HandleClientOrganizationAdminImageUpload(FileUploadResultDto fileUpload)
    {
        RegisterClientAdminDto.Image = fileUpload.Base64File ?? string.Empty;
        RegisterClientAdminDto.ImageUrl = fileUpload.File;
        ToggleClientOrganizationAdminRegistrationImage();
        StateHasChanged();
    }
    #endregion
    
    #region Register Client
    private bool IsCreateModalOpen { get; set; }
    
    private CreateOrganizationDto RegisterOrganizationDto { get; set; } = new();
    
    private bool OpenClientImageDrawerToggle { get; set; }
    
    private bool OpenClientOrganizationAdminImageDrawerToggle { get; set; }
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateClientButtonDisabled
    {
        get => _isCreateButtonDisabled ||
               string.IsNullOrEmpty(RegisterOrganizationDto.Name);

        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleClientCreateBusySubmit(bool isBusySubmitting)
    {
        IsCreateClientButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private void ToggleClientRegistrationImage()
    {
        OpenClientImageDrawerToggle = !OpenClientImageDrawerToggle;
    }
    
    private void ToggleClientOrganizationAdminRegistrationImage()
    {
        OpenClientOrganizationAdminImageDrawerToggle = !OpenClientOrganizationAdminImageDrawerToggle;
    }
    
    private void OpenRegisterOrganizationModal()
    {
        IsCreateModalOpen = true;
        RegisterOrganizationDto = new CreateOrganizationDto();
        StateHasChanged();
    }
    
    private async Task RegisterNewOrganization(bool isClosed)
    {
        if (isClosed)
        {
            IsCreateModalOpen = false;
            return;
        }

        try
        {
            HandleClientCreateBusySubmit(true);

            var result = await OrganizationService.InsertOrganization(RegisterOrganizationDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllClientOrganizations();
                    IsCreateModalOpen = false;
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status400BadRequest:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
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
            HandleClientCreateBusySubmit(false);
        }
    }
    
    private void HandleOrganizationRegisterImageUpload(FileUploadResultDto fileUpload)
    {
        RegisterOrganizationDto.ImageUrl = fileUpload.File;
        UpdateOrganizationDto.ImageUrl = fileUpload.File;
        ToggleClientRegistrationImage();
        StateHasChanged();
    }
    #endregion

    #region Update Client
    private bool IsUpdateModalOpen { get; set; }
    
    private UpdateOrganizationDto UpdateOrganizationDto { get; set; } = new();
    
    private GetOrganizationDto GetOrganizationDto { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateClientButtonDisabled
    {
        get => _isUpdateButtonDisabled ||
               string.IsNullOrEmpty(UpdateOrganizationDto.Name);
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleClientUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateClientButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }
    
    private async Task OpenUpdateOrganizationModal(Guid organizationId)
    {
        var response = await OrganizationService.GetOrganizationById(organizationId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        GetOrganizationDto = response.Result;

        UpdateOrganizationDto = new UpdateOrganizationDto()
        {
            Id = GetOrganizationDto.Id,
            Name = GetOrganizationDto.Name,
            Address = GetOrganizationDto.Address,
            Description = GetOrganizationDto.Description,
            ImageUrl = UpdateOrganizationDto.ImageUrl,
        };

        OpenCloseEditModal();
        StateHasChanged();
    }
    
    private void OpenCloseEditModal()
    {
        IsUpdateModalOpen = !IsUpdateModalOpen;

        StateHasChanged();
    }
    
    private async Task UpdateOrganization(bool isClosed)
    {
        if (isClosed)
        {
            IsUpdateModalOpen = false;
            return;
        }

        try
        {
            HandleClientUpdateBusySubmit(true);

            var result = await OrganizationService.UpdateOrganization(UpdateOrganizationDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    IsUpdateModalOpen = false;
                    await GetAllClientOrganizations();
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    break;
                case StatusCode.Status401Unauthorized:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
                case StatusCode.Status400BadRequest:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
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
            HandleClientUpdateBusySubmit(false);
        }
    }
    #endregion
    
    #region Delete
    private bool IsDeleteModalOpen { get; set; }
    
    private bool IsActivationStatusModalOpen { get; set; }
    
    private GetOrganizationDto DeleteOrganizationDto { get; set; } = new();

    private async Task OpenOrganizationDeleteModal(Guid organizationId)
    {
        var response = await OrganizationService.GetOrganizationById(organizationId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteOrganizationDto = response.Result;

        IsDeleteModalOpen = true;
        
        StateHasChanged();
    }
    
    private async Task OpenOrganizationActivationStatusModal(Guid organizationId)
    {
        var response = await OrganizationService.GetOrganizationById(organizationId);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteOrganizationDto = response.Result;

        IsActivationStatusModalOpen = true;
        
        StateHasChanged();
    }
    
    private async Task ActivateDeactivateOrganization(bool isClosed)
    {
        if (isClosed)
        {
            IsActivationStatusModalOpen = false;
            return;
        }
        
        try
        {
            var result = await OrganizationService.ActivateDeactivateOrganization(DeleteOrganizationDto.Id);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    IsActivationStatusModalOpen = false;
                    await GetAllClientOrganizations();
                    break;
                case StatusCode.Status401Unauthorized:
                case StatusCode.Status400BadRequest:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Warning, Variant.Outlined);
                    break;
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
    
    private async Task DeleteOrganization(bool isClosed)
    {
        if (isClosed)
        {
            IsDeleteModalOpen = false;
            return;
        }
        
        try
        {
            var result = await OrganizationService.DeleteOrganization(DeleteOrganizationDto.Id);
            
            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }
            
            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    await GetAllClientOrganizations();
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

    #region Admin Details
    private async Task HandleAdminActions(GetClientOrganizationDto clientOrganization)
    {
        if (clientOrganization.Admin == null)
        {
            OpenClientAdminRegisterModal(clientOrganization.Id);
        }
        else
        {
            await OpenAdminDetailsModal(clientOrganization.Admin.Id);
        }
    }
    
    private bool IsAdminDetailsModalOpen { get; set; }

    private UpdateUserRequestDto UserRequest { get; set; } = new();

    private bool IsAdminUpdateButtonDisabled =>
        string.IsNullOrEmpty(UserRequest.Name) ||
        string.IsNullOrEmpty(UserRequest.EmailAddress) ||
        string.IsNullOrEmpty(UserRequest.PhoneNumber) ||
        UserRequest.CountryId == Guid.Empty;

    private void OpenCloseAdminDetailsModal()
    {
        IsAdminDetailsModalOpen = !IsAdminDetailsModalOpen;
        StateHasChanged();
    }
    
    private async Task OpenAdminDetailsModal(Guid userId)
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
            Organization = userDetails.Organization,
            DesignationId = userDetails.DesignationId,
            Address = userDetails.Address
        };

        OpenCloseAdminDetailsModal();
    }
    
    private async Task UpdateUserDetails(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseAdminDetailsModal();
            
            return;
        }
        
        try
        {
            var result = await UserModuleService.UpdateUserDetails(UserRequest);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    SnackbarService.ShowSnackbar(result.Message, Severity.Success, Variant.Outlined);
                    OpenCloseAdminDetailsModal();
                    await GetAllClientOrganizations();
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
}