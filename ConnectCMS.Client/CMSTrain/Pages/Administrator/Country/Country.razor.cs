using MudBlazor;
using CMSTrain.Client.Models.Base;
using Microsoft.AspNetCore.Components;
using CMSTrain.Client.Models.Constants;
using CMSTrain.Client.Layout.Application;
using CMSTrain.Client.Models.Requests.Country;
using CMSTrain.Client.Models.Responses.Country;

namespace CMSTrain.Client.Pages.Administrator.Country;

public partial class Country : ComponentBase
{
    protected override async Task OnInitializedAsync()
    {
        SetPageTitle();
        await GetAllCountries();
    }

    #region Page Title
    [CascadingParameter] public MainLayout Layout { get; set; } = new();

    private void SetPageTitle()
    {
        Layout.PageTitle = PageTitle.Country;
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
        
        await GetAllCountries();
    }

    private bool? IsActive { get; set; } = Constants.ActivationStatus.Active;

    private async Task OnStatusFilter(bool? isActive)
    {
        IsActive = isActive; 
        
        PageSize = Constants.Pagination.Size;

        PageNumber = Constants.Pagination.Page;

        Countries = null;

        await GetAllCountries();
    }
    #endregion
    
    #region Get All Country Details
    private CollectionDto<GetCountryDto>? Countries { get; set; }
    
    private int PageNumber { get; set; } = Constants.Pagination.Page;
    
    private int PageSize { get; set; } = Constants.Pagination.Size; 

    private async Task UpdatePageNumber(int pageNumber)
    {
        PageNumber = pageNumber;

        Countries = null;

        await GetAllCountries(); 
    }

    private async Task UpdatePageSize(int pageSize)
    {
        PageNumber = 1;
        
        PageSize = pageSize;

        Countries = null;
        
        await GetAllCountries();
    }
    
    private async Task GetAllCountries()
    {
        try
        {
            var response = await CountryService.GetAllCountries(PageNumber, PageSize, Search, IsActive);

            if (response?.Result is null)
            {
                SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            Countries = response;
        
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowSnackbar(ex.Message, Severity.Error, Variant.Outlined);
        }
        
    }
    #endregion

    #region Global Countries
    private List<GetCountryDto> GlobalCountries { get; set; } = new();

    private async Task GetGlobalCountries()
    {
        var response = await CountryService.GetGlobalCountries();
        
        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }
        
        GlobalCountries = response.Result;
    }

    private void OnCountrySelected(string country)
    {
        var countryModel = GlobalCountries.FirstOrDefault(c => c.Name == country);
        
        if (countryModel != null)
        {
            CreateCountryDto.Name = countryModel.Name;
            CreateCountryDto.Code = countryModel.Code; 
        }
    }
    #endregion

    #region Create 
    private bool IsCreateModalOpen { get; set; }

    private CreateCountryDto CreateCountryDto { get; set; } = new();
    
    private bool _isCreateButtonDisabled;

    private bool IsCreateButtonDisabled
    {
        get => _isCreateButtonDisabled || 
               string.IsNullOrEmpty(CreateCountryDto.Name) ||
               string.IsNullOrEmpty(CreateCountryDto.PhoneCode) ||
               string.IsNullOrEmpty(CreateCountryDto.Code);
        set => _isCreateButtonDisabled = value;
    }
    
    private void HandleCreateBusySubmit(bool isBusySubmitting)
    {
        IsCreateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private async Task OpenCloseCreateCountryModal()
    {
        IsCreateModalOpen = !IsCreateModalOpen;
        
        await GetGlobalCountries();

        StateHasChanged();
    }

    private async Task OpenCreateCountryModal()
    {
        CreateCountryDto = new CreateCountryDto();

        await OpenCloseCreateCountryModal();
    }

    private async Task InsertCountry(bool isClosed)
    {
        if (isClosed)
        {
            await OpenCloseCreateCountryModal();

            return;
        }

        try
        {
            HandleCreateBusySubmit(true);

            var result = await CountryService.InsertCountry(CreateCountryDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);

                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    await GetAllCountries();
                    await OpenCloseCreateCountryModal();
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
            HandleCreateBusySubmit(false);
        }
    }
    #endregion

    #region Details
    private GetCountryDto GetCountry { get; set; } = new();

    private bool IsDetailsModalOpen { get; set; }

    private void OpenCloseCountryDetailsModal()
    {
        IsDetailsModalOpen = !IsDetailsModalOpen;

        StateHasChanged();
    }
    #endregion

    #region Update 
    private bool IsEditModalOpen { get; set; }

    private UpdateCountryDto UpdateCountryDto { get; set; } = new();
    
    private bool _isUpdateButtonDisabled;
    
    private bool IsUpdateButtonDisabled
    {
        get => _isUpdateButtonDisabled || 
               string.IsNullOrEmpty(UpdateCountryDto.Name) ||
               string.IsNullOrEmpty(UpdateCountryDto.PhoneCode) ||
               string.IsNullOrEmpty(UpdateCountryDto.Code);
        set => _isUpdateButtonDisabled = value;
    }
    
    private void HandleUpdateBusySubmit(bool isBusySubmitting)
    {
        IsUpdateButtonDisabled = isBusySubmitting;
        
        StateHasChanged();
    }

    private void OpenCloseEditModal()
    {
        IsEditModalOpen = !IsEditModalOpen;

        StateHasChanged();
    }

    private async Task OpenCountryUpdateModal(Guid country)
    {
        var response = await CountryService.GetCountryById(country);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        GetCountry = response.Result;

        UpdateCountryDto = new UpdateCountryDto()
        {
            Id = GetCountry.Id,
            Name = GetCountry.Name,
            Code = GetCountry.Code,
            PhoneCode = GetCountry.PhoneCode,
        };

        OpenCloseEditModal();
    }

    private async Task UpdateCountry(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseEditModal();

            return;
        }

        try
        {
            HandleUpdateBusySubmit(true);

            var result = await CountryService.UpdateCountry(UpdateCountryDto);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error,
                    Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseEditModal();
                    await GetAllCountries();
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
            HandleUpdateBusySubmit(false);
        }
    }
    #endregion

    #region Delete
    private bool IsDeleteModalOpen { get; set; }

    private GetCountryDto DeleteCountryDto { get; set; } = new();

    private void OpenCloseDeleteModal()
    {
        IsDeleteModalOpen = !IsDeleteModalOpen;

        StateHasChanged();
    }

    private async Task OpenCountryDeleteModal(Guid country)
    {
        var response = await CountryService.GetCountryById(country);

        if (response?.Result is null)
        {
            SnackbarService.ShowSnackbar(response?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
            return;
        }

        DeleteCountryDto = response.Result;

        OpenCloseDeleteModal();
    }

    private async Task DeleteCountry(bool isClosed)
    {
        if (isClosed)
        {
            OpenCloseDeleteModal();
            return;
        }

        try
        {
            var result = await CountryService.ActivateDeactivateCountry(DeleteCountryDto.Id);

            if (result?.Result is null)
            {
                SnackbarService.ShowSnackbar(result?.Message ?? Constants.Message.ExceptionMessage, Severity.Error, Variant.Outlined);
                return;
            }

            switch (result.StatusCode)
            {
                case StatusCode.Status200Ok:
                    OpenCloseDeleteModal();
                    await GetAllCountries();
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
    }
    #endregion
}